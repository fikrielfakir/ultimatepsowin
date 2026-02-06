using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;
using UltimatePOS.Core.Models;

namespace UltimatePOS.Core.ViewModels.Product;

public partial class ProductListViewModel : ObservableObject
{
    private readonly IProductService _productService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<Entities.Product> _products = new();

    [ObservableProperty]
    private ObservableCollection<Brand> _brands = new();

    [ObservableProperty]
    private ObservableCollection<Category> _categories = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _searchTerm;

    [ObservableProperty]
    private int? _selectedCategoryId;

    [ObservableProperty]
    private int? _selectedBrandId;

    [ObservableProperty]
    private ProductType? _selectedType;

    [ObservableProperty]
    private bool? _showLowStockOnly;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _pageSize = 20;

    [ObservableProperty]
    private int _totalPages;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private bool _hasNextPage;

    [ObservableProperty]
    private bool _hasPreviousPage;

    [ObservableProperty]
    private Entities.Product? _selectedProduct;

    [ObservableProperty]
    private ObservableCollection<Entities.Product> _selectedProducts = new();

    public bool HasNoProducts => !IsLoading && Products.Count == 0;

    public ProductListViewModel(
        IProductService productService,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _productService = productService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    public async Task LoadProductsAsync()
    {
        IsLoading = true;
        try
        {
            var filter = new ProductFilter
            {
                SearchTerm = SearchTerm,
                CategoryId = SelectedCategoryId,
                BrandId = SelectedBrandId,
                Type = SelectedType,
                LowStock = ShowLowStockOnly,
                PageNumber = CurrentPage,
                PageSize = PageSize,
                SortBy = "Name",
                Descending = false
            };

            var result = await _productService.GetProductsAsync(filter);

            Products.Clear();
            foreach (var product in result.Items)
            {
                Products.Add(product);
            }

            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            HasNextPage = result.HasNextPage;
            HasPreviousPage = result.HasPreviousPage;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task LoadFiltersAsync()
    {
        // Load brands
        var brands = await _productService.GetBrandsAsync();
        Brands.Clear();
        foreach (var brand in brands)
        {
            Brands.Add(brand);
        }

        // Load categories
        var categories = await _productService.GetCategoriesAsync();
        Categories.Clear();
        foreach (var category in categories)
        {
            Categories.Add(category);
        }
    }

    [RelayCommand]
    public async Task SearchAsync()
    {
        CurrentPage = 1; // Reset to first page on search
        await LoadProductsAsync();
    }

    [RelayCommand]
    public async Task ClearFiltersAsync()
    {
        SearchTerm = null;
        SelectedCategoryId = null;
        SelectedBrandId = null;
        SelectedType = null;
        ShowLowStockOnly = null;
        CurrentPage = 1;
        await LoadProductsAsync();
    }

    [RelayCommand]
    public async Task NextPageAsync()
    {
        if (HasNextPage)
        {
            CurrentPage++;
            await LoadProductsAsync();
        }
    }

    [RelayCommand]
    public async Task PreviousPageAsync()
    {
        if (HasPreviousPage)
        {
            CurrentPage--;
            await LoadProductsAsync();
        }
    }

    [RelayCommand]
    public async Task AddProductAsync()
    {
        // TODO: Navigate to ProductFormPage
        await _dialogService.ShowMessageAsync("Add Product", "Product form not yet implemented");
    }

    [RelayCommand]
    public async Task EditProductAsync(Entities.Product? product)
    {
        if (product == null) return;
        // TODO: Navigate to ProductFormPage with product ID
        await _dialogService.ShowMessageAsync("Edit Product", $"Editing product: {product.Name}");
    }

    [RelayCommand]
    public async Task DeleteProductAsync(Entities.Product? product)
    {
        if (product == null) return;

        var confirmed = await _dialogService.ShowConfirmationAsync(
            "Delete Product",
            $"Are you sure you want to delete '{product.Name}'?",
            "Delete",
            "Cancel"
        );

        if (confirmed)
        {
            try
            {
                await _productService.DeleteProductAsync(product.Id);
                await LoadProductsAsync();
                await _dialogService.ShowMessageAsync("Success", "Product deleted successfully");
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowMessageAsync("Error", $"Failed to delete product: {ex.Message}");
            }
        }
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        await LoadProductsAsync();
    }

    [RelayCommand]
    public async Task BulkDeleteAsync()
    {
        if (SelectedProducts.Count == 0)
        {
            await _dialogService.ShowMessageAsync("No Selection", "Please select products to delete");
            return;
        }

        var confirmed = await _dialogService.ShowConfirmationAsync(
            "Delete Products",
            $"Are you sure you want to delete {SelectedProducts.Count} product(s)?",
            "Delete",
            "Cancel"
        );

        if (confirmed)
        {
            try
            {
                foreach (var product in SelectedProducts.ToList())
                {
                    await _productService.DeleteProductAsync(product.Id);
                }
                SelectedProducts.Clear();
                await LoadProductsAsync();
                await _dialogService.ShowMessageAsync("Success", "Products deleted successfully");
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowMessageAsync("Error", $"Failed to delete products: {ex.Message}");
            }
        }
    }

    [RelayCommand]
    public async Task BulkPrintAsync()
    {
        if (SelectedProducts.Count == 0)
        {
            await _dialogService.ShowMessageAsync("No Selection", "Please select products to print labels for");
            return;
        }

        var productIds = SelectedProducts.Select(p => p.Id).ToArray();
        await _dialogService.ShowBarcodePrintDialogAsync(productIds);
    }

    public bool IsLowStock(Entities.Product product)
    {
        // Product is low stock if it has stock records and any are below alert quantity
        // This is a simplified check - full implementation would check ProductStock table
        return product.AlertQuantity > 0;
    }
}

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Core.ViewModels.Stock;

public partial class ReorderLevelViewModel : ObservableObject
{
    private readonly IProductService _productService;
    private readonly ILocationService _locationService;
    private readonly ISessionService _sessionService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<Product> _products = new();

    [ObservableProperty]
    private ObservableCollection<Location> _locations = new();

    [ObservableProperty]
    private Location? _selectedLocation;

    [ObservableProperty]
    private string? _searchText;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasChanges;

    public ReorderLevelViewModel(
        IProductService productService,
        ILocationService locationService,
        ISessionService sessionService,
        IDialogService dialogService)
    {
        _productService = productService;
        _locationService = locationService;
        _sessionService = sessionService;
        _dialogService = dialogService;
    }

    public async Task InitializeAsync(int? locationId = null)
    {
        IsLoading = true;
        try
        {
            var businessId = _sessionService.CurrentBusiness?.Id;
            if (businessId == null)
            {
                await _dialogService.ShowErrorAsync("Error", "No active business session found.");
                return;
            }

            // Load locations
            var locations = await _locationService.GetLocationsByBusinessIdAsync(businessId.Value);
            Locations.Clear();
            foreach (var loc in locations)
            {
                Locations.Add(loc);
            }

            if (locationId.HasValue)
            {
                SelectedLocation = Locations.FirstOrDefault(l => l.Id == locationId.Value);
            }
            else if (Locations.Count > 0)
            {
                SelectedLocation = Locations[0];
            }

            await LoadProductsAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task LoadProductsAsync()
    {
        IsLoading = true;
        try
        {
            var businessId = _sessionService.CurrentBusiness?.Id;
            if (businessId == null) return;
            
            var allProducts = await _productService.GetProductsByBusinessIdAsync(businessId.Value);
            
            Products.Clear();
            foreach (var product in allProducts.Where(p => p.IsActive))
            {
                // Filter by search text if present
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    if (product.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                        product.SKU.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    {
                        Products.Add(product);
                    }
                }
                else
                {
                    Products.Add(product);
                }
            }

            HasChanges = false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (!HasChanges) return;

        IsLoading = true;
        try
        {
            foreach (var product in Products)
            {
                await _productService.UpdateProductAsync(product);
            }

            HasChanges = false;
            await _dialogService.ShowMessageAsync("Success", "Reorder levels updated successfully.");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Save Failed", $"Failed to save reorder levels: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void MarkAsChanged()
    {
        HasChanges = true;
    }

    partial void OnSearchTextChanged(string? value)
    {
        // In a real implementation, we'd filter the products collection
        // For now, we'll just reload
        if (!string.IsNullOrWhiteSpace(value))
        {
            _ = LoadProductsAsync();
        }
    }
}

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
        ILocationService locationService)
    {
        _productService = productService;
        _locationService = locationService;
    }

    public async Task InitializeAsync(int? locationId = null)
    {
        IsLoading = true;
        try
        {
            // Load locations
            var locations = await _locationService.GetLocationsByBusinessIdAsync(1); // TODO: Get current business ID
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
            var allProducts = await _productService.GetProductsByBusinessIdAsync(1); // TODO: Get current business ID
            
            Products.Clear();
            foreach (var product in allProducts.Where(p => p.IsActive))
            {
                Products.Add(product);
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

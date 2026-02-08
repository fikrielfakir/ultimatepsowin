using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;
using UltimatePOS.Core.Models;

namespace UltimatePOS.Core.ViewModels.Stock;

public partial class StockAdjustmentFormViewModel : ObservableObject
{
    private readonly IStockService _stockService;
    private readonly ILocationService _locationService;
    private readonly IProductService _productService;
    private readonly ISessionService _sessionService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private Entities.Product? _selectedProduct;

    [ObservableProperty]
    private ProductStock? _selectedStock;

    [ObservableProperty]
    private Location? _selectedLocation;
    
    [ObservableProperty]
    private decimal _quantity;
    
    [ObservableProperty]
    private StockAdjustmentType _adjustmentType = StockAdjustmentType.Add;
    
    [ObservableProperty]
    private string? _reason;
    
    [ObservableProperty]
    private string? _reference;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<Location> _locations = new();

    [ObservableProperty]
    private ObservableCollection<Entities.Product> _products = new();

    // Validation Errors
    [ObservableProperty]
    private string? _productError;

    [ObservableProperty]
    private string? _locationError;

    [ObservableProperty]
    private string? _quantityError;

    public StockAdjustmentFormViewModel(
        IStockService stockService,
        ILocationService locationService,
        IProductService productService,
        ISessionService sessionService,
        IDialogService dialogService)
    {
        _stockService = stockService;
        _locationService = locationService;
        _productService = productService;
        _sessionService = sessionService;
        _dialogService = dialogService;
    }
    
    public async Task InitializeAsync(ProductStock? stock = null)
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

            // Load products for selection if not provided
            if (stock == null)
            {
                var products = await _productService.GetProductsAsync(businessId.Value);
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }

            if (stock != null)
            {
                SelectedStock = stock;
                SelectedProduct = stock.Product;
                SelectedLocation = Locations.FirstOrDefault(l => l.Id == stock.LocationId);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task<bool> SaveAsync()
    {
        if (!Validate()) return false;

        IsSaving = true;
        try
        {
            decimal adjustmentQuantity = Quantity;

            if (AdjustmentType == StockAdjustmentType.Remove)
            {
                adjustmentQuantity = -Quantity;
            }
            else if (AdjustmentType == StockAdjustmentType.Set)
            {
                if (SelectedStock == null)
                {
                    adjustmentQuantity = Quantity;
                }
                else
                {
                    adjustmentQuantity = Quantity - SelectedStock.Quantity;
                }
            }

            if (adjustmentQuantity == 0)
            {
                await _dialogService.ShowWarningAsync("No Change", "The adjustment results in no change to the stock quantity.");
                return false;
            }

            await _stockService.AdjustStockAsync(
                SelectedProduct!.Id,
                SelectedLocation!.Id,
                adjustmentQuantity,
                AdjustmentType.ToString(),
                Reason ?? "Manual Adjustment",
                Reference
            );
            
            await _dialogService.ShowMessageAsync("Success", "Stock adjusted successfully.");
            return true;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Error", $"Failed to save adjustment: {ex.Message}");
            return false;
        }
        finally
        {
            IsSaving = false;
        }
    }

    private bool Validate()
    {
        bool isValid = true;
        
        if (SelectedProduct == null)
        {
            ProductError = "Please select a product.";
            isValid = false;
        }
        else
        {
            ProductError = null;
        }

        if (SelectedLocation == null)
        {
            LocationError = "Please select a location.";
            isValid = false;
        }
        else
        {
            LocationError = null;
        }

        if (Quantity < 0)
        {
            QuantityError = "Quantity cannot be negative.";
            isValid = false;
        }
        else
        {
            QuantityError = null;
        }

        return isValid;
    }

    [RelayCommand]
    public void Cancel()
    {
        // Navigation handled by View
    }
}

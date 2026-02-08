using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;
using UltimatePOS.Core.Models;

namespace UltimatePOS.Core.ViewModels.Stock;

public partial class StockAdjustmentViewModel : ObservableObject
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
    private bool _isSaveSuccessful;

    [ObservableProperty]
    private ObservableCollection<Location> _locations = new();

    public StockAdjustmentViewModel(
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
    
    public async Task InitializeAsync(ProductStock? stock)
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

        if (stock != null)
        {
            SelectedStock = stock;
            SelectedProduct = stock.Product;
            SelectedLocation = Locations.FirstOrDefault(l => l.Id == stock.LocationId);
        }
    }

    [RelayCommand]
    public async Task<bool> SaveAsync()
    {
        if (SelectedProduct == null || SelectedLocation == null)
        {
            await _dialogService.ShowWarningAsync("Validation Error", "Please select a product and location.");
            IsSaveSuccessful = false;
            return false;
        }

        if (Quantity < 0)
        {
            await _dialogService.ShowWarningAsync("Validation Error", "Quantity cannot be negative.");
            IsSaveSuccessful = false;
            return false;
        }

        decimal adjustmentQuantity = Quantity;

        if (AdjustmentType == StockAdjustmentType.Remove)
        {
            adjustmentQuantity = -Quantity;
        }
        else if (AdjustmentType == StockAdjustmentType.Set)
        {
            if (SelectedStock == null)
            {
                // New stock record
                adjustmentQuantity = Quantity;
            }
            else
            {
                // Calculate difference
                adjustmentQuantity = Quantity - SelectedStock.Quantity;
            }
        }

        if (adjustmentQuantity == 0)
        {
            await _dialogService.ShowWarningAsync("No Change", "The adjustment results in no change to the stock quantity.");
            IsSaveSuccessful = false;
            return false;
        }

        try
        {
            await _stockService.AdjustStockAsync(
                SelectedProduct.Id,
                SelectedLocation.Id,
                adjustmentQuantity,
                AdjustmentType.ToString(),
                Reason ?? "Manual Adjustment",
                Reference
            );
            
            IsSaveSuccessful = true;
            return true;
        }
        catch (System.Exception ex)
        {
            await _dialogService.ShowErrorAsync("Error", $"Failed to save adjustment: {ex.Message}");
            IsSaveSuccessful = false;
            return false;
        }
    }
}

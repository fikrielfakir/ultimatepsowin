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
    private ObservableCollection<Location> _locations = new();

    public StockAdjustmentViewModel(
        IStockService stockService,
        ILocationService locationService,
        IProductService productService,
        ISessionService sessionService)
    {
        _stockService = stockService;
        _locationService = locationService;
        _productService = productService;
        _sessionService = sessionService;
    }
    
    public async Task InitializeAsync(ProductStock? stock)
    {
        // Load locations
        var locations = await _locationService.GetLocationsByBusinessIdAsync(1); // TODO: Use business id
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
    public async Task SaveAsync()
    {
        if (SelectedProduct == null || SelectedLocation == null || Quantity < 0)
        {
            return;
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

        if (adjustmentQuantity == 0) return;

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
            
            // Close dialog handled by view code-behind or DialogService result
        }
        catch (System.Exception ex)
        {
            // TODO: Handle error
        }
    }
}

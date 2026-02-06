using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;
using System.Linq;

namespace UltimatePOS.Core.ViewModels.Stock;

public partial class StockTransferViewModel : ObservableObject
{
    private readonly IStockService _stockService;
    private readonly ILocationService _locationService;
    private readonly IProductService _productService;
    private readonly ISessionService _sessionService;

    [ObservableProperty]
    private Entities.Product? _selectedProduct;

    [ObservableProperty]
    private Location? _sourceLocation;

    [ObservableProperty]
    private Location? _destinationLocation;
    
    [ObservableProperty]
    private decimal _quantity;
    
    [ObservableProperty]
    private string? _reference;

    [ObservableProperty]
    private ObservableCollection<Location> _locations = new();

    public StockTransferViewModel(
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
    
    public async Task InitializeAsync()
    {
        // Load locations
        var locations = await _locationService.GetLocationsByBusinessIdAsync(1); // TODO: Use business id
        Locations.Clear();
        foreach (var loc in locations)
        {
            Locations.Add(loc);
        }

        if (Locations.Count > 0)
        {
            SourceLocation = Locations[0];
            if (Locations.Count > 1)
            {
                DestinationLocation = Locations[1];
            }
        }
    }

    public async Task SetProductAsync(int productId)
    {
        SelectedProduct = await _productService.GetProductByIdAsync(productId);
    }

    [RelayCommand]
    public async Task TransferAsync()
    {
        if (SelectedProduct == null || SourceLocation == null || DestinationLocation == null || Quantity <= 0)
        {
            return;
        }

        if (SourceLocation.Id == DestinationLocation.Id)
        {
            // TODO: correct way to show error via dialog service?
            // For now assuming validation happens in UI or service throws
            return;
        }

        try
        {
            await _stockService.TransferStockAsync(
                SelectedProduct.Id,
                SourceLocation.Id,
                DestinationLocation.Id,
                Quantity,
                Reference
            );
            
            // Close dialog handled by view code-behind
        }
        catch (System.Exception ex)
        {
            // TODO: Handle error
        }
    }
}

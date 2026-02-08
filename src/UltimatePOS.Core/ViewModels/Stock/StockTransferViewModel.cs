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
    private readonly IDialogService _dialogService;

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
    private bool _isTransferSuccessful;

    [ObservableProperty]
    private ObservableCollection<Location> _locations = new();

    public StockTransferViewModel(
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
    
    public async Task InitializeAsync()
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
    public async Task<bool> TransferAsync()
    {
        if (SelectedProduct == null)
        {
            await _dialogService.ShowWarningAsync("Validation Error", "Please select a product.");
            IsTransferSuccessful = false;
            return false;
        }

        if (SourceLocation == null || DestinationLocation == null)
        {
            await _dialogService.ShowWarningAsync("Validation Error", "Please select source and destination locations.");
            IsTransferSuccessful = false;
            return false;
        }

        if (Quantity <= 0)
        {
            await _dialogService.ShowWarningAsync("Validation Error", "Quantity must be greater than zero.");
            IsTransferSuccessful = false;
            return false;
        }

        if (SourceLocation.Id == DestinationLocation.Id)
        {
            await _dialogService.ShowWarningAsync("Validation Error", "Source and destination locations cannot be the same.");
            IsTransferSuccessful = false;
            return false;
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
            
            await _dialogService.ShowMessageAsync("Success", "Stock transfer completed successfully.");
            IsTransferSuccessful = true;
            return true;
        }
        catch (System.Exception ex)
        {
            await _dialogService.ShowErrorAsync("Transfer Failed", $"Error transferring stock: {ex.Message}");
            IsTransferSuccessful = false;
            return false;
        }
    }
}

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Core.ViewModels.Stock;

public partial class StockListViewModel : ObservableObject
{
    private readonly IStockService _stockService;
    private readonly ILocationService _locationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<ProductStock> _stocks = new();

    [ObservableProperty]
    private ObservableCollection<Location> _locations = new();

    [ObservableProperty]
    private Location? _selectedLocation;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _showLowStockOnly;

    public StockListViewModel(
        IStockService stockService,
        ILocationService locationService,
        IDialogService dialogService)
    {
        _stockService = stockService;
        _locationService = locationService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
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

            if (SelectedLocation == null && Locations.Count > 0)
            {
                SelectedLocation = Locations[0];
            }

            await LoadStocksAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task LoadStocksAsync()
    {
        if (SelectedLocation == null) return;

        IsLoading = true;
        try
        {
            IEnumerable<ProductStock> result;
            if (ShowLowStockOnly)
            {
                result = await _stockService.GetLowStockProductsAsync(SelectedLocation.Id);
            }
            else
            {
                result = await _stockService.GetStockByLocationAsync(SelectedLocation.Id);
            }

            Stocks.Clear();
            foreach (var stock in result)
            {
                Stocks.Add(stock);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task AdjustStockAsync(ProductStock? stock)
    {
        if (stock == null) return;
        await _dialogService.ShowStockAdjustmentDialogAsync(stock);
        // Refresh after adjustment
        await LoadStocksAsync();
    }

    [RelayCommand]
    public async Task TransferStockAsync()
    {
        await _dialogService.ShowStockTransferDialogAsync();
        // Refresh after transfer
        await LoadStocksAsync();
    }

    [RelayCommand]
    public async Task ViewHistoryAsync(ProductStock? stock)
    {
        if (stock == null) return;
        await _dialogService.ShowStockHistoryDialogAsync(stock.ProductId);
    }

    [RelayCommand]
    public async Task ManageReorderLevelsAsync()
    {
        await _dialogService.ShowReorderLevelDialogAsync(SelectedLocation?.Id);
        // Refresh after changes
        await LoadStocksAsync();
    }

    partial void OnSelectedLocationChanged(Location? value)
    {
        if (value != null)
        {
            _ = LoadStocksAsync();
        }
    }

    partial void OnShowLowStockOnlyChanged(bool value)
    {
        _ = LoadStocksAsync();
    }
}

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
    private readonly ISessionService _sessionService;

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

    [ObservableProperty]
    private ObservableCollection<StockTake> _stockTakes = new();

    [ObservableProperty]
    private bool _isStockTakesTabSelected;

    public StockListViewModel(
        IStockService stockService,
        ILocationService locationService,
        IDialogService dialogService,
        ISessionService sessionService)
    {
        _stockService = stockService;
        _locationService = locationService;
        _dialogService = dialogService;
        _sessionService = sessionService;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
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

            if (SelectedLocation == null && Locations.Count > 0)
            {
                SelectedLocation = Locations[0];
            }

            if (IsStockTakesTabSelected)
            {
                await LoadStockTakesAsync();
            }
            else
            {
                await LoadStocksAsync();
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Error", $"Failed to load data: {ex.Message}");
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
    public async Task LoadStockTakesAsync()
    {
        if (SelectedLocation == null) return;

        IsLoading = true;
        try
        {
            var result = await _stockService.GetStockTakesAsync(SelectedLocation.Id);
            StockTakes.Clear();
            foreach (var st in result)
            {
                StockTakes.Add(st);
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

    [RelayCommand]
    public async Task NewStockTakeAsync()
    {
        await _dialogService.ShowStockTakeDialogAsync(null, SelectedLocation?.Id);
        await LoadStockTakesAsync();
    }

    [RelayCommand]
    public async Task OpenStockTakeAsync(StockTake? stockTake)
    {
        if (stockTake == null) return;
        await _dialogService.ShowStockTakeDialogAsync(stockTake.Id);
        await LoadStockTakesAsync();
    }

    partial void OnSelectedLocationChanged(Location? value)
    {
        if (value != null)
        {
            if (IsStockTakesTabSelected)
            {
                _ = LoadStockTakesAsync();
            }
            else
            {
                _ = LoadStocksAsync();
            }
        }
    }

    partial void OnShowLowStockOnlyChanged(bool value)
    {
        if (!IsStockTakesTabSelected)
        {
            _ = LoadStocksAsync();
        }
    }

    partial void OnIsStockTakesTabSelectedChanged(bool value)
    {
        if (value)
        {
            _ = LoadStockTakesAsync();
        }
        else
        {
            _ = LoadStocksAsync();
        }
    }
}

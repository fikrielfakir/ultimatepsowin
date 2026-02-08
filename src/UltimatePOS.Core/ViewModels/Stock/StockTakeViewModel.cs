using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Core.ViewModels.Stock;

public partial class StockTakeViewModel : ObservableObject
{
    private readonly IStockService _stockService;
    private readonly ILocationService _locationService;
    private readonly ISessionService _sessionService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private StockTake? _stockTake;

    [ObservableProperty]
    private ObservableCollection<StockTakeDetailViewModel> _details = new();

    [ObservableProperty]
    private ObservableCollection<Location> _locations = new();

    [ObservableProperty]
    private Location? _selectedLocation;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isReadOnly;

    [ObservableProperty]
    private bool _isCompleteSuccessful;

    public StockTakeViewModel(
        IStockService stockService,
        ILocationService locationService,
        ISessionService sessionService,
        IDialogService dialogService)
    {
        _stockService = stockService;
        _locationService = locationService;
        _sessionService = sessionService;
        _dialogService = dialogService;
    }

    public async Task InitializeAsync(int? stockTakeId = null, int? locationId = null)
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

            if (stockTakeId.HasValue)
            {
                // Load existing stock take
                await LoadStockTakeAsync(stockTakeId.Value);
                IsReadOnly = StockTake?.Status != StockTakeStatus.InProgress;
            }
            else
            {
                // Create new stock take
                if (locationId.HasValue)
                {
                    SelectedLocation = Locations.FirstOrDefault(l => l.Id == locationId.Value);
                }
                else if (Locations.Count > 0)
                {
                    SelectedLocation = Locations[0];
                }

                if (SelectedLocation != null)
                {
                    await CreateNewStockTakeAsync();
                }
            }
        }
        catch (Exception ex)
        {
             await _dialogService.ShowErrorAsync("Initialization Error", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task CreateNewStockTakeAsync()
    {
        if (SelectedLocation == null) return;

        IsLoading = true;
        try
        {
            StockTake = await _stockService.CreateStockTakeAsync(SelectedLocation.Id);
            await LoadDetailsAsync();
            IsReadOnly = false;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Error", $"Failed to create stock take: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadStockTakeAsync(int stockTakeId)
    {
        StockTake = await _stockService.GetStockTakeByIdAsync(stockTakeId);
        if (StockTake != null)
        {
            SelectedLocation = Locations.FirstOrDefault(l => l.Id == StockTake.LocationId);
            await LoadDetailsAsync();
        }
        else
        {
             await _dialogService.ShowErrorAsync("Error", "Stock take not found.");
        }
    }

    private async Task LoadDetailsAsync()
    {
        if (StockTake == null) return;

        Details.Clear();
        
        // Reload from database to get latest
        var refreshed = await _stockService.GetStockTakeByIdAsync(StockTake.Id);
        if (refreshed?.Details != null)
        {
            foreach (var detail in refreshed.Details)
            {
                Details.Add(new StockTakeDetailViewModel
                {
                    DetailId = detail.Id,
                    ProductId = detail.ProductId,
                    ProductName = detail.Product?.Name ?? "Unknown",
                    ProductSKU = detail.Product?.SKU ?? "",
                    ExpectedQuantity = detail.ExpectedQuantity,
                    CountedQuantity = detail.CountedQuantity,
                    Variance = detail.Variance,
                    Notes = detail.Notes
                });
            }
        }
    }

    [RelayCommand]
    public async Task UpdateCountAsync(StockTakeDetailViewModel detail)
    {
        if (StockTake == null || IsReadOnly) return;

        try
        {
            // Update local VM first for responsiveness
            detail.Variance = detail.CountedQuantity - detail.ExpectedQuantity;
            
            await _stockService.UpdateStockTakeDetailAsync(
                StockTake.Id,
                detail.ProductId,
                detail.CountedQuantity,
                detail.Notes);
        }
        catch (Exception ex)
        {
            // Log or show unobtrusive error
             System.Diagnostics.Debug.WriteLine($"Error updating stock take detail: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task<bool> CompleteStockTakeAsync(bool autoAdjust)
    {
        if (StockTake == null || IsReadOnly) return false;

        var confirmed = await _dialogService.ShowConfirmationAsync(
            "Complete Stock Take", 
            "Are you sure you want to complete this stock take? This action cannot be undone." + 
            (autoAdjust ? "\n\nStock levels will be adjusted to match counted quantities." : ""));

        if (!confirmed) return false;

        IsLoading = true;
        try
        {
            var updatedStockTake = await _stockService.CompleteStockTakeAsync(StockTake.Id, autoAdjust);
            StockTake = updatedStockTake;
            IsReadOnly = true;
            IsCompleteSuccessful = true;
            
            await _dialogService.ShowMessageAsync("Success", "Stock take completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Completion Failed", $"Failed to complete stock take: {ex.Message}");
            IsCompleteSuccessful = false;
            return false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        if (StockTake != null)
        {
            await LoadStockTakeAsync(StockTake.Id);
        }
    }
}

// Helper class for UI binding
public partial class StockTakeDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private int _detailId;

    [ObservableProperty]
    private int _productId;

    [ObservableProperty]
    private string _productName = string.Empty;

    [ObservableProperty]
    private string _productSKU = string.Empty;

    [ObservableProperty]
    private decimal _expectedQuantity;

    [ObservableProperty]
    private decimal _countedQuantity;

    [ObservableProperty]
    private decimal _variance;

    [ObservableProperty]
    private string? _notes;
}

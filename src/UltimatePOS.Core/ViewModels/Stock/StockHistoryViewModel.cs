using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Core.ViewModels.Stock;

public partial class StockHistoryViewModel : ObservableObject
{
    private readonly IStockService _stockService;
    private readonly IProductService _productService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<StockHistory> _histories = new();

    [ObservableProperty]
    private Product? _selectedProduct;

    [ObservableProperty]
    private DateTime? _fromDate;

    [ObservableProperty]
    private DateTime? _toDate;

    [ObservableProperty]
    private bool _isLoading;

    public StockHistoryViewModel(
        IStockService stockService,
        IProductService productService,
        IDialogService dialogService)
    {
        _stockService = stockService;
        _productService = productService;
        _dialogService = dialogService;
        
        // Default to last 30 days
        _fromDate = DateTime.Now.AddDays(-30);
        _toDate = DateTime.Now;
    }

    public async Task InitializeAsync(int productId)
    {
        IsLoading = true;
        try
        {
            SelectedProduct = await _productService.GetProductByIdAsync(productId);
            await LoadHistoryAsync();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Initialization Error", $"Failed to load product: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task LoadHistoryAsync()
    {
        if (SelectedProduct == null) return;

        IsLoading = true;
        try
        {
            var result = await _stockService.GetStockHistoryAsync(
                SelectedProduct.Id,
                FromDate,
                ToDate);

            Histories.Clear();
            foreach (var history in result)
            {
                Histories.Add(history);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Error", $"Failed to load history: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnFromDateChanged(DateTime? value)
    {
        if (SelectedProduct != null)
        {
            _ = LoadHistoryAsync();
        }
    }

    partial void OnToDateChanged(DateTime? value)
    {
        if (SelectedProduct != null)
        {
            _ = LoadHistoryAsync();
        }
    }
}

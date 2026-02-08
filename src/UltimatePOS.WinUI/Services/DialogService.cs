using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using UltimatePOS.Core.Interfaces;
using UltimatePOS.WinUI.Dialogs;
using UltimatePOS.Core.Entities;

namespace UltimatePOS.WinUI.Services;

/// <summary>
/// Implementation of dialog service for WinUI 3
/// </summary>
public class DialogService : IDialogService
{
    private readonly Func<ContentDialog> _dialogFactory;

    public DialogService(Func<ContentDialog> dialogFactory)
    {
        _dialogFactory = dialogFactory;
    }

    public async Task ShowMessageAsync(string title, string message)
    {
        var dialog = _dialogFactory();
        dialog.XamlRoot = App.CurrentWindow?.Content?.XamlRoot;
        dialog.Title = title;
        dialog.Content = message;
        dialog.CloseButtonText = "OK";
        await dialog.ShowAsync();
    }

    public async Task ShowWarningAsync(string title, string message)
    {
        var dialog = _dialogFactory();
        dialog.XamlRoot = App.CurrentWindow?.Content?.XamlRoot;
        dialog.Title = "⚠️ " + title;
        dialog.Content = message;
        dialog.CloseButtonText = "OK";
        await dialog.ShowAsync();
    }

    public async Task ShowErrorAsync(string title, string message)
    {
        var dialog = _dialogFactory();
        dialog.XamlRoot = App.CurrentWindow?.Content?.XamlRoot;
        dialog.Title = "❌ " + title;
        dialog.Content = message;
        dialog.CloseButtonText = "OK";
        await dialog.ShowAsync();
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        return await ShowConfirmationAsync(title, message, "Yes", "No");
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message, string confirmText, string cancelText)
    {
        var dialog = _dialogFactory();
        dialog.XamlRoot = App.CurrentWindow?.Content?.XamlRoot;
        dialog.Title = title;
        dialog.Content = message;
        dialog.PrimaryButtonText = confirmText;
        dialog.CloseButtonText = cancelText;

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    public async Task<string?> ShowInputAsync(string title, string message, string defaultValue = "")
    {
        var textBox = new TextBox
        {
            Text = defaultValue,
            PlaceholderText = message
        };

        var dialog = _dialogFactory();
        dialog.XamlRoot = App.CurrentWindow?.Content?.XamlRoot;
        dialog.Title = title;
        dialog.Content = textBox;
        dialog.PrimaryButtonText = "OK";
        dialog.CloseButtonText = "Cancel";

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary ? textBox.Text : null;
    }

    public async Task<TResult?> ShowDialogAsync<TViewModel, TResult>(object? parameter = null) where TViewModel : class
    {
        throw new NotImplementedException("Custom dialog with ViewModel not yet implemented");
    }

    public async Task ShowBarcodePrintDialogAsync(int[] productIds)
    {
        var dialog = new BarcodePrintDialog();
        dialog.XamlRoot = App.CurrentWindow?.Content?.XamlRoot;
        await dialog.ViewModel.LoadProductsAsync(productIds);
        await dialog.ShowAsync();
    }

    public async Task ShowStockAdjustmentDialogAsync(ProductStock? stock)
    {
        var dialog = new StockAdjustmentDialog();
        dialog.XamlRoot = App.CurrentWindow?.Content?.XamlRoot;
        await dialog.ViewModel.InitializeAsync(stock);
        await dialog.ShowAsync();
    }

    public async Task ShowStockTransferDialogAsync(int? productId = null)
    {
        var dialog = new StockTransferDialog();
        dialog.XamlRoot = App.CurrentWindow?.Content?.XamlRoot;
        await dialog.ViewModel.InitializeAsync();
        
        if (productId.HasValue)
        {
            await dialog.ViewModel.SetProductAsync(productId.Value);
        }
        
        await dialog.ShowAsync();
    }

    public async Task ShowStockHistoryDialogAsync(int productId)
    {
        var dialog = new StockHistoryDialog();
        dialog.XamlRoot = App.CurrentWindow?.Content?.XamlRoot;
        await dialog.ViewModel.InitializeAsync(productId);
        await dialog.ShowAsync();
    }

    public async Task ShowReorderLevelDialogAsync(int? locationId = null)
    {
        var dialog = new ReorderLevelDialog();
        dialog.XamlRoot = App.CurrentWindow?.Content?.XamlRoot;
        await dialog.ViewModel.InitializeAsync(locationId);
        await dialog.ShowAsync();
    }

    public async Task ShowStockTakeDialogAsync(int? stockTakeId = null, int? locationId = null)
    {
        var dialog = new StockTakeDialog();
        dialog.XamlRoot = App.CurrentWindow?.Content?.XamlRoot;
        await dialog.ViewModel.InitializeAsync(stockTakeId, locationId);
        await dialog.ShowAsync();
    }
}

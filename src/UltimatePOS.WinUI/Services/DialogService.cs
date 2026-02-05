using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using UltimatePOS.Core.Interfaces;

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
        dialog.Title = title;
        dialog.Content = message;
        dialog.CloseButtonText = "OK";
        await dialog.ShowAsync();
    }

    public async Task ShowWarningAsync(string title, string message)
    {
        var dialog = _dialogFactory();
        dialog.Title = "⚠️ " + title;
        dialog.Content = message;
        dialog.CloseButtonText = "OK";
        await dialog.ShowAsync();
    }

    public async Task ShowErrorAsync(string title, string message)
    {
        var dialog = _dialogFactory();
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
        dialog.Title = title;
        dialog.Content = textBox;
        dialog.PrimaryButtonText = "OK";
        dialog.CloseButtonText = "Cancel";

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary ? textBox.Text : null;
    }

    public async Task<TResult?> ShowDialogAsync<TViewModel, TResult>(object? parameter = null) where TViewModel : class
    {
        // This would require a more complex implementation with custom dialogs
        // For now, we'll throw NotImplementedException
        // In a full implementation, you would:
        // 1. Resolve TViewModel from DI
        // 2. Find the corresponding View
        // 3. Create a ContentDialog with that View as content
        // 4. Show the dialog and return the result
        throw new NotImplementedException("Custom dialog with ViewModel not yet implemented");
    }
}

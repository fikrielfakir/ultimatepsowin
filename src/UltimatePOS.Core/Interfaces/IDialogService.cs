using System.Threading.Tasks;

namespace UltimatePOS.Core.Interfaces;

/// <summary>
/// Service for displaying dialogs and user interactions
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Show an information message
    /// </summary>
    Task ShowMessageAsync(string title, string message);

    /// <summary>
    /// Show a warning message
    /// </summary>
    Task ShowWarningAsync(string title, string message);

    /// <summary>
    /// Show an error message
    /// </summary>
    Task ShowErrorAsync(string title, string message);

    /// <summary>
    /// Show a confirmation dialog and return user's choice
    /// </summary>
    Task<bool> ShowConfirmationAsync(string title, string message);

    /// <summary>
    /// Show a confirmation dialog with custom button text
    /// </summary>
    Task<bool> ShowConfirmationAsync(string title, string message, string confirmText, string cancelText);

    /// <summary>
    /// Show an input dialog and return the entered text
    /// </summary>
    Task<string?> ShowInputAsync(string title, string message, string defaultValue = "");

    /// <summary>
    /// Show a custom dialog with a ViewModel
    /// </summary>
    Task<TResult?> ShowDialogAsync<TViewModel, TResult>(object? parameter = null) where TViewModel : class;
}

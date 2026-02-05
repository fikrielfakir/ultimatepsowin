using System;

namespace UltimatePOS.Core.Interfaces;

/// <summary>
/// Service for managing navigation between views
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigate to a view by ViewModel type
    /// </summary>
    void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class;

    /// <summary>
    /// Navigate to a view by ViewModel type name
    /// </summary>
    void NavigateTo(string viewModelName, object? parameter = null);

    /// <summary>
    /// Navigate back to the previous view
    /// </summary>
    bool GoBack();

    /// <summary>
    /// Check if navigation back is possible
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Clear navigation history
    /// </summary>
    void ClearHistory();

    /// <summary>
    /// Event raised when navigation occurs
    /// </summary>
    event EventHandler<NavigationEventArgs>? Navigated;
}

/// <summary>
/// Event arguments for navigation events
/// </summary>
public class NavigationEventArgs : EventArgs
{
    public string? ViewModelName { get; set; }
    public object? Parameter { get; set; }
}

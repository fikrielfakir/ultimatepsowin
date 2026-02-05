using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.WinUI.Services;

/// <summary>
/// Implementation of navigation service for WinUI 3
/// </summary>
public class NavigationService : INavigationService
{
    private readonly Dictionary<string, Type> _viewMapping = new();
    private Frame? _frame;

    public event EventHandler<NavigationEventArgs>? Navigated;

    public bool CanGoBack => _frame?.CanGoBack ?? false;

    /// <summary>
    /// Initialize the navigation service with a frame
    /// </summary>
    public void Initialize(Frame frame)
    {
        _frame = frame;
        _frame.Navigated += (s, e) =>
        {
            Navigated?.Invoke(this, new NavigationEventArgs
            {
                ViewModelName = e.SourcePageType.Name,
                Parameter = e.Parameter
            });
        };
    }

    /// <summary>
    /// Register a view type with its corresponding ViewModel type
    /// </summary>
    public void RegisterView<TView, TViewModel>()
        where TView : Page
        where TViewModel : class
    {
        var viewModelName = typeof(TViewModel).Name;
        _viewMapping[viewModelName] = typeof(TView);
    }

    public void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class
    {
        var viewModelName = typeof(TViewModel).Name;
        NavigateTo(viewModelName, parameter);
    }

    public void NavigateTo(string viewModelName, object? parameter = null)
    {
        if (_frame == null)
        {
            throw new InvalidOperationException("Navigation service not initialized. Call Initialize() first.");
        }

        if (!_viewMapping.TryGetValue(viewModelName, out var viewType))
        {
            throw new ArgumentException($"No view registered for ViewModel: {viewModelName}");
        }

        _frame.Navigate(viewType, parameter);
    }

    public bool GoBack()
    {
        if (_frame?.CanGoBack == true)
        {
            _frame.GoBack();
            return true;
        }
        return false;
    }

    public void ClearHistory()
    {
        if (_frame != null)
        {
            _frame.BackStack.Clear();
        }
    }
}

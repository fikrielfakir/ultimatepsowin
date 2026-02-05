using CommunityToolkit.Mvvm.ComponentModel;

namespace UltimatePOS.Core.ViewModels;

/// <summary>
/// Base ViewModel class inheriting from ObservableObject for MVVM Toolkit support
/// </summary>
public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;
}

using Microsoft.UI.Xaml.Controls;
using UltimatePOS.Core.ViewModels.Stock;
using Microsoft.Extensions.DependencyInjection;

namespace UltimatePOS.WinUI.Dialogs;

public sealed partial class ReorderLevelDialog : ContentDialog
{
    public ReorderLevelViewModel ViewModel { get; }

    public ReorderLevelDialog()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<ReorderLevelViewModel>();
    }

    private void OnAlertQuantityChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        // Mark as changed when any alert quantity is modified
        ViewModel.MarkAsChangedCommand.Execute(null);
    }
}

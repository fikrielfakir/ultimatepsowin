using Microsoft.UI.Xaml.Controls;
using UltimatePOS.Core.ViewModels.Stock;
using UltimatePOS.Core.Models;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace UltimatePOS.WinUI.Dialogs;

public sealed partial class StockAdjustmentDialog : ContentDialog
{
    public StockAdjustmentViewModel ViewModel { get; }
    public StockAdjustmentType[] AvailableTypes { get; } = Enum.GetValues<StockAdjustmentType>();

    public StockAdjustmentDialog()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<StockAdjustmentViewModel>();
        this.PrimaryButtonClick += StockAdjustmentDialog_PrimaryButtonClick;
    }

    private async void StockAdjustmentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Defer close to allow async operation
        var deferral = args.GetDeferral();
        
        var success = await ViewModel.SaveAsync();
        
        // Cancel the close if save failed
        if (!success)
        {
            args.Cancel = true;
        }
        
        deferral.Complete();
    }
}

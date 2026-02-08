using Microsoft.UI.Xaml.Controls;
using UltimatePOS.Core.ViewModels.Stock;
using Microsoft.Extensions.DependencyInjection;

namespace UltimatePOS.WinUI.Dialogs;

public sealed partial class StockTransferDialog : ContentDialog
{
    public StockTransferViewModel ViewModel { get; }

    public StockTransferDialog()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<StockTransferViewModel>();
        this.PrimaryButtonClick += StockTransferDialog_PrimaryButtonClick;
    }

    private async void StockTransferDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Defer close to allow async operation
        var deferral = args.GetDeferral();
        
        var success = await ViewModel.TransferAsync();
        
        // Cancel the close if transfer failed
        if (!success)
        {
            args.Cancel = true;
        }
        
        deferral.Complete();
    }
}

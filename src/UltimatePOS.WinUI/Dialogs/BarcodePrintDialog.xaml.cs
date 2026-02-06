using Microsoft.UI.Xaml.Controls;
using UltimatePOS.Core.ViewModels.Product;

namespace UltimatePOS.WinUI.Dialogs;

public sealed partial class BarcodePrintDialog : ContentDialog
{
    public BarcodePrintViewModel ViewModel { get; }

    public BarcodePrintDialog(BarcodePrintViewModel viewModel)
    {
        ViewModel = viewModel;
        this.InitializeComponent();

        this.PrimaryButtonClick += BarcodePrintDialog_PrimaryButtonClick;
    }

    private async void BarcodePrintDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Prevent dialog from closing immediately to allow async print
        var deferral = args.GetDeferral();
        args.Cancel = true; // Handle closing manually after print or error
        
        await ViewModel.PrintCommand.ExecuteAsync(null);
        
        deferral.Complete();
        this.Hide(); // Close dialog after print
    }
}

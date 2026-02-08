using Microsoft.UI.Xaml.Controls;
using UltimatePOS.Core.ViewModels.Stock;
using UltimatePOS.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace UltimatePOS.WinUI.Dialogs;

public sealed partial class StockTakeDialog : ContentDialog
{
    public StockTakeViewModel ViewModel { get; }

    public StockTakeDialog()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<StockTakeViewModel>();
        this.PrimaryButtonClick += StockTakeDialog_PrimaryButtonClick;
        this.SecondaryButtonClick += StockTakeDialog_SecondaryButtonClick;
        this.Loaded += StockTakeDialog_Loaded;
    }

    private void StockTakeDialog_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Update status badge based on stock take status
        UpdateStatusBadge();
        
        // Hide primary button if read-only
        if (ViewModel.IsReadOnly)
        {
            this.IsPrimaryButtonEnabled = false;
            this.IsSecondaryButtonEnabled = false;
        }
    }

    private void UpdateStatusBadge()
    {
        if (ViewModel.StockTake == null) return;

        switch (ViewModel.StockTake.Status)
        {
            case StockTakeStatus.InProgress:
                StatusBadge.Value = "In Progress";
                break;
            case StockTakeStatus.Completed:
                StatusBadge.Value = "Completed";
                break;
            case StockTakeStatus.Cancelled:
                StatusBadge.Value = "Cancelled";
                break;
        }
    }

    private async void StockTakeDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Complete stock take
        var deferral = args.GetDeferral();
        
        bool autoAdjust = AutoAdjustCheckBox.IsChecked ?? false;
        var success = await ViewModel.CompleteStockTakeAsync(autoAdjust);
        
        if (!success)
        {
            args.Cancel = true;
        }
        else
        {
            // Update UI after completion
            UpdateStatusBadge();
        }
        
        deferral.Complete();
    }

    private async void StockTakeDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Save progress without completing
        args.Cancel = true; // Don't close dialog
        
        // Just refresh to ensure all changes are saved
        await ViewModel.RefreshAsync();
    }

    private async void OnCountedQuantityChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        // Find the detail this NumberBox belongs to
        var detail = sender.DataContext as StockTakeDetailViewModel;
        if (detail != null)
        {
            await ViewModel.UpdateCountAsync(detail);
        }
    }
}

using Microsoft.UI.Xaml.Controls;
using UltimatePOS.Core.ViewModels.Stock;
using Microsoft.Extensions.DependencyInjection;

namespace UltimatePOS.WinUI.Dialogs;

public sealed partial class StockHistoryDialog : ContentDialog
{
    public StockHistoryViewModel ViewModel { get; }

    public StockHistoryDialog()
    {
        this.InitializeComponent();
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetService<StockHistoryViewModel>();
    }
}

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using UltimatePOS.Core.ViewModels.Stock;

namespace UltimatePOS.WinUI.Views.Stock;

public sealed partial class StockListPage : Page
{
    public StockListViewModel ViewModel { get; }

    public StockListPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<StockListViewModel>();
        this.Name = "PageRoot";
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _ = ViewModel.LoadDataCommand.ExecuteAsync(null);
    }
}

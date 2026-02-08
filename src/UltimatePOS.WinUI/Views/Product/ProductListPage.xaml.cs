using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UltimatePOS.Core.ViewModels.Products;
using UltimatePOS.WinUI.Helpers;

namespace UltimatePOS.WinUI.Views.Product;

public sealed partial class ProductListPage : Page
{
    public ProductListViewModel ViewModel { get; }

    public ProductListPage()
    {
        ViewModel = App.GetService<ProductListViewModel>();
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadFiltersAsync();
        await ViewModel.LoadProductsAsync();
    }
}

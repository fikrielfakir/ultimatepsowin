using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UltimatePOS.WinUI.Views;
using UltimatePOS.Core.ViewModels;

namespace UltimatePOS.WinUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        Title = "UltimatePOS - Point of Sale System";
        
        // Configure NavigationService with this frame (AppFrame is defined in XAML)
        var navService = App.GetService<UltimatePOS.Core.Interfaces.INavigationService>();
        if (navService is UltimatePOS.WinUI.Services.NavigationService winNavService)
        {
            winNavService.Initialize(AppFrame);
            
            // Register views
            winNavService.RegisterView<LoginPage, LoginViewModel>();
            winNavService.RegisterView<Views.Business.BusinessListPage, Core.ViewModels.Business.BusinessListViewModel>();
            winNavService.RegisterView<Views.Business.LocationListPage, Core.ViewModels.Business.LocationListViewModel>();
            winNavService.RegisterView<Views.DashboardPage, Core.ViewModels.DashboardViewModel>();
            winNavService.RegisterView<Views.Product.ProductListPage, Core.ViewModels.Product.ProductListViewModel>();
            winNavService.RegisterView<Views.Stock.StockListPage, Core.ViewModels.Stock.StockListViewModel>();
        }

        // Navigate to Login Page
        AppFrame.Navigate(typeof(LoginPage));

        // Set window size
        var appWindow = this.AppWindow;
        if (appWindow != null)
        {
            var size = new Windows.Graphics.SizeInt32(1280, 800);
            appWindow.Resize(size);
        }
    }
}

using Microsoft.UI.Xaml;

namespace UltimatePOS.WinUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        Title = "UltimatePOS - Point of Sale System";
        
        // Initialize simple Frame for navigation
        var frame = new Frame();
        Content = frame;

        // Configure NavigationService with this frame
        var navService = App.GetService<UltimatePOS.Core.Interfaces.INavigationService>();
        if (navService is UltimatePOS.WinUI.Services.NavigationService winNavService)
        {
            winNavService.Initialize(frame);
            
            // Register views
            winNavService.RegisterView<Views.LoginPage, Core.ViewModels.LoginViewModel>();
        }

        // Navigate to Login Page
        frame.Navigate(typeof(Views.LoginPage));

        // Set window size
        var appWindow = this.AppWindow;
        if (appWindow != null)
        {
            var size = new Windows.Graphics.SizeInt32(1280, 800);
            appWindow.Resize(size);
        }
    }
}

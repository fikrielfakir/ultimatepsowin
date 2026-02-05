using Microsoft.UI.Xaml;

namespace UltimatePOS.WinUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        Title = "UltimatePOS - Point of Sale System";
        
        // Set window size
        var appWindow = this.AppWindow;
        if (appWindow != null)
        {
            var size = new Windows.Graphics.SizeInt32(1280, 800);
            appWindow.Resize(size);
        }
    }
}

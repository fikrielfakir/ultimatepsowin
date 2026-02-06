using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UltimatePOS.Core.ViewModels.Product;
using UltimatePOS.WinUI.Helpers;
using Windows.Storage.Pickers;
using System.IO;

namespace UltimatePOS.WinUI.Views.Product;

public sealed partial class ProductFormPage : Page
{
    public ProductFormViewModel ViewModel { get; }

    public ProductFormPage()
    {
        ViewModel = App.GetService<ProductFormViewModel>();
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        // If product ID is passed, load the product for editing
        if (e.Parameter is int productId)
        {
            await ViewModel.LoadProductCommand.ExecuteAsync(productId);
        }
    }

    public async void SelectImage_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");

            // Get the current window handle for the picker
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using var stream = await file.OpenStreamForReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var imageData = memoryStream.ToArray();

                // Update ViewModel
                ViewModel.SetImage(imageData, file.Path);
            }
        }
        catch (System.Exception ex)
        {
            // Show error dialog
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = $"Failed to select image: {ex.Message}",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}

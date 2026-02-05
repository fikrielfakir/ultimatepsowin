using Microsoft.UI.Xaml.Controls;
using UltimatePOS.Core.ViewModels;

namespace UltimatePOS.WinUI.Views;

public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel { get; }

    public LoginPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<LoginViewModel>();
    }
}

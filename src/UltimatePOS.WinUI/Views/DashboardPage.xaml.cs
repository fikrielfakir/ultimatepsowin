using Microsoft.UI.Xaml.Controls;
using UltimatePOS.Core.ViewModels;

namespace UltimatePOS.WinUI.Views;

public sealed partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel { get; }

    public DashboardPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<DashboardViewModel>();
    }
}

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UltimatePOS.Core.ViewModels.Business;

namespace UltimatePOS.WinUI.Views.Business;

public sealed partial class BusinessListPage : Page
{
    public BusinessListViewModel ViewModel { get; }

    public BusinessListPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<BusinessListViewModel>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _ = ViewModel.LoadBusinessesAsync();
    }
}

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UltimatePOS.Core.ViewModels.Business;

namespace UltimatePOS.WinUI.Views.Business;

public sealed partial class LocationListPage : Page
{
    public LocationListViewModel ViewModel { get; }

    public LocationListPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<LocationListViewModel>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // ViewModel loads automatically on BusinessChanged, but we can force refresh if needed
        _ = ViewModel.LoadLocationsAsync();
    }
}

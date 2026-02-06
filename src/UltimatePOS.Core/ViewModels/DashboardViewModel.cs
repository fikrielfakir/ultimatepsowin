using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Core.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IAuthenticationService _authenticationService;

    [ObservableProperty]
    private string _welcomeMessage = "Welcome to UltimatePOS";

    public DashboardViewModel(
        INavigationService navigationService,
        IAuthenticationService authenticationService)
    {
        _navigationService = navigationService;
        _authenticationService = authenticationService;
        
        if (_authenticationService.CurrentUser != null)
        {
            WelcomeMessage = $"Welcome, {_authenticationService.CurrentUser.Username}";
        }
    }

    [RelayCommand]
    private void NavigateToBusinesses()
    {
        _navigationService.NavigateTo(typeof(Business.BusinessListViewModel).FullName!);
    }

    [RelayCommand]
    private void NavigateToLocations()
    {
        _navigationService.NavigateTo(typeof(Business.LocationListViewModel).Name);
    }

    [RelayCommand]
    private void NavigateToProducts()
    {
        _navigationService.NavigateTo(typeof(Product.ProductListViewModel).Name);
    }

    [RelayCommand]
    private void NavigateToStock()
    {
        _navigationService.NavigateTo(typeof(Stock.StockListViewModel).Name);
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task LogoutAsync()
    {
        await _authenticationService.LogoutAsync();
        _navigationService.NavigateTo(typeof(LoginViewModel).FullName!);
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Core.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    public LoginViewModel(
        IAuthenticationService authenticationService,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _authenticationService = authenticationService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        
        Title = "Login";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter both username and password.";
            HasError = true;
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            HasError = false;

            var success = await _authenticationService.LoginAsync(Username, Password);

            if (success)
            {
                // Navigate to main shell/dashboard
                // We'll define the navigation key for Dashboard later, assuming "Dashboard" for now
                _navigationService.NavigateTo("Dashboard");
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
                HasError = true;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}

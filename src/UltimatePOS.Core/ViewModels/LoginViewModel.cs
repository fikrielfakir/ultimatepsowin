using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Interfaces;
using System.Threading.Tasks;
using System;

namespace UltimatePOS.Core.ViewModels
{
    public partial class LoginViewModel : ObservableObject
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

        [ObservableProperty]
        private bool _isBusy;

        public string Title { get; set; }

        public LoginViewModel(
            IAuthenticationService authenticationService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            
            Title = "Ultimate POS - Login";
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
                    _navigationService.NavigateTo<DashboardViewModel>();
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

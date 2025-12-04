using SoccerLink.Helpers;
using SoccerLink.Services;
using SoccerLink.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels.Auth
{
    public class LoginViewModel : BaseViewModel
    {
        private string _email;
        private string _password; // Przechowujemy hasło w VM (LoginService wymaga stringa)
        private string _errorMessage;
        private bool _isErrorVisible;

        // Zdarzenia nawigacyjne (View je subskrybuje)
        public event EventHandler RequestNavigateToDashboard;
        public event EventHandler RequestNavigateToRegister;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login, CanLogin);
            GoToRegisterCommand = new RelayCommand(GoToRegister);
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetProperty(ref _errorMessage, value))
                {
                    IsErrorVisible = !string.IsNullOrEmpty(value);
                }
            }
        }

        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set => SetProperty(ref _isErrorVisible, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
        }

        private async void Login()
        {
            ErrorMessage = string.Empty;

            try
            {
                var trener = await LoginService.LoginAsync(Email, Password);
                if (trener != null)
                {
                    SessionService.SetUser(trener);
                    RequestNavigateToDashboard?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    ErrorMessage = "Błędny email lub hasło.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Błąd logowania: {ex.Message}";
            }
        }

        private void GoToRegister()
        {
            RequestNavigateToRegister?.Invoke(this, EventArgs.Empty);
        }
    }
}

using SoccerLink.Helpers;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _email;
        private string _password;
        private string _passwordRepeat;
        private string _firstName;
        private string _lastName;
        private string _phoneNumber;
        private string _statusMessage;
        private string _statusColor = "Red"; // Domyślnie czerwony (błąd)

        public event EventHandler RequestNavigateToLogin;

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(Register, CanRegister);
            GoToLoginCommand = new RelayCommand(GoToLogin);
        }

        // Właściwości (Bindings)
        public string Email { get => _email; set { SetProperty(ref _email, value); UpdateCommandState(); } }
        public string Password { get => _password; set { SetProperty(ref _password, value); UpdateCommandState(); } }
        public string PasswordRepeat { get => _passwordRepeat; set { SetProperty(ref _passwordRepeat, value); UpdateCommandState(); } }
        public string FirstName { get => _firstName; set { SetProperty(ref _firstName, value); UpdateCommandState(); } }
        public string LastName { get => _lastName; set { SetProperty(ref _lastName, value); UpdateCommandState(); } }
        public string PhoneNumber { get => _phoneNumber; set { SetProperty(ref _phoneNumber, value); UpdateCommandState(); } }

        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
        public string StatusColor { get => _statusColor; set => SetProperty(ref _statusColor, value); }

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        private void UpdateCommandState() => (RegisterCommand as RelayCommand)?.RaiseCanExecuteChanged();

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(PasswordRepeat) &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber);
        }

        private async void Register()
        {
            StatusMessage = "";
            StatusColor = "Red";

            if (Password != PasswordRepeat)
            {
                StatusMessage = "Hasła nie są takie same.";
                return;
            }

            try
            {
                bool result = await RegisterService.RegisterAsync(Email, Password, FirstName, LastName, PhoneNumber);

                if (result)
                {
                    StatusColor = "Green";
                    StatusMessage = "Konto utworzone! Przekierowanie...";
                    await System.Threading.Tasks.Task.Delay(1500);
                    GoToLogin();
                }
                else
                {
                    StatusMessage = "Użytkownik z takim email lub telefonem już istnieje.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd połączenia: {ex.Message}";
            }
        }

        private void GoToLogin()
        {
            RequestNavigateToLogin?.Invoke(this, EventArgs.Empty);
        }
    }
}

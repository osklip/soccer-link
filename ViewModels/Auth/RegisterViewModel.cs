using SoccerLink.Helpers;
using SoccerLink.Services;
using System;
using System.Text.RegularExpressions; 
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels.Auth
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
        private string _statusColor = "Red";

        public event EventHandler RequestNavigateToLogin;

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(Register, CanRegister);
            GoToLoginCommand = new RelayCommand(GoToLogin);
        }

        
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

            
            if (!Regex.IsMatch(Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                StatusMessage = "Niepoprawny format adresu email.";
                return;
            }

            
            var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
            if (!passwordRegex.IsMatch(Password))
            {
                StatusMessage = "Hasło musi mieć min. 8 znaków, zawierać dużą i małą literę, cyfrę oraz znak specjalny.";
                return;
            }

            if (Password != PasswordRepeat)
            {
                StatusMessage = "Hasła nie są takie same.";
                return;
            }

            
            if (Regex.IsMatch(FirstName, @"\d") || Regex.IsMatch(LastName, @"\d"))
            {
                StatusMessage = "Imię i Nazwisko nie mogą zawierać cyfr.";
                return;
            }

            
            if (!Regex.IsMatch(PhoneNumber.Trim(), @"^\d{9}$"))
            {
                StatusMessage = "Numer telefonu musi składać się z dokładnie 9 cyfr.";
                return;
            }

            

            try
            {
                
                bool result = await RegisterService.RegisterAsync(Email, Password, FirstName, LastName, PhoneNumber);

                if (result)
                {
                    StatusColor = "Green";
                    StatusMessage = "Konto utworzone! Przekierowanie...";
                    await Task.Delay(1500);
                    GoToLogin();
                }
                else
                {
                    
                    StatusMessage = "Użytkownik z takim adresem email lub numerem telefonu już istnieje.";
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
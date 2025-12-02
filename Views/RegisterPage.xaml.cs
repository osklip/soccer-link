using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SoccerLink.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "";

            var email = EmailTextBox.Text?.Trim();
            var password = PasswordBox.Password?.Trim();
            var passwordRepeat = PasswordConfirmationBox.Password?.Trim();
            var firstName = NameTextBox.Text?.Trim();
            var lastName = SurnameTextBox.Text?.Trim();
            var phone = PhoneNumberTextBox.Text?.Trim();

            // sprawdzenie czy wszystkie dane podane
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(passwordRepeat) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(phone))
            {
                StatusTextBlock.Text = "Uzupe³nij wszystkie pola.";
                return;
            }

            if (password != passwordRepeat)
            {
                StatusTextBlock.Text = "Has³a nie s¹ takie same.";
                return;
            }

            // wywo³anie serwisu rejestracji
            bool result;

            try
            {
                result = await RegisterService.RegisterAsync(
                    email,
                    password,
                    firstName,
                    lastName,
                    phone);
            }
            catch
            {
                StatusTextBlock.Text = "B³¹d po³¹czenia z baz¹.";
                return;
            }

            // wynik
            if (!result)
            {
                StatusTextBlock.Text = "U¿ytkownik z takim email ju¿ istnieje.";
                return;
            }

            StatusTextBlock.Text = "Konto zosta³o utworzone. Mo¿esz siê zalogowaæ.";

            this.Frame.Navigate(typeof(LoginPage));
        }
    }
}

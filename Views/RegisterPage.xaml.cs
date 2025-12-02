using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Services;
using SoccerLink.ViewModels;
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
        public RegisterViewModel ViewModel { get; }

        public RegisterPage()
        {
            ViewModel = new RegisterViewModel();
            this.InitializeComponent();

            ViewModel.RequestNavigateToLogin += (s, e) => this.Frame.Navigate(typeof(LoginPage));
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null) ViewModel.Password = PasswordBox.Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null) ViewModel.PasswordRepeat = ConfirmPasswordBox.Password;
        }
    }
}

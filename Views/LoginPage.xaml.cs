using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Models;
using SoccerLink.ModelViews;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SoccerLink.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginViewModel ViewModel { get; }

        public LoginPage()
        {
            ViewModel = new LoginViewModel();
            this.InitializeComponent();

            // Subskrypcja zdarzeñ nawigacji z ViewModelu
            ViewModel.RequestNavigateToDashboard += (s, e) => this.Frame.Navigate(typeof(DashboardPage));
            ViewModel.RequestNavigateToRegister += (s, e) => this.Frame.Navigate(typeof(RegisterPage));
        }

        // Rêczne przekazanie has³a do ViewModelu (PasswordBox nie wspiera Bindingu)
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.Password = PasswordBox.Password;
            }
        }
    }
}

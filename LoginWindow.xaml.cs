using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Services;
using SoccerLink.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SoccerLink
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            /*  Debug.WriteLine("?? Testuję połączenie z bazą");
              await DatabaseConnection.TestConnectionAsync();
              Debug.WriteLine("? Zakończono test połączenia"); */

            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            var loginSuccess = await LoginService.LoginAsync(email, password);
            if (loginSuccess)
            {
                Debug.WriteLine("Logowanie powiodło się.");
                this.Content = new DashboardPage();
            }
            else
            {
                Debug.WriteLine("Logowanie nie powiodło się.");
            }
        }
    }
}

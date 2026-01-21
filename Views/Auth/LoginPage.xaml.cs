using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.ViewModels.Auth; 


namespace SoccerLink.Views
{
    
    public sealed partial class LoginPage : Page
    {
        public LoginViewModel ViewModel { get; }

        public LoginPage()
        {
            ViewModel = new LoginViewModel();
            this.InitializeComponent();

            
            ViewModel.RequestNavigateToDashboard += (s, e) => this.Frame.Navigate(typeof(DashboardPage));
            ViewModel.RequestNavigateToRegister += (s, e) => this.Frame.Navigate(typeof(RegisterPage));
        }

  
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                
                ViewModel.Password = PasswordBox.Password; 
            }
        }
    }
}
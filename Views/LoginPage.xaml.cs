// osklip/soccer-link/soccer-link-e0c06893844287e5a5f4d82194d873fa5c12266b/Views/LoginPage.xaml.cs

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.ModelViews; // U¿ywasz ModelViews zamiast ViewModels
// ... reszta usingów ...

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
                // To jest kluczowy wiersz, który ustawia has³o w ViewModelu
                ViewModel.Password = PasswordBox.Password; // PasswordBox to x:Name z XAML
            }
        }
    }
}
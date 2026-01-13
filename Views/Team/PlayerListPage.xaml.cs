using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.ViewModels.Team;

namespace SoccerLink.Views
{
    public sealed partial class PlayerListPage : Page
    {
        // Instancja ViewModelu przypisana do strony
        public PlayerListViewModel ViewModel { get; } = new PlayerListViewModel();

        public PlayerListPage()
        {
            this.InitializeComponent();
        }

        // £adowanie danych przy wejœciu na stronê
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadPlayersAsync();
        }

        // --- Obs³uga przycisku WSTECZ (bezpoœrednia nawigacja) ---
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null && this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        // --- Obs³uga przycisku WYMIEÑ (z wnêtrza listy) ---
        private async void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            // Pobieramy przycisk i element, do którego jest przypisany (DataContext)
            if (sender is Button btn && btn.DataContext is PlayerDisplayItem item)
            {
                // Wywo³ujemy logikê wymiany w ViewModelu dla konkretnego zawodnika
                await ViewModel.ReplacePlayerAsync(item.Source);
            }
        }
    }
}
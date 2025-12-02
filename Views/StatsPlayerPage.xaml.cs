using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;
using SoccerLink.Services; // Pamiêtaj o imporcie serwisów
using System;
using System.Linq;

namespace SoccerLink.Views
{
    public sealed partial class StatsPlayerPage : Page
    {
        public StatsPlayerPage()
        {
            InitializeComponent();
            this.Loaded += StatsPlayerPage_Loaded;
        }

        private async void StatsPlayerPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pobieramy prawdziwych zawodników z bazy
                var players = await ZawodnikService.PobierzZawodnikowDlaAktualnegoTreneraAsync();
                PlayersListView.ItemsSource = players;
            }
            catch (Exception ex)
            {
                // Opcjonalnie: obs³uga b³êdu
                System.Diagnostics.Debug.WriteLine($"B³¹d ³adowania graczy: {ex.Message}");
            }
        }

        private void PlayersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlayersListView.SelectedItem is Zawodnik selectedPlayer)
            {
                // Przekazujemy prawdziwy obiekt Zawodnika, który ma ID z bazy
                this.Frame.Navigate(typeof(PlayerStatsDetailsPage), selectedPlayer);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StatsNaviPage));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DashboardPage));
        }
    }
}
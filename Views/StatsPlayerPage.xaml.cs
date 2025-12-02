using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models; // Upewnij siê, ¿e masz ten namespace
using System.Collections.Generic;

namespace SoccerLink.Views
{
    public sealed partial class StatsPlayerPage : Page
    {
        public StatsPlayerPage()
        {
            InitializeComponent();
            LoadMockPlayers();
        }

        private void LoadMockPlayers()
        {
            // Tymczasowa lista "hardcoded", ¿ebyœ móg³ przetestowaæ nawigacjê
            var players = new List<Zawodnik>
            {
                new Zawodnik { Imie = "Jan", Nazwisko = "Kowalski", Pozycja = "Napastnik" },
                new Zawodnik { Imie = "Piotr", Nazwisko = "Nowak", Pozycja = "Bramkarz" },
                new Zawodnik { Imie = "Adam", Nazwisko = "Wiœniewski", Pozycja = "Obroñca" },
                new Zawodnik { Imie = "Robert", Nazwisko = "Lewandowski", Pozycja = "Napastnik" }
            };

            PlayersListView.ItemsSource = players;
        }

        private void PlayersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlayersListView.SelectedItem is Zawodnik selectedPlayer)
            {
                this.Content = new PlayerStatsDetailsPage(selectedPlayer);
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
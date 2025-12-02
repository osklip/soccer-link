using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;
using System.Collections.Generic;

namespace SoccerLink.Views
{
    public sealed partial class SelectMatchPage : Page
    {
        public SelectMatchPage()
        {
            InitializeComponent();
            LoadMatches();
        }

        private void LoadMatches()
        {
            // SYMULACJA DANYCH: Mecze bez statystyk
            var matches = new List<Mecz>
            {
                new Mecz { Przeciwnik = "FC Barcelona", DataRozpoczecia = new System.DateTime(2025, 12, 20), Miejsce = "Camp Nou" },
                new Mecz { Przeciwnik = "Real Madryt", DataRozpoczecia = new System.DateTime(2025, 12, 20), Miejsce = "Santiago Bernabéu" },
                new Mecz { Przeciwnik = "Bayern Monachium", DataRozpoczecia = new System.DateTime(2025, 12, 20), Miejsce = "Allianz Arena" }
            };

            MatchesListView.ItemsSource = matches;
        }

        private void MatchesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MatchesListView.SelectedItem is Mecz selectedMatch)
            {
                // Przechodzimy do HUB-a, przekazuj¹c wybrany mecz
                this.Frame.Navigate(typeof(AddStatsHubPage), selectedMatch);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StatsNaviPage));
        }
    }
}
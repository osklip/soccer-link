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
                new Mecz { Przeciwnik = "FC Barcelona", Data = "2025-11-20", Miejsce = "Camp Nou" },
                new Mecz { Przeciwnik = "Real Madryt", Data = "2025-11-15", Miejsce = "Santiago Bernabéu" },
                new Mecz { Przeciwnik = "Bayern Monachium", Data = "2025-11-10", Miejsce = "Allianz Arena" }
            };

            MatchesListView.ItemsSource = matches;
        }

        private void MatchesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MatchesListView.SelectedItem is Mecz selectedMatch)
            {
                // Przechodzimy do HUB-a, przekazuj¹c wybrany mecz
                this.Content = new AddStatsHubPage(selectedMatch);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new StatsNaviPage();
        }
    }
}
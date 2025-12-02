using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Models;
using SoccerLink.Services; // Dodaj ten namespace
using System;
using System.Collections.Generic;

namespace SoccerLink.Views
{
    public sealed partial class SelectMatchPage : Page
    {
        public SelectMatchPage()
        {
            this.InitializeComponent();
            this.Loaded += SelectMatchPage_Loaded;
        }

        private async void SelectMatchPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pobieramy prawdziwe mecze z bazy (te bez statystyk)
                var matches = await StatsService.GetMatchesWithoutStatsAsync();

                if (matches.Count == 0)
                {
                    // Opcjonalnie: Poka¿ komunikat, jeœli brak meczów
                    // np. TextBlockEmpty.Visibility = Visibility.Visible;
                }

                MatchesListView.ItemsSource = matches;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"B³¹d pobierania meczów: {ex.Message}");
            }
        }

        private void MatchesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MatchesListView.SelectedItem is Mecz selectedMatch)
            {
                // Przechodzimy do HUB-a, przekazuj¹c wybrany (prawdziwy) mecz z ID
                this.Frame.Navigate(typeof(AddStatsHubPage), selectedMatch);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StatsNaviPage));
        }
    }
}
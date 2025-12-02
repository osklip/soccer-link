using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;
using SoccerLink.ViewModels;
using System;

namespace SoccerLink.Views
{
    public sealed partial class SelectMatchPage : Page
    {
        public SelectMatchViewModel ViewModel { get; }

        public SelectMatchPage()
        {
            ViewModel = new SelectMatchViewModel();
            this.InitializeComponent();

            // Nawigacja
            ViewModel.RequestNavigateBack += (s, e) => this.Frame.Navigate(typeof(StatsNaviPage));
            ViewModel.RequestNavigateToStatsHub += (s, match) => this.Frame.Navigate(typeof(AddStatsHubPage), match);

            this.Loaded += SelectMatchPage_Loaded;
        }

        private async void SelectMatchPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadMatchesAsync();
        }

        private void MatchesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MatchesListView.SelectedItem is Mecz selectedMatch)
            {
                ViewModel.SelectMatch(selectedMatch);
                MatchesListView.SelectedItem = null; // Reset zaznaczenia
            }
        }
    }
}
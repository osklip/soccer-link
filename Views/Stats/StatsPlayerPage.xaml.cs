using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;
using SoccerLink.ViewModels.Stats;
using System;

namespace SoccerLink.Views
{
    public sealed partial class StatsPlayerPage : Page
    {
        public StatsPlayerViewModel ViewModel { get; }

        public StatsPlayerPage()
        {
            ViewModel = new StatsPlayerViewModel();
            this.InitializeComponent();

            ViewModel.RequestNavigateBack += (s, e) => this.Frame.Navigate(typeof(StatsNaviPage));
            ViewModel.RequestNavigateHome += (s, e) => this.Frame.Navigate(typeof(DashboardPage));
            ViewModel.RequestNavigateToDetails += (s, player) => this.Frame.Navigate(typeof(PlayerStatsDetailsPage), player);

            this.Loaded += StatsPlayerPage_Loaded;
        }

        private async void StatsPlayerPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadPlayersAsync();
        }

        private void PlayersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlayersListView.SelectedItem is Zawodnik selectedPlayer)
            {
                ViewModel.SelectPlayer(selectedPlayer);
                PlayersListView.SelectedItem = null; // Reset wyboru
            }
        }
    }
}
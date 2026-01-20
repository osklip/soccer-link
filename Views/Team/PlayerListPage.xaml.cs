using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.ViewModels.Team;
using System;

namespace SoccerLink.Views
{
    public sealed partial class PlayerListPage : Page
    {
        public PlayerListViewModel ViewModel { get; }

        public PlayerListPage()
        {
            ViewModel = new PlayerListViewModel();
            this.InitializeComponent();

            ViewModel.RequestNavigateBack += (s, e) => this.Frame.Navigate(typeof(TeamManagementPage));
            this.Loaded += PlayerListPage_Loaded;
        }

        private async void PlayerListPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadPlayersAsync();
        }
    }
}
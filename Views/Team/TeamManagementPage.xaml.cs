using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.ViewModels.Team;
using System;

namespace SoccerLink.Views
{
    public sealed partial class TeamManagementPage : Page
    {
        public TeamManagementViewModel ViewModel { get; }

        public TeamManagementPage()
        {
            ViewModel = new TeamManagementViewModel();
            this.InitializeComponent();

            ViewModel.RequestNavigateBack += (s, e) => this.Frame.Navigate(typeof(DashboardPage));
            ViewModel.RequestNavigateToPlayerList += (s, e) => this.Frame.Navigate(typeof(PlayerListPage));

            
            ViewModel.RequestNavigateToSquad += (s, e) => this.Frame.Navigate(typeof(SquadPage));
        }
    }
}
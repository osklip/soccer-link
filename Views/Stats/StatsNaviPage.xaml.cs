using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.ViewModels.Stats;
using System;

namespace SoccerLink.Views
{
    public sealed partial class StatsNaviPage : Page
    {
        public StatsNaviViewModel ViewModel { get; }

        public StatsNaviPage()
        {
            ViewModel = new StatsNaviViewModel();
            this.InitializeComponent();

            // Obs³uga nawigacji z ViewModelu
            ViewModel.RequestNavigateBack += (s, e) => this.Frame.Navigate(typeof(DashboardPage));
            ViewModel.RequestNavigateToSoloStats += (s, e) => this.Frame.Navigate(typeof(StatsPlayerPage));
            ViewModel.RequestNavigateToTeamStats += (s, e) => this.Frame.Navigate(typeof(StatsTeamPage));
            ViewModel.RequestNavigateToAddStats += (s, e) => this.Frame.Navigate(typeof(SelectMatchPage));
        }
    }
}
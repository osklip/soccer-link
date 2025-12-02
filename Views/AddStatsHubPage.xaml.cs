using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Models;
using SoccerLink.ViewModels;
using System;

namespace SoccerLink.Views
{
    public sealed partial class AddStatsHubPage : Page
    {
        public AddStatsHubViewModel ViewModel { get; }

        public AddStatsHubPage()
        {
            ViewModel = new AddStatsHubViewModel();
            this.InitializeComponent();

            ViewModel.RequestNavigateBack += (s, e) => this.Frame.Navigate(typeof(SelectMatchPage));
            ViewModel.RequestNavigateToTeamStats += (s, match) => this.Frame.Navigate(typeof(AddTeamStatsPage), match);
            ViewModel.RequestNavigateToPlayerStats += (s, match) => this.Frame.Navigate(typeof(AddPlayerStatsPage), match);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Mecz match)
            {
                ViewModel.Initialize(match);
            }
        }
    }
}
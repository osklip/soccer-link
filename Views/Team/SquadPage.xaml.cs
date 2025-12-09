using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.ViewModels.Team;
using System;

namespace SoccerLink.Views
{
    public sealed partial class SquadPage : Page
    {
        public SquadViewModel ViewModel { get; }

        public SquadPage()
        {
            ViewModel = new SquadViewModel();
            this.InitializeComponent();

            ViewModel.RequestNavigateBack += (s, e) => this.Frame.Navigate(typeof(TeamManagementPage));
            this.Loaded += SquadPage_Loaded;
        }

        private async void SquadPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
        }
    }
}
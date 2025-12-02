using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Models;

namespace SoccerLink.Views
{
    public sealed partial class AddStatsHubPage : Page
    {
        private Mecz _selectedMatch;

        public AddStatsHubPage() { InitializeComponent(); }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Mecz match)
            {
                _selectedMatch = match;
                MatchInfoTextBlock.Text = $"{_selectedMatch.Data} vs {_selectedMatch.Przeciwnik}";
            }  
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SelectMatchPage));
        }

        private void TeamStatsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddTeamStatsPage), _selectedMatch);
        }

        private void PlayerStatsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddPlayerStatsPage), _selectedMatch);
        }
    }
}
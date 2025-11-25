using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;

namespace SoccerLink.Views
{
    public sealed partial class AddStatsHubPage : Page
    {
        private Mecz _selectedMatch;

        public AddStatsHubPage() { InitializeComponent(); }

        internal AddStatsHubPage(Mecz match) : this()
        {
            _selectedMatch = match;
            if (_selectedMatch != null)
            {
                MatchInfoTextBlock.Text = $"{_selectedMatch.Data} vs {_selectedMatch.Przeciwnik}";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new SelectMatchPage();
        }

        private void TeamStatsButton_Click(object sender, RoutedEventArgs e)
        {
            // Przechodzimy do formularza, który w³aœnie stworzyliœmy
            this.Content = new AddTeamStatsPage(_selectedMatch);
        }

        private void PlayerStatsButton_Click(object sender, RoutedEventArgs e)
        {
            // Przejœcie do listy zawodników z licznikami
            this.Content = new AddPlayerStatsPage(_selectedMatch);
        }
    }
}
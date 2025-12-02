using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Models;
using System;

namespace SoccerLink.Views
{
    public sealed partial class AddTeamStatsPage : Page
    {
        private Mecz _selectedMatch;

        public AddTeamStatsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Mecz match)
            {
                _selectedMatch = match;
                MatchTitleText.Text = $"{_selectedMatch.Data} vs {_selectedMatch.Przeciwnik}";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Pobranie danych z pól (z prost¹ walidacj¹ czy to liczby)
            int.TryParse(GoalsBox.Text, out int goals);
            int.TryParse(PossessionBox.Text, out int possession);
            int.TryParse(ShotsBox.Text, out int shots);
            int.TryParse(ShotsOnTargetBox.Text, out int shotsOn);
            int.TryParse(ShotsOffTargetBox.Text, out int shotsOff);
            int.TryParse(CornersBox.Text, out int corners);
            int.TryParse(FoulsBox.Text, out int fouls);
            bool cleanSheet = CleanSheetSwitch.IsOn;

            // TODO: Tutaj w przysz³oœci bêdzie wywo³anie StatsService.SaveTeamStats(...)
            // Na razie symulacja sukcesu:

            ShowSuccessDialog();
        }

        private async void ShowSuccessDialog()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Sukces",
                Content = "Statystyki dru¿ynowe zosta³y zapisane (symulacja).",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();

            this.Frame.Navigate(typeof(AddStatsHubPage), _selectedMatch);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddStatsHubPage), _selectedMatch);
        }
    }
}
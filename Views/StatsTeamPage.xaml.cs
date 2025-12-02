using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.ViewModels;
using System;
using System.Globalization;

namespace SoccerLink.Views
{
    public sealed partial class StatsTeamPage : Page
    {
        // Publiczna w³aœciwoœæ ViewModelu, aby x:Bind w XAML mia³ do niej dostêp
        public StatsTeamViewModel ViewModel { get; }

        private DispatcherTimer _timer;

        public StatsTeamPage()
        {
            ViewModel = new StatsTeamViewModel();

            this.InitializeComponent();

        private void InitializeSeasons()
        {
            SeasonComboBox.Items.Add("Season 24/25");
            SeasonComboBox.Items.Add("Season 23/24");
            SeasonComboBox.Items.Add("Season 22/23");

            // 3. Uruchomienie zegara
            StartClock();
        }

        private void StartClock()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => UpdateTime();
            _timer.Start();
            UpdateTime(); // Wywo³anie natychmiastowe, ¿eby nie czekaæ 1 sekundy na start
        }

        private void UpdateTime()
        {
            var polishCulture = new CultureInfo("pl-PL");
            // Format: 26 lis 2025   18:11
            DateTextBlock.Text = DateTime.Now.ToString("dd MMM yyyy   HH:mm", polishCulture);
        }

        private void SeasonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SeasonComboBox.SelectedItem is string selectedSeason)
            {
                // Wersja synchroniczna (bez bazy danych)
                LoadStatsForSeason(selectedSeason);
            }
        }

        // Metoda bez async/Task, bo nie ³¹czy siê z baz¹
        private void LoadStatsForSeason(string season)
        {
            // SYMULACJA DANYCH (Hardcoded)
            if (season == "Season 24/25")
            {
                GoalsPerMatchValue.Text = "2.4";
                ShotsPerMatchValue.Text = "12.5";
                ShotsOnTargetValue.Text = "5.1";
                ShotsOffTargetValue.Text = "7.4";
                PossessionValue.Text = "54%";
                CornersValue.Text = "6.2";
                CleanSheetsValue.Text = "8";
                FoulsValue.Text = "10.1";
            }
            else
            {
                // Inne dane dla starszych sezonów
                GoalsPerMatchValue.Text = "1.8";
                ShotsPerMatchValue.Text = "9.0";
                ShotsOnTargetValue.Text = "3.5";
                ShotsOffTargetValue.Text = "5.5";
                PossessionValue.Text = "48%";
                CornersValue.Text = "4.5";
                CleanSheetsValue.Text = "5";
                FoulsValue.Text = "12.0";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StatsNaviPage));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DashboardPage));
        }
    }
}
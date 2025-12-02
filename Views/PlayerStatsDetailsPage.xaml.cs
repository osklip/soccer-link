using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Models;
using System;
using System.Globalization;

namespace SoccerLink.Views
{
    public sealed partial class PlayerStatsDetailsPage : Page
    {
        private DispatcherTimer _timer;
        private Zawodnik _selectedPlayer;

        // 1. Domyœlny konstruktor (wymagany przez XAML)
        public PlayerStatsDetailsPage()
        {
            InitializeComponent();
            InitializeSeasons();
            StartClock();
        }

        // 2. NOWY KONSTRUKTOR przyjmuj¹cy zawodnika
        internal PlayerStatsDetailsPage(Zawodnik player) : this()
        {
            _selectedPlayer = player;
            if (_selectedPlayer != null)
            {
                PlayerNameTextBlock.Text = $"{_selectedPlayer.Imie} {_selectedPlayer.Nazwisko}";
                // Tutaj mo¿na za³adowaæ dane dla konkretnego ID, np:
                // LoadStatsForPlayer("Season 24/25");
            }
        }

        private void InitializeSeasons()
        {
            SeasonComboBox.Items.Add("Season 24/25");
            SeasonComboBox.Items.Add("Season 23/24");
            SeasonComboBox.SelectedIndex = 0;
        }

        private void StartClock()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => UpdateTime();
            _timer.Start();
            UpdateTime();
        }

        private void UpdateTime()
        {
            var polishCulture = new CultureInfo("pl-PL");
            DateTextBlock.Text = DateTime.Now.ToString("dd MMM yyyy   HH:mm", polishCulture);
        }

        private void SeasonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SeasonComboBox.SelectedItem is string selectedSeason)
            {
                LoadStatsForPlayer(selectedSeason);
            }
        }

        private void LoadStatsForPlayer(string season)
        {
            // SYMULACJA DANYCH
            if (season == "Season 24/25")
            {
                GoalsValue.Text = "0.8";
                ShotsValue.Text = "3.2";
                ShotsOnTargetValue.Text = "1.5";
                ShotsOffTargetValue.Text = "1.7";
                PassesValue.Text = "24";
                CleanSheetsValue.Text = "0";
                FoulsValue.Text = "1.2";
                CardsValue.Text = "0.2 / 0";
            }
            else
            {
                GoalsValue.Text = "0.5";
                ShotsValue.Text = "2.1";
                ShotsOnTargetValue.Text = "1.0";
                ShotsOffTargetValue.Text = "1.1";
                PassesValue.Text = "18";
                CleanSheetsValue.Text = "0";
                FoulsValue.Text = "0.8";
                CardsValue.Text = "0.1 / 1";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Powrót do listy (przez podmianê Content)
            this.Frame.Navigate(typeof(StatsPlayerPage));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DashboardPage));
        }
    }
}
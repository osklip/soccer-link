using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SoccerLink.Views
{
    public class PlayerStatsEntry : INotifyPropertyChanged
    {
        public Zawodnik Player { get; set; }
        public string FullName => $"{Player.Imie} {Player.Nazwisko}";
        public string Position => Player.Pozycja;

        // STATYSTYKI
        private int _goals; public int Goals { get => _goals; set { _goals = value; OnPropertyChanged(); } }
        private int _shots; public int Shots { get => _shots; set { _shots = value; OnPropertyChanged(); } }
        private int _shotsOnTarget; public int ShotsOnTarget { get => _shotsOnTarget; set { _shotsOnTarget = value; OnPropertyChanged(); } }
        private int _shotsOffTarget; public int ShotsOffTarget { get => _shotsOffTarget; set { _shotsOffTarget = value; OnPropertyChanged(); } }
        private int _passes; public int Passes { get => _passes; set { _passes = value; OnPropertyChanged(); } }
        private int _fouls; public int Fouls { get => _fouls; set { _fouls = value; OnPropertyChanged(); } }

        private bool _cleanSheet; public bool CleanSheet { get => _cleanSheet; set { _cleanSheet = value; OnPropertyChanged(); } }

        private int _yellowCards; public int YellowCards { get => _yellowCards; set { _yellowCards = value; OnPropertyChanged(); } }
        private bool _hasRedCard; public bool HasRedCard { get => _hasRedCard; set { _hasRedCard = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public sealed partial class AddPlayerStatsPage : Page
    {
        private Mecz _selectedMatch;
        public ObservableCollection<PlayerStatsEntry> PlayersList { get; set; } = new();

        public AddPlayerStatsPage() { InitializeComponent(); }

        internal AddPlayerStatsPage(Mecz match) : this()
        {
            _selectedMatch = match;
            if (_selectedMatch != null) MatchTitleText.Text = $"{_selectedMatch.Data} vs {_selectedMatch.Przeciwnik}";
            LoadMockPlayers();
        }

        private void LoadMockPlayers()
        {
            PlayersList.Add(new PlayerStatsEntry { Player = new Zawodnik { Imie = "Robert", Nazwisko = "Lewandowski", Pozycja = "Napastnik" } });
            PlayersList.Add(new PlayerStatsEntry { Player = new Zawodnik { Imie = "Piotr", Nazwisko = "Zieliñski", Pozycja = "Pomocnik" } });
            PlayersList.Add(new PlayerStatsEntry { Player = new Zawodnik { Imie = "Wojciech", Nazwisko = "Szczêsny", Pozycja = "Bramkarz" } });
            PlayersList.Add(new PlayerStatsEntry { Player = new Zawodnik { Imie = "Jan", Nazwisko = "Bednarek", Pozycja = "Obroñca" } });
            PlayersStatsList.ItemsSource = PlayersList;
        }

        // --- HELPERY ---
        private void ModifyStat(object sender, string statName, int delta)
        {
            if ((sender as Button)?.Tag is PlayerStatsEntry entry)
            {
                switch (statName)
                {
                    case "Goals": if (entry.Goals + delta >= 0) entry.Goals += delta; break;
                    case "Shots": if (entry.Shots + delta >= 0) entry.Shots += delta; break;
                    case "ShotsOn": if (entry.ShotsOnTarget + delta >= 0) entry.ShotsOnTarget += delta; break;
                    case "ShotsOff": if (entry.ShotsOffTarget + delta >= 0) entry.ShotsOffTarget += delta; break;
                    case "Passes": if (entry.Passes + delta >= 0) entry.Passes += delta; break;
                    case "Fouls": if (entry.Fouls + delta >= 0) entry.Fouls += delta; break;
                    case "Yellow":
                        if (delta > 0) // Dodawanie
                        {
                            if (entry.YellowCards < 2)
                            {
                                entry.YellowCards++;
                                if (entry.YellowCards == 2) entry.HasRedCard = true;
                            }
                        }
                        else // Odejmowanie (NOWE)
                        {
                            if (entry.YellowCards > 0) entry.YellowCards--;
                        }
                        break;
                }
            }
        }

        // Handlery przycisków
        private void IncGoal(object s, RoutedEventArgs e) => ModifyStat(s, "Goals", 1);
        private void DecGoal(object s, RoutedEventArgs e) => ModifyStat(s, "Goals", -1);
        private void IncShot(object s, RoutedEventArgs e) => ModifyStat(s, "Shots", 1);
        private void DecShot(object s, RoutedEventArgs e) => ModifyStat(s, "Shots", -1);
        private void IncShotOn(object s, RoutedEventArgs e) => ModifyStat(s, "ShotsOn", 1);
        private void DecShotOn(object s, RoutedEventArgs e) => ModifyStat(s, "ShotsOn", -1);
        private void IncShotOff(object s, RoutedEventArgs e) => ModifyStat(s, "ShotsOff", 1);
        private void DecShotOff(object s, RoutedEventArgs e) => ModifyStat(s, "ShotsOff", -1);
        private void IncPass(object s, RoutedEventArgs e) => ModifyStat(s, "Passes", 1);
        private void DecPass(object s, RoutedEventArgs e) => ModifyStat(s, "Passes", -1);
        private void IncFoul(object s, RoutedEventArgs e) => ModifyStat(s, "Fouls", 1);
        private void DecFoul(object s, RoutedEventArgs e) => ModifyStat(s, "Fouls", -1);

        // Kartki
        private void AddYellow(object s, RoutedEventArgs e) => ModifyStat(s, "Yellow", 1);
        private void RemoveYellow(object s, RoutedEventArgs e) => ModifyStat(s, "Yellow", -1); // NOWE

        // Specjalny handler do odznaczania czerwonej
        private void RedCardCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.Tag is PlayerStatsEntry entry)
            {
                // Jeœli odznaczono czerwon¹, cofnij jedn¹ ¿ó³t¹ (jeœli jest co cofaæ)
                if (cb.IsChecked == false && entry.YellowCards > 0)
                {
                    entry.YellowCards--;
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool globalCleanSheet = GlobalCleanSheetSwitch.IsOn;
            foreach (var entry in PlayersList)
            {
                entry.CleanSheet = globalCleanSheet;
                // TU ZAPIS DO BAZY
            }

            ContentDialog dialog = new ContentDialog
            {
                Title = "Zapisano",
                Content = $"Zapisano komplet statystyk.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();

            this.Content = new AddStatsHubPage(_selectedMatch);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new AddStatsHubPage(_selectedMatch);
        }
    }
}
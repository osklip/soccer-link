using SoccerLink.Helpers;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels.Stats
{
    public class PlayerStatsDetailsViewModel : BaseViewModel
    {
        private string _playerName = "Ładowanie...";
        private string _position = "-";
        private string _goals = "-";
        private string _shots = "-";
        private string _shotsOn = "-";
        private string _shotsOff = "-";
        private string _passes = "-";
        private string _fouls = "-";
        private string _cards = "- / -";
        private string _cleanSheets = "-";

        public event EventHandler RequestNavigateBack;
        public ICommand GoBackCommand { get; }

        public PlayerStatsDetailsViewModel()
        {
            GoBackCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
        }

        public string PlayerName { get => _playerName; set => SetProperty(ref _playerName, value); }
        public string Position { get => _position; set => SetProperty(ref _position, value); }
        public string Goals { get => _goals; set => SetProperty(ref _goals, value); }
        public string Shots { get => _shots; set => SetProperty(ref _shots, value); }
        public string ShotsOn { get => _shotsOn; set => SetProperty(ref _shotsOn, value); }
        public string ShotsOff { get => _shotsOff; set => SetProperty(ref _shotsOff, value); }
        public string Passes { get => _passes; set => SetProperty(ref _passes, value); }
        public string Fouls { get => _fouls; set => SetProperty(ref _fouls, value); }
        public string Cards { get => _cards; set => SetProperty(ref _cards, value); }
        public string CleanSheets { get => _cleanSheets; set => SetProperty(ref _cleanSheets, value); }

        public async Task LoadStatsForPlayerAsync(Zawodnik player)
        {
            if (player == null) return;

            PlayerName = $"{player.Imie} {player.Nazwisko}";
            Position = player.Pozycja;

            var stats = await StatsService.GetPlayerStatsSummaryAsync(player.ZawodnikId);

            Goals = stats.Gole.ToString();
            Shots = stats.Strzaly.ToString();
            ShotsOn = stats.StrzalyCelne.ToString();
            ShotsOff = stats.StrzalyNiecelne.ToString();
            Passes = stats.PodaniaCelne.ToString();
            Fouls = stats.Faule.ToString();
            int redCardCount = stats.CzerwonaKartka ? 1 : 0;
            Cards = $"{stats.ZolteKartki} / {redCardCount}";
            CleanSheets = stats.CzysteKonto ? "Tak" : "Nie";
        }
    }
}
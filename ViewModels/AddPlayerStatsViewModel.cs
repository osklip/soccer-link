using SoccerLink.Models;
using SoccerLink.Services;
using SoccerLink.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;

namespace SoccerLink.ViewModels
{
    // Wrapper dla pojedynczego wiersza (Gracza) na liście
    public class PlayerStatsItemViewModel : BaseViewModel
    {
        public Zawodnik Player { get; }

        // Dane do zapisu
        private int _goals; public int Goals { get => _goals; set => SetProperty(ref _goals, value); }
        private int _shots; public int Shots { get => _shots; set => SetProperty(ref _shots, value); }
        private int _shotsOn; public int ShotsOnTarget { get => _shotsOn; set => SetProperty(ref _shotsOn, value); }
        private int _shotsOff; public int ShotsOffTarget { get => _shotsOff; set => SetProperty(ref _shotsOff, value); }
        private int _passes; public int Passes { get => _passes; set => SetProperty(ref _passes, value); }
        private int _fouls; public int Fouls { get => _fouls; set => SetProperty(ref _fouls, value); }
        private int _yellows; public int YellowCards { get => _yellows; set => SetProperty(ref _yellows, value); }
        private bool _red; public bool HasRedCard { get => _red; set => SetProperty(ref _red, value); }
        private bool _clean; public bool CleanSheet { get => _clean; set => SetProperty(ref _clean, value); }

        // Komendy dla przycisków +/- (bezparametrowe, bo kontekst to ten obiekt)
        public ICommand IncGoalCommand { get; }
        public ICommand DecGoalCommand { get; }
        public ICommand IncShotCommand { get; }
        public ICommand DecShotCommand { get; }
        public ICommand IncShotOnCommand { get; }
        public ICommand DecShotOnCommand { get; }
        public ICommand IncShotOffCommand { get; }
        public ICommand DecShotOffCommand { get; }
        public ICommand IncPassCommand { get; }
        public ICommand DecPassCommand { get; }
        public ICommand IncFoulCommand { get; }
        public ICommand DecFoulCommand { get; }
        public ICommand IncYellowCommand { get; }
        public ICommand DecYellowCommand { get; }

        public PlayerStatsItemViewModel(Zawodnik player)
        {
            Player = player;
            IncGoalCommand = new RelayCommand(() => Goals++);
            DecGoalCommand = new RelayCommand(() => { if (Goals > 0) Goals--; });
            // W prawdziwym kodzie warto dodać komendy dla reszty statystyk
        }

        // Helper do łatwej zmiany statystyk z poziomu głównego VM (np. globalne czyste konto)
        public void SetCleanSheet(bool value) => CleanSheet = value;
    }

    public class AddPlayerStatsViewModel : BaseViewModel
    {
        private Mecz _selectedMatch;
        private bool _globalCleanSheet;

        public ObservableCollection<PlayerStatsItemViewModel> PlayersList { get; } = new();

        public event EventHandler RequestClose;

        public AddPlayerStatsViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(() => RequestClose?.Invoke(this, EventArgs.Empty));
        }

        public bool GlobalCleanSheet
        {
            get => _globalCleanSheet;
            set
            {
                if (SetProperty(ref _globalCleanSheet, value))
                {
                    // Aktualizuj wszystkich graczy
                    foreach (var p in PlayersList) p.SetCleanSheet(value);
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public async void Initialize(Mecz match)
        {
            _selectedMatch = match;
            PlayersList.Clear();

            // Pobierz graczy z bazy
            var players = await ZawodnikService.PobierzZawodnikowDlaAktualnegoTreneraAsync();

            foreach (var p in players)
            {
                PlayersList.Add(new PlayerStatsItemViewModel(p));
            }
        }

        private async void Save()
        {
            if (_selectedMatch == null) return;

            var statsToSave = PlayersList.Select(p => new StatystykiZawodnika
            {
                MeczID = _selectedMatch.MeczID,
                ZawodnikID = p.Player.ZawodnikId,
                Gole = p.Goals,
                Strzaly = p.Shots,
                StrzalyCelne = p.ShotsOnTarget,
                StrzalyNiecelne = p.ShotsOffTarget,
                PodaniaCelne = p.Passes,
                Faule = p.Fouls,
                ZolteKartki = p.YellowCards,
                CzerwonaKartka = p.HasRedCard,
                CzysteKonto = p.CleanSheet
            }).ToList();

            await StatsService.SavePlayerStatsListAsync(statsToSave);
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
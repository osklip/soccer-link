using SoccerLink.Helpers;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels.Team
{
    public class SquadViewModel : BaseViewModel
    {
        private Mecz _selectedMatch;
        private string _statusMessage;
        private string _statusColor = "White";
        private string _selectedFormation = "4-4-2";

        public ObservableCollection<Mecz> Matches { get; } = new();
        public ObservableCollection<Zawodnik> AvailablePlayers { get; } = new();
        public List<string> Formations { get; } = new List<string> { "4-4-2", "4-3-3" };

        // Flagi widoczności
        public bool Is442 => SelectedFormation == "4-4-2";
        public bool Is433 => SelectedFormation == "4-3-3";

        // Zawodnicy
        private Zawodnik _gk; public Zawodnik GK { get => _gk; set => SetProperty(ref _gk, value); }
        private Zawodnik _p1; public Zawodnik P1 { get => _p1; set => SetProperty(ref _p1, value); }
        private Zawodnik _p2; public Zawodnik P2 { get => _p2; set => SetProperty(ref _p2, value); }
        private Zawodnik _p3; public Zawodnik P3 { get => _p3; set => SetProperty(ref _p3, value); }
        private Zawodnik _p4; public Zawodnik P4 { get => _p4; set => SetProperty(ref _p4, value); }
        private Zawodnik _p5; public Zawodnik P5 { get => _p5; set => SetProperty(ref _p5, value); }
        private Zawodnik _p6; public Zawodnik P6 { get => _p6; set => SetProperty(ref _p6, value); }
        private Zawodnik _p7; public Zawodnik P7 { get => _p7; set => SetProperty(ref _p7, value); }
        private Zawodnik _p8; public Zawodnik P8 { get => _p8; set => SetProperty(ref _p8, value); }
        private Zawodnik _p9; public Zawodnik P9 { get => _p9; set => SetProperty(ref _p9, value); }
        private Zawodnik _p10; public Zawodnik P10 { get => _p10; set => SetProperty(ref _p10, value); }

        public ICommand SaveCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand GoBackCommand { get; }
        public event EventHandler RequestNavigateBack;

        public SquadViewModel()
        {
            SaveCommand = new RelayCommand(SaveSquad);
            ClearCommand = new RelayCommand(ResetPositions);
            GoBackCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
        }

        public Mecz SelectedMatch
        {
            get => _selectedMatch;
            set
            {
                if (SetProperty(ref _selectedMatch, value))
                {
                    _ = LoadSquadForMatchAsync();
                }
            }
        }

        public string SelectedFormation
        {
            get => _selectedFormation;
            set
            {
                if (SetProperty(ref _selectedFormation, value))
                {
                    OnPropertyChanged(nameof(Is442));
                    OnPropertyChanged(nameof(Is433));
                    StatusMessage = $"Zmieniono na {value}";
                    StatusColor = "#9ECBFF";
                }
            }
        }

        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
        public string StatusColor { get => _statusColor; set => SetProperty(ref _statusColor, value); }

        public async Task InitializeAsync()
        {
            AvailablePlayers.Clear();
            var players = await ZawodnikService.PobierzZawodnikowDlaAktualnegoTreneraAsync();
            AvailablePlayers.Add(new Zawodnik { ZawodnikId = -1, Nazwisko = "(Brak)", Imie = "---" });
            foreach (var p in players) AvailablePlayers.Add(p);

            Matches.Clear();
            var allEvents = await CalendarService.GetAllEventsAsync();
            var matches = allEvents.Where(e => e.EventType == "Mecz" && e.DateTimeStart.Date >= DateTime.Now.Date).OrderBy(e => e.DateTimeStart);

            foreach (var m in matches)
            {
                Matches.Add(new Mecz { MeczID = m.Id, Przeciwnik = m.Title, DataRozpoczecia = m.DateTimeStart });
            }

            if (Matches.Any()) SelectedMatch = Matches.First();
        }

        private void ResetPositions()
        {
            GK = P1 = P2 = P3 = P4 = P5 = P6 = P7 = P8 = P9 = P10 = null;
            StatusMessage = "Wyczyszczono skład.";
            StatusColor = "#FFD700";
        }

        private async Task LoadSquadForMatchAsync()
        {
            if (SelectedMatch == null) return;
            ResetPositions();

            var squadEntries = await SquadService.GetSquadForMatchAsync(SelectedMatch.MeczID);

            foreach (var entry in squadEntries)
            {
                var player = AvailablePlayers.FirstOrDefault(p => p.ZawodnikId == entry.ZawodnikID);
                if (player != null)
                {
                    if (entry.PozycjaKod == "GK") GK = player;
                    else if (entry.PozycjaKod == "P1") P1 = player;
                    else if (entry.PozycjaKod == "P2") P2 = player;
                    else if (entry.PozycjaKod == "P3") P3 = player;
                    else if (entry.PozycjaKod == "P4") P4 = player;
                    else if (entry.PozycjaKod == "P5") P5 = player;
                    else if (entry.PozycjaKod == "P6") P6 = player;
                    else if (entry.PozycjaKod == "P7") P7 = player;
                    else if (entry.PozycjaKod == "P8") P8 = player;
                    else if (entry.PozycjaKod == "P9") P9 = player;
                    else if (entry.PozycjaKod == "P10") P10 = player;
                }
            }
            StatusMessage = "Wczytano skład.";
            StatusColor = "#E6F6FF";
        }

        private async void SaveSquad()
        {
            if (SelectedMatch == null) { StatusMessage = "Wybierz mecz!"; StatusColor = "Red"; return; }

            // WALIDACJA: Sprawdź czy są duplikaty
            var allSlots = new List<Zawodnik> { GK, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10 };
            var selectedRealPlayers = allSlots.Where(p => p != null && p.ZawodnikId > 0).ToList();

            var distinctCount = selectedRealPlayers.Select(p => p.ZawodnikId).Distinct().Count();

            if (selectedRealPlayers.Count != distinctCount)
            {
                StatusMessage = "BŁĄD: Jeden zawodnik jest wybrany na kilku pozycjach!";
                StatusColor = "#E74C3C"; // Czerwony
                return;
            }

            var entries = new List<SkladEntry>();
            void Add(Zawodnik p, string code) { if (p != null && p.ZawodnikId > 0) entries.Add(new SkladEntry { MeczID = SelectedMatch.MeczID, ZawodnikID = p.ZawodnikId, PozycjaKod = code }); }

            Add(GK, "GK");
            Add(P1, "P1"); Add(P2, "P2"); Add(P3, "P3"); Add(P4, "P4"); Add(P5, "P5");
            Add(P6, "P6"); Add(P7, "P7"); Add(P8, "P8"); Add(P9, "P9"); Add(P10, "P10");

            try
            {
                await SquadService.SaveSquadAsync(SelectedMatch.MeczID, entries);
                StatusMessage = "Skład zapisany pomyślnie!";
                StatusColor = "#27AE60"; // Zielony
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd zapisu: {ex.Message}";
                StatusColor = "Red";
            }
        }
    }
}
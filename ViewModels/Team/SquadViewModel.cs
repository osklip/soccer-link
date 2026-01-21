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

        // Pola dla statusu kompletności składu (Nowe)
        private string _completenessInfo;
        private string _completenessColor = "Transparent";

        public ObservableCollection<Mecz> Matches { get; } = new();
        public ObservableCollection<Zawodnik> AvailablePlayers { get; } = new();
        public List<string> Formations { get; } = new List<string> { "4-4-2", "4-3-3" };

        public bool Is442 => SelectedFormation == "4-4-2";
        public bool Is433 => SelectedFormation == "4-3-3";

        // Właściwości informujące o kompletności składu (Nowe)
        public string CompletenessInfo { get => _completenessInfo; set => SetProperty(ref _completenessInfo, value); }
        public string CompletenessColor { get => _completenessColor; set => SetProperty(ref _completenessColor, value); }

        // ZAWODNICY (Podstawowa 11)
        // Settery zostały rozbudowane o usuwanie duplikatów i aktualizację statusu
        private Zawodnik _gk; public Zawodnik GK { get => _gk; set { if (SetProperty(ref _gk, value)) { ClearDuplicateSelection(value, "GK"); UpdateCompletenessStatus(); } } }
        private Zawodnik _p1; public Zawodnik P1 { get => _p1; set { if (SetProperty(ref _p1, value)) { ClearDuplicateSelection(value, "P1"); UpdateCompletenessStatus(); } } }
        private Zawodnik _p2; public Zawodnik P2 { get => _p2; set { if (SetProperty(ref _p2, value)) { ClearDuplicateSelection(value, "P2"); UpdateCompletenessStatus(); } } }
        private Zawodnik _p3; public Zawodnik P3 { get => _p3; set { if (SetProperty(ref _p3, value)) { ClearDuplicateSelection(value, "P3"); UpdateCompletenessStatus(); } } }
        private Zawodnik _p4; public Zawodnik P4 { get => _p4; set { if (SetProperty(ref _p4, value)) { ClearDuplicateSelection(value, "P4"); UpdateCompletenessStatus(); } } }
        private Zawodnik _p5; public Zawodnik P5 { get => _p5; set { if (SetProperty(ref _p5, value)) { ClearDuplicateSelection(value, "P5"); UpdateCompletenessStatus(); } } }
        private Zawodnik _p6; public Zawodnik P6 { get => _p6; set { if (SetProperty(ref _p6, value)) { ClearDuplicateSelection(value, "P6"); UpdateCompletenessStatus(); } } }
        private Zawodnik _p7; public Zawodnik P7 { get => _p7; set { if (SetProperty(ref _p7, value)) { ClearDuplicateSelection(value, "P7"); UpdateCompletenessStatus(); } } }
        private Zawodnik _p8; public Zawodnik P8 { get => _p8; set { if (SetProperty(ref _p8, value)) { ClearDuplicateSelection(value, "P8"); UpdateCompletenessStatus(); } } }
        private Zawodnik _p9; public Zawodnik P9 { get => _p9; set { if (SetProperty(ref _p9, value)) { ClearDuplicateSelection(value, "P9"); UpdateCompletenessStatus(); } } }
        private Zawodnik _p10; public Zawodnik P10 { get => _p10; set { if (SetProperty(ref _p10, value)) { ClearDuplicateSelection(value, "P10"); UpdateCompletenessStatus(); } } }

        // REZERWOWI (R1-R5) (Nowe)
        private Zawodnik _r1; public Zawodnik R1 { get => _r1; set { if (SetProperty(ref _r1, value)) ClearDuplicateSelection(value, "R1"); } }
        private Zawodnik _r2; public Zawodnik R2 { get => _r2; set { if (SetProperty(ref _r2, value)) ClearDuplicateSelection(value, "R2"); } }
        private Zawodnik _r3; public Zawodnik R3 { get => _r3; set { if (SetProperty(ref _r3, value)) ClearDuplicateSelection(value, "R3"); } }
        private Zawodnik _r4; public Zawodnik R4 { get => _r4; set { if (SetProperty(ref _r4, value)) ClearDuplicateSelection(value, "R4"); } }
        private Zawodnik _r5; public Zawodnik R5 { get => _r5; set { if (SetProperty(ref _r5, value)) ClearDuplicateSelection(value, "R5"); } }

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

        // Metoda zapobiegająca duplikatom: Jeśli wybierzesz gracza X na pozycję A, usuwa go z pozycji B
        private void ClearDuplicateSelection(Zawodnik player, string currentPosCode)
        {
            if (player == null || player.ZawodnikId <= 0) return;

            // Sprawdź każdą inną pozycję. Jeśli jest tam ten sam gracz, ustaw null.
            if (currentPosCode != "GK" && GK?.ZawodnikId == player.ZawodnikId) GK = null;
            if (currentPosCode != "P1" && P1?.ZawodnikId == player.ZawodnikId) P1 = null;
            if (currentPosCode != "P2" && P2?.ZawodnikId == player.ZawodnikId) P2 = null;
            if (currentPosCode != "P3" && P3?.ZawodnikId == player.ZawodnikId) P3 = null;
            if (currentPosCode != "P4" && P4?.ZawodnikId == player.ZawodnikId) P4 = null;
            if (currentPosCode != "P5" && P5?.ZawodnikId == player.ZawodnikId) P5 = null;
            if (currentPosCode != "P6" && P6?.ZawodnikId == player.ZawodnikId) P6 = null;
            if (currentPosCode != "P7" && P7?.ZawodnikId == player.ZawodnikId) P7 = null;
            if (currentPosCode != "P8" && P8?.ZawodnikId == player.ZawodnikId) P8 = null;
            if (currentPosCode != "P9" && P9?.ZawodnikId == player.ZawodnikId) P9 = null;
            if (currentPosCode != "P10" && P10?.ZawodnikId == player.ZawodnikId) P10 = null;

            if (currentPosCode != "R1" && R1?.ZawodnikId == player.ZawodnikId) R1 = null;
            if (currentPosCode != "R2" && R2?.ZawodnikId == player.ZawodnikId) R2 = null;
            if (currentPosCode != "R3" && R3?.ZawodnikId == player.ZawodnikId) R3 = null;
            if (currentPosCode != "R4" && R4?.ZawodnikId == player.ZawodnikId) R4 = null;
            if (currentPosCode != "R5" && R5?.ZawodnikId == player.ZawodnikId) R5 = null;
        }

        // Metoda sprawdzająca czy podstawowa 11 jest pełna
        private void UpdateCompletenessStatus()
        {
            int missingCount = 0;

            if (!IsValidPlayer(GK)) missingCount++;
            if (!IsValidPlayer(P1)) missingCount++;
            if (!IsValidPlayer(P2)) missingCount++;
            if (!IsValidPlayer(P3)) missingCount++;
            if (!IsValidPlayer(P4)) missingCount++;
            if (!IsValidPlayer(P5)) missingCount++;
            if (!IsValidPlayer(P6)) missingCount++;
            if (!IsValidPlayer(P7)) missingCount++;
            if (!IsValidPlayer(P8)) missingCount++;
            if (!IsValidPlayer(P9)) missingCount++;
            if (!IsValidPlayer(P10)) missingCount++;

            if (missingCount > 0)
            {
                CompletenessInfo = $"⚠ Brakuje {missingCount} w podst. składzie";
                CompletenessColor = "#FF6B6B"; // Jasny czerwony
            }
            else
            {
                CompletenessInfo = "✅ Skład gotowy";
                CompletenessColor = "#66BB6A"; // Zielony
            }
        }

        private bool IsValidPlayer(Zawodnik z) => z != null && z.ZawodnikId > 0;

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
            // Pusta opcja do odznaczania
            AvailablePlayers.Add(new Zawodnik { ZawodnikId = -1, Nazwisko = "(Brak)", Imie = "---" });

            // Filtrowanie zawodników: tylko ci z CzyDyspozycyjny == 1
            foreach (var p in players.Where(z => z.CzyDyspozycyjny == 1))
            {
                AvailablePlayers.Add(p);
            }

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
            R1 = R2 = R3 = R4 = R5 = null;
            StatusMessage = "Wyczyszczono skład.";
            StatusColor = "#FFD700";
            UpdateCompletenessStatus();
        }

        private async Task LoadSquadForMatchAsync()
        {
            if (SelectedMatch == null) return;
            ResetPositions(); // To zresetuje też status na "Brakuje..."

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
                    // Rezerwowi
                    else if (entry.PozycjaKod == "R1") R1 = player;
                    else if (entry.PozycjaKod == "R2") R2 = player;
                    else if (entry.PozycjaKod == "R3") R3 = player;
                    else if (entry.PozycjaKod == "R4") R4 = player;
                    else if (entry.PozycjaKod == "R5") R5 = player;
                }
            }
            StatusMessage = "Wczytano skład.";
            StatusColor = "#E6F6FF";
            UpdateCompletenessStatus(); // Upewnij się, że status jest aktualny po wczytaniu
        }

        private async void SaveSquad()
        {
            if (SelectedMatch == null) { StatusMessage = "Wybierz mecz!"; StatusColor = "Red"; return; }

            var entries = new List<SkladEntry>();
            void Add(Zawodnik p, string code) { if (p != null && p.ZawodnikId > 0) entries.Add(new SkladEntry { MeczID = SelectedMatch.MeczID, ZawodnikID = p.ZawodnikId, PozycjaKod = code }); }

            Add(GK, "GK");
            Add(P1, "P1"); Add(P2, "P2"); Add(P3, "P3"); Add(P4, "P4"); Add(P5, "P5");
            Add(P6, "P6"); Add(P7, "P7"); Add(P8, "P8"); Add(P9, "P9"); Add(P10, "P10");
            // Zapis rezerwowych
            Add(R1, "R1"); Add(R2, "R2"); Add(R3, "R3"); Add(R4, "R4"); Add(R5, "R5");

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
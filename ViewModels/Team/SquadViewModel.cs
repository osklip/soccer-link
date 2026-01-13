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

        public bool Is442 => SelectedFormation == "4-4-2";
        public bool Is433 => SelectedFormation == "4-3-3";

        // --- ZAWODNICY ---
        // Każdy setter używa teraz inteligentnej metody SetPlayer, która pilnuje porządku
        private Zawodnik _gk; public Zawodnik GK { get => _gk; set => SetPlayer(ref _gk, value, nameof(GK)); }
        private Zawodnik _p1; public Zawodnik P1 { get => _p1; set => SetPlayer(ref _p1, value, nameof(P1)); }
        private Zawodnik _p2; public Zawodnik P2 { get => _p2; set => SetPlayer(ref _p2, value, nameof(P2)); }
        private Zawodnik _p3; public Zawodnik P3 { get => _p3; set => SetPlayer(ref _p3, value, nameof(P3)); }
        private Zawodnik _p4; public Zawodnik P4 { get => _p4; set => SetPlayer(ref _p4, value, nameof(P4)); }
        private Zawodnik _p5; public Zawodnik P5 { get => _p5; set => SetPlayer(ref _p5, value, nameof(P5)); }
        private Zawodnik _p6; public Zawodnik P6 { get => _p6; set => SetPlayer(ref _p6, value, nameof(P6)); }
        private Zawodnik _p7; public Zawodnik P7 { get => _p7; set => SetPlayer(ref _p7, value, nameof(P7)); }
        private Zawodnik _p8; public Zawodnik P8 { get => _p8; set => SetPlayer(ref _p8, value, nameof(P8)); }
        private Zawodnik _p9; public Zawodnik P9 { get => _p9; set => SetPlayer(ref _p9, value, nameof(P9)); }
        private Zawodnik _p10; public Zawodnik P10 { get => _p10; set => SetPlayer(ref _p10, value, nameof(P10)); }

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
                    StatusMessage = ""; // Czyścimy komunikaty przy zmianie formacji
                }
            }
        }

        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
        public string StatusColor { get => _statusColor; set => SetProperty(ref _statusColor, value); }

        // --- GŁÓWNA LOGIKA AUTO-PRZENOSZENIA ---
        private void SetPlayer(ref Zawodnik field, Zawodnik value, string propertyName)
        {
            // 1. Jeśli użytkownik czyści pole (wybiera null lub pustego), po prostu to robimy
            if (value == null || value.ZawodnikId <= 0)
            {
                SetProperty(ref field, value, propertyName);
                return;
            }

            // 2. Sprawdzamy, czy ten zawodnik jest już na innej pozycji
            string occupiedPosition = FindOccupiedPosition(value, propertyName);

            if (occupiedPosition != null)
            {
                // 3. Jeśli tak, usuwamy go ze starej pozycji (automatyczne przeniesienie)
                ClearPosition(occupiedPosition);
            }

            // 4. Przypisujemy zawodnika do nowej pozycji
            SetProperty(ref field, value, propertyName);

            // Opcjonalnie: czyścimy status, żeby nie straszył starymi błędami
            if (StatusMessage != null && StatusMessage.Contains("BŁĄD"))
                StatusMessage = "";
        }

        // Pomocnicza: znajduje nazwę właściwości (np. "GK"), na której już jest dany zawodnik
        private string FindOccupiedPosition(Zawodnik player, string currentPositionName)
        {
            var currentSelections = new Dictionary<string, Zawodnik>
            {
                { nameof(GK), GK },
                { nameof(P1), P1 }, { nameof(P2), P2 }, { nameof(P3), P3 }, { nameof(P4), P4 },
                { nameof(P5), P5 }, { nameof(P6), P6 }, { nameof(P7), P7 }, { nameof(P8), P8 },
                { nameof(P9), P9 }, { nameof(P10), P10 }
            };

            foreach (var item in currentSelections)
            {
                // Pomijamy pozycję, którą właśnie edytujemy
                if (item.Key == currentPositionName) continue;

                // Jeśli ID się zgadza, zwracamy nazwę zajętej pozycji
                if (item.Value != null && item.Value.ZawodnikId == player.ZawodnikId)
                    return item.Key;
            }
            return null;
        }

        // Pomocnicza: czyści wybraną pozycję (ustawia null)
        private void ClearPosition(string positionName)
        {
            switch (positionName)
            {
                case nameof(GK): GK = null; break;
                case nameof(P1): P1 = null; break;
                case nameof(P2): P2 = null; break;
                case nameof(P3): P3 = null; break;
                case nameof(P4): P4 = null; break;
                case nameof(P5): P5 = null; break;
                case nameof(P6): P6 = null; break;
                case nameof(P7): P7 = null; break;
                case nameof(P8): P8 = null; break;
                case nameof(P9): P9 = null; break;
                case nameof(P10): P10 = null; break;
            }
        }

        public async Task InitializeAsync()
        {
            // Czyścimy listę dostępnych
            AvailablePlayers.Clear();

            // 1. Pobieramy wszystkich zawodników trenera
            var allPlayers = await ZawodnikService.PobierzZawodnikowDlaAktualnegoTreneraAsync();

            // 2. FILTROWANIE: Wybieramy tylko tych, którzy są dyspozycyjni (1)
            // Zakładamy, że w bazie: 1 = Dostępny, 0 = Niedostępny (kontuzja/wyjazd)
            var activePlayers = allPlayers.Where(p => p.CzyDyspozycyjny == 1).ToList();

            // 3. Dodajemy opcję "pustą" (żeby można było zdjąć zawodnika z pozycji)
            AvailablePlayers.Add(new Zawodnik { ZawodnikId = -1, Nazwisko = "(Brak)", Imie = "---" });

            // 4. Dodajemy przefiltrowanych zawodników do listy widocznej w ComboBoxach
            foreach (var p in activePlayers)
            {
                AvailablePlayers.Add(p);
            }

            // --- Reszta kodu bez zmian (ładowanie meczów) ---
            Matches.Clear();
            var allEvents = await CalendarService.GetAllEventsAsync();
            var matches = allEvents.Where(e => e.EventType == "Mecz" && e.DateTimeStart.Date >= DateTime.Now.Date)
                                   .OrderBy(e => e.DateTimeStart);

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
                    // Tutaj przypisujemy bezpośrednio do backing fieldów lub przez propertisy
                    // Ponieważ ładujemy z bazy, nie chcemy wyzwalać logiki "czyszczenia", 
                    // ale przy poprawnym stanie bazy nie powinno być konfliktów.
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

            // Logika UI uniemożliwia duplikaty, ale dla bezpieczeństwa bazy zostawiamy proste sprawdzenie
            // bez blokowania UI błędami w trakcie edycji.
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
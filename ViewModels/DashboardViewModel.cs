using Microsoft.UI.Xaml;
using SoccerLink.Helpers;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private DispatcherTimer _timer;
        private string _currentDateTime;

        // Lewa kolumna (Inne wydarzenia)
        private string _nextOtherEventTitle = "Brak najbliższego wydarzenia";
        private string _nextOtherEventDate = "Data: ---";
        private string _nextOtherEventLocation = "Miejsce: ---";

        private string _subOtherEventTitle = "Brak kolejnego wydarzenia";
        private string _subOtherEventDate = "Data: ---";
        private string _subOtherEventLocation = "Miejsce: ---";

        // Prawa kolumna (Mecze)
        private string _nextMatchTitle = "Brak najbliższego meczu";
        private string _nextMatchDate = "Data: ---";
        private string _nextMatchLocation = "Miejsce: ---";

        // Status składu
        private string _nextMatchSquadStatus = "";
        private string _nextMatchSquadColor = "Transparent";
        private bool _isSquadStatusVisible = false;

        // Prawa kolumna - Kolejny mecz
        private string _subMatchTitle = "Brak kolejnego meczu";
        private string _subMatchDate = "Data: ---";
        private string _subMatchLocation = "Miejsce: ---";

        // Zdarzenia nawigacyjne
        public event EventHandler RequestNavigateToMessages;
        public event EventHandler RequestNavigateToCalendar;
        public event EventHandler RequestNavigateToStats;
        public event EventHandler RequestNavigateToTeamManagement;

        public ICommand GoToMessagesCommand { get; }
        public ICommand GoToCalendarCommand { get; }
        public ICommand GoToStatsCommand { get; }
        public ICommand GoToTeamManagementCommand { get; }

        public DashboardViewModel()
        {
            GoToMessagesCommand = new RelayCommand(() => RequestNavigateToMessages?.Invoke(this, EventArgs.Empty));
            GoToCalendarCommand = new RelayCommand(() => RequestNavigateToCalendar?.Invoke(this, EventArgs.Empty));
            GoToStatsCommand = new RelayCommand(() => RequestNavigateToStats?.Invoke(this, EventArgs.Empty));
            GoToTeamManagementCommand = new RelayCommand(() => RequestNavigateToTeamManagement?.Invoke(this, EventArgs.Empty));

            StartClock();
        }

        // --- WŁAŚCIWOŚCI ---

        public string CurrentDateTime { get => _currentDateTime; set => SetProperty(ref _currentDateTime, value); }

        public string NextOtherEventTitle { get => _nextOtherEventTitle; set => SetProperty(ref _nextOtherEventTitle, value); }
        public string NextOtherEventDate { get => _nextOtherEventDate; set => SetProperty(ref _nextOtherEventDate, value); }
        public string NextOtherEventLocation { get => _nextOtherEventLocation; set => SetProperty(ref _nextOtherEventLocation, value); }

        public string SubOtherEventTitle { get => _subOtherEventTitle; set => SetProperty(ref _subOtherEventTitle, value); }
        public string SubOtherEventDate { get => _subOtherEventDate; set => SetProperty(ref _subOtherEventDate, value); }
        public string SubOtherEventLocation { get => _subOtherEventLocation; set => SetProperty(ref _subOtherEventLocation, value); }

        public string NextMatchTitle { get => _nextMatchTitle; set => SetProperty(ref _nextMatchTitle, value); }
        public string NextMatchDate { get => _nextMatchDate; set => SetProperty(ref _nextMatchDate, value); }
        public string NextMatchLocation { get => _nextMatchLocation; set => SetProperty(ref _nextMatchLocation, value); }

        public string NextMatchSquadStatus { get => _nextMatchSquadStatus; set => SetProperty(ref _nextMatchSquadStatus, value); }
        public string NextMatchSquadColor { get => _nextMatchSquadColor; set => SetProperty(ref _nextMatchSquadColor, value); }
        public bool IsSquadStatusVisible { get => _isSquadStatusVisible; set => SetProperty(ref _isSquadStatusVisible, value); }

        public string SubMatchTitle { get => _subMatchTitle; set => SetProperty(ref _subMatchTitle, value); }
        public string SubMatchDate { get => _subMatchDate; set => SetProperty(ref _subMatchDate, value); }
        public string SubMatchLocation { get => _subMatchLocation; set => SetProperty(ref _subMatchLocation, value); }

        // --- METODY ---

        private void StartClock()
        {
            UpdateDateTime();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => UpdateDateTime();
            _timer.Start();
        }

        private void UpdateDateTime()
        {
            CurrentDateTime = $"{DateTime.Now:D} {DateTime.Now:HH:mm}";
        }

        public async Task LoadDataAsync()
        {
            ClearEventInfo();

            try
            {
                if (SessionService.AktualnyTrener == null) return;

                var allEvents = await CalendarService.GetUpcomingEventsAsync();

                if (allEvents != null && allEvents.Any())
                {
                    var matches = allEvents.Where(e => e.EventType == "Mecz").OrderBy(e => e.DateTimeStart).ToList();
                    var otherEvents = allEvents.Where(e => e.EventType != "Mecz").OrderBy(e => e.DateTimeStart).ToList();

                    var nextOther = otherEvents.FirstOrDefault();
                    var subOther = otherEvents.Skip(1).FirstOrDefault();
                    var nextMatch = matches.FirstOrDefault();
                    var subMatch = matches.Skip(1).FirstOrDefault();

                    // Uzupełnianie danych UI
                    if (nextOther != null)
                    {
                        NextOtherEventTitle = $"{nextOther.EventType}: {nextOther.Title}";
                        NextOtherEventDate = $"{nextOther.DisplayDate} ({nextOther.DisplayTimeRange})";
                        NextOtherEventLocation = $"{nextOther.Location}";
                    }

                    if (subOther != null)
                    {
                        SubOtherEventTitle = $"{subOther.EventType}: {subOther.Title}";
                        SubOtherEventDate = $"{subOther.DisplayDate} ({subOther.DisplayTimeRange})";
                        SubOtherEventLocation = $"{subOther.Location}";
                    }

                    if (nextMatch != null)
                    {
                        NextMatchTitle = $"{nextMatch.Title}";
                        NextMatchDate = $"{nextMatch.DisplayDate} ({nextMatch.DisplayTimeRange})";
                        NextMatchLocation = $"{nextMatch.Location}";

                        // --- SPRAWDZANIE STANU SKŁADU DLA NAJBLIŻSZEGO MECZU ---
                        var squad = await SquadService.GetSquadForMatchAsync(nextMatch.Id);

                        var basePositions = new HashSet<string> { "GK", "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9", "P10" };

                        var filledPositionsCount = squad.Count(s => basePositions.Contains(s.PozycjaKod));
                        var missing = 11 - filledPositionsCount;

                        if (missing > 0)
                        {
                            // POPRAWKA: Odmiana słowa (1 gracza, >1 graczy)
                            string suffix = (missing == 1) ? "gracza" : "graczy";

                            NextMatchSquadStatus = $"⚠ Brakuje {missing} {suffix}";
                            NextMatchSquadColor = "#FF6B6B"; // Czerwony
                            IsSquadStatusVisible = true;
                        }
                        else
                        {
                            NextMatchSquadStatus = "✅ Skład gotowy";
                            NextMatchSquadColor = "#66BB6A"; // Zielony
                            IsSquadStatusVisible = true;
                        }
                    }

                    if (subMatch != null)
                    {
                        SubMatchTitle = $"{subMatch.Title}";
                        SubMatchDate = $"{subMatch.DisplayDate} ({subMatch.DisplayTimeRange})";
                        SubMatchLocation = $"{subMatch.Location}";
                    }
                }
            }
            catch (Exception ex)
            {
                NextOtherEventTitle = "Błąd ładowania!";
                NextOtherEventDate = $"Info: {ex.Message}";
                NextMatchTitle = "Błąd!";
            }
        }

        private void ClearEventInfo()
        {
            NextOtherEventTitle = "Brak najbliższego wydarzenia";
            NextOtherEventDate = "Data: ---";
            NextOtherEventLocation = "Miejsce: ---";

            SubOtherEventTitle = "Brak kolejnego wydarzenia";
            SubOtherEventDate = "Data: ---";
            SubOtherEventLocation = "Miejsce: ---";

            NextMatchTitle = "Brak najbliższego meczu";
            NextMatchDate = "Data: ---";
            NextMatchLocation = "Miejsce: ---";

            // Reset statusu składu
            NextMatchSquadStatus = "";
            NextMatchSquadColor = "Transparent";
            IsSquadStatusVisible = false;

            SubMatchTitle = "Brak kolejnego meczu";
            SubMatchDate = "Data: ---";
            SubMatchLocation = "Miejsce: ---";
        }
    }
}
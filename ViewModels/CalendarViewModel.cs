using SoccerLink.Helpers;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels
{
    public class CalendarViewModel : BaseViewModel
    {
        private int _weekOffset = 0;
        private List<UpcomingEvent> _allEvents = new();
        private ObservableCollection<UpcomingEvent> _eventsToShow = new();
        private string _weekRangeText = "Ładowanie...";
        private string _statusMessage = "";
        private bool _isStatusVisible = false;

        // Zdarzenia nawigacyjne
        public event EventHandler RequestNavigateBack;
        public event EventHandler RequestNavigateToAdd;
        // USUNIĘTO: public event EventHandler<int> RequestNavigateToAttendance;
        public event EventHandler<UpcomingEvent> RequestNavigateToEdit;
        public event EventHandler<UpcomingEvent> RequestNavigateToDelete;

        // Komendy
        public ICommand GoBackCommand { get; }
        public ICommand AddEventCommand { get; }
        public ICommand PrevWeekCommand { get; }
        public ICommand NextWeekCommand { get; }

        // Komendy z parametrem
        // USUNIĘTO: public ICommand CheckAttendanceCommand { get; }
        public ICommand EditEventCommand { get; }
        public ICommand DeleteEventCommand { get; }

        public CalendarViewModel()
        {
            GoBackCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
            AddEventCommand = new RelayCommand(() => RequestNavigateToAdd?.Invoke(this, EventArgs.Empty));
            PrevWeekCommand = new RelayCommand(PrevWeek);
            NextWeekCommand = new RelayCommand(NextWeek);

            // USUNIĘTO inicjalizację CheckAttendanceCommand

            EditEventCommand = new RelayCommand<int>(id =>
            {
                var evt = GetEventById(id);
                if (evt != null) RequestNavigateToEdit?.Invoke(this, evt);
            });

            DeleteEventCommand = new RelayCommand<int>(id =>
            {
                var evt = GetEventById(id);
                if (evt != null) RequestNavigateToDelete?.Invoke(this, evt);
            });
        }

        public ObservableCollection<UpcomingEvent> EventsToShow { get => _eventsToShow; set => SetProperty(ref _eventsToShow, value); }
        public string WeekRangeText { get => _weekRangeText; set => SetProperty(ref _weekRangeText, value); }
        public string StatusMessage { get => _statusMessage; set { if (SetProperty(ref _statusMessage, value)) IsStatusVisible = !string.IsNullOrEmpty(value); } }
        public bool IsStatusVisible { get => _isStatusVisible; set => SetProperty(ref _isStatusVisible, value); }

        public async Task LoadEventsAsync()
        {
            StatusMessage = "Ładowanie wydarzeń...";
            try
            {
                _allEvents = await CalendarService.GetAllEventsAsync();
                FilterEventsByCurrentWeek();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd: {ex.Message}";
                EventsToShow.Clear();
            }
        }

        public UpcomingEvent? GetEventById(int id) => _allEvents.FirstOrDefault(e => e.Id == id);

        private void NextWeek() { _weekOffset++; FilterEventsByCurrentWeek(); }
        private void PrevWeek() { _weekOffset--; FilterEventsByCurrentWeek(); }

        private void FilterEventsByCurrentWeek()
        {
            DateTime today = DateTime.Today.AddDays(7 * _weekOffset);
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = today.AddDays(-1 * diff).Date;
            DateTime endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

            WeekRangeText = $"Tydzień: {startOfWeek:dd.MM} - {endOfWeek:dd.MM.yyyy}";

            var filtered = _allEvents
                .Where(e => e.DateTimeStart >= startOfWeek && e.DateTimeStart <= endOfWeek)
                .OrderBy(e => e.DateTimeStart)
                .ToList();

            if (filtered.Count == 0) StatusMessage = "Brak wydarzeń w tym tygodniu.";
            else StatusMessage = "";

            EventsToShow = new ObservableCollection<UpcomingEvent>(filtered);
        }
    }
}
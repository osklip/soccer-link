using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLink.ViewModels
{
    public class CalendarViewModel : BaseViewModel
    {
        private int _weekOffset = 0;
        private List<UpcomingEvent> _allEvents = new(); // Pełna lista z bazy
        private ObservableCollection<UpcomingEvent> _eventsToShow = new(); // Lista przefiltrowana
        private string _weekRangeText = "Ładowanie...";
        private string _statusMessage = "";
        private bool _isStatusVisible = false;

        public CalendarViewModel()
        {
        }

        // Kolekcja widoczna w XAML
        public ObservableCollection<UpcomingEvent> EventsToShow
        {
            get => _eventsToShow;
            set => SetProperty(ref _eventsToShow, value);
        }

        public string WeekRangeText
        {
            get => _weekRangeText;
            set => SetProperty(ref _weekRangeText, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (SetProperty(ref _statusMessage, value))
                {
                    IsStatusVisible = !string.IsNullOrEmpty(value);
                }
            }
        }

        public bool IsStatusVisible
        {
            get => _isStatusVisible;
            set => SetProperty(ref _isStatusVisible, value);
        }

        public async Task LoadEventsAsync()
        {
            StatusMessage = "Ładowanie wydarzeń...";
            try
            {
                // Pobieramy wszystko raz (używając zaktualizowanego serwisu)
                _allEvents = await CalendarService.GetAllEventsAsync();

                // Filtrujemy widok dla aktualnego offsetu (tygodnia)
                FilterEventsByCurrentWeek();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd: {ex.Message}";
                EventsToShow.Clear();
            }
        }

        public void NextWeek()
        {
            _weekOffset++;
            FilterEventsByCurrentWeek();
        }

        public void PrevWeek()
        {
            _weekOffset--;
            FilterEventsByCurrentWeek();
        }

        // Znajdź wydarzenie po ID (pomocne przy edycji/usuwaniu)
        public UpcomingEvent? GetEventById(int id)
        {
            return _allEvents.FirstOrDefault(e => e.Id == id);
        }

        private void FilterEventsByCurrentWeek()
        {
            DateTime today = DateTime.Today.AddDays(7 * _weekOffset);

            // Logika wyznaczania poniedziałku jako początku tygodnia
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = today.AddDays(-1 * diff).Date;
            DateTime endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

            // Aktualizacja tekstu nagłówka
            WeekRangeText = $"Tydzień: {startOfWeek:dd.MM} - {endOfWeek:dd.MM.yyyy}";

            // Filtrowanie listy
            var filtered = _allEvents
                .Where(e => e.DateTimeStart >= startOfWeek && e.DateTimeStart <= endOfWeek)
                .OrderBy(e => e.DateTimeStart)
                .ToList();

            if (filtered.Count == 0)
            {
                StatusMessage = "Brak wydarzeń w tym tygodniu.";
            }
            else
            {
                StatusMessage = ""; // Ukrywa komunikat
            }

            EventsToShow = new ObservableCollection<UpcomingEvent>(filtered);
        }
    }
}
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization;

namespace SoccerLink.Views
{
    public sealed partial class CalendarPage : Page
    {
        // Przesuniêcie tygodnia: 0 = bie¿¹cy tydzieñ, 1 = nastêpny, -1 = poprzedni
        private int _weekOffset = 0;

        // Przechowujemy wszystkie wydarzenia, aby unikn¹æ wielokrotnego ³adowania z bazy
        private List<UpcomingEvent> _allEvents = new List<UpcomingEvent>();

        public CalendarPage()
        {
            InitializeComponent();
            this.Loaded += CalendarPage_Loaded;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new DashboardPage();
        }

        private async void CalendarPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAllEventsOnce();
            FilterEventsByCurrentWeek(); // Wyœwietlenie bie¿¹cego tygodnia
        }

        private async Task LoadAllEventsOnce()
        {
            StatusTextBlock.Visibility = Visibility.Visible;
            StatusTextBlock.Text = "£adowanie wszystkich wydarzeñ z bazy...";

            try
            {
                // U¿ywamy nowej metody, która pobiera wszystkie wydarzenia
                _allEvents = await CalendarService.GetAllEventsAsync();
                StatusTextBlock.Visibility = Visibility.Collapsed;
            }
            catch (InvalidOperationException)
            {
                StatusTextBlock.Text = "B³¹d: Trener nie jest zalogowany.";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"B³¹d ³adowania wydarzeñ: {ex.Message}";
            }
        }

        private void FilterEventsByCurrentWeek()
        {
            DateTime today = DateTime.Today.AddDays(7 * _weekOffset);

            // Logika wyznaczania pocz¹tku i koñca tygodnia
            // U¿ywamy CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek, ale WinUI preferuje Calendar.GetDayOfWeek

            // Ustalanie niedzieli (lub pierwszego dnia tygodnia)
            // Znalezienie pocz¹tku tygodnia na podstawie ustawieñ systemowych (np. Monday/Sunday)
            var calendar = new Calendar();
            System.DayOfWeek firstDay = System.DayOfWeek.Monday; // Standard w Polsce

            int diff = (7 + (today.DayOfWeek - firstDay)) % 7;
            DateTime startOfWeek = today.AddDays(-1 * diff).Date;
            DateTime endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

            // Aktualizacja nag³ówka zakresu dat
            WeekRangeTextBlock.Text = $"Tydzieñ: {startOfWeek:dd.MM} - {endOfWeek:dd.MM.yyyy}";

            // Filtrowanie wydarzeñ
            var eventsToShow = _allEvents
                .Where(e => e.DateTimeStart >= startOfWeek && e.DateTimeStart <= endOfWeek)
                .OrderBy(e => e.DateTimeStart)
                .ToList();

            if (eventsToShow.Count == 0)
            {
                EventsListView.Visibility = Visibility.Collapsed;
                StatusTextBlock.Text = "Brak wydarzeñ w tym tygodniu.";
                StatusTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                EventsListView.ItemsSource = eventsToShow;
                EventsListView.Visibility = Visibility.Visible;
                StatusTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        // --- Obs³uga prze³¹czania tygodni ---

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            _weekOffset++;
            FilterEventsByCurrentWeek();
        }

        private void PrevWeekButton_Click(object sender, RoutedEventArgs e)
        {
            _weekOffset--;
            FilterEventsByCurrentWeek();
        }

        // --- Obs³uga Edycji/Usuwania ---

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new AddEventPage();
        }

        private void EditEventButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int eventId)
            {
                var selectedEvent = _allEvents.FirstOrDefault(e => e.Id == eventId);

                // Przekazanie ID i Typu do nowej strony edycji
                this.Content = new EditEventPage(selectedEvent.EventType, eventId);
            }
        }

        private void DeleteEventButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int eventId)
            {
                var selectedEvent = _allEvents.FirstOrDefault(e => e.Id == eventId);

                // Przekazanie ID i Typu do strony potwierdzenia usuniêcia
                this.Content = new ConfirmDeleteEventPage(selectedEvent.EventType, eventId);
            }
        }
    }
}
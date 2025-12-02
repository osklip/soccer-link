using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Services;
using SoccerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoccerLink.Views
{
    public sealed partial class DashboardPage : Page
    {
        private readonly DispatcherTimer _dateTimer;

        // Pola przechowuj¹ce filtrowane najbli¿sze wydarzenia
        private UpcomingEvent? _nextOtherEvent;
        private UpcomingEvent? _subsequentOtherEvent;
        private UpcomingEvent? _nextMatch;
        private UpcomingEvent? _subsequentMatch;

        public DashboardPage()
        {
            InitializeComponent();

            UpdateHeaderDateTime();
            LoadEventInfo(); // £adowanie informacji o wydarzeniach

            _dateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _dateTimer.Tick += (s, e) => UpdateHeaderDateTime();
            _dateTimer.Start();
        }

        private void UpdateHeaderDateTime()
        {
            HeaderText.Text = $"{DateTime.Now:D} {DateTime.Now:HH:mm}";
        }

        private async void LoadEventInfo()
        {
            // Resetujemy widok do domyœlnych wartoœci
            ClearEventInfo();

            try
            {
                if (SessionService.AktualnyTrener == null) return;

                var allEvents = await CalendarService.GetUpcomingEventsAsync();

                if (allEvents != null && allEvents.Count > 0)
                {
                    // 1. FILTROWANIE I SORTOWANIE
                    var matches = allEvents
                                    .Where(e => e.EventType == "Mecz")
                                    .OrderBy(e => e.DateTimeStart)
                                    .ToList();

                    var otherEvents = allEvents
                                    .Where(e => e.EventType == "Trening" || e.EventType == "Wydarzenie")
                                    .OrderBy(e => e.DateTimeStart)
                                    .ToList();

                    // 2. PRZYPISANIE 2 NAJBLI¯SZYCH WYDARZEÑ
                    // Treningi / Wydarzenia (Lewa Kolumna)
                    _nextOtherEvent = otherEvents.FirstOrDefault();
                    _subsequentOtherEvent = otherEvents.Skip(1).FirstOrDefault();

                    // Mecze (Prawa Kolumna)
                    _nextMatch = matches.FirstOrDefault();
                    _subsequentMatch = matches.Skip(1).FirstOrDefault();

                    UpdateEventDisplay();
                }
            }
            catch (Exception ex)
            {
                // Wyœwietlanie informacji o b³êdzie w panelu Treningi/Wydarzenia
                OtherEventTitleTextBlock.Text = "B³¹d ³adowania wydarzeñ!";
                OtherEventDateTextBlock.Text = $"Szczegó³y: {ex.Message}";
                MatchTitleTextBlock.Text = "B³¹d ³adowania meczów!";
            }
        }

        private void ClearEventInfo()
        {
            // Treningi / Wydarzenia (Lewa Kolumna)
            OtherEventTitleTextBlock.Text = "Brak najbli¿szego wydarzenia";
            OtherEventDateTextBlock.Text = "Data: ---";
            OtherEventLocationTextBlock.Text = "Miejsce: ---";

            NextOtherEventTitleTextBlock.Text = "Brak kolejnego wydarzenia";
            NextOtherEventDateTextBlock.Text = "Data: ---";
            NextOtherEventLocationTextBlock.Text = "Miejsce: ---";

            // Mecze (Prawa Kolumna)
            MatchTitleTextBlock.Text = "Brak najbli¿szego meczu";
            MatchDateTextBlock.Text = "Data: ---";
            MatchLocationTextBlock.Text = "Miejsce: ---";

            NextMatchTitleTextBlock.Text = "Brak kolejnego meczu";
            NextMatchDateTextBlock.Text = "Data: ---";
            NextMatchLocationTextBlock.Text = "Miejsce: ---";
        }

        private void UpdateEventDisplay()
        {
            // LEWA KOLUMNA: Treningi / Wydarzenia
            if (_nextOtherEvent != null)
            {
                OtherEventTitleTextBlock.Text = $"{_nextOtherEvent.EventType}: {_nextOtherEvent.Title}";
                OtherEventDateTextBlock.Text = $"{_nextOtherEvent.DisplayDate} ({_nextOtherEvent.DisplayTimeRange})";
                OtherEventLocationTextBlock.Text = $"{_nextOtherEvent.Location}";
            }

            if (_subsequentOtherEvent != null)
            {
                NextOtherEventTitleTextBlock.Text = $"{_subsequentOtherEvent.EventType}: {_subsequentOtherEvent.Title}";
                NextOtherEventDateTextBlock.Text = $"{_subsequentOtherEvent.DisplayDate} ({_subsequentOtherEvent.DisplayTimeRange})";
                NextOtherEventLocationTextBlock.Text = $"{_subsequentOtherEvent.Location}";
            }

            // PRAWA KOLUMNA: Mecze
            if (_nextMatch != null)
            {
                MatchTitleTextBlock.Text = $"{_nextMatch.Title}";
                MatchDateTextBlock.Text = $"{_nextMatch.DisplayDate} ({_nextMatch.DisplayTimeRange})";
                MatchLocationTextBlock.Text = $"{_nextMatch.Location}";
            }

            if (_subsequentMatch != null)
            {
                NextMatchTitleTextBlock.Text = $"{_subsequentMatch.Title}";
                NextMatchDateTextBlock.Text = $"{_subsequentMatch.DisplayDate} ({_subsequentMatch.DisplayTimeRange})";
                NextMatchLocationTextBlock.Text = $"{_subsequentMatch.Location}";
            }
        }

        // --- Metody nawigacyjne ---

        private void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MessagesPage));
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CalendarPage));
        }

        private void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StatsNaviPage));
        }

        private void TeamManagementButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TeamManagementPage));
        }
    }
}
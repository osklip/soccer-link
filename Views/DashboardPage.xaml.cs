using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SoccerLink.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        private readonly DispatcherTimer _dateTimer;
        private UpcomingEvent? _nextEvent;
        private UpcomingEvent? _subsequentEvent;

        public DashboardPage()
        {
            InitializeComponent();

            UpdateHeaderDateTime();
            LoadEventInfo();

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
                // Wymagamy SessionService, aby pobraæ dane dla zalogowanego trenera
                if (SessionService.AktualnyTrener == null) return;

                var allEvents = await CalendarService.GetUpcomingEventsAsync();

                if (allEvents != null && allEvents.Count > 0)
                {
                    _nextEvent = allEvents.FirstOrDefault();
                    _subsequentEvent = allEvents.Skip(1).FirstOrDefault();

                    UpdateEventDisplay();
                }
            }
            catch (Exception ex)
            {
                // Jeœli jest b³¹d, wyœwietlamy informacjê.
                UpcomingEventTitleTextBlock.Text = "B³¹d ³adowania!";
                NextEventTitleTextBlock.Text = "SprawdŸ po³¹czenie z baz¹.";
                // Opcjonalnie: Logowanie b³êdu ex.Message
            }
        }

        private void ClearEventInfo()
        {
            // Najbli¿sze wydarzenie (dawniej UpcomingEventButton)
            UpcomingEventTitleTextBlock.Text = "Brak najbli¿szego wydarzenia";
            UpcomingEventDateTextBlock.Text = "Data: ---";
            UpcomingEventLocationTextBlock.Text = "Miejsce: ---";

            // Kolejne wydarzenie (dawniej NextEventButton)
            NextEventTitleTextBlock.Text = "Brak kolejnego wydarzenia";
            NextEventDateTextBlock.Text = "Data: ---";
            NextEventLocationTextBlock.Text = "Miejsce: ---";
        }

        private void UpdateEventDisplay()
        {
            if (_nextEvent != null)
            {
                UpcomingEventTitleTextBlock.Text = _nextEvent.Title;
                UpcomingEventDateTextBlock.Text = $"Data: {_nextEvent.DisplayDate} ({_nextEvent.DisplayTimeRange})";
                UpcomingEventLocationTextBlock.Text = $"Miejsce: {_nextEvent.Location}";
            }

            if (_subsequentEvent != null)
            {
                NextEventTitleTextBlock.Text = _subsequentEvent.Title;
                NextEventDateTextBlock.Text = $"Data: {_subsequentEvent.DisplayDate} ({_subsequentEvent.DisplayTimeRange})";
                NextEventLocationTextBlock.Text = $"Miejsce: {_subsequentEvent.Location}";
            }
        }

        private void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new MessagesPage();
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new CalendarPage();
        }

        private void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new StatsNaviPage();
        }

        private void UpcomingEventButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new CalendarPage();
        }

        private void NextEventButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new CalendarPage();
        }

        private void TeamManagementButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new TeamManagementPage();
        }

    }
}

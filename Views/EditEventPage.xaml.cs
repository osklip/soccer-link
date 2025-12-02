using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLink.Views
{
    public sealed partial class EditEventPage : Page
    {
        private readonly string _eventType;
        private readonly int _eventId;
        private UpcomingEvent? _eventToEdit;

        public EditEventPage(string eventType, int eventId)
        {
            this.InitializeComponent();
            _eventType = eventType;
            _eventId = eventId;

            this.Loaded += EditEventPage_Loaded;
        }

        private async void EditEventPage_Loaded(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "£adowanie danych...";
            await LoadEventDetails();
            SetupFormVisibility();
        }

        private async Task LoadEventDetails()
        {
            try
            {
                // Pobieramy wszystkie wydarzenia i znajdujemy to konkretne
                var allEvents = await CalendarService.GetAllEventsAsync();
                _eventToEdit = allEvents.FirstOrDefault(e => e.Id == _eventId && e.EventType == _eventType);

                if (_eventToEdit == null)
                {
                    StatusTextBlock.Text = "B³¹d: Nie znaleziono wydarzenia.";
                    return;
                }

                // Uzupe³nienie formularza
                EventTypeTextBox.Text = _eventType;
                TitleTextBox.Text = _eventToEdit.Title;
                MiejsceTextBox.Text = _eventToEdit.Location;
                DateDatePicker.Date = _eventToEdit.DateTimeStart.Date;
                TimeStartTextBox.Text = _eventToEdit.DisplayTimeStart;

                // Warunkowe pola
                TimeEndTextBox.Text = _eventToEdit.TimeEnd;
                OpisTextBox.Text = _eventToEdit.Description;

                EventInfoTextBlock.Text = $"Edytujesz: {_eventType} (ID: {_eventId})";
                StatusTextBlock.Text = string.Empty;
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"B³¹d ³adowania: {ex.Message}";
            }
        }

        private void SetupFormVisibility()
        {
            if (_eventToEdit == null) return;

            // Ustawienie etykiety pola nazwy
            TitleLabel.Text = _eventType switch
            {
                "Mecz" => "Przeciwnik",
                "Trening" => "Typ treningu",
                _ => "Nazwa wydarzenia"
            };

            // Ustawienie widocznoœci pól czasowych i opisu
            if (_eventType == "Mecz")
            {
                TimeEndPanel.Visibility = Visibility.Collapsed;
                OpisPanel.Visibility = Visibility.Collapsed;
            }
            else if (_eventType == "Wydarzenie")
            {
                TimeEndPanel.Visibility = Visibility.Visible;
                OpisPanel.Visibility = Visibility.Visible;
            }
            else // Trening
            {
                TimeEndPanel.Visibility = Visibility.Visible;
                OpisPanel.Visibility = Visibility.Collapsed;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_eventToEdit == null || string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                StatusTextBlock.Text = "Brak danych do zapisu lub tytu³ jest pusty.";
                return;
            }

            // Walidacja czasu zakoñczenia dla Treningu
            if (_eventType == "Trening" && string.IsNullOrWhiteSpace(TimeEndTextBox.Text))
            {
                StatusTextBlock.Text = "Dla Treningu wymagana jest Godzina zakoñczenia.";
                return;
            }

            try
            {
                // Aktualizacja obiektu i przygotowanie do zapisu
                _eventToEdit.Title = TitleTextBox.Text.Trim();
                _eventToEdit.Location = MiejsceTextBox.Text.Trim();

                // Przekszta³cenie daty i czasu
                DateTime newDate = DateDatePicker.Date?.Date ?? DateTime.Today.Date;
                string newTimeStart = TimeStartTextBox.Text.Trim();

                if (!DateTime.TryParseExact($"{newDate:yyyy-MM-dd} {newTimeStart}", "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime fullNewStart))
                {
                    StatusTextBlock.Text = "Niepoprawny format godziny rozpoczêcia (u¿yj HH:MM).";
                    return;
                }

                _eventToEdit.DateTimeStart = fullNewStart;
                _eventToEdit.TimeEnd = TimeEndTextBox.Text.Trim();
                _eventToEdit.Description = OpisTextBox.Text.Trim();

                await CalendarService.UpdateEventAsync(_eventToEdit);

                StatusTextBlock.Text = "Zapisano pomyœlnie!";
                await Task.Delay(500);
                this.Frame.Navigate(typeof(CalendarPage));
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"B³¹d zapisu: {ex.Message}";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CalendarPage));
        }
    }
}
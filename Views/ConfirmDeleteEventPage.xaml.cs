using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Services;
using System;
using System.Threading.Tasks;

namespace SoccerLink.Views
{
    public sealed partial class ConfirmDeleteEventPage : Page
    {
        private readonly string _eventType;
        private readonly int _eventId;

        public ConfirmDeleteEventPage(string eventType, int eventId)
        {
            this.InitializeComponent();
            _eventType = eventType;
            _eventId = eventId;
            ConfirmationTextBlock.Text = $"Czy na pewno usun¹æ wydarzenie typu '{_eventType}' o ID: {_eventId}? Tej operacji nie mo¿na cofn¹æ.";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CalendarPage));
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Wywo³anie metody usuwaj¹cej z serwisu
                await CalendarService.DeleteEventAsync(_eventType, _eventId);

                ConfirmationTextBlock.Text = $"Usuniêto '{_eventType}' (ID: {_eventId})! Powrót za 1 sekundê...";

                await Task.Delay(1000);

                this.Content = new CalendarPage();
            }
            catch (Exception ex)
            {
                ConfirmationTextBlock.Text = $"B³¹d podczas usuwania: {ex.Message}";
            }
        }
    }
}
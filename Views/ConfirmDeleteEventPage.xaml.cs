using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Services;
using System;
using System.Threading.Tasks;

namespace SoccerLink.Views
{
    public class ConfirmDeleteEventArgs
    {
        public string EventType { get; set; }
        public int EventId { get; set; }
    }

    public sealed partial class ConfirmDeleteEventPage : Page
    {
        private string _eventType;
        private int _eventId;

        public ConfirmDeleteEventPage(string eventType, int eventId)
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is ConfirmDeleteEventArgs args)
            {
                _eventType = args.EventType;
                _eventId = args.EventId;
                ConfirmationTextBlock.Text = $"Czy na pewno usun¹æ wydarzenie typu '{_eventType}' o ID: {_eventId}? Tej operacji nie mo¿na cofn¹æ.";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                this.Frame.Navigate(typeof(CalendarPage));
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Wywo³anie metody usuwaj¹cej z serwisu
                await CalendarService.DeleteEventAsync(_eventType, _eventId);

                ConfirmationTextBlock.Text = $"Usuniêto '{_eventType}' (ID: {_eventId})! Powrót za 1 sekundê...";

                await Task.Delay(1000);

                this.Frame.Navigate(typeof(CalendarPage));
            }
            catch (Exception ex)
            {
                ConfirmationTextBlock.Text = $"B³¹d podczas usuwania: {ex.Message}";
            }
        }
    }
}
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.ViewModels.Events;
using System;

namespace SoccerLink.Views
{
    public sealed partial class CalendarPage : Page
    {
        public CalendarViewModel ViewModel { get; }

        public CalendarPage()
        {
            ViewModel = new CalendarViewModel();
            this.InitializeComponent();

            ViewModel.RequestNavigateBack += (s, e) => this.Frame.Navigate(typeof(DashboardPage));
            ViewModel.RequestNavigateToAdd += (s, e) => this.Frame.Navigate(typeof(AddEventPage));

            // USUNIÊTO: Obs³uga nawigacji do AttendancePage (Obecnoœæ)

            ViewModel.RequestNavigateToEdit += (s, evt) => this.Frame.Navigate(typeof(EditEventPage), new EditEventArgs { EventType = evt.EventType, EventId = evt.Id });
            ViewModel.RequestNavigateToDelete += (s, evt) => this.Frame.Navigate(typeof(ConfirmDeleteEventPage), new ConfirmDeleteEventArgs { EventType = evt.EventType, EventId = evt.Id });

            this.Loaded += CalendarPage_Loaded;
        }

        private async void CalendarPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadEventsAsync();
        }
    }
}
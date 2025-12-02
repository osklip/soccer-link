using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;
using SoccerLink.Services;
using SoccerLink.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization;

namespace SoccerLink.Views
{
    public sealed partial class CalendarPage : Page
    {
        public CalendarViewModel ViewModel { get; }

        public CalendarPage()
        {
            ViewModel = new CalendarViewModel();
            this.InitializeComponent();
            this.Loaded += CalendarPage_Loaded;
        }

        private async void CalendarPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadEventsAsync();
        }

        // Obs³uga przycisków tygodni - delegujemy do ViewModelu
        private void PrevWeekButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PrevWeek();
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NextWeek();
        }

        // Nawigacja
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DashboardPage));
        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddEventPage));
        }

        // Edycja i Usuwanie wymagaj¹ pobrania ID klikniêtego elementu
        private void EditEventButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is int eventId)
            {
                var evt = ViewModel.GetEventById(eventId);
                if (evt != null)
                {
                    this.Frame.Navigate(typeof(EditEventPage), new EditEventArgs
                    {
                        EventType = evt.EventType,
                        EventId = eventId
                    });
                }
            }
        }

        private void DeleteEventButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is int eventId)
            {
                var evt = ViewModel.GetEventById(eventId);
                if (evt != null)
                {
                    this.Frame.Navigate(typeof(ConfirmDeleteEventPage), new ConfirmDeleteEventArgs
                    {
                        EventType = evt.EventType,
                        EventId = eventId
                    });
                }
            }
        }
    }
}
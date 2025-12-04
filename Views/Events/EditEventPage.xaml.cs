using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.ViewModels.Events;
using System;

namespace SoccerLink.Views
{
    // PRZYWRÓCONA KLASA: Potrzebna do nawigacji z CalendarPage
    public class EditEventArgs
    {
        public string EventType { get; set; }
        public int EventId { get; set; }
    }

    public sealed partial class EditEventPage : Page
    {
        public EditEventViewModel ViewModel { get; }

        public EditEventPage()
        {
            ViewModel = new EditEventViewModel();
            this.InitializeComponent();

            ViewModel.RequestNavigateBack += (s, e) =>
            {
                if (this.Frame.CanGoBack) this.Frame.GoBack();
                else this.Frame.Navigate(typeof(CalendarPage));
            };
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is EditEventArgs args)
            {
                await ViewModel.LoadEventAsync(args.EventId, args.EventType);
            }
        }
    }
}
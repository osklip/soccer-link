using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.ViewModels.Events;

namespace SoccerLink.Views
{
    // Klasa argumentów (musi byæ publiczna, ¿eby CalendarPage j¹ widzia³)
    public class ConfirmDeleteEventArgs
    {
        public string EventType { get; set; }
        public int EventId { get; set; }
    }

    public sealed partial class ConfirmDeleteEventPage : Page
    {
        public ConfirmDeleteViewModel ViewModel { get; }

        public ConfirmDeleteEventPage()
        {
            ViewModel = new ConfirmDeleteViewModel();
            this.InitializeComponent();

            // Obs³uga powrotu
            ViewModel.RequestNavigateBack += (s, e) =>
            {
                if (this.Frame.CanGoBack) this.Frame.GoBack();
                else this.Frame.Navigate(typeof(CalendarPage));
            };
        }

        // Ta metoda uruchamia siê przy wejœciu na stronê
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Sprawdzamy, czy otrzymaliœmy poprawne parametry
            if (e.Parameter is ConfirmDeleteEventArgs args)
            {
                // PRZEKAZANIE DANYCH DO VIEWMODELU
                ViewModel.Initialize(args.EventType, args.EventId);
            }
        }
    }
}
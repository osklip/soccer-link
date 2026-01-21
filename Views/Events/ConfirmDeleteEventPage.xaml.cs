using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.ViewModels.Events;

namespace SoccerLink.Views
{
    
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

            
            ViewModel.RequestNavigateBack += (s, e) =>
            {
                if (this.Frame.CanGoBack) this.Frame.GoBack();
                else this.Frame.Navigate(typeof(CalendarPage));
            };
        }

        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            
            if (e.Parameter is ConfirmDeleteEventArgs args)
            {
                
                ViewModel.Initialize(args.EventType, args.EventId);
            }
        }
    }
}
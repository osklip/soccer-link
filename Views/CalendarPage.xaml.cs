using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Services;
using System;

namespace SoccerLink.Views
{
    public sealed partial class CalendarPage : Page
    {
        public CalendarPage()
        {
            InitializeComponent();

            this.Loaded += CalendarPage_Loaded;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Nawigacja zgodnie ze wzorcem projektu
            this.Content = new DashboardPage();
        }

        private void CalendarPage_Loaded(object sender, RoutedEventArgs e)
        {
            // £adowanie wydarzeñ po za³adowaniu strony
            LoadUpcomingEvents();
        }

        private async void LoadUpcomingEvents()
        {
            // Resetowanie stanu widoku
            EventsListView.ItemsSource = null;
            EventsListView.Visibility = Visibility.Collapsed;
            StatusTextBlock.Visibility = Visibility.Visible;
            StatusTextBlock.Text = "£adowanie nadchodz¹cych wydarzeñ...";

            try
            {
                var events = await CalendarService.GetUpcomingEventsAsync();

                if (events == null || events.Count == 0)
                {
                    StatusTextBlock.Text = "Brak nadchodz¹cych wydarzeñ.";
                }
                else
                {
                    EventsListView.ItemsSource = events;
                    EventsListView.Visibility = Visibility.Visible;
                    StatusTextBlock.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                // Wyœwietlanie b³êdu (np. problem z tokenem/po³¹czeniem)
                StatusTextBlock.Text = $"B³¹d ³adowania wydarzeñ. SprawdŸ po³¹czenie i autoryzacjê tokena. Szczegó³y: {ex.Message}";
                StatusTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            // Nawigacja zgodnie ze wzorcem projektu
            this.Content = new AddEventPage();
        }
    }
}
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace SoccerLink.Views
{
    public sealed partial class CalendarPage : Page
    {
        public CalendarPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new DashboardPage();
        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            // Ustawiamy Frame na widoczny i nawigujemy do formularza dodawania
            ModalFrame.Visibility = Visibility.Visible;

            // Nawigujemy do formularza. Przekazujemy referencjê do Frame, aby AddEventPage mog³a siê zamkn¹æ (ParentFrame)
            var addPage = new AddEventPage { ParentFrame = ModalFrame };
            ModalFrame.Navigate(addPage.GetType(), addPage);

            // W przysz³oœci mo¿na tu te¿ przekazaæ wybran¹ datê z MainCalendar.SelectedDate
        }

        // Wymagane metody do obs³ugi ³adowania wydarzeñ po zamkniêciu modala
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Jeœli wracamy z modala (AddEventPage), ukrywamy go
            if (ModalFrame.Content is null || ModalFrame.Visibility == Visibility.Collapsed)
            {
                ModalFrame.Visibility = Visibility.Collapsed;
            }

            // TO DO: Uruchomienie metody PobierzWydarzeniaDlaKalendarza()
        }
    }
}
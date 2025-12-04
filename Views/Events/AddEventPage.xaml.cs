using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;
using SoccerLink.Services;
using SoccerLink.ViewModels.Events;
using System;
using System.Threading.Tasks;

namespace SoccerLink.Views
{
    public sealed partial class AddEventPage : Page
    {
        public AddEventViewModel ViewModel { get; }

        public AddEventPage()
        {
            // Inicjalizacja ViewModelu przed komponentami
            ViewModel = new AddEventViewModel();
            this.InitializeComponent();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = await ViewModel.AddEventAsync();
            if (success)
            {
                // Krótkie opóŸnienie, ¿eby u¿ytkownik zobaczy³ komunikat "Sukces"
                await Task.Delay(500);
                this.Frame.Navigate(typeof(CalendarPage));
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CalendarPage));
        }
    }
}
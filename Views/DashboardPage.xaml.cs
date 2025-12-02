using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.ViewModels;
using System;

namespace SoccerLink.Views
{
    public sealed partial class DashboardPage : Page
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage()
        {
            ViewModel = new DashboardViewModel();
            this.InitializeComponent();

            // Subskrypcja nawigacji
            ViewModel.RequestNavigateToMessages += (s, e) => this.Frame.Navigate(typeof(MessagesPage));
            ViewModel.RequestNavigateToCalendar += (s, e) => this.Frame.Navigate(typeof(CalendarPage));
            ViewModel.RequestNavigateToStats += (s, e) => this.Frame.Navigate(typeof(StatsNaviPage));
            ViewModel.RequestNavigateToTeamManagement += (s, e) => this.Frame.Navigate(typeof(TeamManagementPage));

            this.Loaded += DashboardPage_Loaded;
        }

        private async void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadDataAsync();
        }
    }
}
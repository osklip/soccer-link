using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.ViewModels.Stats;
using System;
using System.Globalization;

namespace SoccerLink.Views
{
    public sealed partial class StatsTeamPage : Page
    {
        public StatsTeamViewModel ViewModel { get; }
        private DispatcherTimer _timer;

        public StatsTeamPage()
        {
            ViewModel = new StatsTeamViewModel();
            this.InitializeComponent();

            this.Loaded += StatsTeamPage_Loaded;
            StartClock();
        }

        private async void StatsTeamPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadStatsAsync();
        }

        private void StartClock()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => UpdateTime();
            _timer.Start();
            UpdateTime();
        }

        private void UpdateTime()
        {
            var polishCulture = new CultureInfo("pl-PL");
            DateTextBlock.Text = DateTime.Now.ToString("dd MMM yyyy   HH:mm", polishCulture);
        }

        // Nawigacja
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StatsNaviPage));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DashboardPage));
        }
    }
}
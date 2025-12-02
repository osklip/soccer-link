using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Models;
using SoccerLink.ViewModels;
using System;
using System.Globalization;

namespace SoccerLink.Views
{
    public sealed partial class PlayerStatsDetailsPage : Page
    {
        private DispatcherTimer _timer;
        public PlayerStatsDetailsViewModel ViewModel { get; }

        public PlayerStatsDetailsPage()
        {
            ViewModel = new PlayerStatsDetailsViewModel();
            this.InitializeComponent();

            ViewModel.RequestNavigateBack += (s, e) =>
            {
                if (this.Frame.CanGoBack) this.Frame.GoBack();
                else this.Frame.Navigate(typeof(StatsPlayerPage));
            };

            StartClock();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Zawodnik player)
            {
                await ViewModel.LoadStatsForPlayerAsync(player);
            }
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
    }
}
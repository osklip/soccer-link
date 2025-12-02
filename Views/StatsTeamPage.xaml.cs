using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.ViewModels;
using System;
using System.Globalization;

namespace SoccerLink.Views
{
    public sealed partial class StatsTeamPage : Page
    {
        // Publiczna w³aœciwoœæ ViewModelu, aby binding x:Bind w XAML móg³ z niej korzystaæ
        public StatsTeamViewModel ViewModel { get; }

        private DispatcherTimer _timer;

        public StatsTeamPage()
        {
            // 1. Inicjalizacja ViewModelu (niezbêdne do dzia³ania bindingów)
            ViewModel = new StatsTeamViewModel();

            this.InitializeComponent();

            // 2. £adowanie danych z bazy po za³adowaniu widoku
            this.Loaded += async (s, e) =>
            {
                if (ViewModel != null)
                {
                    await ViewModel.LoadStatsAsync();
                }
            };

            // 3. Uruchomienie zegara
            StartClock();
        }

        private void StartClock()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => UpdateTime();
            _timer.Start();
            UpdateTime(); // Wywo³anie natychmiastowe, ¿eby nie czekaæ 1 sekundy na start
        }

        private void UpdateTime()
        {
            var polishCulture = new CultureInfo("pl-PL");
            // Sprawdzenie czy element DateTextBlock istnieje (zabezpieczenie)
            if (DateTextBlock != null)
            {
                DateTextBlock.Text = DateTime.Now.ToString("dd MMM yyyy   HH:mm", polishCulture);
            }
        }

        // --- Nawigacja ---

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
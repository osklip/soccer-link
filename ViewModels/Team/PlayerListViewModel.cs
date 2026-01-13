using SoccerLink.Helpers; 
using SoccerLink.Models;
using SoccerLink.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;

namespace SoccerLink.ViewModels.Team
{
    // 1. Klasa pomocnicza - "opakowanie" dla zawodnika
    public class PlayerDisplayItem
    {
        public Zawodnik Source { get; }

        public string FullName => $"{Source.Imie} {Source.Nazwisko}";
        public SolidColorBrush StatusColor { get; }
        public Visibility ReplaceVisibility { get; }
        public string DyspozycyjnoscText { get; }

        public PlayerDisplayItem(Zawodnik z, bool isMatchMode)
        {
            Source = z;

            bool isAvailable = z.CzyDyspozycyjny == 1;

            if (isAvailable)
            {
                StatusColor = new SolidColorBrush(Colors.LightGreen);
                DyspozycyjnoscText = "Dostępny";
                ReplaceVisibility = Visibility.Collapsed;
            }
            else
            {
                StatusColor = new SolidColorBrush(Colors.Red);
                DyspozycyjnoscText = "Niedostępny";
                ReplaceVisibility = isMatchMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }

    public class PlayerListViewModel : BaseViewModel
    {
        private bool _isMatchMode;
        private Mecz _selectedMatch;
        private ObservableCollection<Zawodnik> _allPlayersCache = new();

        public ObservableCollection<PlayerDisplayItem> DisplayPlayers { get; } = new();
        public ObservableCollection<Mecz> UpcomingMatches { get; } = new();

        public bool IsMatchMode
        {
            get => _isMatchMode;
            set
            {
                if (SetProperty(ref _isMatchMode, value))
                {
                    RefreshDisplayList();
                    OnPropertyChanged(nameof(IsGeneralMode));
                }
            }
        }
        public bool IsGeneralMode => !IsMatchMode;

        public Mecz SelectedMatch
        {
            get => _selectedMatch;
            set
            {
                if (SetProperty(ref _selectedMatch, value))
                {
                    RefreshDisplayList();
                }
            }
        }

        public ICommand NavigateBackCommand { get; }
        public ICommand SendAvailabilityRequestCommand { get; }
        public ICommand ImportCommand { get; }
        public event EventHandler RequestNavigateBack;

        public PlayerListViewModel()
        {
            // Teraz RelayCommand zostanie rozpoznany dzięki using SoccerLink.Helpers;
            NavigateBackCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
            SendAvailabilityRequestCommand = new RelayCommand(SendAvailabilityRequests);
            ImportCommand = new RelayCommand(() => { });
        }

        public async Task LoadPlayersAsync()
        {
            DisplayPlayers.Clear();
            _allPlayersCache.Clear();
            UpcomingMatches.Clear();

            try
            {
                var list = await ZawodnikService.PobierzZawodnikowDlaAktualnegoTreneraAsync();
                foreach (var p in list) _allPlayersCache.Add(p);

                var events = await CalendarService.GetAllEventsAsync();
                var matches = events.Where(e => e.EventType == "Mecz" && e.DateTimeStart >= DateTime.Now)
                                    .OrderBy(e => e.DateTimeStart).Take(5);

                foreach (var m in matches)
                    UpcomingMatches.Add(new Mecz { MeczID = m.Id, Przeciwnik = m.Title, DataRozpoczecia = m.DateTimeStart });

                if (UpcomingMatches.Any()) SelectedMatch = UpcomingMatches.First();

                RefreshDisplayList();
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }

        private void RefreshDisplayList()
        {
            DisplayPlayers.Clear();
            foreach (var p in _allPlayersCache)
            {
                DisplayPlayers.Add(new PlayerDisplayItem(p, IsMatchMode));
            }
        }

        private async void SendAvailabilityRequests()
        {
            if (SelectedMatch == null) return;

            var rnd = new Random();
            foreach (var p in _allPlayersCache)
            {
                p.CzyDyspozycyjny = rnd.Next(0, 5) == 0 ? 0 : 1;
            }

            var window = (Application.Current as App)?.MainWindow;
            if (window != null && window.Content is FrameworkElement fe)
            {
                var dialog = new ContentDialog
                {
                    Title = "Wysłano prośbę",
                    Content = "Zawodnicy otrzymali powiadomienie.",
                    CloseButtonText = "OK",
                    XamlRoot = fe.XamlRoot
                };
                await dialog.ShowAsync();
            }

            RefreshDisplayList();
        }

        public async Task ReplacePlayerAsync(Zawodnik p)
        {
            var window = (Application.Current as App)?.MainWindow;
            if (window != null && window.Content is FrameworkElement fe)
            {
                var dialog = new ContentDialog
                {
                    Title = "Wymiana",
                    Content = $"Czy wymienić zawodnika: {p.Nazwisko}?",
                    PrimaryButtonText = "Tak",
                    CloseButtonText = "Nie",
                    XamlRoot = fe.XamlRoot
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    _allPlayersCache.Remove(p);
                    RefreshDisplayList();
                }
            }
        }
    }
}
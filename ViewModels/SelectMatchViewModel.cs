using SoccerLink.Helpers;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels
{
    public class SelectMatchViewModel : BaseViewModel
    {
        private string _statusMessage = "Ładowanie meczów...";
        private bool _isStatusVisible = true;

        public ObservableCollection<Mecz> Matches { get; } = new();

        public event EventHandler RequestNavigateBack;
        public event EventHandler<Mecz> RequestNavigateToStatsHub;

        public ICommand NavigateBackCommand { get; }

        public SelectMatchViewModel()
        {
            NavigateBackCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
        }

        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
        public bool IsStatusVisible { get => _isStatusVisible; set => SetProperty(ref _isStatusVisible, value); }

        public async Task LoadMatchesAsync()
        {
            IsStatusVisible = true;
            StatusMessage = "Ładowanie...";
            Matches.Clear();

            try
            {
                var matches = await StatsService.GetMatchesWithoutStatsAsync();

                if (matches.Count == 0)
                {
                    StatusMessage = "Brak rozegranych meczów do uzupełnienia.";
                }
                else
                {
                    foreach (var m in matches) Matches.Add(m);
                    IsStatusVisible = false;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd: {ex.Message}";
            }
        }

        public void SelectMatch(Mecz match)
        {
            if (match != null)
            {
                RequestNavigateToStatsHub?.Invoke(this, match);
            }
        }
    }
}
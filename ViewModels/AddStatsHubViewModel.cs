using SoccerLink.Helpers;
using SoccerLink.Models;
using System;
using System.Windows.Input;

namespace SoccerLink.ViewModels
{
    public class AddStatsHubViewModel : BaseViewModel
    {
        private Mecz _selectedMatch;
        private string _matchInfoText = "Mecz: ---";

        public event EventHandler RequestNavigateBack;
        public event EventHandler<Mecz> RequestNavigateToTeamStats;
        public event EventHandler<Mecz> RequestNavigateToPlayerStats;

        public ICommand GoBackCommand { get; }
        public ICommand GoToTeamStatsCommand { get; }
        public ICommand GoToPlayerStatsCommand { get; }

        public AddStatsHubViewModel()
        {
            GoBackCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
            GoToTeamStatsCommand = new RelayCommand(() => RequestNavigateToTeamStats?.Invoke(this, _selectedMatch));
            GoToPlayerStatsCommand = new RelayCommand(() => RequestNavigateToPlayerStats?.Invoke(this, _selectedMatch));
        }

        public string MatchInfoText
        {
            get => _matchInfoText;
            set => SetProperty(ref _matchInfoText, value);
        }

        public void Initialize(Mecz match)
        {
            _selectedMatch = match;
            if (_selectedMatch != null)
            {
                MatchInfoText = $"{_selectedMatch.DataDisplay} vs {_selectedMatch.Przeciwnik}";
            }
        }
    }
}
using SoccerLink.Helpers;
using System;
using System.Windows.Input;

namespace SoccerLink.ViewModels
{
    public class StatsNaviViewModel : BaseViewModel
    {
        // Zdarzenia, na które nasłuchuje widok
        public event EventHandler RequestNavigateBack;
        public event EventHandler RequestNavigateToSoloStats;
        public event EventHandler RequestNavigateToTeamStats;
        public event EventHandler RequestNavigateToAddStats;

        // Komendy bindowane do przycisków
        public ICommand GoBackCommand { get; }
        public ICommand GoToSoloStatsCommand { get; }
        public ICommand GoToTeamStatsCommand { get; }
        public ICommand GoToAddStatsCommand { get; }

        public StatsNaviViewModel()
        {
            GoBackCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
            GoToSoloStatsCommand = new RelayCommand(() => RequestNavigateToSoloStats?.Invoke(this, EventArgs.Empty));
            GoToTeamStatsCommand = new RelayCommand(() => RequestNavigateToTeamStats?.Invoke(this, EventArgs.Empty));
            GoToAddStatsCommand = new RelayCommand(() => RequestNavigateToAddStats?.Invoke(this, EventArgs.Empty));
        }
    }
}
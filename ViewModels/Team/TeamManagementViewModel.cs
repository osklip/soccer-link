using SoccerLink.Helpers;
using System;
using System.Windows.Input;

namespace SoccerLink.ViewModels.Team
{
    public class TeamManagementViewModel : BaseViewModel
    {
        public event EventHandler RequestNavigateBack;
        public event EventHandler RequestNavigateToPlayerList;
        public event EventHandler RequestNavigateToSquad;

        public ICommand GoBackCommand { get; }
        public ICommand GoToPlayerListCommand { get; }
        public ICommand GoToSquadCommand { get; }

        public TeamManagementViewModel()
        {
            GoBackCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
            GoToPlayerListCommand = new RelayCommand(() => RequestNavigateToPlayerList?.Invoke(this, EventArgs.Empty));
            GoToSquadCommand = new RelayCommand(() => RequestNavigateToSquad?.Invoke(this, EventArgs.Empty));
        }
    }
}
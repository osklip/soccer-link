using SoccerLink.Helpers;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels.Team
{
    public class PlayerListViewModel : BaseViewModel
    {
        public ObservableCollection<Zawodnik> Players { get; } = new();

        public event EventHandler RequestNavigateBack;
        public ICommand NavigateBackCommand { get; }

        public PlayerListViewModel()
        {
            NavigateBackCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
        }

        public async Task LoadPlayersAsync()
        {
            Players.Clear();
            try
            {
                var list = await ZawodnikService.PobierzZawodnikowDlaAktualnegoTreneraAsync();
                foreach (var p in list)
                {
                    Players.Add(p);
                }
            }
            catch (Exception ex)
            {
                // Tutaj można dodać obsługę błędu (np. Property StatusMessage)
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
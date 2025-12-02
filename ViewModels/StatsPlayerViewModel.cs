using SoccerLink.Helpers;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels
{
    public class StatsPlayerViewModel : BaseViewModel
    {
        public ObservableCollection<Zawodnik> Players { get; } = new();

        public event EventHandler RequestNavigateBack;
        public event EventHandler RequestNavigateHome;
        public event EventHandler<Zawodnik> RequestNavigateToDetails;

        public ICommand NavigateBackCommand { get; }
        public ICommand NavigateHomeCommand { get; }

        public StatsPlayerViewModel()
        {
            NavigateBackCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
            NavigateHomeCommand = new RelayCommand(() => RequestNavigateHome?.Invoke(this, EventArgs.Empty));
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
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void SelectPlayer(Zawodnik player)
        {
            if (player != null)
            {
                RequestNavigateToDetails?.Invoke(this, player);
            }
        }
    }
}
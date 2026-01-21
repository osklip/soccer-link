using SoccerLink.Models;
using SoccerLink.Services;
using SoccerLink.Helpers; 
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels.Stats
{
    public class AddTeamStatsViewModel : BaseViewModel
    {
        private Mecz _selectedMatch;

        
        private string _goals;
        private string _possession;
        private string _shots;
        private string _shotsOnTarget;
        private string _shotsOffTarget;
        private string _corners;
        private string _fouls;
        private bool _cleanSheet;

        public event EventHandler RequestClose; 

        public AddTeamStatsViewModel()
        {
            SaveCommand = new RelayCommand(SaveStats);
            CancelCommand = new RelayCommand(() => RequestClose?.Invoke(this, EventArgs.Empty));
        }

        public void Initialize(Mecz match)
        {
            _selectedMatch = match;
            
        }

        
        public string Goals { get => _goals; set => SetProperty(ref _goals, value); }
        public string Possession { get => _possession; set => SetProperty(ref _possession, value); }
        public string Shots { get => _shots; set => SetProperty(ref _shots, value); }
        public string ShotsOnTarget { get => _shotsOnTarget; set => SetProperty(ref _shotsOnTarget, value); }
        public string ShotsOffTarget { get => _shotsOffTarget; set => SetProperty(ref _shotsOffTarget, value); }
        public string Corners { get => _corners; set => SetProperty(ref _corners, value); }
        public string Fouls { get => _fouls; set => SetProperty(ref _fouls, value); }
        public bool CleanSheet { get => _cleanSheet; set => SetProperty(ref _cleanSheet, value); }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async void SaveStats()
        {
            if (_selectedMatch == null) return;

            var stats = new StatystykiDruzyny
            {
                MeczID = _selectedMatch.MeczID,
                Gole = int.TryParse(Goals, out int g) ? g : 0,
                PosiadaniePilki = int.TryParse(Possession, out int p) ? p : 50,
                Strzaly = int.TryParse(Shots, out int s) ? s : 0,
                StrzalyCelne = int.TryParse(ShotsOnTarget, out int so) ? so : 0,
                StrzalyNiecelne = int.TryParse(ShotsOffTarget, out int sn) ? sn : 0,
                RzutyRozne = int.TryParse(Corners, out int c) ? c : 0,
                Faule = int.TryParse(Fouls, out int f) ? f : 0,
                CzysteKonto = CleanSheet
            };

            await StatsService.SaveTeamStatsAsync(stats);
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
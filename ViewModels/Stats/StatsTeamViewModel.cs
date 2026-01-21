using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoccerLink.ViewModels.Stats
{
    public class StatsTeamViewModel : BaseViewModel
    {
        
        private string _avgGoals = "-";
        private string _avgPossession = "-";
        private string _avgShots = "-";
        private string _avgShotsOn = "-";
        private string _avgShotsOff = "-";
        private string _avgCorners = "-";
        private string _avgFouls = "-";
        private string _totalCleanSheets = "-";

        
        private int _selectedMonthIndex = 0;
        private int _selectedYearIndex = 0;

        public List<string> Months { get; } = new List<string>
        {
            "Wszystkie", "Styczeń", "Luty", "Marzec", "Kwiecień", "Maj", "Czerwiec",
            "Lipiec", "Sierpień", "Wrzesień", "Październik", "Listopad", "Grudzień"
        };

        public List<string> Years { get; private set; } = new List<string> { "Wszystkie" };

        public StatsTeamViewModel()
        {
            var currentYear = DateTime.Now.Year;
            for (int i = 0; i < 5; i++)
            {
                Years.Add((currentYear - i).ToString());
            }
        }

        public string AvgGoals { get => _avgGoals; set => SetProperty(ref _avgGoals, value); }
        public string AvgPossession { get => _avgPossession; set => SetProperty(ref _avgPossession, value); }
        public string AvgShots { get => _avgShots; set => SetProperty(ref _avgShots, value); }
        public string AvgShotsOn { get => _avgShotsOn; set => SetProperty(ref _avgShotsOn, value); }
        public string AvgShotsOff { get => _avgShotsOff; set => SetProperty(ref _avgShotsOff, value); }
        public string AvgCorners { get => _avgCorners; set => SetProperty(ref _avgCorners, value); }
        public string AvgFouls { get => _avgFouls; set => SetProperty(ref _avgFouls, value); }
        public string TotalCleanSheets { get => _totalCleanSheets; set => SetProperty(ref _totalCleanSheets, value); }

        public int SelectedMonthIndex
        {
            get => _selectedMonthIndex;
            set
            {
                if (SetProperty(ref _selectedMonthIndex, value))
                {
                    _ = LoadStatsAsync();
                }
            }
        }

        public int SelectedYearIndex
        {
            get => _selectedYearIndex;
            set
            {
                if (SetProperty(ref _selectedYearIndex, value))
                {
                    _ = LoadStatsAsync();
                }
            }
        }

        public async Task LoadStatsAsync()
        {
            int? month = SelectedMonthIndex == 0 ? null : SelectedMonthIndex;

            int? year = null;
            if (SelectedYearIndex > 0 && int.TryParse(Years[SelectedYearIndex], out int y))
            {
                year = y;
            }

            var stats = await StatsService.GetAverageTeamStatsAsync(month, year);

            AvgGoals = stats.Gole.ToString();
            AvgPossession = $"{stats.PosiadaniePilki}%";
            AvgShots = stats.Strzaly.ToString();
            AvgShotsOn = stats.StrzalyCelne.ToString();
            AvgShotsOff = stats.StrzalyNiecelne.ToString();
            AvgCorners = stats.RzutyRozne.ToString();
            AvgFouls = stats.Faule.ToString();
            TotalCleanSheets = stats.CzysteKonto ? "Tak" : "Nie";
        }
    }
}
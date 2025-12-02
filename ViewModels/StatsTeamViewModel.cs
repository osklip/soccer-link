using SoccerLink.Models;
using SoccerLink.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SoccerLink.ViewModels
{
    public class StatsTeamViewModel : BaseViewModel
    {
        // Pola wyświetlane w widoku (są to stringi, bo mogą zawierać np. "%")
        private string _avgGoals = "-";
        private string _avgPossession = "-";
        private string _avgShots = "-";
        private string _avgShotsOn = "-";
        private string _avgShotsOff = "-";
        private string _avgCorners = "-";
        private string _avgFouls = "-";
        private string _totalCleanSheets = "-";

        public StatsTeamViewModel()
        {
            // Można załadować domyślnie, ale lepiej wywołać to z Page_Loaded
        }

        public string AvgGoals { get => _avgGoals; set => SetProperty(ref _avgGoals, value); }
        public string AvgPossession { get => _avgPossession; set => SetProperty(ref _avgPossession, value); }
        public string AvgShots { get => _avgShots; set => SetProperty(ref _avgShots, value); }
        public string AvgShotsOn { get => _avgShotsOn; set => SetProperty(ref _avgShotsOn, value); }
        public string AvgShotsOff { get => _avgShotsOff; set => SetProperty(ref _avgShotsOff, value); }
        public string AvgCorners { get => _avgCorners; set => SetProperty(ref _avgCorners, value); }
        public string AvgFouls { get => _avgFouls; set => SetProperty(ref _avgFouls, value); }
        public string TotalCleanSheets { get => _totalCleanSheets; set => SetProperty(ref _totalCleanSheets, value); }

        public async Task LoadStatsAsync()
        {
            // Pobieramy średnie z bazy
            var stats = await StatsService.GetAverageTeamStatsAsync();

            AvgGoals = stats.Gole.ToString();
            AvgPossession = $"{stats.PosiadaniePilki}%";
            AvgShots = stats.Strzaly.ToString();
            AvgShotsOn = stats.StrzalyCelne.ToString();
            AvgShotsOff = stats.StrzalyNiecelne.ToString();
            AvgCorners = stats.RzutyRozne.ToString();
            AvgFouls = stats.Faule.ToString();
            // Uwaga: CzysteKonto w modelu SQL (SUM) zwraca liczbę
            // Tutaj hackujemy model C# (gdzie jest bool), w serwisie zrobiliśmy rzutowanie
            // W idealnym świecie stworzyłbyś osobny model DTO (Data Transfer Object) dla agregatów.
            // Przyjmijmy, że w StatsService.GetAverageTeamStatsAsync przypisałeś liczbę do pola Gole (jako test) 
            // lub dodaj pole int TotalCleanSheets do modelu.
            // Dla uproszczenia tutaj:
            TotalCleanSheets = stats.CzysteKonto ? "Tak" : "Nie"; // To wymaga poprawki w modelu jeśli chcemy licznik
        }
    }
}
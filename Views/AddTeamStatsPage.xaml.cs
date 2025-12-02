using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Models;
using SoccerLink.ViewModels;

namespace SoccerLink.Views
{
    public sealed partial class AddTeamStatsPage : Page
    {
        public AddTeamStatsViewModel ViewModel { get; }

        public AddTeamStatsPage()
        {
            ViewModel = new AddTeamStatsViewModel();
            this.InitializeComponent();

            // Obs³uga nawigacji powrotnej wywo³ywanej z ViewModelu
            ViewModel.RequestClose += (s, e) =>
            {
                if (this.Frame.CanGoBack) this.Frame.GoBack();
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Mecz match)
            {
                ViewModel.Initialize(match);
                MatchTitleText.Text = $"{match.DataDisplay} vs {match.Przeciwnik}";
            }
        }
    }
}
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Models;
using SoccerLink.ViewModels;

namespace SoccerLink.Views
{
    public sealed partial class AddPlayerStatsPage : Page
    {
        public AddPlayerStatsViewModel ViewModel { get; }

        public AddPlayerStatsPage()
        {
            ViewModel = new AddPlayerStatsViewModel();
            this.InitializeComponent();

            // Obs³uga proœby o zamkniêcie (powrót) wys³anej z ViewModelu
            ViewModel.RequestClose += (s, e) =>
            {
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Przekazanie parametru (Meczu) do ViewModelu
            if (e.Parameter is Mecz match)
            {
                ViewModel.Initialize(match);
                // Opcjonalnie ustawiamy tytu³ w CodeBehind, jeœli nie bindujemy go w ViewModelu
                MatchTitleText.Text = $"{match.DataDisplay} vs {match.Przeciwnik}";
            }
        }
    }
}
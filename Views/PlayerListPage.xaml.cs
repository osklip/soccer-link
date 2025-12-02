using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SoccerLink.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayerListPage : Page
    {
        public PlayerListPage()
        {
            InitializeComponent();
            LoadPlayers();
        }

        private async void LoadPlayers()
        {
            try
            {
                // Wywo³anie serwisu do pobrania zawodników
                var players = await ZawodnikService.PobierzZawodnikowDlaAktualnegoTreneraAsync();

                // Przypisanie pobranej listy do kontrolki ListView
                PlayersListView.ItemsSource = players;
            }
            catch (Exception)
            {
                // TO DO: Lepsza obs³uga b³êdu (np. wyœwietlenie go w TextBlocku)
                // tymczasowo: ignorujemy b³¹d i lista bêdzie pusta
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Nawigacja powrotna do strony zarz¹dzania zespo³em
            this.Frame.Navigate(typeof(TeamManagementPage));
        }
    }
}

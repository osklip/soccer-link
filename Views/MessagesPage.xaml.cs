using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SoccerLink.Models;
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
    public sealed partial class MessagesPage : Page
    {
        private List<Wiadomosc> _wiadomosci;

        public MessagesPage()
        {
            InitializeComponent();
            ZaladujWiadomosci();
        }

        private async void ZaladujWiadomosci()
        {
            _wiadomosci = await WiadomoscService.PobierzWiadomosciDlaAktualnegoTreneraAsync();
            MessagesList.ItemsSource = _wiadomosci;   // ListView po lewej
        }

        private void MessagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var msg = (Wiadomosc)MessagesList.SelectedItem;
            if (msg is null) return;

            ToTextBlock.Text = $"Do: {msg.TypOdbiorcy}";
            FromTextBlock.Text = $"Od: {msg.NadawcaNazwa}";
            SubjectTextBlock.Text = $"Temat: {msg.Temat}";
            BodyTextBlock.Text = msg.Tresc;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new DashboardPage();
        }

        private void MessagesFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SessionService.AktualnyTrener == null || _wiadomosci == null)
                return;

            int trenerId = SessionService.AktualnyTrener.Id;

            var selectedTag = (MessagesFilterComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            // Domyœlnie traktujemy wszystko jako "odebrane"
            if (selectedTag == "sent") // Wys³ane
            {
                var wyslane = _wiadomosci
                    .Where(m => m.TypNadawcy == "Trener" && m.NadawcaID == trenerId)
                    .ToList();

                MessagesList.ItemsSource = wyslane;
            }
            else // "received" = Odebrane
            {
                var odebrane = _wiadomosci
                    .Where(m => m.TypOdbiorcy == "Trener" && m.OdbiorcaID == trenerId)
                    .ToList();

                MessagesList.ItemsSource = odebrane;
            }

            // czyœcimy szczegó³y, ¿eby nie wisia³a stara wiadomoœæ
            MessagesList.SelectedItem = null;
            ToTextBlock.Text = "";
            FromTextBlock.Text = "";
            SubjectTextBlock.Text = "";
            BodyTextBlock.Text = "";
        }

    }
}

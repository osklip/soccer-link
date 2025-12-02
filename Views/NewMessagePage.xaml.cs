using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLink.Views
{
    public sealed partial class NewMessagePage : Page
    {
        private List<Zawodnik> _allPlayers = new List<Zawodnik>();

        public NewMessagePage()
        {
            InitializeComponent();
            this.Loaded += NewMessagePage_Loaded;
            // Dodajemy obs³ugê zdarzenia SelectionChanged, aby aktualizowaæ licznik
            RecipientListView.SelectionChanged += RecipientListView_SelectionChanged;
        }

        private void NewMessagePage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRecipients();
        }

        private async void LoadRecipients()
        {
            RecipientListView.Visibility = Visibility.Collapsed;
            StatusTextBlock.Text = "£adowanie adresatów...";

            try
            {
                // Pobieranie listy zawodników dla aktualnego trenera
                _allPlayers = (await ZawodnikService.PobierzZawodnikowDlaAktualnegoTreneraAsync())
                                .OrderBy(z => z.Nazwisko)
                                .ToList();

                // Ustawienie Ÿród³a danych dla ListView
                RecipientListView.ItemsSource = _allPlayers;
                RecipientListView.DisplayMemberPath = "PelneImieNazwisko";

                RecipientListView.Visibility = Visibility.Visible;
                StatusTextBlock.Text = string.Empty;
                SelectedRecipientsTextBlock.Text = $"Wybrani: 0 zawodników";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"B³¹d ³adowania adresatów: {ex.Message}";
                RecipientListView.Visibility = Visibility.Collapsed;
            }
        }

        private void RecipientListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Aktualizacja licznika wybranych adresatów
            SelectedRecipientsTextBlock.Text = $"Wybrani: {RecipientListView.SelectedItems.Count} zawodników";
        }

        private void SendAllButton_Click(object sender, RoutedEventArgs e)
        {
            // Opcja "Wyœlij do wszystkich": zaznaczamy wszystkie elementy w ListView
            RecipientListView.SelectAll();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = string.Empty;

            var temat = SubjectTextBox.Text?.Trim();
            var tresc = BodyTextBox.Text?.Trim();

            var selectedRecipients = RecipientListView.SelectedItems.Cast<Zawodnik>().ToList();

            if (string.IsNullOrWhiteSpace(temat) || string.IsNullOrWhiteSpace(tresc))
            {
                StatusTextBlock.Text = "Temat i treœæ wiadomoœci s¹ wymagane.";
                return;
            }

            if (!selectedRecipients.Any())
            {
                StatusTextBlock.Text = "Musisz wybraæ przynajmniej jednego adresata.";
                return;
            }

            var recipientIds = selectedRecipients.Select(z => z.ZawodnikId).ToList();

            SendButton.IsEnabled = false;
            StatusTextBlock.Text = "Wysy³anie wiadomoœci...";

            try
            {
                // Wywo³anie serwisu do wysy³ki (logika pêtli jest w serwisie)
                //await WiadomoscService.SendMessagesAsync(recipientIds, temat, tresc);

                StatusTextBlock.Text = $"Wys³ano pomyœlnie do {recipientIds.Count} adresatów!";
                await Task.Delay(1000);

                // Powrót do skrzynki odbiorczej
                this.Content = new MessagesPage();
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"B³¹d wysy³ki: {ex.Message}";
            }
            finally
            {
                SendButton.IsEnabled = true;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new MessagesPage();
        }
    }
}
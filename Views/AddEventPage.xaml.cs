using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Threading.Tasks;

namespace SoccerLink.Views
{
    public sealed partial class AddEventPage : Page
    {
        // Usuniêto: public Frame ParentFrame { get; set; }

        public AddEventPage()
        {
            this.InitializeComponent();

            this.Loaded += AddEventPage_Loaded;
        }

        private void AddEventPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Natychmiastowe wywo³anie logiki ComboBox po za³adowaniu strony
            EventTypeComboBox_SelectionChanged(EventTypeComboBox, null);
        }

        private void EventTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // OCHRONA PRZED NULL
            if (NameTextBox == null || TimeEndTextBox == null || SpecificFieldsStackPanel == null)
            {
                return;
            }

            var selectedTag = (EventTypeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            // Ustawiamy domyœlne widocznoœci: koniec czasu jest widoczny, pola specyficzne ukryte
            SpecificFieldsStackPanel.Visibility = Visibility.Collapsed;
            TimeEndTextBox.Visibility = Visibility.Visible;

            NameTextBox.PlaceholderText = "Nazwa wydarzenia / Przeciwnik / Typ treningu";
            OpisTextBox.Text = string.Empty;


            switch (selectedTag)
            {
                case "Mecz":
                    TimeEndTextBox.Visibility = Visibility.Collapsed;
                    NameTextBox.PlaceholderText = "Przeciwnik (np. FC Dobre Wnioski)";
                    break;
                case "Trening":
                    NameTextBox.PlaceholderText = "Typ treningu (np. Taktyka / Strzelecki)";
                    break;
                case "Wydarzenie":
                default:
                    SpecificFieldsStackPanel.Visibility = Visibility.Visible;
                    NameTextBox.PlaceholderText = "Nazwa wydarzenia (np. Zebranie Zarz¹du)";
                    break;
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (SessionService.AktualnyTrener == null)
            {
                StatusTextBlock.Text = "B³¹d: Brak zalogowanego trenera.";
                return;
            }

            var type = (EventTypeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var nameOrType = NameTextBox.Text?.Trim();
            var date = DateDatePicker.Date?.ToString("yyyy-MM-dd");
            var place = MiejsceTextBox.Text?.Trim();
            var timeStart = TimeStartTextBox.Text?.Trim();
            var timeEnd = TimeEndTextBox.Text?.Trim();

            // Walidacja podstawowa
            if (string.IsNullOrWhiteSpace(nameOrType) || string.IsNullOrWhiteSpace(date) || string.IsNullOrWhiteSpace(place) || string.IsNullOrWhiteSpace(timeStart))
            {
                StatusTextBlock.Text = "Wype³nij wymagane pola (Nazwa/Typ, Data, Miejsce, Start).";
                return;
            }

            // Walidacja dla Treningu
            if (type == "Trening" && string.IsNullOrWhiteSpace(timeEnd))
            {
                StatusTextBlock.Text = "Podaj Godzinê zakoñczenia dla Treningu.";
                return;
            }

            try
            {
                switch (type)
                {
                    case "Mecz":
                        var mecz = new Mecz
                        {
                            Przeciwnik = nameOrType,
                            Data = date,
                            Godzina = timeStart,
                            Miejsce = place
                        };
                        await CalendarService.AddMeczAsync(mecz);
                        break;

                    case "Trening":
                        var trening = new Trening
                        {
                            Typ = nameOrType,
                            Data = date,
                            GodzinaRozpoczecia = timeStart,
                            GodzinaZakonczenia = timeEnd,
                            Miejsce = place
                        };
                        await CalendarService.AddTreningAsync(trening);
                        break;

                    case "Wydarzenie":
                    default:
                        var wydarzenie = new Wydarzenie
                        {
                            Nazwa = nameOrType,
                            Miejsce = place,
                            Data = date,
                            GodzinaStart = timeStart,
                            GodzinaKoniec = timeEnd ?? string.Empty,
                            Opis = OpisTextBox.Text?.Trim() ?? string.Empty
                        };
                        await CalendarService.AddWydarzenieAsync(wydarzenie);
                        break;
                }

                StatusTextBlock.Text = "Wydarzenie zosta³o dodane!";
                await Task.Delay(500);

                // NAWIGACJA: Powrót do CalendarPage
                this.Content = new CalendarPage();

            }
            catch (InvalidOperationException ex)
            {
                StatusTextBlock.Text = $"B³¹d: {ex.Message}";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"B³¹d: Nie uda³o siê dodaæ wydarzenia. SprawdŸ po³¹czenie i TOKEN: {ex.Message}";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // NAWIGACJA: Powrót do CalendarPage
            this.Content = new CalendarPage();
        }
    }
}
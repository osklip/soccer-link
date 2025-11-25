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
        public Frame ParentFrame { get; set; }

        public AddEventPage()
        {
            this.InitializeComponent();

            // U¿ywamy zdarzenia Loaded, aby zagwarantowaæ, ¿e wszystkie elementy XAML (w tym NameTextBox) s¹ zainicjowane
            this.Loaded += AddEventPage_Loaded;
        }

        private void AddEventPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Natychmiastowe wywo³anie logiki ComboBox po za³adowaniu strony
            // Jest to najbezpieczniejszy moment, aby manipulowaæ widocznoœci¹ i placeholderami.
            EventTypeComboBox_SelectionChanged(EventTypeComboBox, null);
        }

        private void EventTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Dodajemy sprawdzanie null, aby obs³u¿yæ ewentualne wczesne wywo³ania, chocia¿ Loaded powinno to naprawiæ
            if (NameTextBox == null || TimeEndTextBox == null || SpecificFieldsStackPanel == null)
            {
                return;
            }

            var selectedTag = (EventTypeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            // Ustawiamy domyœlne widocznoœci na podstawie typu
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

            if (string.IsNullOrWhiteSpace(nameOrType) || string.IsNullOrWhiteSpace(date) || string.IsNullOrWhiteSpace(place) || string.IsNullOrWhiteSpace(timeStart))
            {
                StatusTextBlock.Text = "Wype³nij wymagane pola (Nazwa/Typ, Data, Miejsce, Start).";
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
                        if (string.IsNullOrWhiteSpace(timeEnd))
                        {
                            StatusTextBlock.Text = "Podaj Godzinê zakoñczenia dla Treningu.";
                            return;
                        }
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

                if (ParentFrame != null && ParentFrame.CanGoBack)
                {
                    ParentFrame.GoBack();
                }
                else
                {
                    // U¿ywamy DashboardPage jako awaryjnej opcji nawigacji
                    if (this.Content is Frame frame) frame.Content = new DashboardPage();
                }
            }
            catch (InvalidOperationException ex)
            {
                StatusTextBlock.Text = $"B³¹d: {ex.Message}";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"B³¹d: Nie uda³o siê dodaæ wydarzenia. {ex.Message}";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ParentFrame != null && ParentFrame.CanGoBack)
            {
                ParentFrame.GoBack();
            }
            else
            {
                this.Content = new CalendarPage();
            }
        }
    }
}
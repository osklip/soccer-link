using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.ViewModels
{
    public class AddEventViewModel : BaseViewModel
    {
        // Pola formularza
        private int _selectedTypeIndex = 0; // 0: Wydarzenie, 1: Trening, 2: Mecz
        private string _title = string.Empty;
        private DateTimeOffset? _date = DateTimeOffset.Now;
        private string _location = string.Empty;
        private string _timeStart = string.Empty;
        private string _timeEnd = string.Empty;
        private string _description = string.Empty;
        private string _statusMessage = string.Empty;
        private string _statusColor = "Red"; // Kolor komunikatu

        // Właściwości sterujące UI (zależne od wybranego typu)
        private string _titlePlaceholder = "Nazwa wydarzenia";
        private bool _isTimeEndVisible = true;
        private bool _isDescriptionVisible = true;

        public AddEventViewModel()
        {
            // Inicjalizacja domyślnego stanu
            UpdateUiState();
        }

        // --- WŁAŚCIWOŚCI (Bindingi) ---

        public int SelectedTypeIndex
        {
            get => _selectedTypeIndex;
            set
            {
                if (SetProperty(ref _selectedTypeIndex, value))
                {
                    UpdateUiState(); // Zmiana typu aktualizuje widok
                }
            }
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public DateTimeOffset? Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public string TimeStart
        {
            get => _timeStart;
            set => SetProperty(ref _timeStart, value);
        }

        public string TimeEnd
        {
            get => _timeEnd;
            set => SetProperty(ref _timeEnd, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string StatusColor
        {
            get => _statusColor;
            set => SetProperty(ref _statusColor, value);
        }

        // --- WŁAŚCIWOŚCI UI ---

        public string TitlePlaceholder
        {
            get => _titlePlaceholder;
            set => SetProperty(ref _titlePlaceholder, value);
        }

        public bool IsTimeEndVisible
        {
            get => _isTimeEndVisible;
            set => SetProperty(ref _isTimeEndVisible, value);
        }

        public bool IsDescriptionVisible
        {
            get => _isDescriptionVisible;
            set => SetProperty(ref _isDescriptionVisible, value);
        }

        // --- LOGIKA ---

        private void UpdateUiState()
        {
            // Indeksy zgodne z kolejnością w ComboBox (Wydarzenie=0, Trening=1, Mecz=2)
            switch (_selectedTypeIndex)
            {
                case 2: // Mecz
                    TitlePlaceholder = "Przeciwnik (np. FC Dobre Wnioski)";
                    IsTimeEndVisible = false;
                    IsDescriptionVisible = false;
                    break;
                case 1: // Trening
                    TitlePlaceholder = "Typ treningu (np. Taktyka)";
                    IsTimeEndVisible = true;
                    IsDescriptionVisible = false;
                    break;
                case 0: // Wydarzenie (domyślne)
                default:
                    TitlePlaceholder = "Nazwa wydarzenia (np. Zebranie)";
                    IsTimeEndVisible = true;
                    IsDescriptionVisible = true;
                    break;
            }
        }

        public async Task<bool> AddEventAsync()
        {
            StatusMessage = "";
            StatusColor = "Red";

            if (SessionService.AktualnyTrener == null)
            {
                StatusMessage = "Błąd: Brak zalogowanego trenera.";
                return false;
            }

            // Walidacja podstawowa
            if (string.IsNullOrWhiteSpace(Title) ||
                !Date.HasValue ||
                string.IsNullOrWhiteSpace(Location) ||
                string.IsNullOrWhiteSpace(TimeStart))
            {
                StatusMessage = "Wypełnij wymagane pola (Nazwa, Data, Miejsce, Start).";
                return false;
            }

            // Walidacja dla Treningu (musi mieć koniec)
            if (_selectedTypeIndex == 1 && string.IsNullOrWhiteSpace(TimeEnd))
            {
                StatusMessage = "Podaj godzinę zakończenia dla treningu.";
                return false;
            }

            string dateStr = Date.Value.ToString("yyyy-MM-dd");

            try
            {
                switch (_selectedTypeIndex)
                {
                    case 2: // Mecz
                        var mecz = new Mecz
                        {
                            Przeciwnik = Title.Trim(),
                            Data = dateStr,
                            Godzina = TimeStart.Trim(),
                            Miejsce = Location.Trim()
                        };
                        await CalendarService.AddMeczAsync(mecz);
                        break;

                    case 1: // Trening
                        var trening = new Trening
                        {
                            Typ = Title.Trim(),
                            Data = dateStr,
                            GodzinaRozpoczecia = TimeStart.Trim(),
                            GodzinaZakonczenia = TimeEnd.Trim(),
                            Miejsce = Location.Trim()
                        };
                        await CalendarService.AddTreningAsync(trening);
                        break;

                    case 0: // Wydarzenie
                    default:
                        var wydarzenie = new Wydarzenie
                        {
                            Nazwa = Title.Trim(),
                            Miejsce = Location.Trim(),
                            Data = dateStr,
                            GodzinaStart = TimeStart.Trim(),
                            GodzinaKoniec = TimeEnd?.Trim() ?? "",
                            Opis = Description?.Trim() ?? ""
                        };
                        await CalendarService.AddWydarzenieAsync(wydarzenie);
                        break;
                }

                StatusColor = "Green";
                StatusMessage = "Wydarzenie zostało dodane!";
                return true; // Sukces
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd zapisu: {ex.Message}";
                return false;
            }
        }
    }
}

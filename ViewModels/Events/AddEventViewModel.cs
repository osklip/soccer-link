using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace SoccerLink.ViewModels.Events
{
    public class AddEventViewModel : BaseViewModel
    {
        private int _selectedTypeIndex = 0; 
        private string _title = string.Empty;
        private DateTimeOffset? _date = DateTimeOffset.Now;
        private string _location = string.Empty;
        private string _timeStart = string.Empty; 
        private string _timeEnd = string.Empty;   
        private string _description = string.Empty;
        private string _statusMessage = string.Empty;
        private string _statusColor = "Red";

        private string _titlePlaceholder = "Nazwa wydarzenia";
        private bool _isTimeEndVisible = true;
        private bool _isDescriptionVisible = true;

        public AddEventViewModel()
        {
            UpdateUiState();
        }

        

        public int SelectedTypeIndex { get => _selectedTypeIndex; set { if (SetProperty(ref _selectedTypeIndex, value)) UpdateUiState(); } }
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public DateTimeOffset? Date { get => _date; set => SetProperty(ref _date, value); }
        public string Location { get => _location; set => SetProperty(ref _location, value); }
        public string TimeStart { get => _timeStart; set => SetProperty(ref _timeStart, value); }
        public string TimeEnd { get => _timeEnd; set => SetProperty(ref _timeEnd, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
        public string StatusColor { get => _statusColor; set => SetProperty(ref _statusColor, value); }

        public string TitlePlaceholder { get => _titlePlaceholder; set => SetProperty(ref _titlePlaceholder, value); }
        public bool IsTimeEndVisible { get => _isTimeEndVisible; set => SetProperty(ref _isTimeEndVisible, value); }
        public bool IsDescriptionVisible { get => _isDescriptionVisible; set => SetProperty(ref _isDescriptionVisible, value); }

        private void UpdateUiState()
        {
            switch (_selectedTypeIndex)
            {
                case 2: 
                    TitlePlaceholder = "Przeciwnik (np. FC Dobre Wnioski)";
                    IsTimeEndVisible = false;
                    IsDescriptionVisible = false;
                    break;
                case 1: 
                    TitlePlaceholder = "Typ treningu (np. Taktyka)";
                    IsTimeEndVisible = true;
                    IsDescriptionVisible = false;
                    break;
                case 0:
                default: 
                    TitlePlaceholder = "Nazwa wydarzenia (np. Zebranie)";
                    IsTimeEndVisible = true;
                    IsDescriptionVisible = true;
                    break;
            }
        }

        
        private DateTime? ParseDateTime(DateTimeOffset? date, string time)
        {
            if (!date.HasValue || string.IsNullOrWhiteSpace(time)) return null;

            string dateStr = date.Value.ToString("yyyy-MM-dd");
            string fullStr = $"{dateStr} {time.Trim()}";

            if (DateTime.TryParseExact(fullStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            
            if (DateTime.TryParseExact(fullStr, "yyyy-MM-dd H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result2))
            {
                return result2;
            }
            return null;
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

            if (string.IsNullOrWhiteSpace(Title) || !Date.HasValue || string.IsNullOrWhiteSpace(Location) || string.IsNullOrWhiteSpace(TimeStart))
            {
                StatusMessage = "Wypełnij wymagane pola.";
                return false;
            }

            
            DateTime? startDt = ParseDateTime(Date, TimeStart);
            if (startDt == null)
            {
                StatusMessage = "Niepoprawny format godziny rozpoczęcia (HH:mm).";
                return false;
            }

           
            if (startDt < DateTime.Now)
            {
                StatusMessage = "Nie można dodawać wydarzeń z przeszłości.";
                return false;
            }
           

            
            DateTime endDt = startDt.Value; 
            if (_selectedTypeIndex != 2) 
            {
                if (string.IsNullOrWhiteSpace(TimeEnd))
                {
                    StatusMessage = "Podaj godzinę zakończenia.";
                    return false;
                }

                var parsedEnd = ParseDateTime(Date, TimeEnd);
                if (parsedEnd == null)
                {
                    StatusMessage = "Niepoprawny format godziny zakończenia.";
                    return false;
                }
                endDt = parsedEnd.Value;

                if (endDt < startDt)
                {
                    StatusMessage = "Koniec nie może być przed początkiem.";
                    return false;
                }
            }

            try
            {
                switch (_selectedTypeIndex)
                {
                    case 2: 
                        var mecz = new Mecz
                        {
                            Przeciwnik = Title.Trim(),
                            DataRozpoczecia = startDt.Value,
                            Miejsce = Location.Trim()
                        };
                        await CalendarService.AddMeczAsync(mecz);
                        break;

                    case 1: 
                        var trening = new Trening
                        {
                            Typ = Title.Trim(),
                            DataRozpoczecia = startDt.Value,
                            DataZakonczenia = endDt,
                            Miejsce = Location.Trim()
                        };
                        await CalendarService.AddTreningAsync(trening);
                        break;

                    case 0:
                    default: 
                        var wydarzenie = new Wydarzenie
                        {
                            Nazwa = Title.Trim(),
                            Miejsce = Location.Trim(),
                            DataRozpoczecia = startDt.Value,
                            DataZakonczenia = endDt,
                            Opis = Description?.Trim() ?? ""
                        };
                        await CalendarService.AddWydarzenieAsync(wydarzenie);
                        break;
                }

                StatusColor = "Green";
                StatusMessage = "Dodano pomyślnie!";
                return true;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd zapisu: {ex.Message}";
                return false;
            }
        }
    }
}
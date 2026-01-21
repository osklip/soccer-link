using SoccerLink.Helpers;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels.Events
{
    public class EditEventViewModel : BaseViewModel
    {
        private int _eventId;
        private string _eventType;
        private UpcomingEvent _originalEvent;

        
        private string _title;
        private string _location;
        private DateTimeOffset? _date;
        private string _timeStart;
        private string _timeEnd;
        private string _description;

        
        private string _headerText = "Edycja Wydarzenia";
        private string _eventInfoText = "";
        private string _titleLabel = "Nazwa";
        private bool _isTimeEndVisible;
        private bool _isDescriptionVisible;
        private string _statusMessage;
        private string _statusColor = "Red";

        public event EventHandler RequestNavigateBack;

        public EditEventViewModel()
        {
            SaveCommand = new RelayCommand(SaveAsync);
            CancelCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
        }

        

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Location { get => _location; set => SetProperty(ref _location, value); }
        public DateTimeOffset? Date { get => _date; set => SetProperty(ref _date, value); }
        public string TimeStart { get => _timeStart; set => SetProperty(ref _timeStart, value); }
        public string TimeEnd { get => _timeEnd; set => SetProperty(ref _timeEnd, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }
        public string EventType { get => _eventType; set => SetProperty(ref _eventType, value); } 

        public string HeaderText { get => _headerText; set => SetProperty(ref _headerText, value); }
        public string EventInfoText { get => _eventInfoText; set => SetProperty(ref _eventInfoText, value); }
        public string TitleLabel { get => _titleLabel; set => SetProperty(ref _titleLabel, value); }

        public bool IsTimeEndVisible { get => _isTimeEndVisible; set => SetProperty(ref _isTimeEndVisible, value); }
        public bool IsDescriptionVisible { get => _isDescriptionVisible; set => SetProperty(ref _isDescriptionVisible, value); }

        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
        public string StatusColor { get => _statusColor; set => SetProperty(ref _statusColor, value); }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        

        public async Task LoadEventAsync(int eventId, string eventType)
        {
            _eventId = eventId;
            EventType = eventType;
            StatusMessage = "Ładowanie...";
            StatusColor = "#E6F6FF";

            try
            {
                var allEvents = await CalendarService.GetAllEventsAsync();
                _originalEvent = allEvents.FirstOrDefault(e => e.Id == _eventId && e.EventType == _eventType);

                if (_originalEvent == null)
                {
                    StatusColor = "Red";
                    StatusMessage = "Nie znaleziono wydarzenia.";
                    return;
                }

                
                Title = _originalEvent.Title;
                Location = _originalEvent.Location;
                Date = _originalEvent.DateTimeStart;
                TimeStart = _originalEvent.DisplayTimeStart;
                TimeEnd = _originalEvent.TimeEnd;
                Description = _originalEvent.Description;

                EventInfoText = $"Edytujesz: {_eventType} (ID: {_eventId})";
                StatusMessage = "";

                SetupUiForType(_eventType);
            }
            catch (Exception ex)
            {
                StatusColor = "Red";
                StatusMessage = $"Błąd: {ex.Message}";
            }
        }

        private void SetupUiForType(string type)
        {
            switch (type)
            {
                case "Mecz":
                    TitleLabel = "Przeciwnik";
                    IsTimeEndVisible = false;
                    IsDescriptionVisible = false;
                    break;
                case "Trening":
                    TitleLabel = "Typ treningu";
                    IsTimeEndVisible = true;
                    IsDescriptionVisible = false;
                    break;
                default: 
                    TitleLabel = "Nazwa wydarzenia";
                    IsTimeEndVisible = true;
                    IsDescriptionVisible = true;
                    break;
            }
        }

        private async void SaveAsync()
        {
            StatusMessage = "";
            StatusColor = "Red";

            if (_originalEvent == null) return;

            if (string.IsNullOrWhiteSpace(Title))
            {
                StatusMessage = "Tytuł/Nazwa jest wymagana.";
                return;
            }

            if (_eventType == "Trening" && string.IsNullOrWhiteSpace(TimeEnd))
            {
                StatusMessage = "Dla Treningu wymagana jest godzina zakończenia.";
                return;
            }

            
            if (!Date.HasValue)
            {
                StatusMessage = "Data jest wymagana.";
                return;
            }

            string dateStr = Date.Value.ToString("yyyy-MM-dd");
            string fullDateStr = $"{dateStr} {TimeStart?.Trim()}";

            if (!DateTime.TryParseExact(fullDateStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newStart))
            {
                
                if (!DateTime.TryParseExact(fullDateStr, "yyyy-MM-dd H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out newStart))
                {
                    StatusMessage = "Niepoprawny format godziny rozpoczęcia (HH:mm).";
                    return;
                }
            }

            
            _originalEvent.Title = Title.Trim();
            _originalEvent.Location = Location?.Trim() ?? "";
            _originalEvent.DateTimeStart = newStart;
            _originalEvent.TimeEnd = TimeEnd?.Trim() ?? "";
            _originalEvent.Description = Description?.Trim() ?? "";

            try
            {
                await CalendarService.UpdateEventAsync(_originalEvent);

                StatusColor = "Green";
                StatusMessage = "Zapisano pomyślnie!";
                await Task.Delay(500);
                RequestNavigateBack?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd zapisu: {ex.Message}";
            }
        }
    }
}
using SoccerLink.Helpers;
using SoccerLink.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels
{
    public class ConfirmDeleteViewModel : BaseViewModel
    {
        private string _eventType;
        private int _eventId;
        private string _confirmationText = "Czy na pewno chcesz usunąć to wydarzenie?";
        private bool _isDeleting;

        public event EventHandler RequestNavigateBack;

        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }

        public ConfirmDeleteViewModel()
        {
            // Komenda usuwania - blokuje przycisk, jeśli trwa usuwanie (_isDeleting)
            DeleteCommand = new RelayCommand(DeleteAsync, () => !_isDeleting);

            // Komenda anulowania - po prostu wraca
            CancelCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
        }

        public string ConfirmationText
        {
            get => _confirmationText;
            set => SetProperty(ref _confirmationText, value);
        }

        // Ta metoda jest kluczowa - musi zostać wywołana z widoku (CodeBehind)
        public void Initialize(string eventType, int eventId)
        {
            _eventType = eventType;
            _eventId = eventId;
            ConfirmationText = $"Czy na pewno usunąć wydarzenie typu '{_eventType}' o ID: {_eventId}? Tej operacji nie można cofnąć.";
        }

        private async void DeleteAsync()
        {
            if (_eventId <= 0)
            {
                ConfirmationText = "Błąd: Nieprawidłowe ID wydarzenia.";
                return;
            }

            _isDeleting = true;
            (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            ConfirmationText = "Usuwanie w toku...";

            try
            {
                // Wywołanie serwisu
                await CalendarService.DeleteEventAsync(_eventType, _eventId);

                ConfirmationText = "Usunięto pomyślnie! Powrót...";
                await Task.Delay(1000); // Krótkie opóźnienie, żeby użytkownik przeczytał komunikat

                // Powrót do kalendarza
                RequestNavigateBack?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ConfirmationText = $"Błąd podczas usuwania: {ex.Message}";
                _isDeleting = false;
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }
}
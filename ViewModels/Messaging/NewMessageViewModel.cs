using SoccerLink.Helpers;
using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoccerLink.ViewModels.Messaging
{
   
    public class RecipientItemViewModel : BaseViewModel
    {
        public Zawodnik Player { get; }
        private bool _isSelected;

        public RecipientItemViewModel(Zawodnik player)
        {
            Player = player;
        }

        public string DisplayName => $"{Player.Imie} {Player.Nazwisko}";

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    public class NewMessageViewModel : BaseViewModel
    {
        private string _subject = string.Empty;
        private string _body = string.Empty;
        private string _statusMessage = string.Empty;
        private string _statusColor = "Red";
        private string _recipientsCountText = "Wybrani: 0 zawodników";
        private bool _isSending;

        public ObservableCollection<RecipientItemViewModel> Recipients { get; } = new();

        public event EventHandler RequestNavigateBack;

        public NewMessageViewModel()
        {
            SendAllCommand = new RelayCommand(SelectAll);
            SendCommand = new RelayCommand(SendAsync, () => !_isSending);
            CancelCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
        }

        
        public string Subject { get => _subject; set => SetProperty(ref _subject, value); }
        public string Body { get => _body; set => SetProperty(ref _body, value); }

        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
        public string StatusColor { get => _statusColor; set => SetProperty(ref _statusColor, value); }

        public string RecipientsCountText { get => _recipientsCountText; set => SetProperty(ref _recipientsCountText, value); }

        public ICommand SendAllCommand { get; }
        public ICommand SendCommand { get; }
        public ICommand CancelCommand { get; }

        

        public async Task LoadRecipientsAsync()
        {
            StatusMessage = "Ładowanie adresatów...";
            StatusColor = "#E6F6FF"; 
            Recipients.Clear();

            try
            {
                var players = await ZawodnikService.PobierzZawodnikowDlaAktualnegoTreneraAsync();

                foreach (var p in players.OrderBy(x => x.Nazwisko))
                {
                    var item = new RecipientItemViewModel(p);
                    
                    item.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(RecipientItemViewModel.IsSelected))
                            UpdateCount();
                    };
                    Recipients.Add(item);
                }

                StatusMessage = "";
                UpdateCount();
            }
            catch (Exception ex)
            {
                StatusColor = "Red";
                StatusMessage = $"Błąd ładowania: {ex.Message}";
            }
        }

        private void SelectAll()
        {
            foreach (var item in Recipients)
            {
                item.IsSelected = true;
            }
        }

        private void UpdateCount()
        {
            int count = Recipients.Count(x => x.IsSelected);
            RecipientsCountText = $"Wybrani: {count} zawodników";
        }

        private async void SendAsync()
        {
            StatusMessage = "";
            StatusColor = "Red";

            
            if (string.IsNullOrWhiteSpace(Subject) || string.IsNullOrWhiteSpace(Body))
            {
                StatusMessage = "Temat i treść wiadomości są wymagane.";
                return;
            }

            var selectedIds = Recipients
                .Where(r => r.IsSelected)
                .Select(r => r.Player.ZawodnikId)
                .ToList();

            if (!selectedIds.Any())
            {
                StatusMessage = "Musisz wybrać przynajmniej jednego adresata.";
                return;
            }

            
            _isSending = true;
            (SendCommand as RelayCommand)?.RaiseCanExecuteChanged();
            StatusColor = "#E6F6FF";
            StatusMessage = "Wysyłanie wiadomości...";

            try
            {
                await WiadomoscService.SendMessagesAsync(selectedIds, Subject.Trim(), Body.Trim());

                StatusColor = "Green";
                StatusMessage = $"Wysłano pomyślnie do {selectedIds.Count} adresatów!";

                await Task.Delay(1000);
                RequestNavigateBack?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                StatusColor = "Red";
                StatusMessage = $"Błąd wysyłki: {ex.Message}";
            }
            finally
            {
                _isSending = false;
                (SendCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }
}
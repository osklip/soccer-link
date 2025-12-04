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
    public class MessagesViewModel : BaseViewModel
    {
        private List<Wiadomosc> _allMessages = new();
        private ObservableCollection<Wiadomosc> _filteredMessages = new();
        private Wiadomosc? _selectedMessage;
        private string _filterType = "received";

        public event EventHandler RequestNavigateBack;
        public event EventHandler RequestNavigateToNewMessage;

        public ICommand GoBackCommand { get; }
        public ICommand CreateNewMessageCommand { get; }

        public MessagesViewModel()
        {
            GoBackCommand = new RelayCommand(() => RequestNavigateBack?.Invoke(this, EventArgs.Empty));
            CreateNewMessageCommand = new RelayCommand(() => RequestNavigateToNewMessage?.Invoke(this, EventArgs.Empty));
        }

        public ObservableCollection<Wiadomosc> FilteredMessages { get => _filteredMessages; set => SetProperty(ref _filteredMessages, value); }
        public Wiadomosc? SelectedMessage { get => _selectedMessage; set => SetProperty(ref _selectedMessage, value); }

        public async Task LoadMessagesAsync()
        {
            _allMessages = await WiadomoscService.PobierzWiadomosciDlaAktualnegoTreneraAsync();
            ApplyFilter(_filterType);
        }

        public void ApplyFilter(string filterTag)
        {
            _filterType = filterTag;
            if (SessionService.AktualnyTrener == null) return;
            int trenerId = SessionService.AktualnyTrener.Id;
            var result = _filterType == "sent"
                ? _allMessages.Where(m => m.TypNadawcy == "Trener" && m.NadawcaID == trenerId).ToList()
                : _allMessages.Where(m => m.TypOdbiorcy == "Trener" && m.OdbiorcaID == trenerId).ToList();
            FilteredMessages = new ObservableCollection<Wiadomosc>(result);
            SelectedMessage = null;
        }
    }
}
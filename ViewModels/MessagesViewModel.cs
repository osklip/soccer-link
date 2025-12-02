using SoccerLink.Models;
using SoccerLink.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.ViewModels
{
    public class MessagesViewModel : BaseViewModel
    {
        private List<Wiadomosc> _allMessages = new();
        private ObservableCollection<Wiadomosc> _filteredMessages = new();
        private Wiadomosc? _selectedMessage;
        private string _filterType = "received"; // "received" lub "sent"

        public MessagesViewModel()
        {
            // Konstruktor - inicjalizacja
        }

        // Kolekcja, do której bindować będzie widok (ListView)
        public ObservableCollection<Wiadomosc> FilteredMessages
        {
            get => _filteredMessages;
            set => SetProperty(ref _filteredMessages, value);
        }

        // Aktualnie wybrana wiadomość (szczegóły)
        public Wiadomosc? SelectedMessage
        {
            get => _selectedMessage;
            set => SetProperty(ref _selectedMessage, value);
        }

        // Metoda ładowania danych
        public async Task LoadMessagesAsync()
        {
            // Używamy serwisu (który zaktualizowaliśmy w kroku 1 o DatabaseConfig)
            _allMessages = await WiadomoscService.PobierzWiadomosciDlaAktualnegoTreneraAsync();
            ApplyFilter(_filterType);
        }

        // Logika filtrowania
        public void ApplyFilter(string filterTag)
        {
            _filterType = filterTag;
            if (SessionService.AktualnyTrener == null) return;

            int trenerId = SessionService.AktualnyTrener.Id;
            List<Wiadomosc> result;

            if (_filterType == "sent")
            {
                result = _allMessages
                    .Where(m => m.TypNadawcy == "Trener" && m.NadawcaID == trenerId)
                    .ToList();
            }
            else // "received"
            {
                result = _allMessages
                    .Where(m => m.TypOdbiorcy == "Trener" && m.OdbiorcaID == trenerId)
                    .ToList();
            }

            // Aktualizacja kolekcji widoku
            FilteredMessages = new ObservableCollection<Wiadomosc>(result);

            // Reset wyboru
            SelectedMessage = null;
        }
    }
}

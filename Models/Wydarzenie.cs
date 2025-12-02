using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Models
{
    class Wydarzenie
    {
        public int WydarzenieID { get; set; } // WydarzenieID INTEGER PRIMARY KEY AUTOINCREMENT
        public string Nazwa { get; set; } // Nazwa TEXT NOT NULL
        public string Miejsce { get; set; } // Miejsce TEXT NOT NULL
        public string Data { get; set; } // Data TEXT NOT NULL
        public DateTime DataRozpoczecia { get; set; }
        public DateTime DataZakonczenia { get; set; }
        public string DataDisplay => DataRozpoczecia.ToString("dd.MM.yyyy");
        public string GodzinaRangeDisplay => $"{DataRozpoczecia:HH:mm} - {DataZakonczenia:HH:mm}";
        public string Opis { get; set; } // Opis TEXT
        public int TrenerID { get; set; } // TrenerID INTEGER NOT NULL
    }
}

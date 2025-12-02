using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Models
{
    class Trening
    {
        public int TreningID { get; set; } // TreningID INTEGER PRIMARY KEY AUTOINCREMENT
        public string Typ { get; set; } // Typ TEXT NOT NULL
        public int ListaObecnosciID { get; set; } // ListaObecnosciID INTEGER NOT NULL
        public DateTime DataRozpoczecia { get; set; }
        public DateTime DataZakonczenia { get; set; }

        public string DataDisplay => DataRozpoczecia.ToString("dd.MM.yyyy");
        public string GodzinaRangeDisplay => $"{DataRozpoczecia:HH:mm} - {DataZakonczenia:HH:mm}";
        public string Miejsce { get; set; } // Miejsce TEXT NOT NULL
        public int TrenerID { get; set; } // TrenerID INTEGER NOT NULL
    }
}

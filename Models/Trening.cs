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
        public string Data { get; set; } // Data TEXT NOT NULL
        public string GodzinaRozpoczecia { get; set; } // GodzinaRozpoczecia TEXT NOT NULL
        public string GodzinaZakonczenia { get; set; } // GodzinaZakonczenia TEXT NOT NULL
        public string Miejsce { get; set; } // Miejsce TEXT NOT NULL
        public int TrenerID { get; set; } // TrenerID INTEGER NOT NULL
    }
}

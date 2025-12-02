using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Models
{
    class Mecz
    {
        public int MeczID { get; set; } // MeczyID INTEGER PRIMARY KEY AUTOINCREMENT
        public int SkladMeczowyID { get; set; } // SkladMeczowyID INTEGER NOT NULL
        public string Przeciwnik { get; set; } // Przeciwnik TEXT NOT NULL
        public DateTime DataRozpoczecia { get; set; } // Data TEXT NOT NULL
        public string DataDisplay => DataRozpoczecia.ToString("dd.MM.yyyy");
        public string GodzinaDisplay => DataRozpoczecia.ToString("HH:mm"); // Godzina TEXT NOT NULL
        public string Miejsce { get; set; } // Miejsce TEXT NOT NULL
        public int TrenerID { get; set; } // TrenerID INTEGER NOT NULL
    }
}

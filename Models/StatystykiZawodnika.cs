using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Models
{
    public class StatystykiZawodnika
    {
        public int StatZawodnikaID { get; set; }
        public int MeczID { get; set; }
        public int ZawodnikID { get; set; }
        public int Gole { get; set; }
        public int Strzaly { get; set; }
        public int StrzalyCelne { get; set; }
        public int StrzalyNiecelne { get; set; }
        public int PodaniaCelne { get; set; } // Wcześniej w UI było "Passes"
        public int Faule { get; set; }
        public int ZolteKartki { get; set; }
        public bool CzerwonaKartka { get; set; } // W bazie 0/1, tutaj bool dla wygody
        public bool CzysteKonto { get; set; }
    }
}

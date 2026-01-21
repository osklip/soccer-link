using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Models
{
    class Wydarzenie
    {
        public int WydarzenieID { get; set; } 
        public string Nazwa { get; set; } 
        public string Miejsce { get; set; } 
        public string Data { get; set; } 
        public DateTime DataRozpoczecia { get; set; }
        public DateTime DataZakonczenia { get; set; }
        public string DataDisplay => DataRozpoczecia.ToString("dd.MM.yyyy");
        public string GodzinaRangeDisplay => $"{DataRozpoczecia:HH:mm} - {DataZakonczenia:HH:mm}";
        public string Opis { get; set; } 
        public int TrenerID { get; set; } 
    }
}

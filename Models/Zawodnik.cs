using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Models
{
    internal class Zawodnik
    {
        public int ZawodnikId { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Pozycja { get; set; }
        public string NumerTelefonu { get; set; }
        public string AdresEmail { get; set; }
        public int CzyDyspozycyjny { get; set; }
        public int NumerKoszulki { get; set; }
        public string LepszaNoga { get; set; }
        public int TrenerId { get; set; }  
    }
}

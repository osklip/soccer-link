using System;

namespace SoccerLink.Models
{
    public class MeczDostepnosc
    {
        public int DostepnoscID { get; set; }
        public int MeczID { get; set; }
        public int ZawodnikID { get; set; }
        // 0 = Oczekujący, 1 = Potwierdzony, 2 = Niedostępny
        public int Status { get; set; }
        public string DataZgloszenia { get; set; }
    }
}
namespace SoccerLink.Models
{
    public class SkladEntry
    {
        public int SkladID { get; set; }
        public int MeczID { get; set; }
        public int ZawodnikID { get; set; }
        // Kod pozycji, np. "GK" (Bramkarz), "ST" (Napastnik), "CB_L" (Lewy stoper)
        public string PozycjaKod { get; set; }
    }
}
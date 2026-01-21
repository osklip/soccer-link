using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Models
{
    public class Wiadomosc
    {
        public int WiadomoscID { get; set; }
        public string TypNadawcy { get; set; }
        public int NadawcaID { get; set; }
        public string TypOdbiorcy { get; set; }
        public int OdbiorcaID { get; set; }
        public string Tresc { get; set; }
        public string DataWyslania { get; set; }   
        public string Temat { get; set; }
        public string NadawcaNazwa { get; set; }
    }
}

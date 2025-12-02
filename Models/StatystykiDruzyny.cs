using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Models
{
    public class StatystykiDruzyny
    {
        public int StatDruzynyID { get; set; }
        public int MeczID { get; set; }
        public int Gole { get; set; }
        public int PosiadaniePilki { get; set; }
        public int Strzaly { get; set; }
        public int StrzalyCelne { get; set; }
        public int StrzalyNiecelne { get; set; }
        public int RzutyRozne { get; set; }
        public int Faule { get; set; }
        public bool CzysteKonto { get; set; }
    }
}

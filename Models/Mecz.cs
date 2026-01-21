using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Models
{
    public class Mecz
    {
        public int MeczID { get; set; } 
        public int SkladMeczowyID { get; set; } 
        public string Przeciwnik { get; set; } 
        public DateTime DataRozpoczecia { get; set; } 
        public string DataDisplay => DataRozpoczecia.ToString("dd.MM.yyyy");
        public string GodzinaDisplay => DataRozpoczecia.ToString("HH:mm");
        public string Miejsce { get; set; } 
        public int TrenerID { get; set; } 
    }
}

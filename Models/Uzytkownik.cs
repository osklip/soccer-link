using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Models
{
    internal class Uzytkownik
    {
        public int UzytkownikId { get; set; }
        public string Email { get; set; }
        public string Haslo { get; set; }
        public int Rola { get; set; }
    }
}

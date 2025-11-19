using SoccerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    class SessionService
    {
        public static ZalogowanyTrener? AktualnyTrener { get; private set; }

        public static bool IsLoggedIn => AktualnyTrener != null;

        public static void SetUser(ZalogowanyTrener user)
        {
            AktualnyTrener = user;
        }

        public static void Logout()
        {
            AktualnyTrener = null;
        }
    }
}

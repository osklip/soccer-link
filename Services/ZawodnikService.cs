using Libsql.Client;
using SoccerLink.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    class ZawodnikService
    {
        public static async Task<List<Zawodnik>> PobierzZawodnikowDlaAktualnegoTreneraAsync()
        {
            if (SessionService.AktualnyTrener == null) return new List<Zawodnik>();
            var trenerId = SessionService.AktualnyTrener.Id;

            using var client = await DatabaseConfig.CreateClientAsync();

            // Zastąpienie @trenerId znakiem '?'
            var sql = @"
                SELECT ZawodnikID, Imie, Nazwisko, Pozycja, NumerTelefonu, AdresEmail, CzyDyspozycyjny, NumerKoszulki
                FROM Zawodnik
                WHERE TrenerID = ?
                ORDER BY Nazwisko, Imie;
            ";

            var result = await client.Execute(sql, trenerId);
            var list = new List<Zawodnik>();

            if (result.Rows == null) return list;

            foreach (var row in result.Rows)
            {
                var c = row.ToArray();
                list.Add(new Zawodnik
                {
                    ZawodnikId = int.Parse(c[0].ToString()),
                    Imie = c[1]?.ToString() ?? "",
                    Nazwisko = c[2]?.ToString() ?? "",
                    Pozycja = c[3]?.ToString() ?? "",
                    NumerTelefonu = c[4]?.ToString() ?? "",
                    AdresEmail = c[5]?.ToString() ?? "",
                    CzyDyspozycyjny = int.Parse(c[6].ToString()),
                    NumerKoszulki = int.Parse(c[7].ToString())
                });
            }
            return list;
        }
    }
}
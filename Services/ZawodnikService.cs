using Libsql.Client;
using SoccerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    class ZawodnikService
    {
        private const string Url = "https://soccerlinkdb-enbixd.aws-eu-west-1.turso.io";
        private const string Token = "eyJhbGciOiJFZERTQSIsInR5cCI6IkpXVCJ9.eyJhIjoicnciLCJnaWQiOiJjYTliNTRlNy0zMGNkLTQwOWEtOGEzMy03MjJkZmQxYmFmNGIiLCJpYXQiOjE3NjEzNDEwOTQsInJpZCI6ImEwYjU0YzNmLWZmZGMtNDIyMi1iNmExLWRkYWU3MTdiNTJmOCJ9.dbND6Ysq3h8RphlNnJF9f8TFNdgwyWsHNADEPWDi_iKlwJHGWPBmUIaKuEuWlU_QdvvQSkcf8SN_OWGNoWP4DQ";

        public static async Task<List<Zawodnik>> PobierzZawodnikowDlaAktualnegoTreneraAsync()
        {
            if (SessionService.AktualnyTrener == null)
                return new List<Zawodnik>();

            var trenerId = SessionService.AktualnyTrener.Id;

            using var client = await DatabaseClient.Create(o =>
            {
                o.Url = Url;
                o.AuthToken = Token;
                o.UseHttps = true;
            });

            // TO DO: Należy zaimplementować logikę powiązania zawodników z TrenerID
            // ZAKŁADAMY, że tabela Zawodnik ma kolumnę TrenerID lub jest tabela pośrednicząca
            var sql = $@"
                        SELECT 
                            ZawodnikID, Imie, Nazwisko, Pozycja, NumerTelefonu, AdresEmail, CzyDyspozycyjny, NumerKoszulki
                        FROM Zawodnik
                        WHERE TrenerID = {trenerId} -- Zmodyfikuj tę klauzulę, jeśli masz inną strukturę
                        ORDER BY Nazwisko, Imie;
                    ";

            var result = await client.Execute(sql);

            var list = new List<Zawodnik>();

            if (result.Rows == null)
                return list;

            foreach (var row in result.Rows)
            {
                var cells = row.ToArray();

                list.Add(new Zawodnik
                {
                    ZawodnikId = int.Parse(cells[0].ToString()),
                    Imie = cells[1]?.ToString() ?? string.Empty,
                    Nazwisko = cells[2]?.ToString() ?? string.Empty,
                    Pozycja = cells[3]?.ToString() ?? string.Empty,
                    NumerTelefonu = cells[4]?.ToString() ?? string.Empty,
                    AdresEmail = cells[5]?.ToString() ?? string.Empty,
                    CzyDyspozycyjny = int.Parse(cells[6].ToString()),
                    NumerKoszulki = int.Parse(cells[7].ToString())
                });
            }

            return list;
        }
    }
}

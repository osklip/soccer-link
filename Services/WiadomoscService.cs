using Libsql.Client;
using SoccerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    class WiadomoscService
    {
        private const string Url = "https://soccerlinkdb-enbixd.aws-eu-west-1.turso.io";
        private const string Token = "eyJhbGciOiJFZERTQSIsInR5cCI6IkpXVCJ9.eyJhIjoicnciLCJleHAiOjE3OTU2MzcwODksImdpZCI6ImNhOWI1NGU3LTMwY2QtNDA5YS04YTMzLTcyMmRmZDFiYWY0YiIsImlhdCI6MTc2NDEwMTA4OSwicmlkIjoiYTBiNTRjM2YtZmZkYy00MjIyLWI2YTEtZGRhZTcxN2I1MmY4In0.dnupQBG2k5tiShROTpDhcHjm8b36JHLd4tebvAWESVZ-PtLlz40gq0ywuhf3c9MefzIFmZLkTVCZpgm5dw20Dg";

        public static async Task<List<Wiadomosc>> PobierzWiadomosciDlaAktualnegoTreneraAsync()
        {
            if (SessionService.AktualnyTrener == null)
                return new List<Wiadomosc>();

            var trenerId = SessionService.AktualnyTrener.Id;

            using var client = await DatabaseClient.Create(o =>
            {
                o.Url = Url;
                o.AuthToken = Token;
                o.UseHttps = true;
            });

            // wiadomości, gdzie trener jest odbiorcą
            var sql = $@"
                        SELECT 
                            w.WiadomoscID,
                            w.TypNadawcy,
                            w.NadawcaID,
                            w.TypOdbiorcy,
                            w.OdbiorcaID,
                            w.Tresc,
                            w.DataWyslania,
                            w.Temat,
                            CASE 
                                WHEN w.TypNadawcy = 'Zawodnik' THEN z.Imie || ' ' || z.Nazwisko
                                WHEN w.TypNadawcy = 'Trener'   THEN t.Imie || ' ' || t.Nazwisko
                                ELSE w.TypNadawcy
                            END AS NadawcaNazwa
                        FROM Wiadomosc w
                        LEFT JOIN Zawodnik z ON w.TypNadawcy = 'Zawodnik' AND w.NadawcaID = z.ZawodnikID
                        LEFT JOIN Trener   t ON w.TypNadawcy = 'Trener'   AND w.NadawcaID = t.TrenerID
                        WHERE 
                            (w.TypOdbiorcy = 'Trener' AND w.OdbiorcaID = {trenerId})
                            OR
                            (w.TypNadawcy = 'Trener' AND w.NadawcaID = {trenerId})
                        ORDER BY datetime(w.DataWyslania) DESC;
                    ";

            var result = await client.Execute(sql);

            var list = new List<Wiadomosc>();

            if (result.Rows == null)
                return list;

            foreach (var row in result.Rows)
            {
                var cells = row.ToArray();

                list.Add(new Wiadomosc
                {
                    WiadomoscID = int.Parse(cells[0].ToString()),
                    TypNadawcy = cells[1]?.ToString(),
                    NadawcaID = int.Parse(cells[2].ToString()),
                    TypOdbiorcy = cells[3]?.ToString(),
                    OdbiorcaID = int.Parse(cells[4].ToString()),
                    Tresc = cells[5]?.ToString(),
                    DataWyslania = cells[6]?.ToString(),
                    Temat = cells[7]?.ToString(),
                    NadawcaNazwa = cells[8]?.ToString()   // <-- NOWA KOLUMNA
                });
            }

            return list;
        }
    }
}

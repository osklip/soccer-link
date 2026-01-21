using Libsql.Client;
using SoccerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    class WiadomoscService
    {
        
        public static async Task SendMessagesAsync(List<int> recipientIds, string subject, string body)
        {
            if (SessionService.AktualnyTrener == null) return;
            if (recipientIds == null || !recipientIds.Any()) return;

            foreach (var recipientId in recipientIds)
            {
                await WyslijWiadomoscPrywatnaAsync(recipientId, subject, body);
            }
        }

        
        public static async Task WyslijWiadomoscPrywatnaAsync(int recipientId, string subject, string body)
        {
            if (SessionService.AktualnyTrener == null) return;

            var senderId = SessionService.AktualnyTrener.Id;
            var sendDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = @"
                INSERT INTO Wiadomosc 
                (TypNadawcy, NadawcaID, TypOdbiorcy, OdbiorcaID, Tresc, DataWyslania, Temat) 
                VALUES (?, ?, ?, ?, ?, ?, ?)";

            await client.Execute(sql,
                "Trener",      
                senderId,      
                "Zawodnik",    
                recipientId,   
                body,          
                sendDate,      
                subject        
            );
        }

        public static async Task<List<Wiadomosc>> PobierzWiadomosciDlaAktualnegoTreneraAsync()
        {
            if (SessionService.AktualnyTrener == null) return new List<Wiadomosc>();
            var trenerId = SessionService.AktualnyTrener.Id;

            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = @"
                SELECT 
                    w.WiadomoscID, w.TypNadawcy, w.NadawcaID, w.TypOdbiorcy, w.OdbiorcaID, w.Tresc, w.DataWyslania, w.Temat,
                    CASE 
                        WHEN w.TypNadawcy = 'Zawodnik' THEN z.Imie || ' ' || z.Nazwisko
                        WHEN w.TypNadawcy = 'Trener'   THEN t.Imie || ' ' || t.Nazwisko
                        ELSE w.TypNadawcy
                    END AS NadawcaNazwa
                FROM Wiadomosc w
                LEFT JOIN Zawodnik z ON w.TypNadawcy = 'Zawodnik' AND w.NadawcaID = z.ZawodnikID
                LEFT JOIN Trener   t ON w.TypNadawcy = 'Trener'   AND w.NadawcaID = t.TrenerID
                WHERE 
                    (w.TypOdbiorcy = 'Trener' AND w.OdbiorcaID = ?)
                    OR
                    (w.TypNadawcy = 'Trener' AND w.NadawcaID = ?)
                ORDER BY w.WiadomoscID DESC;";

            var result = await client.Execute(sql, trenerId, trenerId);

            var list = new List<Wiadomosc>();
            if (result.Rows == null) return list;

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
                    NadawcaNazwa = cells[8]?.ToString()
                });
            }
            return list;
        }
    }
}
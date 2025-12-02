using SoccerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    public class StatsService
    {
        // --- ZAPIS ---

        public static async Task SaveTeamStatsAsync(StatystykiDruzyny stats)
        {
            if (SessionService.AktualnyTrener == null) return;
            using var client = await DatabaseConfig.CreateClientAsync();

            // Najpierw usuwamy stare statystyki dla tego meczu (jeśli była edycja), żeby nie dublować
            await client.Execute("DELETE FROM StatystykiDruzyny WHERE MeczID = ? AND TrenerID = ?",
                stats.MeczID, SessionService.AktualnyTrener.Id);

            var sql = @"
                INSERT INTO StatystykiDruzyny 
                (MeczID, TrenerID, Gole, PosiadaniePilki, Strzaly, StrzalyCelne, StrzalyNiecelne, RzutyRozne, Faule, CzysteKonto) 
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

            await client.Execute(sql,
                stats.MeczID, SessionService.AktualnyTrener.Id, stats.Gole, stats.PosiadaniePilki,
                stats.Strzaly, stats.StrzalyCelne, stats.StrzalyNiecelne, stats.RzutyRozne, stats.Faule,
                stats.CzysteKonto ? 1 : 0);
        }

        // --- ODCZYT (AGREGACJA) ---

        // Metoda zwraca średnie statystyki dla wszystkich meczów danego trenera
        public static async Task<StatystykiDruzyny> GetAverageTeamStatsAsync()
        {
            if (SessionService.AktualnyTrener == null) return new StatystykiDruzyny();
            using var client = await DatabaseConfig.CreateClientAsync();

            // AVG zwraca double, więc castujemy. COUNT zlicza mecze.
            var sql = @"
                SELECT 
                    AVG(Gole), AVG(PosiadaniePilki), AVG(Strzaly), AVG(StrzalyCelne), 
                    AVG(StrzalyNiecelne), AVG(RzutyRozne), AVG(Faule), SUM(CzysteKonto)
                FROM StatystykiDruzyny
                WHERE TrenerID = ?";

            var result = await client.Execute(sql, SessionService.AktualnyTrener.Id);
            var row = result.Rows.FirstOrDefault()?.ToArray();

            if (row == null || row[0] is DBNull) return new StatystykiDruzyny();

            // Helper do parsowania double -> int (zaokrąglenie)
            int ToInt(object val) => val != null && double.TryParse(val.ToString(), out double d) ? (int)Math.Round(d) : 0;

            return new StatystykiDruzyny
            {
                Gole = ToInt(row[0]),
                PosiadaniePilki = ToInt(row[1]),
                Strzaly = ToInt(row[2]),
                StrzalyCelne = ToInt(row[3]),
                StrzalyNiecelne = ToInt(row[4]),
                RzutyRozne = ToInt(row[5]),
                Faule = ToInt(row[6]),
                CzysteKonto = ToInt(row[7]) > 0 // Tutaj SUM zwraca liczbę czystych kont, w modelu bool służy do checkboxa, ale w Viewmodelu obsłużymy to jako int
            };
        }

        // --- ZAPIS (Lista zawodników) ---
        public static async Task SavePlayerStatsListAsync(List<StatystykiZawodnika> statsList)
        {
            if (statsList == null || !statsList.Any()) return;
            if (SessionService.AktualnyTrener == null) return;

            using var client = await DatabaseConfig.CreateClientAsync();
            var meczId = statsList.First().MeczID;
            var trenerId = SessionService.AktualnyTrener.Id;

            // 1. Czyścimy stare wpisy dla tego meczu (aby uniknąć duplikatów przy edycji)
            await client.Execute("DELETE FROM StatystykiZawodnika WHERE MeczID = ? AND TrenerID = ?",
                meczId, trenerId);

            // 2. Wstawiamy nowe
            var sql = @"
        INSERT INTO StatystykiZawodnika 
        (MeczID, ZawodnikID, TrenerID, Gole, Strzaly, StrzalyCelne, StrzalyNiecelne, PodaniaCelne, Faule, ZolteKartki, CzerwonaKartka, CzysteKonto) 
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

            foreach (var s in statsList)
            {
                await client.Execute(sql,
                    s.MeczID, s.ZawodnikID, trenerId, s.Gole, s.Strzaly, s.StrzalyCelne,
                    s.StrzalyNiecelne, s.PodaniaCelne, s.Faule, s.ZolteKartki,
                    s.CzerwonaKartka ? 1 : 0, s.CzysteKonto ? 1 : 0);
            }
        }

        // --- ODCZYT (Szczegóły gracza - podsumowanie sezonu) ---
        public static async Task<StatystykiZawodnika> GetPlayerStatsSummaryAsync(int zawodnikId)
        {
            if (SessionService.AktualnyTrener == null) return new StatystykiZawodnika();
            using var client = await DatabaseConfig.CreateClientAsync();

            // Sumujemy osiągnięcia ze wszystkich meczów
            var sql = @"
        SELECT 
            SUM(Gole), SUM(Strzaly), SUM(StrzalyCelne), SUM(StrzalyNiecelne), 
            SUM(PodaniaCelne), SUM(Faule), SUM(ZolteKartki), SUM(CzerwonaKartka), SUM(CzysteKonto)
        FROM StatystykiZawodnika
        WHERE ZawodnikID = ? AND TrenerID = ?";

            var result = await client.Execute(sql, zawodnikId, SessionService.AktualnyTrener.Id);
            var row = result.Rows.FirstOrDefault()?.ToArray();

            if (row == null || row[0] is DBNull) return new StatystykiZawodnika();

            int ToInt(object val) => val != null && int.TryParse(val.ToString(), out int i) ? i : 0;

            return new StatystykiZawodnika
            {
                Gole = ToInt(row[0]),
                Strzaly = ToInt(row[1]),
                StrzalyCelne = ToInt(row[2]),
                StrzalyNiecelne = ToInt(row[3]),
                PodaniaCelne = ToInt(row[4]),
                Faule = ToInt(row[5]),
                ZolteKartki = ToInt(row[6]),
                CzerwonaKartka = ToInt(row[7]) > 0, // Tutaj uproszczenie: true jeśli dostał chociaż jedną w sezonie
                CzysteKonto = ToInt(row[8]) > 0
            };
        }
    }
}

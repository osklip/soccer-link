using SoccerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    public class StatsService
    {
        // --- ZAPIS (Drużyna) ---
        public static async Task SaveTeamStatsAsync(StatystykiDruzyny stats)
        {
            if (SessionService.AktualnyTrener == null) return;
            using var client = await DatabaseConfig.CreateClientAsync();

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

        // --- ODCZYT (AGREGACJA - Drużyna) ---
        public static async Task<StatystykiDruzyny> GetAverageTeamStatsAsync(int? month = null, int? year = null)
        {
            if (SessionService.AktualnyTrener == null) return new StatystykiDruzyny();
            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = @"
                SELECT 
                    AVG(s.Gole), AVG(s.PosiadaniePilki), AVG(s.Strzaly), AVG(s.StrzalyCelne), 
                    AVG(s.StrzalyNiecelne), AVG(s.RzutyRozne), AVG(s.Faule), SUM(s.CzysteKonto)
                FROM StatystykiDruzyny s
                JOIN Mecz m ON s.MeczID = m.MeczID
                WHERE s.TrenerID = ?";

            var args = new List<object> { SessionService.AktualnyTrener.Id };

            if (month.HasValue && month.Value > 0)
            {
                sql += " AND strftime('%m', m.Data) = ?";
                args.Add(month.Value.ToString("00"));
            }

            if (year.HasValue && year.Value > 0)
            {
                sql += " AND strftime('%Y', m.Data) = ?";
                args.Add(year.Value.ToString());
            }

            var result = await client.Execute(sql, args.ToArray());
            var row = result.Rows.FirstOrDefault()?.ToArray();

            if (row == null || row[0] is DBNull) return new StatystykiDruzyny();

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
                CzysteKonto = ToInt(row[7]) > 0
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

            await client.Execute("DELETE FROM StatystykiZawodnika WHERE MeczID = ? AND TrenerID = ?", meczId, trenerId);

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

        // --- ODCZYT (Szczegóły gracza - podsumowanie sezonu z filtrami) ---
        public static async Task<StatystykiZawodnika> GetPlayerStatsSummaryAsync(int zawodnikId, int? month = null, int? year = null)
        {
            if (SessionService.AktualnyTrener == null) return new StatystykiZawodnika();
            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = @"
                SELECT 
                    SUM(sz.Gole), SUM(sz.Strzaly), SUM(sz.StrzalyCelne), SUM(sz.StrzalyNiecelne), 
                    SUM(sz.PodaniaCelne), SUM(sz.Faule), SUM(sz.ZolteKartki), SUM(sz.CzerwonaKartka), SUM(sz.CzysteKonto)
                FROM StatystykiZawodnika sz
                JOIN Mecz m ON sz.MeczID = m.MeczID
                WHERE sz.ZawodnikID = ? AND sz.TrenerID = ?";

            var args = new List<object> { zawodnikId, SessionService.AktualnyTrener.Id };

            if (month.HasValue && month.Value > 0)
            {
                sql += " AND strftime('%m', m.Data) = ?";
                args.Add(month.Value.ToString("00"));
            }

            if (year.HasValue && year.Value > 0)
            {
                sql += " AND strftime('%Y', m.Data) = ?";
                args.Add(year.Value.ToString());
            }

            var result = await client.Execute(sql, args.ToArray());
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
                CzerwonaKartka = ToInt(row[7]) > 0,
                CzysteKonto = ToInt(row[8]) > 0
            };
        }

        public static async Task<List<Mecz>> GetMatchesWithoutStatsAsync()
        {
            if (SessionService.AktualnyTrener == null) return new List<Mecz>();
            var trenerId = SessionService.AktualnyTrener.Id;

            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = @"
                SELECT m.MeczID, m.Przeciwnik, m.Data, m.Godzina, m.Miejsce 
                FROM Mecz m
                LEFT JOIN StatystykiDruzyny s ON m.MeczID = s.MeczID
                WHERE m.TrenerID = ? AND s.StatDruzynyID IS NULL
                ORDER BY m.Data DESC, m.Godzina DESC";

            var result = await client.Execute(sql, trenerId);
            var list = new List<Mecz>();

            if (result.Rows != null)
            {
                foreach (var row in result.Rows)
                {
                    var c = row.ToArray();
                    string dataStr = c[2]?.ToString();
                    string godzinaStr = c[3]?.ToString();
                    DateTime dt = DateTime.Now;

                    if (DateTime.TryParse($"{dataStr} {godzinaStr}", out var parsedDate))
                    {
                        dt = parsedDate;
                    }

                    if (dt > DateTime.Now) continue;

                    list.Add(new Mecz
                    {
                        MeczID = int.Parse(c[0].ToString()),
                        Przeciwnik = c[1]?.ToString(),
                        DataRozpoczecia = dt,
                        Miejsce = c[4]?.ToString(),
                        TrenerID = trenerId
                    });
                }
            }
            return list;
        }
    }
}
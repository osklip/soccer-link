using Libsql.Client;
using SoccerLink.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    class CalendarService
    {
        private const string Url = "https://soccerlinkdb-enbixd.aws-eu-west-1.turso.io";
        private const string Token = "eyJhbGciOiJFZERTQSIsInR5cCI6IkpXVCJ9.eyJhIjoicnciLCJleHAiOjE3OTU2MzcwODksImdpZCI6ImNhOWI1NGU3LTMwY2QtNDA5YS04YTMzLTcyMmRmZDFiYWY0YiIsImlhdCI6MTc2NDEwMTA4OSwicmlkIjoiYTBiNTRjM2YtZmZkYy00MjIyLWI2YTEtZGRhZTcxN2I1MmY4In0.dnupQBG2k5tiShROTpDhcHjm8b36JHLd4tebvAWESVZ-PtLlz40gq0ywuhf3c9MefzIFmZLkTVCZpgm5dw20Dg";

        private static string Escape(string value) => value.Replace("'", "''").Trim();

        public static async Task AddMeczAsync(Mecz mecz)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");

            using var client = await DatabaseClient.Create(o => { o.Url = Url; o.AuthToken = Token; o.UseHttps = true; });

            var sql = $@"
                INSERT INTO Mecz (SkladMeczowyID, Przeciwnik, Data, Godzina, Miejsce, TrenerID)
                VALUES ( 0, '{Escape(mecz.Przeciwnik)}', '{Escape(mecz.Data)}', '{Escape(mecz.Godzina)}', '{Escape(mecz.Miejsce)}', {SessionService.AktualnyTrener.Id} );";

            await client.Execute(sql);
        }

        public static async Task AddTreningAsync(Trening trening)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");

            using var client = await DatabaseClient.Create(o => { o.Url = Url; o.AuthToken = Token; o.UseHttps = true; });

            var sql = $@"
                INSERT INTO Trening (ListaObecnosciID, Typ, Data, GodzinaRozpoczecia, GodzinaZakonczenia, Miejsce, TrenerID)
                VALUES ( 0, '{Escape(trening.Typ)}', '{Escape(trening.Data)}', '{Escape(trening.GodzinaRozpoczecia)}', '{Escape(trening.GodzinaZakonczenia)}', '{Escape(trening.Miejsce)}', {SessionService.AktualnyTrener.Id} );";

            await client.Execute(sql);
        }

        public static async Task AddWydarzenieAsync(Wydarzenie wydarzenie)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");

            using var client = await DatabaseClient.Create(o => { o.Url = Url; o.AuthToken = Token; o.UseHttps = true; });

            var sql = $@"
                INSERT INTO Wydarzenie (Nazwa, Miejsce, Data, GodzinaStart, GodzinaKoniec, Opis, TrenerID)
                VALUES ( '{Escape(wydarzenie.Nazwa)}', '{Escape(wydarzenie.Miejsce)}', '{Escape(wydarzenie.Data)}', '{Escape(wydarzenie.GodzinaStart)}', '{Escape(wydarzenie.GodzinaKoniec)}', '{Escape(wydarzenie.Opis)}', {SessionService.AktualnyTrener.Id} );";

            await client.Execute(sql);
        }

        public static async Task<List<UpcomingEvent>> GetUpcomingEventsAsync()
        {
            if (SessionService.AktualnyTrener == null)
            {
                throw new InvalidOperationException("Trener nie jest zalogowany.");
            }

            var allEvents = new List<UpcomingEvent>();
            var trenerId = SessionService.AktualnyTrener.Id;
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            string[] dateTimeFormats = new[]
            {
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-dd H:mm"
            };


            try
            {
                using var client = await DatabaseClient.Create(o =>
                {
                    o.Url = Url;
                    o.AuthToken = Token;
                    o.UseHttps = true;
                });

                // 1. Mecze
                var meczSql = $@"
                    SELECT Przeciwnik, Data, Godzina, Miejsce 
                    FROM Mecz 
                    WHERE TrenerID = {trenerId} AND Data >= '{currentDate}' 
                    ORDER BY Data, Godzina;
                ";
                var meczResult = await client.Execute(meczSql);

                if (meczResult.Rows != null)
                {
                    foreach (var row in meczResult.Rows)
                    {
                        var cells = row.ToArray();
                        var data = cells[1]?.ToString() ?? "";
                        var godzina = cells[2]?.ToString() ?? "";

                        if (DateTime.TryParseExact($"{data} {godzina}", dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeStart))
                        {
                            if (dateTimeStart > DateTime.Now)
                            {
                                allEvents.Add(new UpcomingEvent
                                {
                                    EventType = "Mecz",
                                    Title = cells[0]?.ToString() ?? "Brak przeciwnika",
                                    DateTimeStart = dateTimeStart,
                                    Location = cells[3]?.ToString() ?? "Brak miejsca",
                                    TimeEnd = string.Empty
                                });
                            }
                        }
                    }
                }

                // 2. Treningi
                var treningSql = $@"
                    SELECT Typ, Data, GodzinaRozpoczecia, GodzinaZakonczenia, Miejsce 
                    FROM Trening 
                    WHERE TrenerID = {trenerId} AND Data >= '{currentDate}' 
                    ORDER BY Data, GodzinaRozpoczecia;
                ";
                var treningResult = await client.Execute(treningSql);

                if (treningResult.Rows != null)
                {
                    foreach (var row in treningResult.Rows)
                    {
                        var cells = row.ToArray();
                        var data = cells[1]?.ToString() ?? "";
                        var godzinaStart = cells[2]?.ToString() ?? "";
                        var godzinaEnd = cells[3]?.ToString() ?? "";

                        if (DateTime.TryParseExact($"{data} {godzinaStart}", dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeStart))
                        {
                            if (dateTimeStart > DateTime.Now)
                            {
                                allEvents.Add(new UpcomingEvent
                                {
                                    EventType = "Trening",
                                    Title = cells[0]?.ToString() ?? "Brak typu",
                                    DateTimeStart = dateTimeStart,
                                    TimeEnd = godzinaEnd,
                                    Location = cells[4]?.ToString() ?? "Brak miejsca"
                                });
                            }
                        }
                    }
                }

                // 3. Wydarzenia
                var wydarzenieSql = $@"
                    SELECT Nazwa, Data, GodzinaStart, GodzinaKoniec, Miejsce, Opis 
                    FROM Wydarzenie 
                    WHERE TrenerID = {trenerId} AND Data >= '{currentDate}' 
                    ORDER BY Data, GodzinaStart;
                ";
                var wydarzenieResult = await client.Execute(wydarzenieSql);

                if (wydarzenieResult.Rows != null)
                {
                    foreach (var row in wydarzenieResult.Rows)
                    {
                        var cells = row.ToArray();
                        var data = cells[1]?.ToString() ?? "";
                        var godzinaStart = cells[2]?.ToString() ?? "";
                        var godzinaKoniec = cells[3]?.ToString() ?? "";

                        if (DateTime.TryParseExact($"{data} {godzinaStart}", dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeStart))
                        {
                            if (dateTimeStart > DateTime.Now)
                            {
                                allEvents.Add(new UpcomingEvent
                                {
                                    EventType = "Wydarzenie",
                                    Title = cells[0]?.ToString() ?? "Brak nazwy",
                                    DateTimeStart = dateTimeStart,
                                    TimeEnd = godzinaKoniec,
                                    Location = cells[4]?.ToString() ?? "Brak miejsca",
                                    Description = cells[5]?.ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd połączenia lub pobierania danych z bazy: {ex.Message}", ex);
            }

            return allEvents.OrderBy(e => e.DateTimeStart).ToList();
        }
    }
}
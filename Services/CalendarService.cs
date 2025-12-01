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

        public static async Task<List<UpcomingEvent>> GetAllEventsAsync()
        {
            if (SessionService.AktualnyTrener == null)
            {
                throw new InvalidOperationException("Trener nie jest zalogowany.");
            }

            var allEvents = new List<UpcomingEvent>();
            var trenerId = SessionService.AktualnyTrener.Id;

            // Poprawka: Dodanie formatu 'H:mm' (bez wiodącego zera) dla większej stabilności parsowania
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

                // 1. Mecze (Bez filtrowania datą bieżącą w SQL)
                var meczSql = $@"
                    SELECT MeczID, Przeciwnik, Data, Godzina, Miejsce
                    FROM Mecz
                    WHERE TrenerID = {trenerId} 
                    ORDER BY Data, Godzina;
                ";
                var meczResult = await client.Execute(meczSql);

                if (meczResult.Rows != null)
                {
                    foreach (var row in meczResult.Rows)
                    {
                        var cells = row.ToArray();
                        var meczId = int.Parse(cells[0]?.ToString() ?? "0");
                        var data = cells[2]?.ToString() ?? "";
                        var godzina = cells[3]?.ToString() ?? "";

                        if (DateTime.TryParseExact($"{data} {godzina}", dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeStart))
                        {
                            allEvents.Add(new UpcomingEvent
                            {
                                EventType = "Mecz",
                                Id = meczId, // <-- Dodano ID
                                Title = cells[1]?.ToString() ?? "Brak przeciwnika",
                                DateTimeStart = dateTimeStart,
                                Location = cells[4]?.ToString() ?? "Brak miejsca",
                                TimeEnd = string.Empty
                            });
                        }
                    }
                }

                // 2. Treningi (Bez filtrowania datą bieżącą w SQL)
                var treningSql = $@"
                    SELECT TreningID, Typ, Data, GodzinaRozpoczecia, GodzinaZakonczenia, Miejsce 
                    FROM Trening 
                    WHERE TrenerID = {trenerId} 
                    ORDER BY Data, GodzinaRozpoczecia;
                ";
                var treningResult = await client.Execute(treningSql);

                if (treningResult.Rows != null)
                {
                    foreach (var row in treningResult.Rows)
                    {
                        var cells = row.ToArray();
                        var treningId = int.Parse(cells[0]?.ToString() ?? "0");
                        var data = cells[2]?.ToString() ?? "";
                        var godzinaStart = cells[3]?.ToString() ?? "";
                        var godzinaEnd = cells[4]?.ToString() ?? "";

                        if (DateTime.TryParseExact($"{data} {godzinaStart}", dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeStart))
                        {
                            allEvents.Add(new UpcomingEvent
                            {
                                EventType = "Trening",
                                Id = treningId, // <-- Dodano ID
                                Title = cells[1]?.ToString() ?? "Brak typu",
                                DateTimeStart = dateTimeStart,
                                TimeEnd = godzinaEnd,
                                Location = cells[5]?.ToString() ?? "Brak miejsca"
                            });
                        }
                    }
                }

                // 3. Wydarzenia (Bez filtrowania datą bieżącą w SQL)
                var wydarzenieSql = $@"
                    SELECT WydarzenieID, Nazwa, Data, GodzinaStart, GodzinaKoniec, Miejsce, Opis 
                    FROM Wydarzenie 
                    WHERE TrenerID = {trenerId} 
                    ORDER BY Data, GodzinaStart;
                ";
                var wydarzenieResult = await client.Execute(wydarzenieSql);

                if (wydarzenieResult.Rows != null)
                {
                    foreach (var row in wydarzenieResult.Rows)
                    {
                        var cells = row.ToArray();
                        var wydarzenieId = int.Parse(cells[0]?.ToString() ?? "0");
                        var data = cells[2]?.ToString() ?? "";
                        var godzinaStart = cells[3]?.ToString() ?? "";
                        var godzinaKoniec = cells[4]?.ToString() ?? "";

                        if (DateTime.TryParseExact($"{data} {godzinaStart}", dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeStart))
                        {
                            allEvents.Add(new UpcomingEvent
                            {
                                EventType = "Wydarzenie",
                                Id = wydarzenieId, // <-- Dodano ID
                                Title = cells[1]?.ToString() ?? "Brak nazwy",
                                DateTimeStart = dateTimeStart,
                                TimeEnd = godzinaKoniec,
                                Location = cells[5]?.ToString() ?? "Brak miejsca",
                                Description = cells[6]?.ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd połączenia lub pobierania danych z bazy: {ex.Message}", ex);
            }

            // Zwracamy całą listę posortowaną chronologicznie
            return allEvents.OrderBy(e => e.DateTimeStart).ToList();
        }

        public static async Task UpdateEventAsync(UpcomingEvent eventData)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");
            if (eventData.Id == 0) throw new ArgumentException("Brak ID wydarzenia do edycji.");

            using var client = await DatabaseClient.Create(o => { o.Url = Url; o.AuthToken = Token; o.UseHttps = true; });

            string sql;
            string safeTitle = Escape(eventData.Title);
            string safeData = Escape(eventData.DateTimeStart.ToString("yyyy-MM-dd"));
            string safeLocation = Escape(eventData.Location);
            string safeTimeStart = Escape(eventData.DisplayTimeStart);

            switch (eventData.EventType)
            {
                case "Mecz":
                    sql = $@"
                        UPDATE Mecz SET 
                            Przeciwnik = '{safeTitle}', 
                            Data = '{safeData}', 
                            Godzina = '{safeTimeStart}', 
                            Miejsce = '{safeLocation}'
                        WHERE MeczID = {eventData.Id} AND TrenerID = {SessionService.AktualnyTrener.Id};";
                    break;

                case "Trening":
                    string safeTimeEndTrening = Escape(eventData.TimeEnd);
                    sql = $@"
                        UPDATE Trening SET 
                            Typ = '{safeTitle}', 
                            Data = '{safeData}', 
                            GodzinaRozpoczecia = '{safeTimeStart}', 
                            GodzinaZakonczenia = '{safeTimeEndTrening}', 
                            Miejsce = '{safeLocation}'
                        WHERE TreningID = {eventData.Id} AND TrenerID = {SessionService.AktualnyTrener.Id};";
                    break;

                case "Wydarzenie":
                    string safeTimeEndWydarzenie = Escape(eventData.TimeEnd);
                    string safeOpis = Escape(eventData.Description);
                    sql = $@"
                        UPDATE Wydarzenie SET 
                            Nazwa = '{safeTitle}', 
                            Miejsce = '{safeLocation}', 
                            Data = '{safeData}', 
                            GodzinaStart = '{safeTimeStart}', 
                            GodzinaKoniec = '{safeTimeEndWydarzenie}', 
                            Opis = '{safeOpis}'
                        WHERE WydarzenieID = {eventData.Id} AND TrenerID = {SessionService.AktualnyTrener.Id};";
                    break;

                default:
                    throw new ArgumentException("Nieznany typ wydarzenia.");
            }

            await client.Execute(sql);
        }

        public static async Task DeleteEventAsync(string eventType, int eventId)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");
            if (eventId == 0) throw new ArgumentException("Brak ID wydarzenia do usunięcia.");

            using var client = await DatabaseClient.Create(o => { o.Url = Url; o.AuthToken = Token; o.UseHttps = true; });

            string tableName;
            string idColumn;

            switch (eventType)
            {
                case "Mecz":
                    tableName = "Mecz";
                    idColumn = "MeczID";
                    break;
                case "Trening":
                    tableName = "Trening";
                    idColumn = "TreningID";
                    break;
                case "Wydarzenie":
                    tableName = "Wydarzenie";
                    idColumn = "WydarzenieID";
                    break;
                default:
                    throw new ArgumentException("Nieznany typ wydarzenia.");
            }

            // Usuwamy tylko, jeśli TrenerID się zgadza (środek bezpieczeństwa)
            var sql = $@"
                DELETE FROM {tableName}
                WHERE {idColumn} = {eventId} AND TrenerID = {SessionService.AktualnyTrener.Id};";

            await client.Execute(sql);
        }
    }
}
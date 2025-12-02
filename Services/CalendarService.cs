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
        public static async Task AddMeczAsync(Mecz mecz)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");

            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = @"
                INSERT INTO Mecz (SkladMeczowyID, Przeciwnik, Data, Godzina, Miejsce, TrenerID)
                VALUES (0, @przeciwnik, @data, @godzina, @miejsce, @trenerId);";

            var args = new
            {
                przeciwnik = mecz.Przeciwnik,
                data = mecz.Data,
                godzina = mecz.Godzina,
                miejsce = mecz.Miejsce,
                trenerId = SessionService.AktualnyTrener.Id
            };

            await client.Execute(sql, args);
        }

        public static async Task AddTreningAsync(Trening trening)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");

            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = @"
                INSERT INTO Trening (ListaObecnosciID, Typ, Data, GodzinaRozpoczecia, GodzinaZakonczenia, Miejsce, TrenerID)
                VALUES (0, @typ, @data, @start, @end, @miejsce, @trenerId);";

            var args = new
            {
                typ = trening.Typ,
                data = trening.Data,
                start = trening.GodzinaRozpoczecia,
                end = trening.GodzinaZakonczenia,
                miejsce = trening.Miejsce,
                trenerId = SessionService.AktualnyTrener.Id
            };

            await client.Execute(sql, args);
        }

        public static async Task AddWydarzenieAsync(Wydarzenie wydarzenie)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");

            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = @"
                INSERT INTO Wydarzenie (Nazwa, Miejsce, Data, GodzinaStart, GodzinaKoniec, Opis, TrenerID)
                VALUES (@nazwa, @miejsce, @data, @start, @end, @opis, @trenerId);";

            var args = new
            {
                nazwa = wydarzenie.Nazwa,
                miejsce = wydarzenie.Miejsce,
                data = wydarzenie.Data,
                start = wydarzenie.GodzinaStart,
                end = wydarzenie.GodzinaKoniec,
                opis = wydarzenie.Opis,
                trenerId = SessionService.AktualnyTrener.Id
            };

            await client.Execute(sql, args);
        }

        public static async Task<List<UpcomingEvent>> GetUpcomingEventsAsync()
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");

            var allEvents = new List<UpcomingEvent>();
            var trenerId = SessionService.AktualnyTrener.Id;
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            string[] dateTimeFormats = new[] { "yyyy-MM-dd HH:mm", "yyyy-MM-dd H:mm" };

            try
            {
                using var client = await DatabaseConfig.CreateClientAsync();

                // Parametry dla zapytań (filtrowanie po dacie w SQL!)
                var queryArgs = new { trenerId, currentDate };

                // 1. Mecze
                var meczSql = @"
                    SELECT Przeciwnik, Data, Godzina, Miejsce 
                    FROM Mecz 
                    WHERE TrenerID = @trenerId AND Data >= @currentDate 
                    ORDER BY Data, Godzina;";
                var meczResult = await client.Execute(meczSql, queryArgs);

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
                var treningSql = @"
                    SELECT Typ, Data, GodzinaRozpoczecia, GodzinaZakonczenia, Miejsce 
                    FROM Trening 
                    WHERE TrenerID = @trenerId AND Data >= @currentDate 
                    ORDER BY Data, GodzinaRozpoczecia;";
                var treningResult = await client.Execute(treningSql, queryArgs);

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
                var wydarzenieSql = @"
                    SELECT Nazwa, Data, GodzinaStart, GodzinaKoniec, Miejsce, Opis 
                    FROM Wydarzenie 
                    WHERE TrenerID = @trenerId AND Data >= @currentDate 
                    ORDER BY Data, GodzinaStart;";
                var wydarzenieResult = await client.Execute(wydarzenieSql, queryArgs);

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
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");

            var allEvents = new List<UpcomingEvent>();
            var queryArgs = new { trenerId = SessionService.AktualnyTrener.Id };
            string[] dateTimeFormats = new[] { "yyyy-MM-dd HH:mm", "yyyy-MM-dd H:mm" };

            try
            {
                using var client = await DatabaseConfig.CreateClientAsync();

                // 1. Mecze
                var meczResult = await client.Execute("SELECT MeczID, Przeciwnik, Data, Godzina, Miejsce FROM Mecz WHERE TrenerID = @trenerId ORDER BY Data, Godzina", queryArgs);
                if (meczResult.Rows != null)
                {
                    foreach (var row in meczResult.Rows)
                    {
                        var cells = row.ToArray();
                        var data = cells[2]?.ToString() ?? "";
                        var godzina = cells[3]?.ToString() ?? "";

                        if (DateTime.TryParseExact($"{data} {godzina}", dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeStart))
                        {
                            allEvents.Add(new UpcomingEvent
                            {
                                EventType = "Mecz",
                                Id = int.Parse(cells[0]?.ToString() ?? "0"),
                                Title = cells[1]?.ToString() ?? "Brak",
                                DateTimeStart = dateTimeStart,
                                Location = cells[4]?.ToString() ?? "",
                                TimeEnd = string.Empty
                            });
                        }
                    }
                }

                // 2. Treningi
                var treningResult = await client.Execute("SELECT TreningID, Typ, Data, GodzinaRozpoczecia, GodzinaZakonczenia, Miejsce FROM Trening WHERE TrenerID = @trenerId ORDER BY Data, GodzinaRozpoczecia", queryArgs);
                if (treningResult.Rows != null)
                {
                    foreach (var row in treningResult.Rows)
                    {
                        var cells = row.ToArray();
                        var data = cells[2]?.ToString() ?? "";
                        var godzina = cells[3]?.ToString() ?? "";

                        if (DateTime.TryParseExact($"{data} {godzina}", dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeStart))
                        {
                            allEvents.Add(new UpcomingEvent
                            {
                                EventType = "Trening",
                                Id = int.Parse(cells[0]?.ToString() ?? "0"),
                                Title = cells[1]?.ToString() ?? "Brak",
                                DateTimeStart = dateTimeStart,
                                TimeEnd = cells[4]?.ToString() ?? "",
                                Location = cells[5]?.ToString() ?? ""
                            });
                        }
                    }
                }

                // 3. Wydarzenia
                var wydResult = await client.Execute("SELECT WydarzenieID, Nazwa, Data, GodzinaStart, GodzinaKoniec, Miejsce, Opis FROM Wydarzenie WHERE TrenerID = @trenerId ORDER BY Data, GodzinaStart", queryArgs);
                if (wydResult.Rows != null)
                {
                    foreach (var row in wydResult.Rows)
                    {
                        var cells = row.ToArray();
                        var data = cells[2]?.ToString() ?? "";
                        var godzina = cells[3]?.ToString() ?? "";

                        if (DateTime.TryParseExact($"{data} {godzina}", dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeStart))
                        {
                            allEvents.Add(new UpcomingEvent
                            {
                                EventType = "Wydarzenie",
                                Id = int.Parse(cells[0]?.ToString() ?? "0"),
                                Title = cells[1]?.ToString() ?? "Brak",
                                DateTimeStart = dateTimeStart,
                                TimeEnd = cells[4]?.ToString() ?? "",
                                Location = cells[5]?.ToString() ?? "",
                                Description = cells[6]?.ToString() ?? ""
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd pobierania wszystkich wydarzeń: {ex.Message}", ex);
            }

            return allEvents.OrderBy(e => e.DateTimeStart).ToList();
        }

        public static async Task UpdateEventAsync(UpcomingEvent eventData)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");
            if (eventData.Id == 0) throw new ArgumentException("Brak ID wydarzenia.");

            using var client = await DatabaseConfig.CreateClientAsync();

            string sql;
            // Parametryzacja jest kluczowa dla bezpiecznej aktualizacji
            var args = new
            {
                title = eventData.Title,
                data = eventData.DateTimeStart.ToString("yyyy-MM-dd"),
                start = eventData.DisplayTimeStart,
                location = eventData.Location,
                end = eventData.TimeEnd,
                desc = eventData.Description,
                id = eventData.Id,
                trenerId = SessionService.AktualnyTrener.Id
            };

            switch (eventData.EventType)
            {
                case "Mecz":
                    sql = @"UPDATE Mecz SET Przeciwnik=@title, Data=@data, Godzina=@start, Miejsce=@location WHERE MeczID=@id AND TrenerID=@trenerId";
                    break;
                case "Trening":
                    sql = @"UPDATE Trening SET Typ=@title, Data=@data, GodzinaRozpoczecia=@start, GodzinaZakonczenia=@end, Miejsce=@location WHERE TreningID=@id AND TrenerID=@trenerId";
                    break;
                case "Wydarzenie":
                    sql = @"UPDATE Wydarzenie SET Nazwa=@title, Miejsce=@location, Data=@data, GodzinaStart=@start, GodzinaKoniec=@end, Opis=@desc WHERE WydarzenieID=@id AND TrenerID=@trenerId";
                    break;
                default:
                    throw new ArgumentException("Nieznany typ wydarzenia.");
            }

            await client.Execute(sql, args);
        }

        public static async Task DeleteEventAsync(string eventType, int eventId)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");

            using var client = await DatabaseConfig.CreateClientAsync();
            var args = new { id = eventId, trenerId = SessionService.AktualnyTrener.Id };
            string sql;

            switch (eventType)
            {
                case "Mecz": sql = "DELETE FROM Mecz WHERE MeczID=@id AND TrenerID=@trenerId"; break;
                case "Trening": sql = "DELETE FROM Trening WHERE TreningID=@id AND TrenerID=@trenerId"; break;
                case "Wydarzenie": sql = "DELETE FROM Wydarzenie WHERE WydarzenieID=@id AND TrenerID=@trenerId"; break;
                default: throw new ArgumentException("Nieznany typ wydarzenia.");
            }

            await client.Execute(sql, args);
        }
    }
}
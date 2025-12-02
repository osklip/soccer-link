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

            var sql = "INSERT INTO Mecz (SkladMeczowyID, Przeciwnik, Data, Godzina, Miejsce, TrenerID) VALUES (0, ?, ?, ?, ?, ?);";

            // Kolejność argumentów musi pasować do '?'
            await client.Execute(sql, mecz.Przeciwnik, mecz.Data, mecz.Godzina, mecz.Miejsce, SessionService.AktualnyTrener.Id);
        }

        public static async Task AddTreningAsync(Trening trening)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");
            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = "INSERT INTO Trening (ListaObecnosciID, Typ, Data, GodzinaRozpoczecia, GodzinaZakonczenia, Miejsce, TrenerID) VALUES (0, ?, ?, ?, ?, ?, ?);";

            await client.Execute(sql, trening.Typ, trening.Data, trening.GodzinaRozpoczecia, trening.GodzinaZakonczenia, trening.Miejsce, SessionService.AktualnyTrener.Id);
        }

        public static async Task AddWydarzenieAsync(Wydarzenie wydarzenie)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");
            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = "INSERT INTO Wydarzenie (Nazwa, Miejsce, Data, GodzinaStart, GodzinaKoniec, Opis, TrenerID) VALUES (?, ?, ?, ?, ?, ?, ?);";

            await client.Execute(sql, wydarzenie.Nazwa, wydarzenie.Miejsce, wydarzenie.Data, wydarzenie.GodzinaStart, wydarzenie.GodzinaKoniec, wydarzenie.Opis, SessionService.AktualnyTrener.Id);
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

                // 1. Mecze
                var meczSql = "SELECT Przeciwnik, Data, Godzina, Miejsce FROM Mecz WHERE TrenerID = ? AND Data >= ? ORDER BY Data, Godzina;";
                var meczResult = await client.Execute(meczSql, trenerId, currentDate);

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
                                    Title = cells[0]?.ToString() ?? "Brak",
                                    DateTimeStart = dateTimeStart,
                                    Location = cells[3]?.ToString() ?? "",
                                    TimeEnd = string.Empty
                                });
                            }
                        }
                    }
                }

                // 2. Treningi
                var treningSql = "SELECT Typ, Data, GodzinaRozpoczecia, GodzinaZakonczenia, Miejsce FROM Trening WHERE TrenerID = ? AND Data >= ? ORDER BY Data, GodzinaRozpoczecia;";
                var treningResult = await client.Execute(treningSql, trenerId, currentDate);

                if (treningResult.Rows != null)
                {
                    foreach (var row in treningResult.Rows)
                    {
                        var cells = row.ToArray();
                        var data = cells[1]?.ToString() ?? "";
                        var godzinaStart = cells[2]?.ToString() ?? "";

                        if (DateTime.TryParseExact($"{data} {godzinaStart}", dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeStart))
                        {
                            if (dateTimeStart > DateTime.Now)
                            {
                                allEvents.Add(new UpcomingEvent
                                {
                                    EventType = "Trening",
                                    Title = cells[0]?.ToString() ?? "Brak",
                                    DateTimeStart = dateTimeStart,
                                    TimeEnd = cells[3]?.ToString() ?? "",
                                    Location = cells[4]?.ToString() ?? ""
                                });
                            }
                        }
                    }
                }

                // 3. Wydarzenia
                var wydSql = "SELECT Nazwa, Data, GodzinaStart, GodzinaKoniec, Miejsce, Opis FROM Wydarzenie WHERE TrenerID = ? AND Data >= ? ORDER BY Data, GodzinaStart;";
                var wydResult = await client.Execute(wydSql, trenerId, currentDate);

                if (wydResult.Rows != null)
                {
                    foreach (var row in wydResult.Rows)
                    {
                        var cells = row.ToArray();
                        var data = cells[1]?.ToString() ?? "";
                        var godzinaStart = cells[2]?.ToString() ?? "";

                        if (DateTime.TryParseExact($"{data} {godzinaStart}", dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeStart))
                        {
                            if (dateTimeStart > DateTime.Now)
                            {
                                allEvents.Add(new UpcomingEvent
                                {
                                    EventType = "Wydarzenie",
                                    Title = cells[0]?.ToString() ?? "Brak",
                                    DateTimeStart = dateTimeStart,
                                    TimeEnd = cells[3]?.ToString() ?? "",
                                    Location = cells[4]?.ToString() ?? "",
                                    Description = cells[5]?.ToString() ?? ""
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd pobierania: {ex.Message}", ex);
            }

            return allEvents.OrderBy(e => e.DateTimeStart).ToList();
        }

        public static async Task<List<UpcomingEvent>> GetAllEventsAsync()
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");

            var allEvents = new List<UpcomingEvent>();
            var trenerId = SessionService.AktualnyTrener.Id;
            string[] dateTimeFormats = new[] { "yyyy-MM-dd HH:mm", "yyyy-MM-dd H:mm" };

            try
            {
                using var client = await DatabaseConfig.CreateClientAsync();

                // Mecz
                var meczRes = await client.Execute("SELECT MeczID, Przeciwnik, Data, Godzina, Miejsce FROM Mecz WHERE TrenerID = ? ORDER BY Data, Godzina", trenerId);
                if (meczRes.Rows != null)
                {
                    foreach (var row in meczRes.Rows)
                    {
                        var c = row.ToArray();
                        if (DateTime.TryParseExact($"{c[2]} {c[3]}", dateTimeFormats, null, DateTimeStyles.None, out var dt))
                            allEvents.Add(new UpcomingEvent { EventType = "Mecz", Id = int.Parse(c[0].ToString()), Title = c[1].ToString(), DateTimeStart = dt, Location = c[4].ToString(), TimeEnd = "" });
                    }
                }

                // Trening
                var trenRes = await client.Execute("SELECT TreningID, Typ, Data, GodzinaRozpoczecia, GodzinaZakonczenia, Miejsce FROM Trening WHERE TrenerID = ? ORDER BY Data, GodzinaRozpoczecia", trenerId);
                if (trenRes.Rows != null)
                {
                    foreach (var row in trenRes.Rows)
                    {
                        var c = row.ToArray();
                        if (DateTime.TryParseExact($"{c[2]} {c[3]}", dateTimeFormats, null, DateTimeStyles.None, out var dt))
                            allEvents.Add(new UpcomingEvent { EventType = "Trening", Id = int.Parse(c[0].ToString()), Title = c[1].ToString(), DateTimeStart = dt, TimeEnd = c[4].ToString(), Location = c[5].ToString() });
                    }
                }

                // Wydarzenie
                var wydRes = await client.Execute("SELECT WydarzenieID, Nazwa, Data, GodzinaStart, GodzinaKoniec, Miejsce, Opis FROM Wydarzenie WHERE TrenerID = ? ORDER BY Data, GodzinaStart", trenerId);
                if (wydRes.Rows != null)
                {
                    foreach (var row in wydRes.Rows)
                    {
                        var c = row.ToArray();
                        if (DateTime.TryParseExact($"{c[2]} {c[3]}", dateTimeFormats, null, DateTimeStyles.None, out var dt))
                            allEvents.Add(new UpcomingEvent { EventType = "Wydarzenie", Id = int.Parse(c[0].ToString()), Title = c[1].ToString(), DateTimeStart = dt, TimeEnd = c[4].ToString(), Location = c[5].ToString(), Description = c[6].ToString() });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd: {ex.Message}", ex);
            }
            return allEvents.OrderBy(e => e.DateTimeStart).ToList();
        }

        public static async Task UpdateEventAsync(UpcomingEvent e)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Log in required");
            using var client = await DatabaseConfig.CreateClientAsync();
            var tid = SessionService.AktualnyTrener.Id;
            var data = e.DateTimeStart.ToString("yyyy-MM-dd");

            switch (e.EventType)
            {
                case "Mecz":
                    await client.Execute("UPDATE Mecz SET Przeciwnik=?, Data=?, Godzina=?, Miejsce=? WHERE MeczID=? AND TrenerID=?", e.Title, data, e.DisplayTimeStart, e.Location, e.Id, tid);
                    break;
                case "Trening":
                    await client.Execute("UPDATE Trening SET Typ=?, Data=?, GodzinaRozpoczecia=?, GodzinaZakonczenia=?, Miejsce=? WHERE TreningID=? AND TrenerID=?", e.Title, data, e.DisplayTimeStart, e.TimeEnd, e.Location, e.Id, tid);
                    break;
                case "Wydarzenie":
                    await client.Execute("UPDATE Wydarzenie SET Nazwa=?, Miejsce=?, Data=?, GodzinaStart=?, GodzinaKoniec=?, Opis=? WHERE WydarzenieID=? AND TrenerID=?", e.Title, e.Location, data, e.DisplayTimeStart, e.TimeEnd, e.Description, e.Id, tid);
                    break;
            }
        }

        public static async Task DeleteEventAsync(string eventType, int eventId)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Log in required");
            using var client = await DatabaseConfig.CreateClientAsync();
            var tid = SessionService.AktualnyTrener.Id;

            string table = eventType switch { "Mecz" => "Mecz", "Trening" => "Trening", "Wydarzenie" => "Wydarzenie", _ => throw new Exception("Unknown type") };
            string col = eventType + "ID";

            await client.Execute($"DELETE FROM {table} WHERE {col}=? AND TrenerID=?", eventId, tid);
        }
    }
}
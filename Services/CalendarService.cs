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
        // Formaty używane w bazie danych (SQLite trzyma to jako TEXT)
        private static readonly string[] _formats = { "yyyy-MM-dd HH:mm", "yyyy-MM-dd H:mm" };

        // --- ZAPIS (INSERT) ---
        // Rozbijamy DateTime na stringi, aby pasowały do istniejącej struktury tabel

        public static async Task AddMeczAsync(Mecz mecz)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");
            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = "INSERT INTO Mecz (SkladMeczowyID, Przeciwnik, Data, Godzina, Miejsce, TrenerID) VALUES (0, ?, ?, ?, ?, ?);";

            // Konwersja DateTime -> Stringi dla bazy
            string dataStr = mecz.DataRozpoczecia.ToString("yyyy-MM-dd");
            string godzinaStr = mecz.DataRozpoczecia.ToString("HH:mm");

            await client.Execute(sql, mecz.Przeciwnik, dataStr, godzinaStr, mecz.Miejsce, SessionService.AktualnyTrener.Id);
        }

        public static async Task AddTreningAsync(Trening trening)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");
            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = "INSERT INTO Trening (ListaObecnosciID, Typ, Data, GodzinaRozpoczecia, GodzinaZakonczenia, Miejsce, TrenerID) VALUES (0, ?, ?, ?, ?, ?, ?);";

            string dataStr = trening.DataRozpoczecia.ToString("yyyy-MM-dd");
            string startStr = trening.DataRozpoczecia.ToString("HH:mm");
            string endStr = trening.DataZakonczenia.ToString("HH:mm");

            await client.Execute(sql, trening.Typ, dataStr, startStr, endStr, trening.Miejsce, SessionService.AktualnyTrener.Id);
        }

        public static async Task AddWydarzenieAsync(Wydarzenie wydarzenie)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener nie jest zalogowany.");
            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = "INSERT INTO Wydarzenie (Nazwa, Miejsce, Data, GodzinaStart, GodzinaKoniec, Opis, TrenerID) VALUES (?, ?, ?, ?, ?, ?, ?);";

            string dataStr = wydarzenie.DataRozpoczecia.ToString("yyyy-MM-dd");
            string startStr = wydarzenie.DataRozpoczecia.ToString("HH:mm");
            string endStr = wydarzenie.DataZakonczenia.ToString("HH:mm");

            await client.Execute(sql, wydarzenie.Nazwa, wydarzenie.Miejsce, dataStr, startStr, endStr, wydarzenie.Opis, SessionService.AktualnyTrener.Id);
        }

        // --- AKTUALIZACJA (UPDATE) ---

        public static async Task UpdateEventAsync(UpcomingEvent e)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Log in required");
            using var client = await DatabaseConfig.CreateClientAsync();
            var tid = SessionService.AktualnyTrener.Id;

            // Konwersja DateTime na format bazy danych
            var dataStr = e.DateTimeStart.ToString("yyyy-MM-dd");
            var godzinaStr = e.DateTimeStart.ToString("HH:mm");

            switch (e.EventType)
            {
                case "Mecz":
                    await client.Execute("UPDATE Mecz SET Przeciwnik=?, Data=?, Godzina=?, Miejsce=? WHERE MeczID=? AND TrenerID=?",
                        e.Title, dataStr, godzinaStr, e.Location, e.Id, tid);
                    break;
                case "Trening":
                    await client.Execute("UPDATE Trening SET Typ=?, Data=?, GodzinaRozpoczecia=?, GodzinaZakonczenia=?, Miejsce=? WHERE TreningID=? AND TrenerID=?",
                        e.Title, dataStr, godzinaStr, e.TimeEnd, e.Location, e.Id, tid);
                    break;
                case "Wydarzenie":
                    await client.Execute("UPDATE Wydarzenie SET Nazwa=?, Miejsce=?, Data=?, GodzinaStart=?, GodzinaKoniec=?, Opis=? WHERE WydarzenieID=? AND TrenerID=?",
                        e.Title, e.Location, dataStr, godzinaStr, e.TimeEnd, e.Description, e.Id, tid);
                    break;
            }
        }

        // --- ODCZYT (SELECT) ---
        // Tutaj UpcomingEvent jest już poprawnym modelem z DateTime, więc tylko mapujemy kolumny tekstowe z bazy na niego.

        public static async Task<List<UpcomingEvent>> GetAllEventsAsync()
        {
            if (SessionService.AktualnyTrener == null) return new List<UpcomingEvent>();

            var allEvents = new List<UpcomingEvent>();
            var trenerId = SessionService.AktualnyTrener.Id;

            try
            {
                using var client = await DatabaseConfig.CreateClientAsync();

                // 1. Mecze
                var meczRes = await client.Execute("SELECT MeczID, Przeciwnik, Data, Godzina, Miejsce FROM Mecz WHERE TrenerID = ? ORDER BY Data, Godzina", trenerId);
                if (meczRes.Rows != null)
                {
                    foreach (var row in meczRes.Rows)
                    {
                        var c = row.ToArray();
                        // Sklejamy Data + Godzina w jeden DateTime
                        if (DateTime.TryParseExact($"{c[2]} {c[3]}", _formats, null, DateTimeStyles.None, out var dt))
                        {
                            allEvents.Add(new UpcomingEvent
                            {
                                EventType = "Mecz",
                                Id = int.Parse(c[0].ToString()),
                                Title = c[1].ToString(),
                                DateTimeStart = dt,
                                Location = c[4].ToString(),
                                TimeEnd = ""
                            });
                        }
                    }
                }

                // 2. Treningi
                var trenRes = await client.Execute("SELECT TreningID, Typ, Data, GodzinaRozpoczecia, GodzinaZakonczenia, Miejsce FROM Trening WHERE TrenerID = ? ORDER BY Data, GodzinaRozpoczecia", trenerId);
                if (trenRes.Rows != null)
                {
                    foreach (var row in trenRes.Rows)
                    {
                        var c = row.ToArray();
                        if (DateTime.TryParseExact($"{c[2]} {c[3]}", _formats, null, DateTimeStyles.None, out var dt))
                        {
                            allEvents.Add(new UpcomingEvent
                            {
                                EventType = "Trening",
                                Id = int.Parse(c[0].ToString()),
                                Title = c[1].ToString(),
                                DateTimeStart = dt,
                                TimeEnd = c[4].ToString(), // To na razie zostaje stringiem w UpcomingEvent (do wyświetlania)
                                Location = c[5].ToString()
                            });
                        }
                    }
                }

                // 3. Wydarzenia
                var wydRes = await client.Execute("SELECT WydarzenieID, Nazwa, Data, GodzinaStart, GodzinaKoniec, Miejsce, Opis FROM Wydarzenie WHERE TrenerID = ? ORDER BY Data, GodzinaStart", trenerId);
                if (wydRes.Rows != null)
                {
                    foreach (var row in wydRes.Rows)
                    {
                        var c = row.ToArray();
                        if (DateTime.TryParseExact($"{c[2]} {c[3]}", _formats, null, DateTimeStyles.None, out var dt))
                        {
                            allEvents.Add(new UpcomingEvent
                            {
                                EventType = "Wydarzenie",
                                Id = int.Parse(c[0].ToString()),
                                Title = c[1].ToString(),
                                DateTimeStart = dt,
                                TimeEnd = c[4].ToString(),
                                Location = c[5].ToString(),
                                Description = c[6].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Logowanie błędu można dodać tutaj
                System.Diagnostics.Debug.WriteLine($"DB Error: {ex.Message}");
            }
            return allEvents.OrderBy(e => e.DateTimeStart).ToList();
        }

        public static async Task<List<UpcomingEvent>> GetUpcomingEventsAsync()
        {
            var all = await GetAllEventsAsync();
            return all.Where(e => e.DateTimeStart >= DateTime.Now).OrderBy(e => e.DateTimeStart).ToList();
        }

        public static async Task DeleteEventAsync(string eventType, int eventId)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Wymagane logowanie.");

            using var client = await DatabaseConfig.CreateClientAsync();
            var tid = SessionService.AktualnyTrener.Id;

            // Ustalanie nazwy tabeli i kolumny ID
            string table = eventType switch
            {
                "Mecz" => "Mecz",
                "Trening" => "Trening",
                "Wydarzenie" => "Wydarzenie",
                _ => throw new Exception("Nieznany typ wydarzenia")
            };

            // Ważne: Kolumny w bazie nazywają się MeczID, TreningID, WydarzenieID
            string col = eventType + "ID";

            // Wykonanie zapytania SQL
            await client.Execute($"DELETE FROM {table} WHERE {col}=? AND TrenerID=?", eventId, tid);
        }
    }
}
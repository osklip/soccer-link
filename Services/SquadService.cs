using SoccerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    public class SquadService
    {
        public static async Task EnsureTableExistsAsync()
        {
            using var client = await DatabaseConfig.CreateClientAsync();
            var sql = @"
                CREATE TABLE IF NOT EXISTS SkladMeczowy (
                    SkladID INTEGER PRIMARY KEY AUTOINCREMENT,
                    MeczID INTEGER NOT NULL,
                    ZawodnikID INTEGER NOT NULL,
                    PozycjaKod TEXT NOT NULL
                );";
            await client.Execute(sql);
        }

        // ZMODYFIKOWANA METODA: Zapisuje skład i wysyła powiadomienia
        public static async Task SaveSquadAsync(int meczId, List<SkladEntry> entries)
        {
            if (SessionService.AktualnyTrener == null) return;

            await EnsureTableExistsAsync();
            await AvailabilityService.EnsureTableExistsAsync(); // Upewniamy się, że tabela dostępności istnieje

            using var client = await DatabaseConfig.CreateClientAsync();

            // 1. Usuwamy stary skład dla tego meczu
            await client.Execute("DELETE FROM SkladMeczowy WHERE MeczID = ?", meczId);

            var sqlInsert = "INSERT INTO SkladMeczowy (MeczID, ZawodnikID, PozycjaKod) VALUES (?, ?, ?)";

            foreach (var entry in entries)
            {
                if (entry.ZawodnikID > 0)
                {
                    // A. Zapis do bazy składu
                    await client.Execute(sqlInsert, meczId, entry.ZawodnikID, entry.PozycjaKod);

                    // B. Inicjalizacja dostępności (status Oczekujący)
                    await AvailabilityService.InicjujDostepnoscAsync(meczId, entry.ZawodnikID);

                    // C. Przygotowanie treści wiadomości
                    string opisPozycji = entry.PozycjaKod;

                    // Prosta logika zamiany kodów na tekst (dostosuj do swoich kodów)
                    if (opisPozycji.Contains("SUB") || opisPozycji.Contains("Bench"))
                        opisPozycji = "Ławka rezerwowych";
                    else if (opisPozycji == "GK") opisPozycji = "Bramkarz";
                    // ... możesz dodać więcej warunków

                    string temat = "Powołanie na mecz";
                    string tresc = $"Zostałeś powołany na mecz (ID: {meczId}). " +
                                   $"Twoja przydzielona rola/pozycja: {opisPozycji}. " +
                                   $"Proszę o potwierdzenie obecności w zakładce Mecze.";

                    // D. Wysłanie wiadomości do konkretnego zawodnika
                    await WiadomoscService.WyslijWiadomoscPrywatnaAsync(entry.ZawodnikID, temat, tresc);
                }
            }
        }

        public static async Task<List<SkladEntry>> GetSquadForMatchAsync(int meczId)
        {
            await EnsureTableExistsAsync();
            using var client = await DatabaseConfig.CreateClientAsync();

            var result = await client.Execute("SELECT SkladID, MeczID, ZawodnikID, PozycjaKod FROM SkladMeczowy WHERE MeczID = ?", meczId);
            var list = new List<SkladEntry>();

            if (result.Rows != null)
            {
                foreach (var row in result.Rows)
                {
                    var c = row.ToArray();
                    list.Add(new SkladEntry
                    {
                        SkladID = int.Parse(c[0].ToString()),
                        MeczID = int.Parse(c[1].ToString()),
                        ZawodnikID = int.Parse(c[2].ToString()),
                        PozycjaKod = c[3].ToString()
                    });
                }
            }
            return list;
        }
    }
}
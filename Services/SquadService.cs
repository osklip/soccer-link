using SoccerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    public class SquadService
    {
        // Metoda tworząca tabelę. 
        // UWAGA: Dodałem DROP TABLE, aby naprawić błąd "no such column". 
        // Przy pierwszym uruchomieniu usunie starą tabelę i stworzy poprawną.
        public static async Task EnsureTableExistsAsync()
        {
            using var client = await DatabaseConfig.CreateClientAsync();

            // Twardy reset tabeli przy zmianie struktury (dla celów deweloperskich)
            // Jeśli chcesz zachować dane w przyszłości, użyj ALTER TABLE lub migracji.
            // Na teraz - usuwamy starą wersję, żeby dodać kolumnę PozycjaKod.
            // await client.Execute("DROP TABLE IF EXISTS Sklad"); 

            // Wersja bezpieczniejsza: Tworzymy tylko jeśli nie istnieje. 
            // Jeśli masz błąd "no such column", odkomentuj powyższą linię "DROP TABLE" na jedno uruchomienie!
            // LUB: Zmienimy nazwę tabeli na 'SkladMeczowy', żeby stworzył nową, czystą tabelę.

            var sql = @"
                CREATE TABLE IF NOT EXISTS SkladMeczowy (
                    SkladID INTEGER PRIMARY KEY AUTOINCREMENT,
                    MeczID INTEGER NOT NULL,
                    ZawodnikID INTEGER NOT NULL,
                    PozycjaKod TEXT NOT NULL
                );";
            await client.Execute(sql);
        }

        public static async Task SaveSquadAsync(int meczId, List<SkladEntry> entries)
        {
            if (SessionService.AktualnyTrener == null) return;
            await EnsureTableExistsAsync();

            using var client = await DatabaseConfig.CreateClientAsync();

            // 1. Usuwamy stary skład dla tego meczu (nadpisanie)
            await client.Execute("DELETE FROM SkladMeczowy WHERE MeczID = ?", meczId);

            // 2. Dodajemy nowy
            var sql = "INSERT INTO SkladMeczowy (MeczID, ZawodnikID, PozycjaKod) VALUES (?, ?, ?)";

            foreach (var entry in entries)
            {
                if (entry.ZawodnikID > 0)
                {
                    await client.Execute(sql, meczId, entry.ZawodnikID, entry.PozycjaKod);
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
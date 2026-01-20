using Libsql.Client;
using SoccerLink.Models;
using System;
using System.Linq; // [WAŻNE] Dodano brakujący namespace dla First() i ToArray()
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    public class AvailabilityService
    {
        // 1. Tworzenie tabeli dostępności
        public static async Task EnsureTableExistsAsync()
        {
            using var client = await DatabaseConfig.CreateClientAsync();

            var sql = @"
                CREATE TABLE IF NOT EXISTS MeczDostepnosc (
                    DostepnoscID INTEGER PRIMARY KEY AUTOINCREMENT,
                    MeczID INTEGER NOT NULL,
                    ZawodnikID INTEGER NOT NULL,
                    Status INTEGER DEFAULT 0, -- 0: Pytanie wysłane, 1: Potwierdził, 2: Odmówił
                    DataZgloszenia TEXT
                );";
            await client.Execute(sql);
        }

        // 2. Inicjalizacja statusu (wywoływane przy zapisie składu)
        public static async Task InicjujDostepnoscAsync(int meczId, int zawodnikId)
        {
            await EnsureTableExistsAsync();
            using var client = await DatabaseConfig.CreateClientAsync();

            // Sprawdzamy czy wpis już istnieje
            var checkSql = "SELECT COUNT(*) FROM MeczDostepnosc WHERE MeczID = ? AND ZawodnikID = ?";
            var result = await client.Execute(checkSql, meczId, zawodnikId);

            // Pobieramy wynik bezpiecznie (zgodnie z poprawką)
            var row = result.Rows.First().ToArray();
            var count = int.Parse(row[0].ToString());

            if (count == 0)
            {
                // ZMIANA TUTAJ: Status 1 (Potwierdzony) zamiast 0
                var insertSql = @"
            INSERT INTO MeczDostepnosc (MeczID, ZawodnikID, Status, DataZgloszenia) 
            VALUES (?, ?, 1, ?)";
                await client.Execute(insertSql, meczId, zawodnikId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                // Opcjonalnie: Jeśli już był, a trener go dodał ponownie, ustawiamy na 1 (Obecny)
                var resetSql = "UPDATE MeczDostepnosc SET Status = 1 WHERE MeczID = ? AND ZawodnikID = ?";
                await client.Execute(resetSql, meczId, zawodnikId);
            }
        }

        // 3. KLUCZOWA METODA: Zawodnik zgłasza niedyspozycyjność
        public static async Task ZglosNiedyspozycyjnoscAsync(int meczId, int zawodnikId)
        {
            using var client = await DatabaseConfig.CreateClientAsync();

            // A. Aktualizujemy status w tabeli dostępności na '2' (Niedostępny)
            var updateStatusSql = "UPDATE MeczDostepnosc SET Status = 2, DataZgloszenia = ? WHERE MeczID = ? AND ZawodnikID = ?";
            await client.Execute(updateStatusSql, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), meczId, zawodnikId);

            // B. SYSTEM AUTOMATYCZNIE USUWA ZAWODNIKA ZE SKŁADU MECZOWEGO
            var deleteSquadSql = "DELETE FROM SkladMeczowy WHERE MeczID = ? AND ZawodnikID = ?";
            await client.Execute(deleteSquadSql, meczId, zawodnikId);
        }

        // 4. Metoda do potwierdzenia obecności
        public static async Task PotwierdzObecnoscAsync(int meczId, int zawodnikId)
        {
            using var client = await DatabaseConfig.CreateClientAsync();
            var sql = "UPDATE MeczDostepnosc SET Status = 1, DataZgloszenia = ? WHERE MeczID = ? AND ZawodnikID = ?";
            await client.Execute(sql, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), meczId, zawodnikId);
        }
    }
}
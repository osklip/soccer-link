using Libsql.Client;
using SoccerLink.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    internal class LoginService
    {
        public static async Task<ZalogowanyTrener?> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            // Użycie centralnej konfiguracji
            using var client = await DatabaseConfig.CreateClientAsync();

            // Zapytanie parametryzowane (zabezpieczenie przed SQL Injection)
            var sql = @"
                SELECT TrenerID, AdresEmail, Haslo, Imie, Nazwisko, NumerTelefonu
                FROM Trener
                WHERE AdresEmail = @email
                LIMIT 1;";

            // Przekazanie parametrów
            var parameters = new { email };
            var result = await client.Execute(sql, parameters);

            if (result.Rows == null || !result.Rows.Any())
                return null;

            var row = result.Rows.First().ToArray();
            var storedPassword = row[2]?.ToString();

            // Porównanie haseł (bez hashowania - zgodnie z prośbą)
            if (storedPassword != password)
                return null;

            return new ZalogowanyTrener
            {
                Id = int.Parse(row[0].ToString()),
                AdresEmail = row[1].ToString(),
                Imie = row[3].ToString(),       // Poprawiony indeks (w SQL Imie jest 4. kolumną, czyli index 3)
                Nazwisko = row[4].ToString(),   // Poprawiony indeks
                NumerTelefonu = row[5].ToString() // Poprawiony indeks
            };
        }
    }
}
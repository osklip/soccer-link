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

            using var client = await DatabaseConfig.CreateClientAsync();

            
            var sql = @"
                SELECT TrenerID, AdresEmail, Haslo, Imie, Nazwisko, NumerTelefonu
                FROM Trener
                WHERE AdresEmail = ?
                LIMIT 1;";

            
            var result = await client.Execute(sql, email);

            if (result.Rows == null || !result.Rows.Any())
                return null;

            var row = result.Rows.First().ToArray();
            var storedPassword = row[2]?.ToString();

            if (storedPassword != password)
                return null;

            return new ZalogowanyTrener
            {
                Id = int.Parse(row[0].ToString()),
                AdresEmail = row[1].ToString(),
                Imie = row[3].ToString(),
                Nazwisko = row[4].ToString(),
                NumerTelefonu = row[5].ToString()
            };
        }
    }
}
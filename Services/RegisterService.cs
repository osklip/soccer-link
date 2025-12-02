using Libsql.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    internal class RegisterService
    {
        public static async Task<bool> RegisterAsync(
            string email,
            string password,
            string firstName,
            string lastName,
            string phoneNumber)
        {
            using var client = await DatabaseConfig.CreateClientAsync();

            string cleanEmail = email.Trim();
            string cleanPhone = phoneNumber.Trim();
            string cleanPass = password.Trim();
            string cleanFirst = firstName.Trim();
            string cleanLast = lastName.Trim();

            // 1. Sprawdzenie (4 znaki zapytania w odpowiedniej kolejności)
            var checkSql = @"
                SELECT COUNT(1) FROM (
                    SELECT AdresEmail FROM Trener WHERE AdresEmail = ? OR NumerTelefonu = ?
                    UNION ALL 
                    SELECT AdresEmail FROM Zawodnik WHERE AdresEmail = ? OR NumerTelefonu = ?
                );";

            // Przekazujemy parametry w kolejności występowania '?'
            var checkResult = await client.Execute(checkSql, cleanEmail, cleanPhone, cleanEmail, cleanPhone);

            if (checkResult.Rows != null && checkResult.Rows.Any())
            {
                var firstRow = checkResult.Rows.First();
                var cells = firstRow.ToArray();
                if (cells.Length > 0 && long.TryParse(cells[0]?.ToString(), out var count) && count > 0)
                {
                    return false;
                }
            }

            // 2. Rejestracja (5 znaków zapytania)
            var insertSql = @"
                INSERT INTO Trener (AdresEmail, Haslo, NumerTelefonu, Imie, Nazwisko, ProbyLogowania)
                VALUES (?, ?, ?, ?, ?, 0);";

            // Kolejność: email, hasło, telefon, imię, nazwisko
            await client.Execute(insertSql, cleanEmail, cleanPass, cleanPhone, cleanFirst, cleanLast);

            return true;
        }
    }
}
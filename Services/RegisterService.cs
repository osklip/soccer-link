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

            // 1. Sprawdzenie czy użytkownik istnieje (parametryzowane)
            // Sprawdzamy w obu tabelach
            var checkSql = @"
                SELECT COUNT(1) FROM (
                    SELECT AdresEmail FROM Trener WHERE AdresEmail = @email OR NumerTelefonu = @phone
                    UNION ALL 
                    SELECT AdresEmail FROM Zawodnik WHERE AdresEmail = @email OR NumerTelefonu = @phone
                );";

            var checkParams = new { email = cleanEmail, phone = cleanPhone };
            var checkResult = await client.Execute(checkSql, checkParams);

            if (checkResult.Rows != null && checkResult.Rows.Any())
            {
                var firstRow = checkResult.Rows.First();
                var cells = firstRow.ToArray();
                if (cells.Length > 0 && long.TryParse(cells[0]?.ToString(), out var count) && count > 0)
                {
                    return false; // Użytkownik już istnieje
                }
            }

            // 2. Rejestracja (INSERT parametryzowany)
            var insertSql = @"
                INSERT INTO Trener (AdresEmail, Haslo, NumerTelefonu, Imie, Nazwisko, ProbyLogowania)
                VALUES (@email, @password, @phone, @firstName, @lastName, 0);";

            var insertParams = new
            {
                email = cleanEmail,
                password = password.Trim(), // Hasło jawnym tekstem (zgodnie z życzeniem)
                phone = cleanPhone,
                firstName = firstName.Trim(),
                lastName = lastName.Trim()
            };

            await client.Execute(insertSql, insertParams);

            return true;
        }
    }
}
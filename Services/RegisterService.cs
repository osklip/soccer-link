using Libsql.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    internal class RegisterService
    {
        private const string Url = "https://soccerlinkdb-enbixd.aws-eu-west-1.turso.io";
        private const string Token = "eyJhbGciOiJFZERTQSIsInR5cCI6IkpXVCJ9.eyJhIjoicnciLCJnaWQiOiJjYTliNTRlNy0zMGNkLTQwOWEtOGEzMy03MjJkZmQxYmFmNGIiLCJpYXQiOjE3NjEzNDEwOTQsInJpZCI6ImEwYjU0YzNmLWZmZGMtNDIyMi1iNmExLWRkYWU3MTdiNTJmOCJ9.dbND6Ysq3h8RphlNnJF9f8TFNdgwyWsHNADEPWDi_iKlwJHGWPBmUIaKuEuWlU_QdvvQSkcf8SN_OWGNoWP4DQ";

        public static async Task<bool> RegisterAsync(
            string email,
            string password,
            string firstName,
            string lastName,
            string phoneNumber)
        {

            using var client = await DatabaseClient.Create(o =>
            {
                o.Url = Url;
                o.AuthToken = Token;
                o.UseHttps = true;
            });

            // "Sanityzacja" stringów jak w LoginService
            var safeEmail = email.Replace("'", "''").Trim();
            var safePassword = password.Replace("'", "''").Trim();
            var safeFirstName = firstName.Replace("'", "''").Trim();
            var safeLastName = lastName.Replace("'", "''").Trim();
            var safePhoneNumber = phoneNumber.Replace("'", "''").Trim();

            // 1. Sprawdź, czy użytkownik o takim emailu już istnieje
            var checkSql = $"SELECT COUNT(1) FROM Uzytkownik WHERE AdresEmail = '{safeEmail}' LIMIT 1;";
            var checkResult = await client.Execute(checkSql);

            if (checkResult.Rows != null && checkResult.Rows.Any())
            {
                var firstRow = checkResult.Rows.First();
                var cells = firstRow.ToArray();
                if (cells.Length > 0 && long.TryParse(cells[0]?.ToString(), out var count) && count > 0)
                {
                    // Użytkownik z takim emailem już istnieje
                    return false;
                }
            }

            // 2. Rejestracja nowego użytkownika
            // Rola = 1, ProbyLogowania = 0
            var insertSql = $@"
                            INSERT INTO Uzytkownik
                            (AdresEmail, Haslo, Imie, Nazwisko, NumerTelefonu, Rola, ProbyLogowania)
                            VALUES
                            ('{safeEmail}', '{safePassword}', '{safeFirstName}', '{safeLastName}', '{safePhoneNumber}', 1, 0);";

            await client.Execute(insertSql);

            return true;
        }
    }
}

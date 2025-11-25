using Libsql.Client;
using Microsoft.UI.Xaml.Controls.Primitives;
using SoccerLink.Models;
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
        private const string Token = "eyJhbGciOiJFZERTQSIsInR5cCI6IkpXVCJ9.eyJhIjoicnciLCJleHAiOjE3OTU2MzcwODksImdpZCI6ImNhOWI1NGU3LTMwY2QtNDA5YS04YTMzLTcyMmRmZDFiYWY0YiIsImlhdCI6MTc2NDEwMTA4OSwicmlkIjoiYTBiNTRjM2YtZmZkYy00MjIyLWI2YTEtZGRhZTcxN2I1MmY4In0.dnupQBG2k5tiShROTpDhcHjm8b36JHLd4tebvAWESVZ-PtLlz40gq0ywuhf3c9MefzIFmZLkTVCZpgm5dw20Dg";

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

            var safeEmail = email.Replace("'", "''").Trim();
            var safePassword = password.Replace("'", "''").Trim();
            var safeFirstName = firstName.Replace("'", "''").Trim();
            var safeLastName = lastName.Replace("'", "''").Trim();
            var safePhoneNumber = phoneNumber.Replace("'", "''").Trim();

            var checkSql = $"SELECT COUNT(1) FROM (SELECT AdresEmail, NumerTelefonu FROM Trener WHERE AdresEmail = '{safeEmail}' OR NumerTelefonu = '{safePhoneNumber}' " +
                           $"UNION ALL SELECT AdresEmail, NumerTelefonu FROM Zawodnik WHERE AdresEmail = '{safeEmail}' OR NumerTelefonu = '{safePhoneNumber}');";
            var checkResult = await client.Execute(checkSql);

            if (checkResult.Rows != null && checkResult.Rows.Any())
            {
                var firstRow = checkResult.Rows.First();
                var cells = firstRow.ToArray();
                if (cells.Length > 0 && long.TryParse(cells[0]?.ToString(), out var count) && count > 0)
                {
                    return false;
                }
            }

            var insertSql = $@"
                            INSERT INTO Trener
                            (AdresEmail, Haslo, NumerTelefonu, Imie, Nazwisko, ProbyLogowania)
                            VALUES
                            ('{safeEmail}', '{safePassword}', '{safePhoneNumber}', '{safeFirstName}', '{safeLastName}', 0);";

            await client.Execute(insertSql);

            return true;
        }
    }
}

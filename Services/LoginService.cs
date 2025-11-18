using Libsql.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    internal class LoginService
    {
        private const string Url = "https://soccerlinkdb-enbixd.aws-eu-west-1.turso.io";
        private const string Token = "eyJhbGciOiJFZERTQSIsInR5cCI6IkpXVCJ9.eyJhIjoicnciLCJnaWQiOiJjYTliNTRlNy0zMGNkLTQwOWEtOGEzMy03MjJkZmQxYmFmNGIiLCJpYXQiOjE3NjEzNDEwOTQsInJpZCI6ImEwYjU0YzNmLWZmZGMtNDIyMi1iNmExLWRkYWU3MTdiNTJmOCJ9.dbND6Ysq3h8RphlNnJF9f8TFNdgwyWsHNADEPWDi_iKlwJHGWPBmUIaKuEuWlU_QdvvQSkcf8SN_OWGNoWP4DQ";

        public static async Task<bool> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))           
                return false;

            using var client = await DatabaseClient.Create(o =>
            {
                o.Url = Url;
                o.AuthToken = Token;
                o.UseHttps = true;
            });

            // Jakaś podmianka , aby uniknąć SQL Injection (nw po co ale podobno tak się robi)
            var safeEmail = email.Replace("'", "''").Trim();

            var sql = $"SELECT Haslo FROM Uzytkownik WHERE AdresEmail = '{safeEmail}' LIMIT 1;";

            var result = await client.Execute(sql);

            if (result.Rows == null || !result.Rows.Any())
                return false;

            var firstRow = result.Rows.First();
            var cells = firstRow.ToArray();
            var storedPassword = cells.Length > 0 ? cells[0]?.ToString() : null;

            if (storedPassword == null)
                return false;

            // Miejsce na hash haseł, do zrobienia potem
            return storedPassword == password;
        }
    }
}

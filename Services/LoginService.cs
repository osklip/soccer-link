using Libsql.Client;
using SoccerLink.Models;
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
        private const string Token = "eyJhbGciOiJFZERTQSIsInR5cCI6IkpXVCJ9.eyJhIjoicnciLCJleHAiOjE3OTU2MzcwODksImdpZCI6ImNhOWI1NGU3LTMwY2QtNDA5YS04YTMzLTcyMmRmZDFiYWY0YiIsImlhdCI6MTc2NDEwMTA4OSwicmlkIjoiYTBiNTRjM2YtZmZkYy00MjIyLWI2YTEtZGRhZTcxN2I1MmY4In0.dnupQBG2k5tiShROTpDhcHjm8b36JHLd4tebvAWESVZ-PtLlz40gq0ywuhf3c9MefzIFmZLkTVCZpgm5dw20Dg";

        public static async Task<ZalogowanyTrener?> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))           
                return null;

            using var client = await DatabaseClient.Create(o =>
            {
                o.Url = Url;
                o.AuthToken = Token;
                o.UseHttps = true;
            });

            var safeEmail = email.Replace("'", "''").Trim();

            var sql = $@"
                SELECT TrenerID, AdresEmail, Haslo, Imie, Nazwisko, NumerTelefonu
                FROM Trener
                WHERE AdresEmail = '{safeEmail}'
                LIMIT 1;
            ";

            var result = await client.Execute(sql);

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
                Imie = row[4].ToString(),
                Nazwisko = row[5].ToString(),
                NumerTelefonu = row[3].ToString()
            };
        }
    }
}

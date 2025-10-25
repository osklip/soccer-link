using Libsql.Client;
using Nelknet.LibSQL.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SoccerLink.Services
{
    internal class DatabaseConnection
    {
        private const string Url = "https://soccerlinkdb-enbixd.aws-eu-west-1.turso.io";
        private const string Token = "eyJhbGciOiJFZERTQSIsInR5cCI6IkpXVCJ9.eyJhIjoicnciLCJnaWQiOiJjYTliNTRlNy0zMGNkLTQwOWEtOGEzMy03MjJkZmQxYmFmNGIiLCJpYXQiOjE3NjEzNDEwOTQsInJpZCI6ImEwYjU0YzNmLWZmZGMtNDIyMi1iNmExLWRkYWU3MTdiNTJmOCJ9.dbND6Ysq3h8RphlNnJF9f8TFNdgwyWsHNADEPWDi_iKlwJHGWPBmUIaKuEuWlU_QdvvQSkcf8SN_OWGNoWP4DQ";

        public static async Task TestConnectionAsync()
        {
            using var client = await DatabaseClient.Create(o =>
            {
                o.Url = Url;
                o.AuthToken = Token;
                o.UseHttps = true;
            });

            var sql = "SELECT UzytkownikID, Email, Rola FROM Uzytkownik ORDER BY UzytkownikID;";

            var result = await client.Execute(sql);

            foreach (var row in result.Rows)
            {
                var cells = row.ToArray();
                var id = cells[0]?.ToString();
                var mail = cells[1]?.ToString();
                var rola = cells[2]?.ToString();

                Debug.WriteLine($"🧑 {id} | {mail} | {rola}");
            }
        }
    }
}

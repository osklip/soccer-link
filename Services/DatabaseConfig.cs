using Libsql.Client;
using SoccerLink.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    public class DatabaseConfig
    {
        public static async Task<IDatabaseClient> CreateClientAsync()
        {
            return await DatabaseClient.Create(o => {
                o.Url = Url;
                o.AuthToken = Token;
                o.UseHttps = true;
            });
        }

        public const string Url = "https://soccerlinkdb-enbixd.aws-eu-west-1.turso.io";
        public const string Token = "eyJhbGciOiJFZERTQSIsInR5cCI6IkpXVCJ9.eyJhIjoicnciLCJleHAiOjE3OTU2MzcwODksImdpZCI6ImNhOWI1NGU3LTMwY2QtNDA5YS04YTMzLTcyMmRmZDFiYWY0YiIsImlhdCI6MTc2NDEwMTA4OSwicmlkIjoiYTBiNTRjM2YtZmZkYy00MjIyLWI2YTEtZGRhZTcxN2I1MmY4In0.dnupQBG2k5tiShROTpDhcHjm8b36JHLd4tebvAWESVZ-PtLlz40gq0ywuhf3c9MefzIFmZLkTVCZpgm5dw20Dg";
    }
}

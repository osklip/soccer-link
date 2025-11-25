using Libsql.Client;
using SoccerLink.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    class CalendarService
    {
        private const string Url = "https://soccerlinkdb-enbixd.aws-eu-west-1.turso.io";
        private const string Token = "eyJhbGciOiJFZERTQSIsInR5cCI6IkpXVCJ9.eyJhIjoicnciLCJleHAiOjE3OTU2MzcwODksImdpZCI6ImNhOWI1NGU3LTMwY2QtNDA5YS04YTMzLTcyMmRmZDFiYWY0YiIsImlhdCI6MTc2NDEwMTA4OSwicmlkIjoiYTBiNTRjM2YtZmZkYy00MjIyLWI2YTEtZGRhZTcxN2I1MmY4In0.dnupQBG2k5tiShROTpDhcHjm8b36JHLd4tebvAWESVZ-PtLlz40gq0ywuhf3c9MefzIFmZLkTVCZpgm5dw20Dg";

        // Metoda pomocnicza do ucieczki znaków (jak w innych serwisach)
        private static string Escape(string value) => value.Replace("'", "''").Trim();

        public static async Task AddMeczAsync(Mecz mecz)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener not logged in.");

            // POPRAWKA: Tworzenie klienta bezpośrednio
            using var client = await DatabaseClient.Create(o =>
            {
                o.Url = Url;
                o.AuthToken = Token;
                o.UseHttps = true;
            });

            var sql = $@"
                INSERT INTO Mecz (SkladMeczowyID, Przeciwnik, Data, Godzina, Miejsce, TrenerID)
                VALUES (
                    0, 
                    '{Escape(mecz.Przeciwnik)}', 
                    '{Escape(mecz.Data)}', 
                    '{Escape(mecz.Godzina)}', 
                    '{Escape(mecz.Miejsce)}', 
                    {SessionService.AktualnyTrener.Id}
                );";

            await client.Execute(sql);
        }

        public static async Task AddTreningAsync(Trening trening)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener not logged in.");

            // POPRAWKA: Tworzenie klienta bezpośrednio
            using var client = await DatabaseClient.Create(o =>
            {
                o.Url = Url;
                o.AuthToken = Token;
                o.UseHttps = true;
            });

            var sql = $@"
                INSERT INTO Trening (ListaObecnosciID, Typ, Data, GodzinaRozpoczecia, GodzinaZakonczenia, Miejsce, TrenerID)
                VALUES (
                    0, 
                    '{Escape(trening.Typ)}', 
                    '{Escape(trening.Data)}', 
                    '{Escape(trening.GodzinaRozpoczecia)}', 
                    '{Escape(trening.GodzinaZakonczenia)}', 
                    '{Escape(trening.Miejsce)}', 
                    {SessionService.AktualnyTrener.Id}
                );";

            await client.Execute(sql);
        }

        public static async Task AddWydarzenieAsync(Wydarzenie wydarzenie)
        {
            if (SessionService.AktualnyTrener == null) throw new InvalidOperationException("Trener not logged in.");

            // POPRAWKA: Tworzenie klienta bezpośrednio
            using var client = await DatabaseClient.Create(o =>
            {
                o.Url = Url;
                o.AuthToken = Token;
                o.UseHttps = true;
            });

            var sql = $@"
                INSERT INTO Wydarzenie (Nazwa, Miejsce, Data, GodzinaStart, GodzinaKoniec, Opis, TrenerID)
                VALUES (
                    '{Escape(wydarzenie.Nazwa)}', 
                    '{Escape(wydarzenie.Miejsce)}', 
                    '{Escape(wydarzenie.Data)}', 
                    '{Escape(wydarzenie.GodzinaStart)}', 
                    '{Escape(wydarzenie.GodzinaKoniec)}', 
                    '{Escape(wydarzenie.Opis)}', 
                    {SessionService.AktualnyTrener.Id}
                );";

            await client.Execute(sql);
        }
    }
}
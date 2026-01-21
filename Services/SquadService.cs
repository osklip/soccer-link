using SoccerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLink.Services
{
    public class SquadService
    {
        public static async Task EnsureTableExistsAsync()
        {
            using var client = await DatabaseConfig.CreateClientAsync();
            var sql = @"
                CREATE TABLE IF NOT EXISTS SkladMeczowy (
                    SkladID INTEGER PRIMARY KEY AUTOINCREMENT,
                    MeczID INTEGER NOT NULL,
                    ZawodnikID INTEGER NOT NULL,
                    PozycjaKod TEXT NOT NULL
                );";
            await client.Execute(sql);
        }

        
        public static async Task SaveSquadAsync(int meczId, List<SkladEntry> entries)
        {
            if (SessionService.AktualnyTrener == null) return;

            await EnsureTableExistsAsync();
            await AvailabilityService.EnsureTableExistsAsync(); 

            using var client = await DatabaseConfig.CreateClientAsync();

            
            await client.Execute("DELETE FROM SkladMeczowy WHERE MeczID = ?", meczId);

            var sqlInsert = "INSERT INTO SkladMeczowy (MeczID, ZawodnikID, PozycjaKod) VALUES (?, ?, ?)";

            foreach (var entry in entries)
            {
                if (entry.ZawodnikID > 0)
                {
                    
                    await client.Execute(sqlInsert, meczId, entry.ZawodnikID, entry.PozycjaKod);

                    
                    await AvailabilityService.InicjujDostepnoscAsync(meczId, entry.ZawodnikID);

                    
                    string opisPozycji = entry.PozycjaKod;

                    if (opisPozycji.Contains("SUB") || opisPozycji.Contains("Bench"))
                        opisPozycji = "Ławka rezerwowych";
                    else if (opisPozycji == "GK") opisPozycji = "Bramkarz";
                   

                    string temat = "Powołanie na mecz";
                    string tresc = $"Zostałeś powołany na mecz (ID: {meczId}). " +
                                   $"Twoja przydzielona rola/pozycja: {opisPozycji}. " +
                                   $"Proszę o potwierdzenie obecności w zakładce Mecze.";

                    
                    await WiadomoscService.WyslijWiadomoscPrywatnaAsync(entry.ZawodnikID, temat, tresc);
                }
            }
        }

        public static async Task<List<SkladEntry>> GetSquadForMatchAsync(int meczId)
        {
            await EnsureTableExistsAsync();
            using var client = await DatabaseConfig.CreateClientAsync();

            var result = await client.Execute("SELECT SkladID, MeczID, ZawodnikID, PozycjaKod FROM SkladMeczowy WHERE MeczID = ?", meczId);
            var list = new List<SkladEntry>();

            if (result.Rows != null)
            {
                foreach (var row in result.Rows)
                {
                    var c = row.ToArray();
                    list.Add(new SkladEntry
                    {
                        SkladID = int.Parse(c[0].ToString()),
                        MeczID = int.Parse(c[1].ToString()),
                        ZawodnikID = int.Parse(c[2].ToString()),
                        PozycjaKod = c[3].ToString()
                    });
                }
            }
            return list;
        }
    }
}
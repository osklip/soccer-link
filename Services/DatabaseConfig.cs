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
                o.Url = Secrets.Url;
                o.AuthToken = Secrets.Token;
                o.UseHttps = true;
            });
        }
    }
}

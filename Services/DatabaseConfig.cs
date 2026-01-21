using Libsql.Client;
using Microsoft.VisualBasic;
using SoccerLink;
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

                var url = Constants.DatabaseUrl;

                if (!string.IsNullOrEmpty(url) && url.StartsWith("libsql://"))
                {
                    url = "https://" + url.Substring("libsql://".Length);
                }

                o.Url = url;
                o.AuthToken = Constants.DatabaseToken;
                o.UseHttps = true;
            });
        }
    }
}
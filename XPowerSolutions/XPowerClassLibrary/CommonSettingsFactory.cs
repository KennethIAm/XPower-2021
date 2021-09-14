using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace XPowerClassLibrary
{

    public static class CommonSettingsFactory
    {

        internal static SqlConnection GetDBConnectionString(string username, string password)
        {
            // Tempoary hardcoded DB creadentials. Should be removed when specific users are set up.
            //username = "sa";
            //password = "Kode1234!";
            string connectionString = $"Server=172.16.21.33;Database=XPower_Test;User Id={username};Password={password};";

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            return sqlConnection;
        }

        internal static string JwtSecret()
        {
            string jwtSecretString = @"vxRvSjFCCkiFVDGPqnxdgg4nvqwtEn5EGDQPmmUkv0ug26Rmle2e7UOvQamObvWVvw1diOHb2ueUyaPhots+8n+gRNrP5Y6hBkKPD/Cvq9+Q+A";
            return jwtSecretString;
        }

    }
}

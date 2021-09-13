using XPowerClassLibrary.Users.Tokens.JwtTokens;
using XPowerClassLibrary.Users.Tokens.RefreshTokens;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace XPowerClassLibrary.Users.Tokens
{
    static class TokenFactory
    {
        public static IJwtTokenGenerator GetJwtTokenGenerator()
        {
            return new JwtTokenGeneratorHmacSha256();
        }

        public static IRefreshTokenGenerator GetRefreshTokenGenerator()
        {
            return new RefreshTokenGeneratorRNGCrypto();
        }

        /// <summary>
        /// SqlConnection with permission to create and update Refresh Tokens.
        /// </summary>
        /// <returns>SqlConnection with specific permission</returns>
        internal static SqlConnection GetSqlConnectionRefreshTokenCreator()
        {
            string username = "RefreshTokenCreater";
            string password = "Passw0rd";

            return CommonSettingsFactory.GetDBConnectionString(username, password);
        }

        /// <summary>
        /// SqlConnection with permission to read RefreshTokens.
        /// </summary>
        /// <returns>SqlConnection with specific permission</returns>
        internal static SqlConnection GetSqlConnectionRefreshTokenBasicReader()
        {
            string username = "RefreshTokenBasicReader";
            string password = "Passw0rd";

            return CommonSettingsFactory.GetDBConnectionString(username, password);
        }
    }
}

using XPowerClassLibrary.Users.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace XPowerClassLibrary.Users
{
    public static class UserServiceFactory
    {
        /// <summary>
        /// SqlConnection with permission to create new user via SPCreateNewUser
        /// </summary>
        /// <returns>SqlConnection with specific permission</returns>
        internal static SqlConnection GetSqlConnectionCreateUser()
        {
            string username = "UserCreator";
            string password = "Passw0rd";

            return CommonSettingsFactory.GetDBConnectionString(username, password);
        }

        /// <summary>
        /// SqlConnection with permission to basic select.
        /// </summary>
        /// <returns>SqlConnection with specific permission</returns>
        internal static SqlConnection GetSqlConnectionBasicReader()
        {
            string username = "UserBasicReader";
            string password = "Passw0rd";

            return CommonSettingsFactory.GetDBConnectionString(username, password);
        }

        /// <summary>
        /// SqlConnection with permission to complex select, including View and select SP.
        /// </summary>
        /// <returns>SqlConnection with specific permission</returns>
        internal static SqlConnection GetSqlConnectionComplexSelect()
        {
            string username = "UserComplexReader";
            string password = "Passw0rd";

            return CommonSettingsFactory.GetDBConnectionString(username, password);
        }

        /// <summary>
        /// SqlConnection with permission to delete users.
        /// </summary>
        /// <returns>SqlConnection with specific permission</returns>
        internal static SqlConnection GetSqlConnectionDeleteUser()
        {
            string username = "UserDelete";
            string password = "Passw0rd";

            return CommonSettingsFactory.GetDBConnectionString(username, password);
        }

        public static IUserService GetUserServiceDB()
        {
            return new UserService(new DbUserRepository());
        }

        /// <summary>
        /// Sets the amount of time to JWT token should live, before it needs to be refreshed.
        /// The JWT token is used to authorize users for the API.
        /// </summary>
        /// <returns>DateTime holding the lifetime of JWT token.</returns>
        internal static DateTime GetTokenTTL()
        {
            return DateTime.UtcNow.AddMinutes(2);  // The amount of minutes the JwtToken remains active and usable.
        }

    }
}

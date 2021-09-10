using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XPowerAPI.Entities;
using XPowerAPI.Models;

namespace XPowerAPI.Repositories
{
    public class UserRepositoryDB : IUserRepository
    {
        SqlConnection conn;

        string connectionString = @"TEMP";

        public AuthenticateResponse Authenticate(string email, string password)
        {

            throw new NotImplementedException();
        }

        public IUser CreateUser(CreateUserRequest request)
        {
            User returnUser = new User();

            using (conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("CreateUser", conn); // See "Stored Procedures-script" in XPowerSolutions
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = request.Email;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = request.Password;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    returnUser.Id = Convert.ToInt32(reader.GetValue(1));
                    returnUser.Email = reader.GetValue(2).ToString();
                    returnUser.Password += reader.GetValue(3).ToString();
                }
            }

            return returnUser;
        }

        public AuthenticateResponse RefreshToken(string email, string token)
        {
            throw new NotImplementedException();
        }

        public bool RevokeToken(string email, string token)
        {
            throw new NotImplementedException();
        }
    }
}

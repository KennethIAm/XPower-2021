using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using XPowerClassLibrary.Hashing;
using XPowerClassLibrary.Users.Entities;
using XPowerClassLibrary.Users.Models;
using XPowerClassLibrary.Users.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace XPowerClassLibrary.Users.Repository
{
    /// <summary>
    /// User Repository used while dealing with Database
    /// </summary>
    public class DbUserRepository : IUserRepository
    {
        /// <summary>
        /// Authenticates user by login credentials.
        /// Returns true if credentials is correct,
        /// returns false if creadentials are not correct.
        /// </summary>
        /// <param name="mail">Mail to validate.</param>
        /// <param name="password">Password to validate.</param>
        /// <returns>Bool based on validation success.</returns>
        public AuthenticateResponse Authenticate(string mail, string password, string ipAddress)
        {
            return this.AuthenticateAsync(mail, password, ipAddress).Result;
        }

        /// <summary>
        /// Authenticates user by login credentials.
        /// Returns true if credentials is correct,
        /// returns false if creadentials are not correct.
        /// </summary>
        /// <param name="mail">Mail to validate.</param>
        /// <param name="password">Password to validate.</param>
        /// <returns>Bool based on validation success.</returns>
        public async Task<AuthenticateResponse> AuthenticateAsync(string mail, string password, string ipAddress)
        {
            bool userAuthenticateSuccess = false;
            AuthenticateResponse authenticateResponse = null;

            // Get auth user data from existing entry.
            AuthUsersView authUser = await this.GetAuthUserByMailAsync(mail);

            if (authUser != null)
            {
                // Validate login
                HashingService hashingService = HashingFactory.GetHashingService();
                userAuthenticateSuccess = hashingService.VerifyPassword(password, authUser.Password, authUser.Salt);
            }


            if (userAuthenticateSuccess)
            {
                // Login credentials was correct.

                // Get user 
                IUser selectedUser = this.GetUserByLoginName(mail);

                var jwtToken = TokenFactory.GetJwtTokenGenerator().GenerateJwtToken(selectedUser);
                var refreshToken = TokenFactory.GetRefreshTokenGenerator().CreateRefreshToken(selectedUser, ipAddress);


                // Create AuthenticateResponse object
                authenticateResponse = new AuthenticateResponse(selectedUser, jwtToken, refreshToken.Token);
            }
            else
            {
                // Login credentials was not correct.
                authenticateResponse = null;
            }

            return authenticateResponse;
        }

        /// <summary>
        /// Used to refresh login tokens.
        /// </summary>
        /// <param name="token">Current token.</param>
        /// <param name="ipAddress">Ip address making the request.</param>
        /// <returns>AuthenticateResponse containing refreshed token.</returns>
        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            return this.RefreshTokenAsync(token, ipAddress).Result;
        }

        /// <summary>
        /// Used to refresh login tokens.
        /// </summary>
        /// <param name="token">Current token.</param>
        /// <param name="ipAddress">Ip address making the request.</param>
        /// <returns>AuthenticateResponse containing refreshed token.</returns>
        public async Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress)
        {
            // Get user 
            IUser selectedUser = await this.GetUserByTokenAsync(token);

            // If selecteduser could not be found by token, the token is not valid.
            // Therefore throw an exception
            if (selectedUser == null) throw new ArgumentException("Token does not exist.", nameof(token));

            var jwtToken = TokenFactory.GetJwtTokenGenerator().GenerateJwtToken(selectedUser);
            var refreshToken = TokenFactory.GetRefreshTokenGenerator().ReplaceRefreshToken(selectedUser, ipAddress, token);

            // Create AuthenticateResponse object
            AuthenticateResponse authenticateResponse = new AuthenticateResponse(selectedUser, jwtToken, refreshToken.Token);
            return authenticateResponse;
        }

        /// <summary>
        /// Creates new user in the database, based on provided IUser object.
        /// </summary>
        /// <param name="userToCreate">IUser object to create user entity from</param>
        /// <returns>Created user entity as IUser object</returns>
        public IUser CreateUser(CreateUserRequest userToCreate)
        {
            return this.CreateUserAsync(userToCreate).Result;
        }

        /// <summary>
        /// Creates new user in the database, based on provided IUser object.
        /// </summary>
        /// <param name="userToCreate">IUser object to create user entity from</param>
        /// <returns>Created user entity as IUser object</returns>
        public async Task<IUser> CreateUserAsync(CreateUserRequest userToCreate)
        {
            IUser user = null;

            bool userIsUnique = await this.UserIsUniqueAsync(userToCreate);

            if (!userIsUnique)
                throw new DuplicateNameException("A user with these credentials already exist.");

            // Hash password via hashing service
            HashingService hashingService = HashingFactory.GetHashingService();
            IHashedUser hashedUser = hashingService.CreateHashedUser(userToCreate.Mail, userToCreate.Password);
            int identity = 0;

            using (var conn = UserServiceFactory.GetSqlConnectionCreateUser())
            {
                await conn.OpenAsync();

                // Execute stored procedure to create new user with hashed password.
                var procedure = "[SPCreateNewUser]";
                var values = new
                {
                    @Username = userToCreate.Username,
                    @Email = userToCreate.Mail,
                    @Password = hashedUser.Password,
                    @Salt = hashedUser.Salt
                };
                identity = await conn.ExecuteScalarAsync<int>(procedure, values, commandType: CommandType.StoredProcedure);
            }

            if (identity != 0)
            {
                using (var conn = UserServiceFactory.GetSqlConnectionBasicReader())
                {
                    await conn.OpenAsync();

                    // Get newly created user.
                    user = await conn.GetAsync<User>(identity);
                }
            }

            if (user == null)
                throw new Exception("An unexpected error occured. The user could not be created successfully.");

            return user;
        }

        /// <summary>
        /// Returns user based on id given in argument.
        /// </summary>
        /// <param name="id">Id of requested user entity</param>
        /// <returns>User with requested id, null if no such user exists.</returns>
        public IUser GetUserById(int id)
        {
            return this.GetUserByIdAsync(id).Result;
        }

        /// <summary>
        /// Returns user based on id given in argument.
        /// </summary>
        /// <param name="id">Id of requested user entity</param>
        /// <returns>User with requested id, null if no such user exists.</returns>
        public async Task<IUser> GetUserByIdAsync(int id)
        {
            IUser user = null;

            using (var conn = UserServiceFactory.GetSqlConnectionBasicReader())
            {
                await conn.OpenAsync();
                user = await conn.GetAsync<User>(id);
            }

            return user;
        }

        /// <summary>
        /// Returns user based on login name given in argument.
        /// </summary>
        /// <param name="loginName">login name of requested user entity</param>
        /// <returns>User with requested login name, null if no such user exists.</returns>
        public IUser GetUserByLoginName(string loginName)
        {
            IUser user = null;
            string loginNameEncrypted = loginName;

            using (var conn = UserServiceFactory.GetSqlConnectionComplexSelect())
            {
                conn.Open();
                user = conn.QuerySingleOrDefault<User>("[SPGetUserByLoginName]", new { @LoginName = loginNameEncrypted }, commandType: CommandType.StoredProcedure);
            }

            return user;
        }

        /// <summary>
        /// Returns user based on login name given in argument.
        /// </summary>
        /// <param name="loginName">login name of requested user entity</param>
        /// <returns>User with requested login name, null if no such user exists.</returns>
        public async Task<IUser> GetUserByLoginNameAsync(string loginName)
        {
            IUser user = null;
            string loginNameEncrypted = loginName;

            using (var conn = UserServiceFactory.GetSqlConnectionComplexSelect())
            {
                await conn .OpenAsync();
                user = await conn.QuerySingleOrDefaultAsync<User>("[SPGetUserByLoginName]", new
                {
                    @LoginName = loginNameEncrypted
                }, commandType: CommandType.StoredProcedure);
            }

            return user;
        }

        /// <summary>
        /// Returns IUser based on provided token
        /// </summary>
        /// <param name="token">Token of requested user</param>
        /// <returns>IUser object matching the providede token.</returns>
        public IUser GetUserByToken(string token)
        {
            return this.GetUserByTokenAsync(token).Result;
        }

        /// <summary>
        /// Returns IUser based on provided token
        /// </summary>
        /// <param name="token">Token of requested user</param>
        /// <returns>IUser object matching the providede token.</returns>
        public async Task<IUser> GetUserByTokenAsync(string token)
        {
            IUser user;

            using (var conn = UserServiceFactory.GetSqlConnectionComplexSelect())
            {
                await conn.OpenAsync();
                var procedure = "[SPGetUserByToken]";
                var values = new { @Token = token };
                user = await conn.QuerySingleOrDefaultAsync<User>(procedure, values, commandType: CommandType.StoredProcedure);
                conn.Close();
            }

            return user;
        }

        /// <summary>
        /// Logs out user by destoying token, and revoking token in DB.
        /// </summary>
        /// <param name="token">Current token</param>
        /// <param name="ipAddress">IP Address origin</param>
        /// <returns>Bool true if user was successfully logged out, false if not.</returns>
        public bool Logout(string token, string ipAddress)
        {
            return this.LogoutAsync(token,ipAddress).Result;
        }

        /// <summary>
        /// Logs out user by destoying token, and revoking token in DB.
        /// </summary>
        /// <param name="token">Current token</param>
        /// <param name="ipAddress">IP Address origin</param>
        /// <returns>Bool true if user was successfully logged out, false if not.</returns>
        public async Task<bool> LogoutAsync(string token, string ipAddress)
        {
            bool revokeSuccess = false;
            IUser user = await this.GetUserByTokenAsync(token);

            // Check if the token exists.
            if (user == null) throw new ArgumentException("No user could be found with this token", nameof(token));

            revokeSuccess = TokenFactory.GetRefreshTokenGenerator().RevokeRefreshToken(user, ipAddress, token);

            return revokeSuccess;
        }

        /// <summary>
        /// Checks if a user with thise unique paramerters already exists in the database.
        /// </summary>
        /// <param name="userToCheck">IUser object to check.</param>
        /// <returns>Bool true if the user is unique, false if the user exists.</returns>
        private bool UserIsUnique(IUser userToCheck)
        {
            return this.UserIsUniqueAsync(userToCheck).Result;
        }

        /// <summary>
        /// Checks if a user with thise unique paramerters already exists in the database.
        /// </summary>
        /// <param name="userToCheck">IUser object to check.</param>
        /// <returns>Bool true if the user is unique, false if the user exists.</returns>
        private async Task<bool> UserIsUniqueAsync(IUser userToCheck)
        {
            bool userIsUnique;
            IUser userCopy = new User()
            {
                Username = userToCheck.Username,
                Mail = userToCheck.Mail
            };
            userCopy = userCopy;

            using (var conn = UserServiceFactory.GetSqlConnectionComplexSelect())
            {
                await conn.OpenAsync();

                var procedure = "[SPUserEmailIsUnique]";
                var values = new { @Email = userCopy.Mail };
                var SPreturn = await conn.ExecuteScalarAsync(procedure, values, commandType: CommandType.StoredProcedure);

                conn.Close();

                if ((int)SPreturn == 1)
                    userIsUnique = false;
                else
                    userIsUnique = true;
            }

            return userIsUnique;
        }

        private AuthUsersView GetAuthUserByMail(string mail)
        {
            return this.GetAuthUserByMailAsync(mail).Result;
        }

        private async Task<AuthUsersView> GetAuthUserByMailAsync(string mail)
        {
            AuthUsersView authUser = null;

            using (var conn = UserServiceFactory.GetSqlConnectionComplexSelect())
            {
                await conn.OpenAsync();
                authUser = await conn.QuerySingleOrDefaultAsync<AuthUsersView>("SELECT * FROM AuthUsersView WHERE Mail = @Email", new { @Email = mail });
            }

            return authUser;
        }

    }
}

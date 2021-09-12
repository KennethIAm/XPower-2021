using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using H4_TrashPlusPlus.Entities;
using XPowerClassLibrary.Users.Entities;
using XPowerClassLibrary.Users.Models;
using XPowerClassLibrary.Users.Repository;
using XPowerClassLibrary.Validator;
using Microsoft.IdentityModel.Tokens;

namespace XPowerClassLibrary.Users
{
    /// <summary>
    /// Facade used while communicating with user lib
    /// </summary>
    public class UserService : IUserService
    {

        private IUserRepository userRepository;

        internal UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Creates a user with the repository
        /// </summary>
        /// <param name="mail">the users mail</param>
        /// <param name="password">the users password</param>
        /// <returns>the created user</returns>
        public IUser CreateUser(string mail, string username, string password)   
        {
            return this.CreateUserAsync(mail,username,password).Result;
        }

        /// <summary>
        /// Creates a user with the repository async.
        /// </summary>
        /// <param name="mail">the users mail</param>
        /// <param name="password">the users password</param>
        /// <returns>the created user</returns>
        public async Task<IUser> CreateUserAsync(string mail, string username, string password)
        {
            try
            {
                // Run validation
                DefaultValidators.ValidateMailException(mail);
                DefaultValidators.ValidatePasswordException(password);
                DefaultValidators.ValidateUsernameException(username);

                // Create user object
                IUser user;
                CreateUserRequest createUserRequest = new CreateUserRequest(mail, username, password);

                // Create use via repository.
                user = await this.userRepository.CreateUserAsync(createUserRequest);

                return user;
            }
            catch (DuplicateNameException e)
            {
                // An error occured while creating via Database.
                throw e;
            }
            catch (ArgumentNullException e)
            {
                // An error occured while created via Database.
                throw e;
            }
            catch (ArgumentException e)
            {
                // An error occured while created via Database.
                throw e;
            }
            catch (SqlException e)
            {
                // An error occured while creating via Database.
                throw e;
            }
            catch (Exception e)
            {
                // An unkown error occured.
                throw e;
            }
        }


        /// <summary>
        /// Authenticate user by login credentials.
        /// Returns AuthenticateResponse object if successfull.
        /// </summary>
        /// <param name="email">Email to be used for quthentication.</param>
        /// <param name="password">Password to be used for quthentication.</param>
        /// <returns>AuthenticateResponse Object, Null if IUser could not be authenticated.</returns>
        public AuthenticateResponse Authenticate(string mail, string password, string ipAddress)
        {
            return this.AuthenticateAsync(mail, password, ipAddress).Result;
        }

        /// <summary>
        /// Authenticate user by login credentials.
        /// Returns AuthenticateResponse object if successfull.
        /// </summary>
        /// <param name="email">Email to be used for quthentication.</param>
        /// <param name="password">Password to be used for quthentication.</param>
        /// <returns>AuthenticateResponse Object, Null if IUser could not be authenticated.</returns>
        public async Task<AuthenticateResponse> AuthenticateAsync(string mail, string password, string ipAddress)
        {
            // Run validation
            DefaultValidators.ValidateMailException(mail);
            DefaultValidators.ValidatePasswordException(password);

            AuthenticateResponse authenticateResponse = null;

            try
            {
                authenticateResponse = await this.userRepository.AuthenticateAsync(mail, password, ipAddress);
            }
            catch (SqlException e)
            {
                // An error occured while saving refresh token to db.
                throw e;
            }
            catch (Exception e)
            {
                // An unexpected error occured.
                throw e;
            }

            return authenticateResponse;
        }

        /// <summary>
        /// Return IUser object with requested ID.
        /// Returns null if no such id exists.
        /// </summary>
        /// <param name="id">Id of requested IUser object</param>
        /// <returns>IUser object matching the requested id, or null.</returns>
        public IUser GetUserById(int id)
        {
            return this.GetUserByIdAsync(id).Result;
        }

        /// <summary>
        /// Return IUser object with requested ID.
        /// Returns null if no such id exists.
        /// </summary>
        /// <param name="id">Id of requested IUser object</param>
        /// <returns>IUser object matching the requested id, or null.</returns>
        public async Task<IUser> GetUserByIdAsync(int id)
        {
            // Id validation
            if (id < 1) throw new ArgumentException("Id must be above 0", nameof(id));

            // Get requested user from repository.
            IUser requestedUser = await this.userRepository.GetUserByIdAsync(id);

            return requestedUser;
        }

        /// <summary>
        /// Return IUser object with requested email.
        /// Returns null if no such email exists.
        /// </summary>
        /// <param name="loginName">email of requested IUser object</param>
        /// <returns>IUser object matching the requested email, or null.</returns>
        public IUser GetUserByLoginName(string loginName)
        {
            return this.GetUserByLoginNameAsync(loginName).Result;
        }

        /// <summary>
        /// Return IUser object with requested email.
        /// Returns null if no such email exists.
        /// </summary>
        /// <param name="loginName">email of requested IUser object</param>
        /// <returns>IUser object matching the requested email, or null.</returns>
        public async Task<IUser> GetUserByLoginNameAsync(string loginName)
        {
            // Login name validation
            DefaultValidators.ValidateMailException(loginName);

            // Get requested user from repository.
            IUser requestedUser = await this.userRepository.GetUserByLoginNameAsync(loginName);

            return requestedUser;
        }

        /// <summary>
        /// Return IUser object with requested token.
        /// Returns null if no such token exists.
        /// </summary>
        /// <param name="token">Token of requested user</param>
        /// <returns>IUser object matching the requested token, or null.</returns>
        public IUser GetUserByToken(string token)
        {
            return this.GetUserByTokenAsync(token).Result;
        }

        /// <summary>
        /// Return IUser object with requested token.
        /// Returns null if no such token exists.
        /// </summary>
        /// <param name="token">Token of requested user</param>
        /// <returns>IUser object matching the requested token, or null.</returns>
        public async Task<IUser> GetUserByTokenAsync(string token)
        {
            // Validate argument input
            DefaultValidators.ValidateRefreshTokenException(token);

            // Get requested user from repository.
            IUser requestedUser = await this.userRepository.GetUserByTokenAsync(token);

            return requestedUser;
        }

        /// <summary>
        /// Refreshes JWT Token based on current token and IP address.
        /// </summary>
        /// <param name="token">Current token to refresh.</param>
        /// <param name="ipAddress">IP Address origin</param>
        /// <returns>AuthenticateResponse</returns>
        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            return this.RefreshTokenAsync(token, ipAddress).Result;
        }


        /// <summary>
        /// Refreshes JWT Token based on current token and IP address.
        /// </summary>
        /// <param name="token">Current token to refresh.</param>
        /// <param name="ipAddress">IP Address origin</param>
        /// <returns>AuthenticateResponse</returns>
        public async Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress)
        {
            // Validate argument input
            if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress), "IP Address must not be null.");
            if (string.IsNullOrEmpty(ipAddress) || string.IsNullOrWhiteSpace(ipAddress)) throw new ArgumentException("IP Address must contain a value.", nameof(ipAddress));
            DefaultValidators.ValidateRefreshTokenException(token);

            AuthenticateResponse authenticateResponse = null;

            try
            {
                // Refrsh token via repository.
                authenticateResponse = await this.userRepository.RefreshTokenAsync(token, ipAddress);
            }
            catch (ArgumentException e)
            {
                // Token does not exist in DB
                throw e;
            }
            catch (Exception e)
            {
                // An unexpected error occured.
                throw e;
            }

            return authenticateResponse;
        }

        /// <summary>
        /// Logout user by destoying token.
        /// </summary>
        /// <param name="token">Current token</param>
        /// <param name="ipAddress">IP Address origin</param>
        /// <returns>Bool true if user was successfully logged out, false if not.</returns>
        public bool Logout(string token, string ipAddress)
        {
            return this.LogoutAsync(token, ipAddress).Result;
        }

        /// <summary>
        /// Logout user by destoying token.
        /// </summary>
        /// <param name="token">Current token</param>
        /// <param name="ipAddress">IP Address origin</param>
        /// <returns>Bool true if user was successfully logged out, false if not.</returns>
        public async Task<bool> LogoutAsync(string token, string ipAddress)
        {
            // Validate argument input
            if (ipAddress == null) throw new ArgumentException("IP Address must contain a value.", nameof(ipAddress));
            if (string.IsNullOrEmpty(ipAddress) || string.IsNullOrWhiteSpace(ipAddress)) throw new ArgumentException("IP Address must contain a value.", nameof(ipAddress));
            DefaultValidators.ValidateRefreshTokenException(token);

            bool loggedOutSuccessfully = false;

            try
            {
                loggedOutSuccessfully = await this.userRepository.LogoutAsync(token, ipAddress);
            }
            catch (Exception e)
            {
                // An unexpected error occured.
                throw e;
            }

            return loggedOutSuccessfully;
        }
    }
}

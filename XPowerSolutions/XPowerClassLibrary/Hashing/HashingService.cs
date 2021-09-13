using XPowerClassLibrary.Hashing;
using XPowerClassLibrary.Hashing.Handlers.HashingMethods;
using XPowerClassLibrary.Hashing.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Hashing
{
    /// <summary>
    /// Hashing Service facade.
    /// Used for hashing passwords, and other secure strings.
    /// </summary>
    public class HashingService
    {
        private HashingSettings _settings;
        private IHashingMethod _hashingMethod;

        /// <summary>
        /// Used to setup login functionality.
        /// Takes LoginSettings as argument.
        /// </summary>
        /// <param name="settings">Settings used to setup login.</param>
        public HashingService(HashingSettings settings)
        {
            this._settings = settings;

            IHashingMethod method;
            switch (settings.HashingMethod)
            {
                case HashingMethodType.SHA256:
                    method = new HashingSHA256(settings.NumberOfIterations);
                    break;
                default: throw new ArgumentException("Not a viable hashing method.", "hashingMethod");
            }

            this._hashingMethod = method;
        }

        /// <summary>
        /// Returns IHashedUser object, with hashed passowrd and salt.
        /// Takes Salt lenght as int - Default 32.
        /// </summary>
        /// <param name="username">Username string</param>
        /// <param name="password">Password string</param>
        /// <param name="saltSize">Salt lenght as int - default 32</param>
        /// <returns></returns>
        public IHashedUser CreateHashedUser(string username, string password)
        {
            // Check if user input is valid.
            if (username == string.Empty || username == "")
                throw new ArgumentNullException("Username");

            if (password == string.Empty || password == "")
                throw new ArgumentNullException("Password");

            // Generate secure salt.
            ISaltGenerator saltGenerator = SaltGeneratorFactory.GetSaltGenerator();
            byte[] salt = saltGenerator.GenerateSalt(this._settings.SaltSize);

            // Create return object.
            IHashedUser hashedUser = new HashedUser();
            hashedUser.Username = username; // Set username from user input.
            hashedUser.Password = password;
            hashedUser.Salt = Convert.ToBase64String(salt); // Set SaltByteArray with value generated above.

            // Instanziate hashing class - Select hashing method.
            hashedUser = this._hashingMethod.HashUserObject(hashedUser); // Returns hashed user - inlcuding password.

            return hashedUser;
        }

        /// <summary>
        /// Checks if the password without salt, mathes DB password
        /// </summary>
        /// <param name="inputPassword"></param>
        /// <param name="passwordWithSalt"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public bool VerifyPassword(string inputPassword, string passwordWithSalt, string salt)
        {
            bool hashedSucceded = false;

            // Hash new password with stored password salt.
            string newPasswordHash = this._hashingMethod.GetHashedPasswordString(inputPassword, salt);

            // If new hash matches old hash - return true.
            if (newPasswordHash == passwordWithSalt)
                hashedSucceded = true;

            return hashedSucceded;

        }

    }
}

using XPowerClassLibrary.Hashing.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace XPowerClassLibrary.Hashing.Handlers.SaltGenerators
{
    /// <summary>
    /// Uses RNGCryptoServiceProvider to create random byte[].
    /// Used for generating secure Salt values.
    /// </summary>
    class SaltGeneratorRNGCryptoService : ISaltGenerator
    {
        /// <summary>
        /// Generates secure Salt value as byte[]
        /// Takes salt lenght as param - Default length 32.
        /// </summary>
        /// <param name="saltLength"></param>
        /// <returns></returns>
        public byte[] GenerateSalt(int saltLength = 32)
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[saltLength];
                randomNumberGenerator.GetBytes(randomNumber);

                return randomNumber;
            }
        }
    }
}

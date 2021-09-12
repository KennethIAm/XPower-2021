using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Hashing
{

    public enum HashingMethodType
    {
        SHA256
    }

    /// <summary>
    /// Settings class - Conntains settings used in AuthenticationService
    /// </summary>
    public class HashingSettings
    {

        public HashingMethodType HashingMethod { get; set; } 

        public int SaltSize { get; set; }

        public int NumberOfIterations { get; set; }

        /// <summary>
        /// Settings Constructor includes hashingmethod, salt size, and number of iterations.
        /// </summary>
        /// <param name="hashingMethod">Hashing method to be used (SHA256)</param>
        /// <param name="saltSize">Size of salt used in hashing - default 32</param>
        /// <param name="numberOfIterations">Number of iterations used while hashing - default 50000</param>
        public HashingSettings(HashingMethodType hashingMethod, int saltSize = 32, int numberOfIterations = 50000)
        {
            HashingMethod = hashingMethod;
            SaltSize = saltSize;
            NumberOfIterations = numberOfIterations;
        }
    }
}

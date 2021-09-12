
using XPowerClassLibrary.Hashing.Handlers.SaltGenerators;
using XPowerClassLibrary.Hashing.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Hashing
{
    /// <summary>
    /// Returns salt generator in use
    /// </summary>
    static class SaltGeneratorFactory
    {
        public static ISaltGenerator GetSaltGenerator() => new SaltGeneratorRNGCryptoService();
    }
}

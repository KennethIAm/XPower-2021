using XPowerClassLibrary.Users.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace XPowerClassLibrary.Hashing
{
    public static class HashingFactory
    {
        internal static HashingService GetHashingService()
        {
            HashingSettings settings = new HashingSettings(HashingMethodType.SHA256);
            HashingService hashingService = new HashingService(settings);

            return hashingService;
        }
    }
}

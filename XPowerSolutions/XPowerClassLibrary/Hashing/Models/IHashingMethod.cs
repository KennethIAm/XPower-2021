using XPowerClassLibrary.Hashing;
using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Hashing.Models
{
    interface IHashingMethod
    {
        byte[] HashPasswordWithSalt(byte[] toBeHashed, byte[] salt);
        string GetHashedPasswordString(string inputPassword, string salt64string);
        IHashedUser HashUserObject(IHashedUser userToHash);
    }
}

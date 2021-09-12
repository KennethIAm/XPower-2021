using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Hashing
{
    public interface IHashedUser
    {
        string Username { get; set; }
        string Password { get; set; }
        string Salt { get; set; }
    }
}

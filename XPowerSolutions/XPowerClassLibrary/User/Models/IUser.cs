using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Users.Models
{
    public interface IUser
    {
        int Id { get; }
        string Mail { get; }
        string Username { get; }
    }
}

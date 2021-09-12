using XPowerClassLibrary.Users.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Users.Tokens.JwtTokens
{
    interface IJwtTokenGenerator
    {
        string GenerateJwtToken(IUser user);
    }
}

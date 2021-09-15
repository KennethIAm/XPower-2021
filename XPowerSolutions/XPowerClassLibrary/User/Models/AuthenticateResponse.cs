using XPowerAPI.Entities;
using XPowerClassLibrary.Users.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XPowerClassLibrary.Users.Models
{
    public class AuthenticateResponse
    {
        public User UserObject { get; set; }
        public string JwtToken { get; set; }

        //[JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse() { }

        public AuthenticateResponse(IUser user, string jwtToken, string refreshToken)
        {
            this.UserObject = (User)user;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}

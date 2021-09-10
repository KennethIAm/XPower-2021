using XPowerAPI.Entities;
using XPowerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XPowerAPI.Repositories
{
    interface IUserRepository
    {
        AuthenticateResponse Authenticate(string email, string password);
        IUser CreateUser(CreateUserRequest request);
        AuthenticateResponse RefreshToken(string email, string token);
        bool RevokeToken(string email, string token);
    }
}

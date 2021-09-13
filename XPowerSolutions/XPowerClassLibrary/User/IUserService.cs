using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XPowerClassLibrary.Users.Models;

namespace XPowerClassLibrary.Users
{
    public interface IUserService
    {
        IUser CreateUser(string mail, string username, string password);
        Task<IUser> CreateUserAsync(string mail, string username, string password);

        AuthenticateResponse Authenticate(string mail, string password, string ipAddress);
        Task<AuthenticateResponse> AuthenticateAsync(string mail, string password, string ipAddress);

        AuthenticateResponse RefreshToken(string token, string ipAddress);
        Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress);

        bool Logout(string token, string ipAddress);
        Task<bool> LogoutAsync(string token, string ipAddress);

        IUser GetUserById(int id);
        Task<IUser> GetUserByIdAsync(int id);

        IUser GetUserByToken(string token);
        Task<IUser> GetUserByTokenAsync(string token);

        IUser GetUserByLoginName(string loginName);
        Task<IUser> GetUserByLoginNameAsync(string loginName);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XPowerClassLibrary.Users.Models;

namespace XPowerClassLibrary.Users.Repository
{
    public interface IUserRepository
    {
        IUser CreateUser(CreateUserRequest userToCreate);
        Task<IUser> CreateUserAsync(CreateUserRequest userToCreate);

        IUser GetUserById(int id);
        Task<IUser> GetUserByIdAsync(int id);

        IUser GetUserByLoginName(string loginName);
        Task<IUser> GetUserByLoginNameAsync(string loginName);

        AuthenticateResponse Authenticate(string mail, string password, string ipAddress);
        Task<AuthenticateResponse> AuthenticateAsync(string mail, string password, string ipAddress);

        AuthenticateResponse RefreshToken(string token, string ipAddress);
        Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress);

        IUser GetUserByToken(string token);
        Task<IUser> GetUserByTokenAsync(string token);

        bool Logout(string token, string ipAddress);
        Task<bool> LogoutAsync(string token, string ipAddress);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XPowerClassLibrary.Users.Models;

namespace BlazorServerWebsite.Data.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> LogInAsync(AuthenticateRequest request);
    }
}

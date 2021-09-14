using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XPowerAPI.Models;
using Microsoft.AspNetCore.Http;
using System;
using XPowerClassLibrary.Users;
using XPowerAPI.Entities;
using System.Data;
using XPowerClassLibrary.Users.Models;
using XPowerClassLibrary.Users.Entities;
using Microsoft.Net.Http.Headers;
using XPowerClassLibrary;
using System.Threading.Tasks;

namespace XPowerAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        // helper methods
        protected string GetCurrentUserToken()
        {
            var accessToken = Request.Cookies["refreshToken"];
            return accessToken;
        }

        /// <summary>
        /// Returns the current user, if one is logged in
        /// </summary>
        /// <param name="userService">IUserService to use</param>
        /// <returns>Current logged in user, or null</returns>
        protected async Task<IUser> GetCurrentUser(IUserService userService)
        {
            IUser currentUser = null;
            try
            {
                currentUser = await userService.GetUserByTokenAsync(this.GetCurrentUserToken());
            }
            catch (Exception)
            {
                currentUser = null;
            }
            return currentUser;
        }

    }
}

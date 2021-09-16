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
using System.Data.SqlClient;
using System.Security.Cryptography;
using XPowerClassLibrary.Validator;
using System.Collections.Generic;

namespace XPowerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseController
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest model)
        {
            User response;

            try
            {
                IUser createdUser = await _userService.CreateUserAsync(model.Mail, model.Username, model.Password);
                response = (User)createdUser;

                if (response == null) throw new Exception("An unexpected error occured, user could not be created.");

                return Ok(response);
            }
            catch (DuplicateNameException e)
            {
                // An error occured while creating via Database.
                return BadRequest((new { message = "A user with these credentials already exists." }));
            }
            catch (ArgumentNullException e)
            {
                // An error occured while created via Database.
                return BadRequest((new { message = "Invalid input. User was not created" }));
            }
            catch (ArgumentException e)
            {
                // An error occured while created via Database.
                return BadRequest((new { message = "Invalid input. User was not created" }));
            }
            catch (SqlException e)
            {
                // An error occured while creating via Database.
                return BadRequest((new { message = "Invalid input. User was not created" }));
            }
            catch (Exception e)
            {
                // An unkown error occured.
                return BadRequest((new { message = "Unkown error. User was not created" }));
            }
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
        {
            AuthenticateResponse response = null;

            try
            {
                response = await _userService.AuthenticateAsync(model.Username, model.Password, GetIpAddress());

                if (response == null)
                    return Unauthorized(new { message = "Username or password is incorrect" });

                SetTokenCookie(response.RefreshToken);

                return Ok(response);
            }
            catch (SqlException e)
            {
                // An error occured while creating via Database.
                return BadRequest((new { message = "Invalid input. User was not validated." }));
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "An unexpected error occured. User could not be validated." });
            }

        }

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "Invalid token" });
            
            try
            {
                var response = await _userService.RefreshTokenAsync(refreshToken, GetIpAddress());

                if (response == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                SetTokenCookie(response.RefreshToken);

                return Ok(response);

            }
            catch (SqlException e)
            {
                // An error occured while creating via Database.
                return BadRequest((new { message = "Invalid input. The connection is offline." }));
            }
            catch(ArgumentException e)
            {
                return BadRequest(new { message = "The provided token was not active. Token could not be refreshed." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "An unexpected error occured. Token could not be refreshed." });
            }



        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(string token)
        {
            // accept token from request body or cookie
            var tokenValue = token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(tokenValue))
                return BadRequest(new { message = "Token is required" });

            bool logoutSuccess = false;

            try
            {
                // Logout via user service.
                logoutSuccess = await _userService.LogoutAsync(tokenValue, GetIpAddress());
            }
            catch (ArgumentException e)
            {
                // Token could not be found.
                return NotFound(new { message = "Token not found" });
            }
            catch (Exception e)
            {
                // An unexpected error occured.
                return BadRequest(new { message = "An unexpected error occured." });
            }

            return Ok(new { message = "Token revoked" });

        }

        [AllowAnonymous]
        [HttpGet("TestLogin")]
        public async Task<IActionResult> TestLogin()
        {
            try
            {
                if (await this.IsUserLoggedIn(this._userService))
                {
                    return Ok(new { message = "User is logged in!" });
                }
                else
                {
                    return Unauthorized(new { message = "User is not logged in." });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "An unexpected error occured. User is not logged in." });
            }
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

    }
}

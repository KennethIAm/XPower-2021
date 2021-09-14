using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using XPowerClassLibrary.Users.Models;

namespace BlazorServerWebsite.Data.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AuthenticateResponse> LogInAsync(AuthenticateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerWebsite.Data.Settings
{
    public class ApiSettings : ISettings
    {
        public string JwtKey { get; set; }
        public string RefreshTokenKey { get; set; }
        public string CookieName { get; set; }
        public EndpointSettings Endpoints { get; set; }
    }
}

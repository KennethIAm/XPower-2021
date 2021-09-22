using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerWebsite.Data.Settings
{
    public class EndpointSettings
    {
        public string BaseEndpoint { get; set; }
        public string AuthenticateEndpoint { get; set; }
        public string CreateUserEndpoint { get; set; }
        public string RefreshTokenEndpoint { get; set; }
        public string LogOutEndpoint { get; set; }
        public string AssignDeviceEndpoint { get; set; }
        public string GetDeviceByIdEndpoint { get; set; }
        public string AllUserDevicesEndpoint { get; set; }
        public string ChangeDeviceStatusEndpoint { get; set; }
    }
}

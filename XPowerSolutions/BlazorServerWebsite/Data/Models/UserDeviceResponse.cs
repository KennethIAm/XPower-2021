using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorServerWebsite.Data.Models
{
    public class UserDeviceResponse
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Username { get; set; }
        public List<DeviceResponse> OwnedDevices { get; set; }
    }

    public class DeviceResponse
    {
        public int Id { get; set; }
        public string UniqueDeviceIdentifier { get; set; }
        public string IPAddress { get; set; }
        public int FunctionalStatus { get; set; }
        public int ConnectionState { get; set; }
        public string Name { get; set; }
        public DeviceTypeResponse DeviceType { get; set; }
    }

    public class DeviceTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

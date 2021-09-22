using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Device.Models
{
    public class DeviceCommandRequest
    {
        public string DeviceAction { get; set; }
        public string DeviceIPAddress { get; set; }
    }
}
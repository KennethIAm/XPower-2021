using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Device.Models
{
    public class AssignDeviceToUserRequest
    {
        public int UserId { get; set; }
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public int DeviceTypeId { get; set; }
    }
}

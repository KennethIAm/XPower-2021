using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Device.Models
{
    public class DeviceOnlineRequest
    {
        public int DeviceTypeId { get; set; }
        public string UniqueDeviceIdentifier { get; set; }
        public string IPAddress { get; set; }

        public DeviceOnlineRequest() { }

        public DeviceOnlineRequest(int deviceTypeId, string uniqueDeviceIdentifier, string iPAddress)
        {
            DeviceTypeId = deviceTypeId;
            UniqueDeviceIdentifier = uniqueDeviceIdentifier;
            IPAddress = iPAddress;
        }
    }
}

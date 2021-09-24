using System;
using System.Collections.Generic;
using System.Text;
using XPowerClassLibrary.Device.Enums;

namespace XPowerClassLibrary.Device.Models
{
    public class UpdateDeviceRequest
    {
        public int DeviceId { get; set; }
        public int DeviceTypeId { get; set; }
        public DeviceFunctionalStatus DeviceFunctionalStatus { get; set; }
        public DeviceConnectionState DeviceConnectionState { get; set; }
        public string UniqueDeviceIdentifier { get; set; }
        public string DeviceName { get; set; }
        public string DeviceIpAddress { get; set; }

        public UpdateDeviceRequest() { }

        public UpdateDeviceRequest(int deviceId, int deviceTypeId, DeviceFunctionalStatus deviceFunctionalStatus, DeviceConnectionState deviceConnectionState, string uniqueDeviceIdentifier, string deviceName, string deviceIpAddress)
        {
            DeviceId = deviceId;
            DeviceTypeId = deviceTypeId;
            DeviceFunctionalStatus = deviceFunctionalStatus;
            DeviceConnectionState = deviceConnectionState;
            UniqueDeviceIdentifier = uniqueDeviceIdentifier;
            DeviceName = deviceName;
            DeviceIpAddress = deviceIpAddress;
        }
    }
}

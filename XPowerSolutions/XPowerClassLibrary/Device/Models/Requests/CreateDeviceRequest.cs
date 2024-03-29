﻿using XPowerClassLibrary.Device.Enums;

namespace XPowerClassLibrary.Device.Models
{
    public class CreateDeviceRequest
    {
        public int DeviceTypeId { get; set; }
        public DeviceFunctionalStatus DeviceFunctionalStatus { get; set; }
        public DeviceConnectionState DeviceConnectionState { get; set; }
        public string UniqueDeviceIdentifier { get; set; }
        public string DeviceName { get; set; }
        public string DeviceIpAddress { get; set; }

        public CreateDeviceRequest() { }

        public CreateDeviceRequest(int deviceTypeId, DeviceFunctionalStatus deviceFunctionalStatus, DeviceConnectionState deviceConnectionState, string uniqueDeviceIdentifier, string deviceName, string deviceIpAddress)
        {
            DeviceTypeId = deviceTypeId;
            DeviceFunctionalStatus = deviceFunctionalStatus;
            DeviceConnectionState = deviceConnectionState;
            UniqueDeviceIdentifier = uniqueDeviceIdentifier;
            DeviceName = deviceName;
            DeviceIpAddress = deviceIpAddress;
        }
    }
}

﻿using System;
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
        public string DeviceName { get; set; }
        public string DeviceIpAddress { get; set; }

        public UpdateDeviceRequest() { }

        public UpdateDeviceRequest(int deviceTypeId, DeviceFunctionalStatus deviceFunctionalStatus, DeviceConnectionState deviceConnectionState, string deviceName, string deviceIpAddress)
        {
            DeviceTypeId = deviceTypeId;
            DeviceFunctionalStatus = deviceFunctionalStatus;
            DeviceConnectionState = deviceConnectionState;
            DeviceName = deviceName;
            DeviceIpAddress = deviceIpAddress;
        }
    }
}

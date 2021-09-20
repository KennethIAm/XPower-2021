using System;
using System.Collections.Generic;
using System.Text;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;

namespace XPowerClassLibrary.Device.Entities
{
    public class DeviceInformationView : IDevice
    {
        private int DeviceId { get; set; }
        private int DeviceTypeId { get; set; }
        private string DeviceTypeName { get; set; }
        private int Status { get; set; }
        private int State { get; set; }
        private string DeviceName { get; set; }

        public int Id { get { return DeviceId; } }
        public string UniqueDeviceIdentifier { get; set; }
        public string IPAddress { get; set; }

        public DeviceType DeviceType { get { return new DeviceType { Id = DeviceTypeId, Name = DeviceTypeName }; } }

        public DeviceFunctionalStatus FunctionalStatus { get { return (DeviceFunctionalStatus)Status; } }

        public DeviceConnectionState ConnectionState { get { return (DeviceConnectionState)State; } }

        public string Name { get { return DeviceName; } }
    }
}

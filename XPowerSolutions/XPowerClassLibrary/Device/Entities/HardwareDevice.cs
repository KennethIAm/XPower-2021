using System.ComponentModel.DataAnnotations;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;

namespace XPowerClassLibrary.Device.Entities
{
    public class HardwareDevice : IDevice
    {
        [Key]
        public int Id { get; set; }
        public IDeviceType DeviceType { get; set; }
        public DeviceFunctionalStatus FunctionalStatus { get; set; }
        public DeviceConnectionState ConnectionState { get; set; }
        public string UniqueDeviceIdentifier { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }

        public HardwareDevice() { }

        public HardwareDevice(int id, IDeviceType deviceType, DeviceFunctionalStatus functionalStatus, DeviceConnectionState connectionState, string uniqueDeviceIdentifier, string name, string ipAddress)
        {
            Id = id;
            DeviceType = deviceType;
            FunctionalStatus = functionalStatus;
            ConnectionState = connectionState;
            UniqueDeviceIdentifier = uniqueDeviceIdentifier;
            Name = name;
            IpAddress = ipAddress;
        }
    }
}

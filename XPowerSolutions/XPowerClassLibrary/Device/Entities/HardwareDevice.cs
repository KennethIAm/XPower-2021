using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;

namespace XPowerClassLibrary.Device.Entities
{
    [Dapper.Contrib.Extensions.Table("Devices")]
    public class HardwareDevice : IDevice
    {
        [Key]
        [Column("DeviceID")]
        public int Id { get; set; }

        public DeviceType DeviceType { get; set; }

        [Column("Status")]
        public DeviceFunctionalStatus FunctionalStatus { get; set; }

        [Column("State")]
        public DeviceConnectionState ConnectionState { get; set; }

        [Column("UniqueDeviceIdentifier")]
        public string UniqueDeviceIdentifier { get; set; }

        [Column("DeviceName")]
        public string Name { get; set; }

        [Column("IPAddress")]
        public string IPAddress { get; set; }

        public HardwareDevice() { }

        public HardwareDevice(int id, DeviceType deviceType, DeviceFunctionalStatus functionalStatus, DeviceConnectionState connectionState, string uniqueDeviceIdentifier, string name, string ipAddress)
        {
            Id = id;
            DeviceType = deviceType;
            FunctionalStatus = functionalStatus;
            ConnectionState = connectionState;
            UniqueDeviceIdentifier = uniqueDeviceIdentifier;
            Name = name;
            IPAddress = ipAddress;
        }
    }
}

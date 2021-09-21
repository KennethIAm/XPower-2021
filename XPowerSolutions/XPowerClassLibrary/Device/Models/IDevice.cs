using XPowerClassLibrary.Device.Entities;
using XPowerClassLibrary.Device.Enums;

namespace XPowerClassLibrary.Device.Models
{
    public interface IDevice
    {
        int Id { get; }
        DeviceType DeviceType { get; }
        DeviceFunctionalStatus FunctionalStatus { get; }
        DeviceConnectionState ConnectionState { get; }
        string UniqueDeviceIdentifier { get; }
        string Name { get; }
        string IPAddress { get; }
    }
}

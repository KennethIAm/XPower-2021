using XPowerClassLibrary.Device.Enums;

namespace XPowerClassLibrary.Device.Models
{
    public interface IDevice
    {
        int Id { get; }
        IDeviceType DeviceType { get; }
        DeviceFunctionalStatus FunctionalStatus { get; }
        DeviceConnectionState ConnectionState { get; }
        string Name { get; }
        string IpAddress { get; }
    }
}

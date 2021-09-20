using XPowerClassLibrary.Device.Models;

namespace XPowerClassLibrary.Device.Entities
{
    public class DeviceType : IDeviceType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DeviceType() { }

        public DeviceType(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}

using Dapper.Contrib.Extensions;

namespace XPowerClassLibrary.Device.Models
{
    public interface IDeviceType
    {
        [Key]
        int Id { get; }
        string Name { get; }
    }
}

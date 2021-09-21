using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using XPowerClassLibrary.Device.Models;

namespace XPowerClassLibrary.Device.Entities
{
    [Dapper.Contrib.Extensions.Table("DeviceTypes")]
    public class DeviceType : IDeviceType
    {
        [Column("DeviceTypeID")]
        public int Id { get; set; }
        [Column("DeviceTypeName")]
        public string Name { get; set; }

        public DeviceType() { }

        public DeviceType(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}

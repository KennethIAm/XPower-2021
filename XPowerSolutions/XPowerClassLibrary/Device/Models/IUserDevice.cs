using System.Collections.Generic;
using XPowerClassLibrary.Device.Entities;
using XPowerClassLibrary.Users.Models;

namespace XPowerClassLibrary.Device.Models
{
    public interface IUserDevice : IUser
    {
        List<DeviceInformationView> OwnedDevices { get; }
    }
}

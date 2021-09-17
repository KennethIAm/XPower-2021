using System.Collections.Generic;
using XPowerClassLibrary.Users.Models;

namespace XPowerClassLibrary.Device.Models
{
    public interface IUserDevice : IUser
    {
        IList<IDevice> OwnedDevices { get; }
    }
}

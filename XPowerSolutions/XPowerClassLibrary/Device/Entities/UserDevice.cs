using System.Collections.Generic;
using XPowerClassLibrary.Device.Models;

namespace XPowerClassLibrary.Device.Entities
{
    public class UserDevice : IUserDevice
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Username { get; set; }
        public IList<DeviceInformationView> OwnedDevices { get; set; }

        public UserDevice() { }

        public UserDevice(int id, string mail, string username, IList<DeviceInformationView> ownedDevices)
        {
            Id = id;
            Mail = mail;
            Username = username;
            OwnedDevices = ownedDevices;
        }
    }
}

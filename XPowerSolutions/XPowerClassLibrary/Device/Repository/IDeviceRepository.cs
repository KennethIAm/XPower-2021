using System.Collections.Generic;
using System.Threading.Tasks;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;
using XPowerClassLibrary.Device.Models.Requests;

namespace XPowerClassLibrary.Device.Repository
{
    public interface IDeviceRepository
    {
        Task<IDevice> CreateDeviceAsync(CreateDeviceRequest createRequest);
        Task<IDevice> UpdateDeviceAsync(UpdateDeviceRequest updateRequest);
        Task<IDevice> GetDeviceByIdAsync(int id);
        Task<bool> DeleteDeviceByIdAsync(int id);
        Task<IDevice> DeviceOnlineAsync(DeviceOnlineRequest onlineRequest);
        Task<IDevice> AssignDeviceToUserAsync(AssignDeviceToUserRequest assignDeviceRequest);
        Task<IEnumerable<IDevice>> GetUsersOwnedDevices(int userId);
    }
}

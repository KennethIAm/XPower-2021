using System.Collections.Generic;
using System.Threading.Tasks;
using XPowerClassLibrary.Device.Models;

namespace XPowerClassLibrary.Device.Repository
{
    public interface IDeviceRepository
    {
        Task<IDevice> CreateDeviceAsync(CreateDeviceRequest createRequest);
        Task<IDevice> UpdateDeviceAsync(UpdateDeviceRequest updateRequest);
        Task<IDevice> GetDeviceByIdAsync(int id);
        Task<bool> DeleteDeviceByIdAsync(int id);
        Task<IDevice> FindDeviceByUniqueIdentifier(string uniqueIdentifier);
        Task<IDevice> AssignDeviceToUserAsync(AssignDeviceToUserRequest assignDeviceRequest);
        Task<IEnumerable<IDevice>> GetUsersOwnedDevices(int userId);
    }
}

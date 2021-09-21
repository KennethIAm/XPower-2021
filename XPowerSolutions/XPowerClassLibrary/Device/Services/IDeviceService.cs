using System.Threading.Tasks;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;

namespace XPowerClassLibrary.Device.Services
{
    public interface IDeviceService
    {
        Task<IDevice> CreateDeviceAsync(CreateDeviceRequest request);
        Task<IDevice> UpdateDeviceAsync(UpdateDeviceRequest updateRequest);
        Task<IDevice> GetDeviceByIdAsync(int id);
        Task<bool> DeleteDeviceByIdAsync(int id);
        Task<IDevice> DeviceOnlineAsync(DeviceOnlineRequest onlineRequest);
        Task<IDevice> AssignDeviceToUserAsync(AssignDeviceToUserRequest assignDeviceRequest);
    }
}

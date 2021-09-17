using System.Threading.Tasks;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;

namespace XPowerClassLibrary.Device.Repository
{
    public interface IDeviceRepository
    {
        Task<IDevice> CreateDeviceAsync(CreateDeviceRequest createRequest);
        //Task<DeviceConnectionState> GetDeviceConnectionState(int id);
        //Task<DeviceFunctionalStatus> GetDeviceFunctionalStatus(int id);
        //Task<DeviceConnectionState> UpdateDeviceConnectionState(int id, DeviceConnectionState state);
        //Task<DeviceFunctionalStatus> UpdateDeviceStatus(int id, DeviceFunctionalStatus status);
        Task<IDevice> UpdateDeviceAsync(UpdateDeviceRequest updateRequest);
        Task<IDevice> GetDeviceByIdAsync(int id);
        Task<bool> DeleteDeviceByIdAsync(int id);
        Task<IDevice> DeviceOnlineAsync(DeviceOnlineRequest onlineRequest);
    }
}

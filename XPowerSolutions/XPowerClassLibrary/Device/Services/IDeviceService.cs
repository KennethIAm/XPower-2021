using System.Threading.Tasks;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;

namespace XPowerClassLibrary.Device.Services
{
    public interface IDeviceService
    {
        Task<IDevice> CreateDeviceAsync(CreateDeviceRequest request);
        //Task<DeviceConnectionState> GetDeviceConnectionState(int id);
        //Task<DeviceFunctionalStatus> UpdateDeviceStatus(int id, DeviceFunctionalStatus status);
        //Task<DeviceConnectionState> UpdateDeviceConnectionState(int id, DeviceConnectionState state);
        Task<IDevice> UpdateDevice(UpdateDeviceRequest updateRequest);
        Task<IDevice> GetDeviceById(int id);
        Task<bool> DeleteDeviceByIdAsync(int id);
    }
}

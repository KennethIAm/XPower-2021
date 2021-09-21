using System.Threading.Tasks;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;
using XPowerClassLibrary.Device.Repository;

namespace XPowerClassLibrary.Device.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _repository;

        public DeviceService(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IDevice> AssignDeviceToUserAsync(AssignDeviceToUserRequest assignDeviceRequest)
        {
            return await _repository.AssignDeviceToUserAsync(assignDeviceRequest);
        }

        public async Task<IDevice> CreateDeviceAsync(CreateDeviceRequest request)
        {
            return await _repository.CreateDeviceAsync(request);
        }

        public async Task<bool> DeleteDeviceByIdAsync(int id)
        {
            return await _repository.DeleteDeviceByIdAsync(id);
        }

        public async Task<IDevice> DeviceOnlineAsync(DeviceOnlineRequest onlineRequest)
        {
            return await _repository.DeviceOnlineAsync(onlineRequest);
        }

        public async Task<IDevice> GetDeviceByIdAsync(int id)
        {
            return await _repository.GetDeviceByIdAsync(id);
        }

        public async Task<IDevice> UpdateDeviceAsync(UpdateDeviceRequest updateRequest)
        {
            return await _repository.UpdateDeviceAsync(updateRequest);
        }
    }
}

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

        public async Task<IDevice> CreateDeviceAsync(CreateDeviceRequest request)
        {
            return await _repository.CreateDeviceAsync(request);
        }

        public async Task<bool> DeleteDeviceByIdAsync(int id)
        {
            return await _repository.DeleteDeviceByIdAsync(id);
        }

        public async Task<IDevice> GetDeviceById(int id)
        {
            return await _repository.GetDeviceById(id);
        }

        public async Task<IDevice> UpdateDevice(UpdateDeviceRequest updateRequest)
        {
            return await _repository.UpdateDevice(updateRequest);
        }

        //public async Task<DeviceConnectionState> GetDeviceConnectionState(int id)
        //{
        //    return await _repository.GetDeviceConnectionState(id);
        //}

        //public async Task<DeviceConnectionState> UpdateDeviceConnectionState(int id, DeviceConnectionState state)
        //{
        //    return await _repository.UpdateDeviceConnectionState(id, state);
        //}

        //public async Task<DeviceFunctionalStatus> UpdateDeviceStatus(int id, DeviceFunctionalStatus status)
        //{
        //    return await _repository.UpdateDeviceStatus(id, status);
        //}
    }
}

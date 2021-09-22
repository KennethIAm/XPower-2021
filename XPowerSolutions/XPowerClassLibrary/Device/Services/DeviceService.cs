using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XPowerClassLibrary.Device.Entities;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;
using XPowerClassLibrary.Device.Models.Requests;
using XPowerClassLibrary.Device.Repository;
using XPowerClassLibrary.Users;

namespace XPowerClassLibrary.Device.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _repository;
        private readonly IUserService _userService;

        public DeviceService(IDeviceRepository repository, IUserService userService)
        {
            _repository = repository;
            _userService = userService;
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
            IDevice device = await _repository.FindDeviceByUniqueIdentifier(onlineRequest.UniqueDeviceIdentifier);

            if (device is null)
            {
                return await _repository.CreateDeviceAsync(new CreateDeviceRequest
                {
                    DeviceName = "",
                    DeviceIpAddress = onlineRequest.IPAddress,
                    DeviceFunctionalStatus = DeviceFunctionalStatus.Disabled,
                    DeviceConnectionState = DeviceConnectionState.Connected,
                    UniqueDeviceIdentifier = onlineRequest.UniqueDeviceIdentifier,
                    DeviceTypeId = onlineRequest.DeviceTypeId
                });
            }

            return await _repository.UpdateDeviceAsync(new UpdateDeviceRequest
            {
                DeviceId = device.Id,
                DeviceTypeId = device.DeviceType.Id,
                DeviceName = device.Name,
                DeviceFunctionalStatus = device.FunctionalStatus,
                DeviceConnectionState = device.ConnectionState,
                UniqueDeviceIdentifier = onlineRequest.UniqueDeviceIdentifier,
                DeviceIpAddress = onlineRequest.IPAddress
            });
        }

        public async Task<IDevice> GetDeviceByIdAsync(int id)
        {
            return await _repository.GetDeviceByIdAsync(id);
        }

        public async Task<IUserDevice> GetUsersOwnedDevices(UserDevicesRequest devicesRequest)
        {
            var user = await _userService.GetUserByTokenAsync(devicesRequest.RefreshToken);

            if (user is null)
                throw new NullReferenceException("Returned user was null or not found.");

            var devices = await _repository.GetUsersOwnedDevices(user.Id);

            IUserDevice userDevices = new UserDevice()
            {
                Id = user.Id,
                Mail = user.Mail,
                Username = user.Username,
                OwnedDevices = devices as List<DeviceInformationView>
            };

            return await Task.FromResult(userDevices);
        }

        public async Task<IDevice> UpdateDeviceAsync(UpdateDeviceRequest updateRequest)
        {
            return await _repository.UpdateDeviceAsync(updateRequest);
        }
    }
}

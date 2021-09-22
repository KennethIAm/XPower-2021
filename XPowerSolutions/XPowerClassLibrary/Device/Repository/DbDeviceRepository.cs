using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Data;
using System.Threading.Tasks;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;
using XPowerClassLibrary.Device.Entities;

namespace XPowerClassLibrary.Device.Repository
{
    public class DbDeviceRepository : IDeviceRepository
    {
        public async Task<IDevice> AssignDeviceToUserAsync(AssignDeviceToUserRequest assignDeviceRequest)
        {
            var deviceIsNotAssignedToAnyUser = await DeviceIsNotAssignedToAny(assignDeviceRequest.UniqueDeviceIdentifier);

            if (deviceIsNotAssignedToAnyUser is false)
                throw new DuplicateNameException("Device is already assigned to a user.");

            IDevice device = null;
            int assignedDeviceId = 0;

            using (var conn = DeviceServiceFactory.GetSqlConnectionUpdateDevice())
            {
                await conn.OpenAsync();
                var proc = "[SPAssignDeviceToUser]";
                var values = new
                {
                    @UserId = assignDeviceRequest.UserTokenRequest,
                    @UniqueDeviceIdentifier = assignDeviceRequest.UniqueDeviceIdentifier,
                    @DeviceName = assignDeviceRequest.DeviceName,
                    @DeviceTypeId = assignDeviceRequest.DeviceTypeId
                };

                assignedDeviceId = await conn.ExecuteScalarAsync<int>(proc, values, commandType: CommandType.StoredProcedure);
            }

            if (assignedDeviceId > 0)
            {
                device = await GetDeviceByIdAsync(assignedDeviceId);

                return device;
            }

            return device;
        }

        public async Task<IDevice> CreateDeviceAsync(CreateDeviceRequest request)
        {
            IDevice device = null;

            int entityId = 0;

            using (var conn = DeviceServiceFactory.GetSqlConnectionCreateDevice())
            {
                await conn.OpenAsync();

                var procedure = "[SPCreateNewDevice]";
                var values = new
                {
                    @DeviceTypeId = request.DeviceTypeId,
                    @Status = request.DeviceFunctionalStatus,
                    @State = request.DeviceConnectionState,
                    @UniqueDeviceIdentifier = request.UniqueDeviceIdentifier,
                    @Name = request.DeviceName,
                    @IPAddress = request.DeviceIpAddress
                };

                entityId = await conn.ExecuteScalarAsync<int>(procedure, values, commandType: CommandType.StoredProcedure);
            }


            if (entityId != 0)
            {
                using (var conn = DeviceServiceFactory.GetSqlConnectionBasicReader())
                {
                    await conn.OpenAsync();

                    device = await conn.GetAsync<DeviceInformationView>(entityId);
                }
            }

            if (device is null)
                throw new NullReferenceException("An unexpected error occurred. The device creation could not be handled successfully.");

            return device;
        }

        public async Task<bool> DeleteDeviceByIdAsync(int id)
        {
            bool deviceDeletedSuccessfully = false;

            using (var conn = DeviceServiceFactory.GetSqlConnectionDeleteDevice())
            {
                await conn.OpenAsync();

                var procedure = "[SPDeleteDeviceById]";
                var values = new { @Id = id };

                deviceDeletedSuccessfully = await conn.ExecuteScalarAsync<bool>(procedure, values, commandType: CommandType.StoredProcedure);
            }

            return deviceDeletedSuccessfully;
        }

        public async Task<IDevice> DeviceOnlineAsync(DeviceOnlineRequest onlineRequest)
        {
            IDevice device = null;

            using (var conn = DeviceServiceFactory.GetSqlConnectionComplexSelect())
            {
                await conn.OpenAsync();

                var procedure = "[SPFindDeviceByUniqueDeviceIdentifier]";
                var values = new { @UniqueDeviceIdentifier = onlineRequest.UniqueDeviceIdentifier };

                device = await conn.QuerySingleOrDefaultAsync<DeviceInformationView>(procedure, values, commandType: CommandType.StoredProcedure);
            }

            // Create Device if still null.
            if (device is null)
            {
                return await CreateDeviceAsync(new CreateDeviceRequest()
                {                    
                    DeviceName = "",
                    DeviceIpAddress = onlineRequest.IPAddress,
                    DeviceFunctionalStatus = DeviceFunctionalStatus.Disabled,
                    DeviceConnectionState = DeviceConnectionState.Connected,
                    UniqueDeviceIdentifier = onlineRequest.UniqueDeviceIdentifier,
                    DeviceTypeId = onlineRequest.DeviceTypeId
                });
            }

            // Update device if found.
            return await UpdateDeviceAsync(new UpdateDeviceRequest()
            {
                DeviceId = device.Id,
                DeviceName = device.Name,
                DeviceIpAddress = onlineRequest.IPAddress,
                DeviceFunctionalStatus = device.FunctionalStatus,
                DeviceConnectionState = device.ConnectionState,
                UniqueDeviceIdentifier = onlineRequest.UniqueDeviceIdentifier,
                DeviceTypeId = device.DeviceType.Id
            });
        }

        public async Task<IDevice> GetDeviceByIdAsync(int id)
        {
            IDevice device = null;

            using (var conn = DeviceServiceFactory.GetSqlConnectionComplexSelect())
            {
                await conn.OpenAsync();

                var procedure = "[SPGetDeviceById]";
                var values = new { @Id = id };

                device = await conn.QuerySingleOrDefaultAsync<DeviceInformationView>(procedure, values, commandType: CommandType.StoredProcedure);
            }

            return device;
        }

        public async Task<IDevice> UpdateDeviceAsync(UpdateDeviceRequest updateRequest)
        {
            bool deviceIsUpdated = false;

            using (var conn = DeviceServiceFactory.GetSqlConnectionUpdateDevice())
            {
                await conn.OpenAsync();

                var procedure = "[SPUpdateDevice]";
                var values = new { 
                    @Id = updateRequest.DeviceId,
                    @DeviceTypeId = updateRequest.DeviceTypeId,
                    @Status = updateRequest.DeviceFunctionalStatus,
                    @State = updateRequest.DeviceConnectionState,
                    @UniqueDeviceIdentifier = updateRequest.UniqueDeviceIdentifier,
                    @Name = updateRequest.DeviceName,
                    @IPAddress = updateRequest.DeviceIpAddress
                };

                deviceIsUpdated = await conn.ExecuteScalarAsync<bool>(procedure, values, commandType: CommandType.StoredProcedure);

            }
            IDevice updatedDevice = deviceIsUpdated == true ? await GetDeviceByIdAsync(updateRequest.DeviceId) : null;

            return updatedDevice;
        }

        private async Task<bool> DeviceIsNotAssignedToAny(string uniqueDeviceIdentifier)
        {
            bool isDeviceAssigned = false;

            using (var conn = DeviceServiceFactory.GetSqlConnectionComplexSelect())
            {
                await conn.OpenAsync();

                var proc = "[SPDeviceIsNotAssignedToAny]";
                var values = new
                {
                    @UniqueDeviceIdentifier = uniqueDeviceIdentifier
                };

                isDeviceAssigned = await conn.ExecuteScalarAsync<bool>(proc, values, commandType: CommandType.StoredProcedure);
            }

            return isDeviceAssigned;
        }
    }
}

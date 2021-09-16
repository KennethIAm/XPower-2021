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
        public async Task<IDevice> CreateDeviceAsync(CreateDeviceRequest request)
        {
            IDevice device = null;

            // Check if IP is unique from DB.
            if (await DeviceIpAddressExists(request.DeviceIpAddress))
                throw new Exception("A device with this IP already exists.");

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
                    @Name = request.DeviceName,
                    @IpAddress = request.DeviceIpAddress
                };

                entityId = await conn.ExecuteScalarAsync<int>(procedure, values, commandType: CommandType.StoredProcedure);
            }


            if (entityId != 0)
            {
                using (var conn = DeviceServiceFactory.GetSqlConnectionBasicReader())
                {
                    await conn.OpenAsync();

                    device = await conn.GetAsync<HardwareDevice>(entityId);
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

        public async Task<IDevice> GetDeviceById(int id)
        {
            IDevice device = null;

            using (var conn = DeviceServiceFactory.GetSqlConnectionBasicReader())
            {
                await conn.OpenAsync();

                device = await conn.GetAsync<HardwareDevice>(id);
            }

            return device;
        }

        public async Task<DeviceConnectionState> GetDeviceConnectionState(int id)
        {
            IDevice device = null;

            using (var conn = DeviceServiceFactory.GetSqlConnectionBasicReader())
            {
                await conn.OpenAsync();
                device = await conn.GetAsync<HardwareDevice>(id);
            }

            return device.ConnectionState;
        }

        public async Task<IDevice> UpdateDevice(UpdateDeviceRequest updateRequest)
        {
            IDevice updatedDevice = null;

            using (var conn = DeviceServiceFactory.GetSqlConnectionBasicReader())
            {
                await conn.OpenAsync();

                var procedure = "[SPUpdateDevice]";
                var values = new { 
                    @Id = updateRequest.DeviceId,
                    @DeviceTypeId = updateRequest.DeviceTypeId,
                    @FunctionalStatus = updateRequest.DeviceFunctionalStatus,
                    @ConnectionState = updateRequest.DeviceConnectionState,
                    @Name = updateRequest.DeviceName,
                    @IpAddress = updateRequest.DeviceIpAddress
                };

                var updatedConnectionState = await conn.ExecuteScalarAsync<bool>(procedure, values, commandType: CommandType.StoredProcedure);

                updatedDevice = updatedConnectionState == true ? await conn.GetAsync<HardwareDevice>(updateRequest.DeviceId) : null;
            }

            return updatedDevice;
        }

        public async Task<DeviceConnectionState> UpdateDeviceConnectionState(int id, DeviceConnectionState state)
        {
            DeviceConnectionState deviceState;

            using (var conn = DeviceServiceFactory.GetSqlConnectionBasicReader())
            {
                await conn.OpenAsync();

                var procedure = "[SPUpdateDeviceConnectionState]";
                var values = new { @ConnectionState = state };

                var updatedConnectionState = await conn.ExecuteScalarAsync<bool>(procedure, values, commandType: CommandType.StoredProcedure);

                deviceState = updatedConnectionState == true ? state : DeviceConnectionState.Undefined;
            }

            return deviceState;
        }

        public async Task<DeviceFunctionalStatus> UpdateDeviceStatus(int id, DeviceFunctionalStatus status)
        {
            DeviceFunctionalStatus functionalStatus;

            using (var conn = DeviceServiceFactory.GetSqlConnectionBasicReader())
            {
                await conn.OpenAsync();

                var procedure = "[SPUpdateDeviceFunctionalStatus]";
                var values = new { @FunctionalStatus = status };

                var updatedConnectionState = await conn.ExecuteScalarAsync<bool>(procedure, values, commandType: CommandType.StoredProcedure);

                functionalStatus = updatedConnectionState == true ? status : DeviceFunctionalStatus.Undefined;
            }

            return functionalStatus;
        }

        private async Task<bool> DeviceIpAddressExists(string ipAddress)
        {
            bool ipAddressExists;

            using (var conn = DeviceServiceFactory.GetSqlConnectionComplexSelect())
            {
                await conn.OpenAsync();

                var procedure = "[SPDeviceIpAddressIsUnique]";
                var values = new { @IpAddress = ipAddress };

                var result = await conn.ExecuteScalarAsync<bool>(procedure, values, commandType: CommandType.StoredProcedure);

                ipAddressExists = result;
            }

            return ipAddressExists;
        }
    }
}

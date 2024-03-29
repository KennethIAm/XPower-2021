﻿using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Data;
using System.Threading.Tasks;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;
using XPowerClassLibrary.Device.Entities;
using System.Collections.Generic;

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
                    @UserTokenRequest = assignDeviceRequest.UserTokenRequest,
                    @UniqueDeviceIdentifier = assignDeviceRequest.UniqueDeviceIdentifier,
                    @DeviceName = assignDeviceRequest.DeviceName
                };

                assignedDeviceId = await conn.ExecuteScalarAsync<int>(proc, values, commandType: CommandType.StoredProcedure);
            }

            if (GreaterThanZero(assignedDeviceId))
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


            if (GreaterThanZero(entityId))
            {
                using (var conn = DeviceServiceFactory.GetSqlConnectionComplexSelect())
                {
                    await conn.OpenAsync();

                    //device = await conn.GetAsync<DeviceInformationView>(entityId);
                    device = await conn.QuerySingleOrDefaultAsync<DeviceInformationView>("SELECT * FROM DeviceInformationView WHERE DeviceID = @DeviceID", new { @DeviceID = entityId });
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

        public async Task<IDevice> FindDeviceByUniqueIdentifier(string uniqueIdentifier)
        {
            IDevice device = null;

            using (var conn = DeviceServiceFactory.GetSqlConnectionComplexSelect())
            {
                await conn.OpenAsync();

                var procedure = "[SPFindDeviceByUniqueDeviceIdentifier]";
                var values = new { @UniqueDeviceIdentifier = uniqueIdentifier };

                device = await conn.QuerySingleOrDefaultAsync<DeviceInformationView>(procedure, values, commandType: CommandType.StoredProcedure);
            }

            return device;
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

        /// <summary>
        /// Gets the users owned devices by a user id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IDevice>> GetUsersOwnedDevices(int userId)
        {
            IEnumerable<IDevice> devices = null;

            using (var conn = DeviceServiceFactory.GetSqlConnectionComplexSelect())
            {
                await conn.OpenAsync();

                var procedure = "[SPGetUsersOwnedDevices]";
                var values = new { @UserId = userId };

                devices = await conn.QueryAsync<DeviceInformationView>(procedure, values, commandType: CommandType.StoredProcedure);
            }

            return devices;
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

        private static bool GreaterThanZero(int assignedDeviceId)
        {
            return assignedDeviceId > 0;
        }
    }
}

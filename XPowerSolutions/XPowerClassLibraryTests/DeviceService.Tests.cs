using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XPowerClassLibrary.Device;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;
using XPowerClassLibrary.Device.Services;

namespace XPowerClassLibraryTests
{
    public class DeviceService
    {
        private readonly string _deviceUniqueIdentifier = "UN1T-T3ST-40";
        private readonly string _ipAddress = "127.0.0.1";
        private IDeviceService _deviceService;
        private IDevice _device;

        [SetUp]
        public void Setup()
        {
            _deviceService = DeviceServiceFactory.GetDeviceServiceDB();
            
        }

        [Test, Order(1)]
        public void CreateDeviceAsync_ValidArguments_ShouldCreateDevice()
        {
            // Arrange
            CreateDeviceRequest createDeviceRequest = new CreateDeviceRequest
            {
                DeviceTypeId = 1,
                DeviceFunctionalStatus = DeviceFunctionalStatus.Disabled,
                DeviceConnectionState = DeviceConnectionState.Disconnected,
                DeviceName = "Unit Test",
                DeviceIpAddress = _ipAddress,
                UniqueDeviceIdentifier = _deviceUniqueIdentifier
            };

            // Act
            var actual = Task.Run(async () => await _deviceService.CreateDeviceAsync(createDeviceRequest)).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.DeviceType);
            Assert.Positive(actual.Id);
            Assert.Positive(actual.DeviceType.Id);
            Assert.Positive((int)actual.FunctionalStatus);
            Assert.Positive((int)actual.ConnectionState);
            Assert.DoesNotThrow(() => IPAddress.Parse(createDeviceRequest.DeviceIpAddress));
            Assert.AreEqual(createDeviceRequest.UniqueDeviceIdentifier, actual.UniqueDeviceIdentifier);
            Assert.AreEqual(createDeviceRequest.DeviceName, actual.Name);

            _device = actual;
        }

        [Test, Order(2)]
        public void UpdateDeviceAsync_ValidArguments_ShouldUpdateExistingDevice()
        {
            // Arrange
            UpdateDeviceRequest updateDeviceRequest = new UpdateDeviceRequest
            {
                DeviceTypeId = _device.DeviceType.Id,
                DeviceFunctionalStatus = _device.FunctionalStatus,
                DeviceConnectionState = _device.ConnectionState,
                DeviceName = "Updated Unit Test",
                DeviceIpAddress = _device.IPAddress,
                UniqueDeviceIdentifier = _device.UniqueDeviceIdentifier,
                DeviceId = _device.Id
            };

            // Act
            var actual = Task.Run(async () => await _deviceService.UpdateDeviceAsync(updateDeviceRequest)).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.DeviceType);
            Assert.Positive(actual.Id);
            Assert.Positive(actual.DeviceType.Id);
            Assert.Positive((int)actual.FunctionalStatus);
            Assert.Positive((int)actual.ConnectionState);
            Assert.DoesNotThrow(() => IPAddress.Parse(updateDeviceRequest.DeviceIpAddress));
            Assert.AreEqual(updateDeviceRequest.UniqueDeviceIdentifier, actual.UniqueDeviceIdentifier);
            Assert.AreEqual(updateDeviceRequest.DeviceName, actual.Name);

            _device = actual;
        }

        [Test, Order(3)]
        public void GetDeviceByIdAsync_ValidArgument_ShouldGetDeviceById()
        {
            // Arrange
            int deviceId = _device.Id;

            // Act
            var actual = Task.Run(async () => await _deviceService.GetDeviceByIdAsync(deviceId)).Result;

            // Assert
            Assert.NotNull(actual);
            Assert.Positive(deviceId);
            Assert.AreEqual(deviceId, actual.Id);

            _device = actual;
        }

        [Test, Order(4)]
        public void DeviceOnlineAsync_OnlineRequest_ShouldComeOnline()
        {
            // Arrange
            DeviceOnlineRequest request = new DeviceOnlineRequest
            {
                DeviceTypeId = 1,
                IPAddress = _ipAddress,
                UniqueDeviceIdentifier = _deviceUniqueIdentifier
            };

            // Act
            var actual = Task.Run(async () => await _deviceService.DeviceOnlineAsync(request)).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.DeviceType);
            Assert.Positive(actual.Id);
            Assert.Positive(actual.DeviceType.Id);
            Assert.Positive((int)actual.FunctionalStatus);
            Assert.Positive((int)actual.ConnectionState);
            Assert.DoesNotThrow(() => IPAddress.Parse(actual.IPAddress));
            Assert.AreEqual(request.IPAddress, actual.IPAddress);
            Assert.AreEqual(request.UniqueDeviceIdentifier, actual.UniqueDeviceIdentifier);
            Assert.AreEqual(request.DeviceTypeId, actual.DeviceType.Id);
        }

        [Test]
        public void DeleteDeviceByIdAsync_ValidArgument_ShouldDeleteDeviceById()
        {
            // Arrange
            int deviceId = _device.Id;

            // Act
            var actual = Task.Run(async () => await _deviceService.DeleteDeviceByIdAsync(deviceId)).Result;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual);
        }
    }
}

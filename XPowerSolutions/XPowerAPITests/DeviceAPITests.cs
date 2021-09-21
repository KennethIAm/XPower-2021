using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;

namespace XPowerAPITests
{
    class DeviceAPITests
    {
        // API
        private string apiURL = "https://localhost:44391/";

        // Endpoints
        private string createDeviceEndpoint = "device/CreateDevice";

        [Test]
        public void CreateDeviceAPI_ValidInput_ShouldReturn200()
        {
            // Arrange
            #region Arrange
            var request = new CreateDeviceRequest(1, new DeviceFunctionalStatus(), new DeviceConnectionState(), "JohnnysLampe", "Temp");
            IDevice returnDevice = null;
            #endregion

            // Act
            returnDevice = CreateDevice(request).Result;

            // Assert
            Assert.IsNotNull(returnDevice);
        }

        // Helper methods
        private async System.Threading.Tasks.Task<IDevice> CreateDevice(CreateDeviceRequest request)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(apiURL + createDeviceEndpoint, request);
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    return response.Content.ReadAsAsync<IDevice>().Result;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

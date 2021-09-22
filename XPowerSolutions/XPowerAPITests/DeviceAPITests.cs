using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using XPowerClassLibrary.Device.Entities;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;

namespace XPowerAPITests
{
    class DeviceAPITests
    {

        // API
        private string apiURL = "https://8860-93-176-82-58.ngrok.io/";

        // Endpoints
        private string createDeviceEndpoint = "devices/assign-to-me";

        [Test]
        public void AssignDeviceAPI_ValidInput_ShouldReturn200()
        {
            // Arrange
            #region Arrange
            var request = new AssignDeviceToUserRequest();
            request.UniqueDeviceIdentifier = "0001";
            request.DeviceTypeId = 2;
            request.DeviceName = "BenjiLampe";
            request.UserTokenRequest = "";
            IDevice returnDevice = null;
            #endregion

            // Act
            returnDevice = AssignDevice(request).Result;

            // Assert
            Assert.IsNotNull(returnDevice);
        }

        // Helper methods
        private async System.Threading.Tasks.Task<HardwareDevice> AssignDevice(AssignDeviceToUserRequest request)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PutAsJsonAsync(apiURL + createDeviceEndpoint, request);
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    return response.Content.ReadAsAsync<HardwareDevice>().Result;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

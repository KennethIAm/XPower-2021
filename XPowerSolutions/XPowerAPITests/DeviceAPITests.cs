using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XPowerClassLibrary.Device.Entities;
using XPowerClassLibrary.Device.Models;
using XPowerClassLibrary.Users.Models;

namespace XPowerAPITests
{
    public class DeviceApiTests
    {
        private HttpClient _httpClient;
        private CookieContainer cookieContainer = new CookieContainer();

        private string _baseAddress = "https://xpower.eu.ngrok.io/";
        private string _myDevicesEndpoint = "devices/mine";
        private string authenticateEndpoint = "user/Authenticate";

        private string testMail = "MailTest4@email.com";
        private string testPassword = "PasswordTest";

        [SetUp]
        public void SetUp()
        {
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_baseAddress),
            };
        }

        [Test]
        public async Task UserDevices_Collection_ShouldRetrieveAllDevicesFromUser()
        {
            // Arr

            // Act
            IUserDevice data = await GetUsersDevices();

            // Assert'
            Assert.IsNotNull(data);
        }

        private async Task<IUserDevice> GetUsersDevices()
        {
            using (_httpClient)
            {
                AuthenticateRequest loginRequest = new AuthenticateRequest
                {
                    Username = testMail,
                    Password = testPassword
                };

                var login = new UserAPITests().Login(loginRequest);
                string token = login.RefreshToken;



                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", login.JwtToken);
                cookieContainer.Add(new Uri(_baseAddress), new Cookie("refreshToken", token));

                var response = await _httpClient.GetAsync(_myDevicesEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsAsync<UserDevice>();
                    return res;
                }
                else
                {
                    Console.WriteLine(response.StatusCode);

                    return null;
                }
            }
        }
    }
}

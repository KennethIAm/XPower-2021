using BlazorServerWebsite.Data.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Threading.Tasks;
using XPowerClassLibrary.Device.Entities;
using XPowerClassLibrary.Device.Enums;

namespace BlazorServerWebsite.Pages.Device
{
    public partial class ViewDevicePageBase : ComponentBase
    {
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IHttpClientFactory ClientFactory { get; set; }
        [Inject] protected ISettings Settings { get; set; }

        [CascadingParameter] protected Task<AuthenticationState> AuthenticationState { get; set; }
        private AuthenticationState _context;
        protected EditContext _editContext;

        public HardwareDevice userDevice;

        [Parameter] public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDummyDevice();
            _context = await AuthenticationState;
        }

        // Dummy data
        protected async Task ChangeDummyDeviceStatus()
        {
            if (userDevice.FunctionalStatus == DeviceFunctionalStatus.On)
            {
                userDevice.FunctionalStatus = DeviceFunctionalStatus.Off;
            }
            else if (userDevice.FunctionalStatus == DeviceFunctionalStatus.Off)
            {
                userDevice.FunctionalStatus = DeviceFunctionalStatus.On;
            }
        }

        // Actual data
        protected async Task ChangeDeviceStatus()
        {
            using (var client = GetHttpClient(Settings.Endpoints.BaseEndpoint))
            {
                var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.ChangeDeviceStatusEndpoint}";
                var requestMessage = GetHttpRequest(HttpMethod.Post, endpoint);

                var response = client.PostAsJsonAsync(endpoint, Id).Result;
                userDevice = JsonConvert.DeserializeObject<HardwareDevice>(response.Content.ToString());
            }
        }
        
        // Dummy data
        private async Task LoadDummyDevice()
        {
            IEnumerable<HardwareDevice> UserDevices;

            HardwareDevice d1 = new HardwareDevice()
            {
                Id = 1,
                DeviceType = new DeviceType(1, "Lyselement"),
                FunctionalStatus = DeviceFunctionalStatus.On,
                ConnectionState = DeviceConnectionState.Connected,
                Name = "JohnnysLampe",
                IPAddress = "Temp"
            };

            HardwareDevice d2 = new HardwareDevice()
            {
                Id = 2,
                DeviceType = new DeviceType(1, "Lyselement"),
                FunctionalStatus = DeviceFunctionalStatus.On,
                ConnectionState = DeviceConnectionState.Connected,
                Name = "Badeværelseslys",
                IPAddress = "Temp"
            };

            HardwareDevice d3 = new HardwareDevice()
            {
                Id = 3,
                DeviceType = new DeviceType(1, "Lyselement"),
                FunctionalStatus = DeviceFunctionalStatus.On,
                ConnectionState = DeviceConnectionState.Connected,
                Name = "Soveværelseslampe",
                IPAddress = "Temp"
            };

            HardwareDevice d4 = new HardwareDevice()
            {
                Id = 4,
                DeviceType = new DeviceType(1, "Lyselement"),
                FunctionalStatus = DeviceFunctionalStatus.Off,
                ConnectionState = DeviceConnectionState.Connected,
                Name = "Køkkenlys",
                IPAddress = "Temp"
            };

            HardwareDevice d5 = new HardwareDevice()
            {
                Id = 5,
                DeviceType = new DeviceType(4, "Klimaelement"),
                FunctionalStatus = DeviceFunctionalStatus.On,
                ConnectionState = DeviceConnectionState.Connected,
                Name = "Blæser",
                IPAddress = "Temp"
            };

            UserDevices = new List<HardwareDevice> { d1, d2, d3, d4, d5 };

            foreach (HardwareDevice d in UserDevices)
            {
                if (d.Id == Id)
                {
                    userDevice = d;
                }
            }
        }

        // Actual data
        private async Task LoadDevice()
        {
            using (var client = GetHttpClient(Settings.Endpoints.BaseEndpoint))
            {
                var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.GetDeviceByIdEndpoint}";
                var requestMessage = GetHttpRequest(HttpMethod.Post, endpoint);

                var response = client.PostAsJsonAsync(endpoint, Id).Result;
                userDevice = JsonConvert.DeserializeObject<HardwareDevice>(response.Content.ToString());
            }
        }

        private HttpRequestMessage GetHttpRequest(HttpMethod method, string requestEndpoint)
        {
            return new HttpRequestMessage(
                method, new Uri(requestEndpoint));
        }

        private HttpClient GetHttpClient(string baseEndpoint)
        {
            var client = ClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseEndpoint);

            return client;
        }
    }
}

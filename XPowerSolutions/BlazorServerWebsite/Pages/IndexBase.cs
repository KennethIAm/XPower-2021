using Blazored.LocalStorage;
using BlazorServerWebsite.Data.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XPowerClassLibrary.Device.Entities;
using XPowerClassLibrary.Device.Enums;
using XPowerClassLibrary.Device.Models;
using XPowerClassLibrary.Device.Models.Requests;
using XPowerClassLibrary.Users.Models;

namespace BlazorServerWebsite.Pages
{
    public partial class IndexBase : ComponentBase
    {
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected ILocalStorageService LocalStorage { get; set; }
        [Inject] protected ISettings Settings { get; set; }
        [CascadingParameter] protected Task<AuthenticationState> AuthenticationState { get; set; }
        private AuthenticationState _context;

        public UserDevice UserDevices { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDevices();
            _context = await AuthenticationState;
        }

        protected void GoToCreateDevice()
        {
            NavigationManager.NavigateTo("/device/registerdevice");
        }

        //private async Task LoadMockDevices()
        //{
        //    HardwareDevice d1 = new HardwareDevice()
        //    { 
        //    Id = 1,
        //    DeviceType = new DeviceType(1, "Lyselement"),
        //    FunctionalStatus = DeviceFunctionalStatus.On,
        //    ConnectionState = DeviceConnectionState.Connected,
        //    Name = "JohnnysLampe",
        //    IPAddress = "Temp"
        //    };

        //    HardwareDevice d2 = new HardwareDevice()
        //    {
        //        Id = 2,
        //        DeviceType = new DeviceType(1, "Lyselement"),
        //        FunctionalStatus = DeviceFunctionalStatus.On,
        //        ConnectionState = DeviceConnectionState.Connected,
        //        Name = "Badeværelseslys",
        //        IPAddress = "Temp"
        //    };

        //    HardwareDevice d3 = new HardwareDevice()
        //    {
        //        Id = 3,
        //        DeviceType = new DeviceType(1, "Lyselement"),
        //        FunctionalStatus = DeviceFunctionalStatus.On,
        //        ConnectionState = DeviceConnectionState.Connected,
        //        Name = "Soveværelseslampe",
        //        IPAddress = "Temp"
        //    };

        //    HardwareDevice d4 = new HardwareDevice()
        //    {
        //        Id = 4,
        //        DeviceType = new DeviceType(1, "Lyselement"),
        //        FunctionalStatus = DeviceFunctionalStatus.Off,
        //        ConnectionState = DeviceConnectionState.Connected,
        //        Name = "Køkkenlys",
        //        IPAddress = "Temp"
        //    };

        //    HardwareDevice d5 = new HardwareDevice()
        //    {
        //        Id = 5,
        //        DeviceType = new DeviceType(4, "Klimaelement"),
        //        FunctionalStatus = DeviceFunctionalStatus.On,
        //        ConnectionState = DeviceConnectionState.Connected,
        //        Name = "Blæser",
        //        IPAddress = "Temp"
        //    };

        //    UserDevices = new List<HardwareDevice> { d1, d2, d3, d4, d5 };
        //}

        private async Task LoadDevices()
        {
            string token = await LocalStorage.GetItemAsync<string>(Settings.RefreshTokenKey);



            var cookieContainer = new CookieContainer();
            using var handler = new HttpClientHandler { CookieContainer = cookieContainer };
            using var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) };

            cookieContainer.Add(client.BaseAddress, new Cookie(Settings.RefreshTokenKey, token));
            var result = await client.PostAsJsonAsync<string>(Settings.Endpoints.RefreshTokenEndpoint, token);
            AuthenticateResponse authenticateResponseRefresh = result.Content.ReadAsAsync<AuthenticateResponse>().Result;
            await LocalStorage.SetItemAsync<string>(Settings.RefreshTokenKey, authenticateResponseRefresh.RefreshToken);
            await LocalStorage.SetItemAsync<string>(Settings.JwtKey, authenticateResponseRefresh.JwtToken);

            Console.WriteLine($"Server Response Code from Auth: {result.StatusCode}");

            token = authenticateResponseRefresh.RefreshToken;
            string jwtToken = authenticateResponseRefresh.JwtToken;


            UserDevicesRequest request = new UserDevicesRequest()
            {
                RefreshToken = token
            };

            string requestJSON = JsonConvert.SerializeObject(request);
            StringContent data = new StringContent(requestJSON, Encoding.UTF8, "application/json");


            cookieContainer = new CookieContainer();

            using (client)
            {
                var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.AllUserDevicesEndpoint}";
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);
                cookieContainer.Add(new Uri(Settings.Endpoints.BaseEndpoint), new Cookie("refreshToken", token));

                var createDeviceResponseMessage = await client.GetAsync(endpoint);



var UserDevicesObject = await createDeviceResponseMessage.Content.ReadAsAsync<object>();
                if (createDeviceResponseMessage.IsSuccessStatusCode)
                {

                

                    
                UserDevices = await createDeviceResponseMessage.Content.ReadAsAsync<UserDevice>();
                    Console.WriteLine("Devices retrieved!");
                }
            }
        }
    }
}

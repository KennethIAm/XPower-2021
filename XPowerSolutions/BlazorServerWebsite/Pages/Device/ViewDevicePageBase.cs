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
using Blazored.LocalStorage;
using System.Net;
using XPowerClassLibrary.Users.Models;
using System.Net.Http.Headers;
using XPowerClassLibrary.Device.Models;

namespace BlazorServerWebsite.Pages.Device
{
    public partial class ViewDevicePageBase : ComponentBase
    {
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IHttpClientFactory ClientFactory { get; set; }
        [Inject] protected ILocalStorageService LocalStorage { get; set; }
        [Inject] protected ISettings Settings { get; set; }

        [CascadingParameter] protected Task<AuthenticationState> AuthenticationState { get; set; }
        private AuthenticationState _context;
        protected EditContext _editContext;

        public HardwareDevice userDevice;

        [Parameter] public int Id { get; set; }
        protected string RefreshToken { get; set; }
        protected string JwtToken { get; set; }
        protected string _message { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDevice();
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

        private async Task LoadDevice()
        {
            await RefreshTokensAsync();

            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) })
                {
                    var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.GetDeviceByIdEndpoint}" + Id;
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", JwtToken);
                    cookieContainer.Add(new Uri(Settings.Endpoints.BaseEndpoint), new Cookie("refreshToken", RefreshToken));

                    var createDeviceResponseMessage = await client.GetAsync(endpoint);


                    if (createDeviceResponseMessage.IsSuccessStatusCode)
                    {
                        string jsonString = await createDeviceResponseMessage.Content.ReadAsStringAsync();
                        userDevice = JsonConvert.DeserializeObject<HardwareDevice>(jsonString);

                        Console.WriteLine("Device retrieved!");
                    }
                }
            }
        }

        protected async Task ChangeFunctionalStatus()
        {
            string command = "";
            if (userDevice.FunctionalStatus == DeviceFunctionalStatus.On)
            {
                command = "function1";
            }
            else if (userDevice.FunctionalStatus == DeviceFunctionalStatus.Off)
            {
                command = "function2";
            }

            if (command != "")
            {
                bool changeSuccessful = false;
                await RefreshTokensAsync();

                var cookieContainer = new CookieContainer();

                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                {
                    using (var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) })
                    {
                        var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.DeviceEndpoint}?command=" + command + "&ipAddress=" + userDevice.IPAddress;
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", JwtToken);
                        cookieContainer.Add(new Uri(Settings.Endpoints.BaseEndpoint), new Cookie("refreshToken", RefreshToken));

                        var createDeviceResponseMessage = await client.GetAsync(endpoint);


                        if (createDeviceResponseMessage.IsSuccessStatusCode)
                        {
                            string jsonString = await createDeviceResponseMessage.Content.ReadAsStringAsync();
                            userDevice = JsonConvert.DeserializeObject<HardwareDevice>(jsonString);

                            if (command == "function1")
                            {
                                userDevice.FunctionalStatus = DeviceFunctionalStatus.On;
                            }
                            else if (command == "function2")
                            {
                                userDevice.FunctionalStatus = DeviceFunctionalStatus.Off;
                            }

                            Console.WriteLine("Device retrieved!");

                            changeSuccessful = true;
                        }
                        else
                        {
                            _message = "Kunne ikke oprette forbindelse til enheden.";
                        }
                    }
                }

                if (changeSuccessful)
                {
                    await UpdateDevice();
                }
            }
        }

        private async Task UpdateDevice()
        {
            await RefreshTokensAsync();

            UpdateDeviceRequest request = new UpdateDeviceRequest
            {
                DeviceId = userDevice.Id,
                DeviceName = userDevice.Name,
                DeviceIpAddress = userDevice.IPAddress,
                DeviceConnectionState = userDevice.ConnectionState,
                DeviceFunctionalStatus = userDevice.FunctionalStatus,
                DeviceTypeId = userDevice.DeviceType.Id,
                UniqueDeviceIdentifier = userDevice.UniqueDeviceIdentifier
            };

            CookieContainer cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) })
                {
                    var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.AllUserDevicesEndpoint}";
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", JwtToken);
                    cookieContainer.Add(new Uri(Settings.Endpoints.BaseEndpoint), new Cookie("refreshToken", RefreshToken));

                    var createDeviceResponseMessage = await client.PutAsJsonAsync(endpoint, request);

                    if (createDeviceResponseMessage.IsSuccessStatusCode)
                    {
                        _message = "Enheden er blevet opdateret.";

                        Console.WriteLine("Devices updated!");
                    }
                    else
                    {
                        _message = "Enheden er ikke blevet opdateret.";
                    }
                }
            }
        }

        private async Task RefreshTokensAsync()
        {
            string token = await LocalStorage.GetItemAsync<string>(Settings.RefreshTokenKey);

            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) })
                {
                    cookieContainer.Add(client.BaseAddress, new Cookie(Settings.RefreshTokenKey, token));
                    var result = await client.PostAsJsonAsync<string>(Settings.Endpoints.RefreshTokenEndpoint, token);
                    AuthenticateResponse authenticateResponseRefresh = result.Content.ReadAsAsync<AuthenticateResponse>().Result;
                    await LocalStorage.SetItemAsync<string>(Settings.RefreshTokenKey, authenticateResponseRefresh.RefreshToken);
                    await LocalStorage.SetItemAsync<string>(Settings.JwtKey, authenticateResponseRefresh.JwtToken);

                    Console.WriteLine($"Server Response Code from Auth: {result.StatusCode}");

                    RefreshToken = authenticateResponseRefresh.RefreshToken;
                    JwtToken = authenticateResponseRefresh.JwtToken;
                }
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

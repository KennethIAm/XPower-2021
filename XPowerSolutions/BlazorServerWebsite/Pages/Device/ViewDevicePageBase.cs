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
using System.Threading;

namespace BlazorServerWebsite.Pages.Device
{
    public class MessageObject
    {
        public string Message { get; set; }
    }

    public class CurrentUsageObject
    {
        public bool HasReturnInfo { get; set; }
        public float CurrentUsage { get; set; }
    }

    public partial class ViewDevicePageBase : ComponentBase
    {
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IHttpClientFactory ClientFactory { get; set; }
        [Inject] protected ILocalStorageService LocalStorage { get; set; }
        [Inject] protected ISettings Settings { get; set; }

        [CascadingParameter] protected Task<AuthenticationState> AuthenticationState { get; set; }
        private AuthenticationState _context;
        protected EditContext _editContext;

        public HardwareDevice UserDevice;
        protected bool OnOffDisabled = false;
        protected CurrentUsageObject CurrentUsage = new CurrentUsageObject();
        protected bool DisplayCurrentUsage { get; set; }
        protected string IconPath { get; set; }

        [Parameter] public int Id { get; set; }
        protected string RefreshToken { get; set; }
        protected string JwtToken { get; set; }
        protected string _message { get; set; }
        protected string ButtonOn { get; set; }
        protected string ButtonOff { get; set; }
        protected string StatusOn { get; set; }
        protected string StatusOff { get; set; }
        protected bool WaitForData { get; set; }

        protected override async Task OnInitializedAsync()
        {
            WaitForData = true;
            await LoadDevice();
            _context = await AuthenticationState;

            if (UserDevice is null)
            {
                WaitForData = false;
            }
            StateHasChanged();
        }

        // Request the API for a device by ID
        private async Task LoadDevice()
        {
            await RenewTokensAsync();

            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) })
                {
                    var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.GetDeviceByIdEndpoint}" + Id;
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", JwtToken);
                    cookieContainer.Add(new Uri(Settings.Endpoints.BaseEndpoint), new Cookie("refreshToken", RefreshToken));

                    var response = await client.GetAsync(endpoint);

                    // If successful, convert to HardwareDevice
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        UserDevice = JsonConvert.DeserializeObject<HardwareDevice>(jsonString);

                        Console.WriteLine("Device retrieved!");
                    }
                }
            }
            if (UserDevice.DeviceType.Id != 3)
            {
                // Gets the electricity ussage of the device if it's 
                await GetElectricityUsage();
            }

            SetOnOffText(UserDevice.DeviceType.Id);
        }

        // Change text on UI based on what kind of device is used.
        private void SetOnOffText(int type)
        {
            if (type == 3)
            {
                ButtonOn = "Åben vindue";
                ButtonOff = "Luk vindue";
                StatusOn = "Åbent";
                StatusOff = "Lukket";
            }
            else
            {
                ButtonOn = "Tænd enhed";
                ButtonOff = "Sluk enhed";
                StatusOn = "Tændt";
                StatusOff = "Slukket";
            }
        }

        // Change FunctionalStatus of device
        protected async Task ChangeFunctionalStatus()
        {
            OnOffDisabled = true;
            string command = "";

            // Specifies whether it's an on command or an off command
            if (UserDevice.FunctionalStatus == DeviceFunctionalStatus.On)
            {
                command = "function2";
            }
            else if (UserDevice.FunctionalStatus == DeviceFunctionalStatus.Off || UserDevice.FunctionalStatus == DeviceFunctionalStatus.Disabled)
            {
                command = "function1";
            }

            if (command != "")
            {
                bool changeSuccessful = false;
                await RenewTokensAsync();

                // Sends command to change status to the device
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                {
                    using (var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) })
                    {
                        var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.DeviceEndpoint}?command=" + command + "&ipAddress=" + UserDevice.IPAddress;
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", JwtToken);
                        cookieContainer.Add(new Uri(Settings.Endpoints.BaseEndpoint), new Cookie("refreshToken", RefreshToken));

                        var resopnse = await client.GetAsync(endpoint);

                        if (resopnse.IsSuccessStatusCode)
                        {
                            string jsonString = await resopnse.Content.ReadAsStringAsync();

                            if (command == "function1")
                            {
                                UserDevice.FunctionalStatus = DeviceFunctionalStatus.On;
                            }
                            else if (command == "function2")
                            {
                                UserDevice.FunctionalStatus = DeviceFunctionalStatus.Off;
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
                    // Informs the API about the change
                    await UpdateDevice();
                }
            }
            if (UserDevice.DeviceType.Id != 3)
            {
                await GetElectricityUsage();
            }
            OnOffDisabled = false;
        }

        private async Task GetElectricityUsage()
        {
            string command = "function0";

            if (command != "")
            {
                await RenewTokensAsync();

                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                {
                    using (var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) })
                    {
                        var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.DeviceEndpoint}?command=" + command + "&ipAddress=" + UserDevice.IPAddress;
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", JwtToken);
                        cookieContainer.Add(new Uri(Settings.Endpoints.BaseEndpoint), new Cookie("refreshToken", RefreshToken));

                        var response = await client.GetAsync(endpoint);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Device retrieved!");
                            CurrentUsage = await response.Content.ReadAsAsync<CurrentUsageObject>();
                        }
                        else
                        {
                            _message = "Kunne ikke oprette forbindelse til enheden.";
                        }
                    }
                }
            }
        }

        // Request the API to update the device in the database
        private async Task UpdateDevice()
        {
            await RenewTokensAsync();

            UpdateDeviceRequest request = CreateUpdateDeviceRequest();

            CookieContainer cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) })
                {
                    var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.UpdateDeviceEndpoint}";
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", JwtToken);
                    cookieContainer.Add(new Uri(Settings.Endpoints.BaseEndpoint), new Cookie("refreshToken", RefreshToken));

                    var response = await client.PutAsJsonAsync(endpoint, request);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Devices updated!");
                    }
                    else
                    {
                        MessageObject message = await response.Content.ReadAsAsync<MessageObject>();
                        _message = message.Message;
                    }
                }
            }
        }

        // Create UpdateDeviceRequest
        private UpdateDeviceRequest CreateUpdateDeviceRequest()
        {
            UpdateDeviceRequest request = new UpdateDeviceRequest(UserDevice.Id, UserDevice.DeviceType.Id, UserDevice.FunctionalStatus,
                UserDevice.ConnectionState, UserDevice.UniqueDeviceIdentifier, UserDevice.Name, UserDevice.IPAddress);
            return request;
        }

        // Get new tokens from the API
        private async Task RenewTokensAsync()
        {
            string token = await LocalStorage.GetItemAsync<string>(Settings.RefreshTokenKey);

            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) })
                {
                    cookieContainer.Add(client.BaseAddress, new Cookie(Settings.RefreshTokenKey, token));
                    var response = await client.PostAsJsonAsync<string>(Settings.Endpoints.RefreshTokenEndpoint, token);
                    if (response.IsSuccessStatusCode)
                    {
                        AuthenticateResponse authenticateResponseRefresh = await response.Content.ReadAsAsync<AuthenticateResponse>();
                        await LocalStorage.SetItemAsync<string>(Settings.RefreshTokenKey, authenticateResponseRefresh.RefreshToken);
                        await LocalStorage.SetItemAsync<string>(Settings.JwtKey, authenticateResponseRefresh.JwtToken);

                        Console.WriteLine($"Server Response Code from Auth: {response.StatusCode}");

                        RefreshToken = authenticateResponseRefresh.RefreshToken;
                        JwtToken = authenticateResponseRefresh.JwtToken;
                    }
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

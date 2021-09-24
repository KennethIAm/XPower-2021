using Blazored.LocalStorage;
using BlazorServerWebsite.Data.Models;
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
using System.Threading;
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

        public UserDeviceResponse UserDevices { get; set; }
        protected string RefreshToken { get; set; }
        protected string JwtToken { get; set; }
        protected string Username { get; set; }
        protected bool WaitForData { get; set; }

        protected override async Task OnInitializedAsync()
        {
            WaitForData = true;
            await LoadDevices();
            _context = await AuthenticationState;

            if (UserDevices is null)
            {
                WaitForData = false;
            }
            StateHasChanged();
        }

        // Navigate to AddDevicePage
        protected void GoToCreateDevice()
        {
            NavigationManager.NavigateTo("/device/registerdevice");
        }

        // Request user's devices from the API
        private async Task LoadDevices()
        {
            await RenewTokensAsync();

            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) })
                {
                    var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.AllUserDevicesEndpoint}";
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", JwtToken);
                    cookieContainer.Add(new Uri(Settings.Endpoints.BaseEndpoint), new Cookie("refreshToken", RefreshToken));

                    var resopnse = await client.GetAsync(endpoint);

                    if (resopnse.IsSuccessStatusCode)
                    {
                        string jsonString = await resopnse.Content.ReadAsStringAsync();
                        UserDevices = JsonConvert.DeserializeObject<UserDeviceResponse>(jsonString);

                        Console.WriteLine("Devices retrieved!");
                    }
                }
            }
        }
        
        // Request new tokens from the API
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
                        Username = authenticateResponseRefresh.UserObject.Username;
                        await LocalStorage.SetItemAsync<string>(Settings.RefreshTokenKey, authenticateResponseRefresh.RefreshToken);
                        await LocalStorage.SetItemAsync<string>(Settings.JwtKey, authenticateResponseRefresh.JwtToken);

                        Console.WriteLine($"Server Response Code from Auth: {response.StatusCode}");

                        RefreshToken = authenticateResponseRefresh.RefreshToken;
                        JwtToken = authenticateResponseRefresh.JwtToken;
                    }
                }
            }
        }
    }
}

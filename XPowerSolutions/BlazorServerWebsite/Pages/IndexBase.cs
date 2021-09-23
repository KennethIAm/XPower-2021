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

        protected override async Task OnInitializedAsync()
        {
            await LoadDevices();
            _context = await AuthenticationState;
        }

        protected void GoToCreateDevice()
        {
            NavigationManager.NavigateTo("/device/registerdevice");
        }

        private async Task LoadDevices()
        {
            await RefreshTokensAsync();

            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) })
                {
                    var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.AllUserDevicesEndpoint}";
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", JwtToken);
                    cookieContainer.Add(new Uri(Settings.Endpoints.BaseEndpoint), new Cookie("refreshToken", RefreshToken));

                    var createDeviceResponseMessage = await client.GetAsync(endpoint);

                    if (createDeviceResponseMessage.IsSuccessStatusCode)
                    {
                        string jsonString = await createDeviceResponseMessage.Content.ReadAsStringAsync();
                        UserDevices = JsonConvert.DeserializeObject<UserDeviceResponse>(jsonString);

                        Console.WriteLine("Devices retrieved!");
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
    }
}

using BlazorServerWebsite.Data.Models;
using BlazorServerWebsite.Data.Providers;
using BlazorServerWebsite.Data.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using XPowerClassLibrary.Users.Models;
using XPowerClassLibrary.Device.Models;
using XPowerClassLibrary.Device.Enums;
using Blazored.LocalStorage;
using XPowerClassLibrary.Device.Entities;
using System.Net.Http.Headers;
using System.Net;

namespace BlazorServerWebsite.Pages.Device
{
    public partial class AddDevicePage : ComponentBase
    {
        [Inject] protected IHttpClientFactory ClientFactory { get; set; }
        [Inject] protected AuthStateProvider AuthStateProvider { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected ILocalStorageService LocalStorage { get; set; }
        [Inject] protected ISettings Settings { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }

        private AssignDeviceModel _model;
        private EditContext _editContext;
        private string _message = string.Empty;
        protected string RefreshToken { get; set; }
        protected string JwtToken { get; set; }

        protected override void OnInitialized()
        {
            InitializeNewContext();
        }


        private async Task OnValidForm_AddDeviceAsync()
        {
            _message = "";

            if (!_model.IsValidForm())
            {
                _message = "Ét eller flere felter er ugyldige.";
                return;
            }

            await RenewTokensAsync();

            var assignDeviceRequest = CreateAssignDeviceToUserRequest();

            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint) })
                {
                    var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.AssignDeviceEndpoint}";
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", JwtToken);
                    cookieContainer.Add(new Uri(Settings.Endpoints.BaseEndpoint), new Cookie("refreshToken", RefreshToken));

                    var resopnse = await client.PutAsJsonAsync(endpoint, assignDeviceRequest);


                    if (resopnse.IsSuccessStatusCode)
                    {
                        HardwareDevice device = await resopnse.Content.ReadAsAsync<HardwareDevice>();
                        Console.WriteLine("Device was created!");

                        Console.WriteLine($"Created Device: {assignDeviceRequest.DeviceName}");

                        _message = "Enheden er blevet registreret.";

                        GoToIndex();
                    }
                    else
                    {
                        _message = "Der opstod en fejl.";
                    }
                }
            }
        }

        private AssignDeviceToUserRequest CreateAssignDeviceToUserRequest()
        {
            var assignDeviceRequest = new AssignDeviceToUserRequest
            {
                UniqueDeviceIdentifier = _model.Id,
                DeviceName = _model.Name,
                UserTokenRequest = RefreshToken
            };

            return assignDeviceRequest;
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

        // Navigate to Index
        protected void GoToIndex()
        {
            NavigationManager.NavigateTo("/");
        }

        private void InitializeNewContext()
        {
            _model = new();
            _editContext = new(_model);
            _editContext.AddDataAnnotationsValidation();
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

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

namespace BlazorServerWebsite.Pages.Device
{
    public partial class AddDevicePage : ComponentBase
    {
        [Inject] protected IHttpClientFactory ClientFactory { get; set; }
        [Inject] protected AuthStateProvider AuthStateProvider { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected ISettings Settings { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }

        private CreateDeviceModel _model;
        private EditContext _editContext;
        private string _message = string.Empty;

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

            using (var client = GetHttpClient(Settings.Endpoints.BaseEndpoint))
            {
                var endpoint = $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.CreateDeviceEndpoint}";
                var requestMessage = GetHttpRequest(HttpMethod.Post, endpoint);
                var createDeviceRequest = new CreateDeviceRequest
                {
                    DeviceName = _model.Name,
                    DeviceIpAddress = "Temp",
                    DeviceConnectionState = new DeviceConnectionState(),
                    DeviceFunctionalStatus = new DeviceFunctionalStatus(),
                    DeviceTypeId = Convert.ToInt32(_model.Type)
                };

                var createDeviceResponseMessage = await client.PostAsJsonAsync(endpoint, createDeviceRequest);

                if (createDeviceResponseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("Device was created!");

                    var deviceCreated = await createDeviceResponseMessage.Content.ReadFromJsonAsync<IDevice>();

                    Console.WriteLine($"Created Device: {createDeviceRequest.DeviceName} : {createDeviceRequest.DeviceTypeId}");

                    _message = "Enheden er blevet registreret.";
                }
                else
                {
                    _message = "Fejl, kunne ikke registrere enheden.";
                }
            }
        }

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

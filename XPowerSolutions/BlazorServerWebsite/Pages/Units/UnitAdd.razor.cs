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
using XPowerClassLibrary.Unit.Models;

namespace BlazorServerWebsite.Pages.Units
{
    public partial class UnitAdd : ComponentBase
    {
        [Inject] protected IHttpClientFactory ClientFactory { get; set; }
        [Inject] protected AuthStateProvider AuthStateProvider { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected ISettings Settings { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }

        private RegisterUnitModel _model;
        private EditContext _editContext;
        private string _message = string.Empty;

        protected override void OnInitialized()
        {
            InitializeNewContext();
        }

        //private async Task OnValidForm_AuthenticateAccountRegisterAsync()
        private async Task OnValidForm_AddUnitAsync()
        {
            _message = "";

            if (!_model.IsValidForm())
            {
                _message = "Ét eller flere felter er ugyldige.";
                return;
            }

            var client = GetHttpClient(Settings.Endpoints.BaseEndpoint);

            using (client)
            {
                // Register Unit POST.
                var registerUnitRequestMessage = GetHttpRequest(
                    HttpMethod.Post,
                    $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.RegisterUnitEndpoint}");
                var registerUnitResponseMessage = await client.PostAsJsonAsync(
                registerUnitRequestMessage.RequestUri,
                    new RegisterUnitModel
                    {
                        ID = _model.ID,
                        Name = _model.Name,
                        Type = _model.Type
                    });

                if (registerUnitResponseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("Unit was created!");

                    var registerUnitRequest = await registerUnitResponseMessage.Content.ReadFromJsonAsync<RegisterUnitRequest>();

                    Console.WriteLine($"Created Unit: {registerUnitRequest.Name} : {registerUnitRequest.Type}");

                    _message = "Enheden er blevet registreret.";
                }
                else
                {
                    _message = "Fejl, kunne ikke registrere enheden.";
                }
            }
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

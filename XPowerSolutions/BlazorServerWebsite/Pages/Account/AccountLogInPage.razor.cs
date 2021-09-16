using BlazorServerWebsite.Data;
using BlazorServerWebsite.Data.Models;
using BlazorServerWebsite.Data.Providers;
using BlazorServerWebsite.Data.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using XPowerClassLibrary.Users.Models;

namespace BlazorServerWebsite.Pages.Account
{
    public partial class AccountLogInPage : ComponentBase
    {
        [Inject] protected IHttpClientFactory ClientFactory { get; set; }
        [Inject] protected AuthStateProvider AuthStateProvider { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected ISettings Settings { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }

        private AccountLogInModel _model;
        private EditContext _editContext;
        private string _message = string.Empty;
        private HttpRequestMessage _requestMessage;
        private HttpResponseMessage _responseMessage;
        private HttpClient _client;

        protected override void OnInitialized()
        {
            InitializeNewContext();

            _requestMessage = new HttpRequestMessage(HttpMethod.Post,
                $"{Settings.Endpoints.BaseEndpoint}{Settings.Endpoints.AuthenticateEndpoint}");
        }

        private async Task OnValidForm_AuthenticateAccountLogInAsync()
        {
            _message = string.Empty;

            if (!_model.IsValidForm())
            {
                _message = "En eller flere felter er ikke gyldige.";
                return;
            }

            _client = ClientFactory.CreateClient();
            _client.BaseAddress = new Uri(Settings.Endpoints.BaseEndpoint);

            using (_client)
            {
                _responseMessage = await _client.PostAsJsonAsync(
                    _requestMessage.RequestUri,
                    new AuthenticateRequest
                    {
                        Username = _model.EmailAddress,
                        Password = _model.Password
                    });
            }

            if (_responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine($"We Success Boyyysss.");

                var authRes = await _responseMessage.Content.ReadFromJsonAsync<AuthenticateResponse>();

                if (authRes.UserObject is not null)
                {
                    Console.WriteLine($"Auth User: {authRes.UserObject.Id}");

                    _message = "Logged in success!";
                    await AuthStateProvider.MarkUserAsAuthenticated(authRes);

                    InitializeNewContext();

                    NavigationManager.NavigateTo("/");
                }
            }
            else
            {
                _message = "E-mailadresse eller legitimationsoplysninger er forkerte.";
            }
        }

        private void InitializeNewContext()
        {
            _model = new();
            _editContext = new(_model);
            _editContext.AddDataAnnotationsValidation();
        }
    }
}

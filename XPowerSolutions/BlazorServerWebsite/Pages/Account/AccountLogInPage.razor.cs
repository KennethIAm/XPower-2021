using BlazorServerWebsite.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XPowerClassLibrary.Users.Models;

namespace BlazorServerWebsite.Pages.Account
{
    public partial class AccountLogInPage : ComponentBase
    {
        [Inject] protected IHttpClientFactory ClientFactory { get; set; }

        private AccountLogInModel _model;
        private EditContext _editContext;
        private string _message = string.Empty;
        private string _apiEndpoint = "https://5fc6-93-176-82-57.ngrok.io";
        private HttpRequestMessage _requestMessage;
        private HttpClient _client;

        protected override async Task OnInitializedAsync()
        {
            InitializeNewContext();

            _requestMessage = new HttpRequestMessage(HttpMethod.Post,
                $"{_apiEndpoint}/user/Authenticate");

            await base.OnInitializedAsync();
        }

        private async Task OnValidForm_AuthenticateAccountLogInAsync()
        {
            _client = ClientFactory.CreateClient();
            _client.BaseAddress = new Uri(_apiEndpoint);
            var authRequest = new AuthenticateRequest()
            {
                Username = _model.EmailAddress,
                Password = _model.Password
            };

            var jsonRequest = new StringContent(
                JsonSerializer.Serialize(authRequest), Encoding.UTF8, "application/json");

            var result = await _client.PostAsync("/user/Authenticate", jsonRequest);

            if (result.IsSuccessStatusCode)
            {
                using var responseStream = await result.Content.ReadAsStreamAsync();
                var user = await JsonSerializer.DeserializeAsync<AuthenticateResponse>(responseStream);
            }

            InitializeNewContext();

            await Task.CompletedTask;
        }

        private void InitializeNewContext()
        {
            _model = new();
            _editContext = new(_model);
            _editContext.AddDataAnnotationsValidation();
        }
    }
}

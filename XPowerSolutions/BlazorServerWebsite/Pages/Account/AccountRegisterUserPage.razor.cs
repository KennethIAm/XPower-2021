using BlazorServerWebsite.Data;
using BlazorServerWebsite.Data.Models;
using BlazorServerWebsite.Data.Providers;
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

namespace BlazorServerWebsite.Pages.Account
{
    public partial class AccountRegisterUserPage : ComponentBase
    {
        [Inject] protected IHttpClientFactory ClientFactory { get; set; }
        [Inject] protected AuthStateProvider AuthStateProvider { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected ApiSettings ApiSettings { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }

        private HttpRequestMessage _requestMessage;
        private AccountRegisterModel _model;
        private EditContext _editContext;
        private string _message = string.Empty;

        protected override void OnInitialized()
        {
            InitializeNewContext();
        }

        private async Task OnValidForm_AuthenticateAccountRegisterAsync()
        {
            _message = "";
            var client = GetHttpClient(ApiSettings.BaseEndpoint);

            using (client)
            {
                // Create User POST.
                var createUserRequestMessage = GetHttpRequest(HttpMethod.Post, $"{ApiSettings.BaseEndpoint}{ApiSettings.CreateUserEndpoint}");
                var createUserResponseMessage = await client.PostAsJsonAsync(
                createUserRequestMessage.RequestUri,
                    new CreateUserRequest
                    {
                        Mail = _model.EmailAddress,
                        Username = _model.Username,
                        Password = _model.Password
                    });

                if (createUserResponseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("User was created!");

                    var createUserRequest = await createUserResponseMessage.Content.ReadFromJsonAsync<CreateUserRequest>();

                    Console.WriteLine($"Created User: {createUserRequest.Mail} : {createUserRequest.Password}");

                    _message = "Bruger blev oprettet.";

                    // Login
                    var logInRequestMessage = GetHttpRequest(HttpMethod.Post, $"{ApiSettings.BaseEndpoint}{ApiSettings.AuthenticateEndpoint}");
                    var logInResponseMessage = await client.PostAsJsonAsync(
                        logInRequestMessage.RequestUri,
                        new AuthenticateRequest
                        {
                            Username = _model.EmailAddress,
                            Password = _model.Password
                        });

                    if (logInResponseMessage.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Logging In user after creation!");

                        var authRes = await logInResponseMessage.Content.ReadFromJsonAsync<AuthenticateResponse>();

                        Console.WriteLine($"");

                        if (authRes.UserObject is not null)
                        {
                            Console.WriteLine($"Auth User: {authRes.UserObject.Id}");

                            _message = "Logged in success, redirecting!";
                            await AuthStateProvider.MarkUserAsAuthenticated(authRes);

                            InitializeNewContext();

                            NavigationManager.NavigateTo("/");
                        }
                        else
                        {
                            Console.WriteLine("Given user after creation was null.");

                            _message = "Kunne ikke logge ind, venglist log ind manuelt.";
                            NavigationManager.NavigateTo("/account/login");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Couldn't authenticate user after creation. Status Code: {logInResponseMessage.StatusCode}");

                        _message = "Kunne ikke autorisere bruger, beklager, venligst log ind manuelt.";
                        NavigationManager.NavigateTo("/account/login");
                    }
                }
                else
                {
                    _message = "Fejl, kunne ikke oprette bruger.";
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

using BlazorServerWebsite.Data.Models;
using BlazorServerWebsite.Data.Providers;
using BlazorServerWebsite.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using XPowerClassLibrary.Users.Models;

namespace BlazorServerWebsite.Pages.Account
{
    public partial class AccountLogInPage : ComponentBase
    {
        [Inject] protected AuthStateProvider AuthStateProvider { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IHttpClientService HttpClientService { get; set; }
        [Inject] protected ILogger<AccountLogInPage> Logger { get; set; }

        private AccountLogInModel _model;
        private EditContext _editContext;
        private string _message = string.Empty;

        protected override void OnInitialized()
        {
            InitializeNewContext();
        }

        private async Task OnValidForm_AuthenticateAccountLogInAsync()
        {
            _message = string.Empty;

            if (!_model.IsValidForm())
            {
                _message = "En eller flere felter er ikke gyldige.";
                return;
            }

            var authRequest = new AuthenticateRequest
            {
                Username = _model.EmailAddress,
                Password = _model.Password
            };
            var authenticationResponse = await HttpClientService.AuthenticateAsync(authRequest);

            if (authenticationResponse is not null)
            {
                Logger.LogInformation($"We Success Boyyysss.");
                Logger.LogInformation($"Auth User: {authenticationResponse.UserObject.Id}");

                _message = "Logged in success!";
                await AuthStateProvider.MarkUserAsAuthenticated(authenticationResponse);

                InitializeNewContext();

                NavigationManager.NavigateTo("/");
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

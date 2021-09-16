using BlazorServerWebsite.Data.Models;
using BlazorServerWebsite.Data.Providers;
using BlazorServerWebsite.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using XPowerClassLibrary.Users.Models;

namespace BlazorServerWebsite.Pages.Account
{
    public partial class AccountRegisterUserPage : ComponentBase
    {
        [Inject] protected AuthStateProvider AuthStateProvider { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IHttpClientService HttpClientService { get; set; }
        [Inject] protected ILogger<AccountRegisterUserPage> Logger { get; set; }

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

            if (!_model.IsValidForm())
            {
                _message = "En eller flere felter er ikke gyldige.";
                return;
            }

            var createUserRequest = new CreateUserRequest
            {
                Mail = _model.EmailAddress,
                Username = _model.Username,
                Password = _model.Password
            };
            var userObj = await HttpClientService.CreateUserAsync(createUserRequest);

            if (userObj is not null)
            {
                Logger.LogInformation($"{userObj.Mail} is now created!");
                Logger.LogInformation("Logging in automatically...");

                var authRequest = new AuthenticateRequest
                {
                    Username = userObj.Mail,
                    Password = _model.Password
                };
                var authenticationResponse = await HttpClientService.AuthenticateAsync(authRequest);

                if (authenticationResponse is not null)
                {
                    _message = "Logged in success, redirecting!";
                    await AuthStateProvider.MarkUserAsAuthenticated(authenticationResponse);

                    InitializeNewContext();

                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    Logger.LogInformation("Given user after creation was null.");

                    _message = "Kunne ikke logge ind, venglist log ind manuelt.";
                    NavigationManager.NavigateTo("/account/login");
                }
            }
            else
            {
                _message = "Fejl, kunne ikke oprette bruger.";
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

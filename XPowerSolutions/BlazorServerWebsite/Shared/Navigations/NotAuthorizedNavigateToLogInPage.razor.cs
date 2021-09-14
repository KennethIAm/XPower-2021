using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerWebsite.Shared.Navigations
{
    public partial class NotAuthorizedNavigateToLogInPage : ComponentBase
    {
        [Inject] protected NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            NavigationManager.NavigateTo("/account/login");
        }
    }
}

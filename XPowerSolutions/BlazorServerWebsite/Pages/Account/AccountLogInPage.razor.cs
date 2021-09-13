using BlazorServerWebsite.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerWebsite.Pages.Account
{
    public partial class AccountLogInPage : ComponentBase
    {
        private AccountLogInModel _model;
        private EditContext _editContext;
        private string _message = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            InitializeNewContext();

            await base.OnInitializedAsync();
        }

        private async Task OnValidForm_AuthenticateAccountLogInAsync()
        {
            Console.WriteLine($"EMail: {_model.EmailAddress, -10} | {_model.Password, -10}");

            _message = "Account logged in!";

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

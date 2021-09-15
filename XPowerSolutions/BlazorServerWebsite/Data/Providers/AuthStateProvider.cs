using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using XPowerClassLibrary.Users.Models;

namespace BlazorServerWebsite.Data.Providers
{
    class ErrorMessage
    {
        public string Message { get; set; }
    }

    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ApiSettings _apiSettings;
        
        public AuthStateProvider(ILocalStorageService localStorage, ApiSettings apiSettings)
        {
            _localStorage = localStorage;
            _apiSettings = apiSettings;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await _localStorage.GetItemAsync<string>(_apiSettings.RefreshTokenKey);

            if (TokenIsEmptyOrWhiteSpace(token))
            {
                return new AuthenticationState(
                    new ClaimsPrincipal(
                        new ClaimsIdentity()));
            }

            ClaimsPrincipal principal = new(GetClaimsIdentity(token));

            return await Task.FromResult(new AuthenticationState(principal));

            /* OLD */
            //AuthenticateResponse auth;
            //ClaimsIdentity identity;
            //if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrEmpty(token))
            //{
            //    // Get user from API.
            //    var cookieContainer = new CookieContainer();
            //    using var handler = new HttpClientHandler { CookieContainer = cookieContainer };
            //    using var client = new HttpClient(handler) { BaseAddress = new Uri(_apiSettings.BaseEndpoint) };

            //    cookieContainer.Add(client.BaseAddress, new Cookie(_apiSettings.RefreshTokenKey, token));
            //    var result = await client.PostAsJsonAsync<string>(_apiSettings.RefreshTokenEndpoint, token);

            //    Console.WriteLine($"Server Response Code from Auth: {result.StatusCode}");

            //    if (result.IsSuccessStatusCode)
            //    {
            //        Console.WriteLine($"Refreshed Token success.");

            //        auth = await result.Content.ReadFromJsonAsync<AuthenticateResponse>();

            //        await _localStorage.SetItemAsync(_apiSettings.RefreshTokenKey, auth.RefreshToken);

            //        identity = GetClaimsIdentity(auth);
            //    }
            //    else
            //    {
            //        var message = await result.Content.ReadFromJsonAsync<ErrorMessage>();

            //        Console.WriteLine($"Server Message: {message.Message}");

            //        await MarkUserAsLoggedOut();
            //        identity = new();
            //    }
            //}
            //else
            //{
            //    identity = new();
            //}

            //ClaimsPrincipal principal = new(identity);

            //return await Task.FromResult(new AuthenticationState(principal));
        }

        public async Task MarkUserAsAuthenticated(AuthenticateResponse authenticateResponse)
        {
            if (string.IsNullOrEmpty(authenticateResponse.JwtToken) || string.IsNullOrEmpty(authenticateResponse.RefreshToken))
            {
                throw new NullReferenceException("Couldn't authenticate user.");
            }

            await _localStorage.SetItemAsync(_apiSettings.JwtKey, authenticateResponse.JwtToken);
            await _localStorage.SetItemAsync(_apiSettings.RefreshTokenKey, authenticateResponse.RefreshToken);

            ClaimsIdentity identity = GetClaimsIdentity(authenticateResponse);

            ClaimsPrincipal principal = new(identity);

            NotifyAuthStateChanged(principal);
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.RemoveItemAsync(_apiSettings.JwtKey);
            await _localStorage.RemoveItemAsync(_apiSettings.RefreshTokenKey);

            ClaimsIdentity identity = new();

            ClaimsPrincipal principal = new(identity);

            NotifyAuthStateChanged(principal);
        }

        public void NotifyAuthStateChanged(ClaimsPrincipal user)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private static bool TokenIsEmptyOrWhiteSpace(string token)
        {
            return string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token);
        }

        private static ClaimsIdentity GetClaimsIdentity(AuthenticateResponse authenticateResponse)
        {
            // Initialize new identity.
            ClaimsIdentity identity = new();

            // Check if the object isn't initialized.
            if (authenticateResponse != null)
            {
                // Check if the obj is correct.
                if (authenticateResponse.UserObject.Id != 0)
                {
                    List<Claim> identityClaims = new()
                    {
                        new Claim(ClaimTypes.NameIdentifier, authenticateResponse.UserObject.Id.ToString()),
                        new Claim(ClaimTypes.Authentication, bool.TrueString),
                        new Claim(ClaimTypes.Name, authenticateResponse.UserObject.Username),
                        new Claim(ClaimTypes.Email, authenticateResponse.UserObject.Mail)
                    };

                    identity = new ClaimsIdentity(
                        claims: identityClaims,
                        authenticationType: "Bearer Token");
                }

            }
            return identity;
        }

        private static ClaimsIdentity GetClaimsIdentity(string token)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Authentication, token)
            };

            ClaimsIdentity identity = new(
                claims: claims,
                authenticationType: "Bearer Token");

            return identity;
        }
    }
}

using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XPowerClassLibrary.Users.Models;

namespace BlazorServerWebsite.Data.Providers
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ApiSettings _apiSettings;
        private readonly string _jwtToken = "JwtToken";
        private readonly string _refreshToken = "RefreshToken";

        public AuthStateProvider(ILocalStorageService localStorage, ApiSettings apiSettings)
        {
            _localStorage = localStorage;
            _apiSettings = apiSettings;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsIdentity identity;

            AuthenticateResponse auth;

            string token = await _localStorage.GetItemAsync<string>(_refreshToken);

            if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrEmpty(token))
            {
                // Get user from API.
                var cookieContainer = new CookieContainer();
                using var handler = new HttpClientHandler { CookieContainer = cookieContainer };
                using var client = new HttpClient(handler) { BaseAddress = new Uri(_apiSettings.BaseEndpoint) };

                cookieContainer.Add(client.BaseAddress, new Cookie(_apiSettings.RefreshTokenKey, token));
                var result = await client.PostAsJsonAsync<string>(_apiSettings.RefreshTokenEndpoint, token);

                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Refreshed Token success.");

                    auth = await result.Content.ReadFromJsonAsync<AuthenticateResponse>();

                    await _localStorage.SetItemAsync(_refreshToken, auth.RefreshToken);

                    identity = GetClaimsIdentity(auth);
                }
                else
                {
                    await MarkUserAsLoggedOut();
                    identity = new();
                }
            }
            else
            {
                identity = new();
            }

            ClaimsPrincipal principal = new(identity);

            return await Task.FromResult(new AuthenticationState(principal));
        }

        public async Task MarkUserAsAuthenticated(AuthenticateResponse authenticateResponse)
        {
            if (string.IsNullOrEmpty(authenticateResponse.JwtToken) || string.IsNullOrEmpty(authenticateResponse.RefreshToken))
            {
                throw new NullReferenceException("Couldn't authenticate user.");
            }

            await _localStorage.SetItemAsync(_jwtToken, authenticateResponse.JwtToken);
            await _localStorage.SetItemAsync(_refreshToken, authenticateResponse.RefreshToken);

            ClaimsIdentity identity = GetClaimsIdentity(authenticateResponse);

            ClaimsPrincipal principal = new(identity);

            NotifyAuthStateChanged(principal);
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.RemoveItemAsync(_jwtToken);
            await _localStorage.RemoveItemAsync(_refreshToken);

            ClaimsIdentity identity = new();

            ClaimsPrincipal principal = new(identity);

            NotifyAuthStateChanged(principal);
        }

        public void NotifyAuthStateChanged(ClaimsPrincipal user)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public ClaimsIdentity GetClaimsIdentity(AuthenticateResponse authenticateResponse)
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
    }
}

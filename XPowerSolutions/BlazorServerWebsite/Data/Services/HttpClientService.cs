using BlazorServerWebsite.Data.Settings;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using XPowerClassLibrary.Users.Entities;
using XPowerClassLibrary.Users.Models;

namespace BlazorServerWebsite.Data.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _client;
        private readonly ISettings _settings;

        public HttpClientService(IHttpClientFactory factory, ISettings settings)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri(settings.Endpoints.BaseEndpoint);
            _settings = settings;
        }

        public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request)
        {
            var endpoint = $"{_settings.Endpoints.BaseEndpoint}{_settings.Endpoints.AuthenticateEndpoint}";
            var requestMessage = GetHttpRequest(HttpMethod.Post, endpoint);

            var responseMessage = await _client.PostAsJsonAsync(
                requestMessage.RequestUri, 
                request);

            if (responseMessage.IsSuccessStatusCode)
            {
                var authenticationResponse = await responseMessage.Content.ReadFromJsonAsync<AuthenticateResponse>();
                return await Task.FromResult(authenticationResponse);
            }

            return null;
        }

        public async Task<IUser> CreateUserAsync(CreateUserRequest request)
        {
            var endpoint = $"{_settings.Endpoints.BaseEndpoint}{_settings.Endpoints.CreateUserEndpoint}";
            var requestMessage = GetHttpRequest(HttpMethod.Post, endpoint);
            var responseMessage = await _client.PostAsJsonAsync(requestMessage.RequestUri, request);

            if (responseMessage.IsSuccessStatusCode)
            {
                var userCreated = await responseMessage.Content.ReadFromJsonAsync<User>();
                return await Task.FromResult(userCreated);
            }

            return null;
        }

        private HttpRequestMessage GetHttpRequest(HttpMethod method, string endpoint)
        {
            return new HttpRequestMessage(
                method, new Uri(endpoint));
        }
    }
}

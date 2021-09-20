using System.Threading.Tasks;
using XPowerClassLibrary.Users.Models;

namespace BlazorServerWebsite.Data.Services
{
    public interface IHttpClientService
    {
        public Task<IUser> CreateUserAsync(CreateUserRequest request);
        public Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request);
    }
}

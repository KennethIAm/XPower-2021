namespace BlazorServerWebsite.Data.Settings
{
    public interface ISettings
    {
        string CookieName { get; }
        string JwtKey { get; }
        string RefreshTokenKey { get; }
        EndpointSettings Endpoints { get; }
    }
}
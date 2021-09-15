namespace BlazorServerWebsite.Data
{
    public class ApiSettings
    {
        public string JwtKey { get; set; }
        public string RefreshTokenKey { get; set; }
        public string CookieName { get; set; }
        public string BaseEndpoint { get; set; }
        public string AuthenticateEndpoint { get; set; }
        public string CreateUserEndpoint { get; set; }
        public string RefreshTokenEndpoint { get; set; }
        public string LogoutEndpoint { get; set; }
    }
}

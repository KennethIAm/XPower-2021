using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace XPowerAPI.Entities
{
    public interface IUser
    {
        int Id { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        List<RefreshToken> RefreshTokens { get; set; }
    }

    public class User : IUser
    {
        public int Id { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
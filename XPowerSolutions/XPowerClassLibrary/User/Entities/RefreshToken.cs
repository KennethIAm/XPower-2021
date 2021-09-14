using Dapper.Contrib.Extensions;
using System;

namespace XPowerAPI.Entities
{
    [Table("RefreshToken")]
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }

        [Write(false)]
        public bool IsExpired => DateTime.UtcNow >= Expires;
        
        [Write(false)]
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
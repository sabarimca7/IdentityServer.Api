using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Password { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? EmailAddress { get; set; }
        public int? AccessFailedCount { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public int? OAuthClientId { get; set; }
        public virtual ICollection<UserClientScope> UserClientScopes { get; set; } = new List<UserClientScope>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}

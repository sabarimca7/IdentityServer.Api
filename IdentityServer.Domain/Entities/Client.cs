using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Entities
{
    public class Client
    {
        public int OAuthClientId { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public string? Name { get; set; }
        public int? GrantTypeId { get; set; }
        public int? ApplicationId { get; set; }
        public string? PublicKey { get; set; }
        public string? PrivateKey { get; set; }
        public int? AccessTokenValidity { get; set; }
        public string? SignatureKey { get; set; }
        public int? SignatureVality { get; set; }

        // Navigation properties
        public virtual GrantType? GrantType { get; set; }
        public virtual ICollection<UserClientScope> UserClientScopes { get; set; } = new List<UserClientScope>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}

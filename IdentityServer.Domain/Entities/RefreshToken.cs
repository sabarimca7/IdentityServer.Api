using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Entities
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }
        public string? Token { get; set; }
        public int? UserId { get; set; }
        public int? OAuthClientId { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public bool? IsRevoked { get; set; }
        public string? CreatedOn { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual Client? Client { get; set; }
    }
}

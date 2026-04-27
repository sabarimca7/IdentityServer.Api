using System;

namespace IdentityServer.Domain.Entities
{
    public class AuthorizationCode
    {
        public string Code { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public int? OAuthClientId { get; set; }
        public string? RedirectUri { get; set; }
        public string? Scope { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsUsed { get; set; }
    }
}

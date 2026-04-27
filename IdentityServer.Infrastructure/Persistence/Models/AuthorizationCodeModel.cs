using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.Infrastructure.Persistence.Models;

[Table("AuthorizationCode", Schema = "OAuth")]
public class AuthorizationCodeModel
{
    [Key]
    public int AuthorizationCodeId { get; set; }

    public string Code { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public int? OAuthClientId { get; set; }
    public string? RedirectUri { get; set; }
    public string? Scope { get; set; }
    public DateTime ExpiresOn { get; set; }
    public bool IsUsed { get; set; }
}

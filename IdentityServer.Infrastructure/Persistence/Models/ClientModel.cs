using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.Infrastructure.Persistence.Models;

[Table("Client", Schema = "OAuth")]
public class ClientModel
{
    [Key]
    public int OAuthClientId { get; set; }

    [Required]
    [StringLength(300)]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string ClientSecret { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }
    public bool IsActive { get; set; }

    [StringLength(250)]
    public string? Name { get; set; }

    public int? GrantTypeId { get; set; }
    public int? ApplicationId { get; set; }
    public string? PublicKey { get; set; }
    public string? PrivateKey { get; set; }
    public int? AccessTokenValidity { get; set; }
    public string? SignatureKey { get; set; }
    public int? SignatureVality { get; set; }

    // Navigation properties
    [ForeignKey("GrantTypeId")]
    public virtual GrantTypeModel? GrantType { get; set; }

    public virtual ICollection<UserClientScopeModel> UserClientScopes { get; set; } = new List<UserClientScopeModel>();
    public virtual ICollection<RefreshTokenModel> RefreshTokens { get; set; } = new List<RefreshTokenModel>();
}
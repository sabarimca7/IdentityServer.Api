using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.Infrastructure.Persistence.Models;

[Table("RefreshToken", Schema = "OAuth")]
public class RefreshTokenModel
{
    [Key]
    public int RefreshTokenId { get; set; }

    public string? Token { get; set; }
    public int? UserId { get; set; }
    public int? OAuthClientId { get; set; }
    public DateTime? ExpiresOn { get; set; }
    public bool? IsRevoked { get; set; }

    [StringLength(10)]
    public string? CreatedOn { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual UserModel? User { get; set; }

    [ForeignKey("OAuthClientId")]
    public virtual ClientModel? Client { get; set; }
}
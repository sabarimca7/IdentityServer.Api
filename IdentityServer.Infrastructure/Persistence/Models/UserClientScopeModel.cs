using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.Infrastructure.Persistence.Models;

[Table("UserClientScope", Schema = "OAuth")]
public class UserClientScopeModel
{
    [Key]
    public int UserClientScopeId { get; set; }

    public int OAuthClientId { get; set; }
    public int ScopeId { get; set; }
    public int? UserId { get; set; }
    public bool? ScopeGranted { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }

    // Navigation properties
    [ForeignKey("OAuthClientId")]
    public virtual ClientModel Client { get; set; } = null!;

    [ForeignKey("ScopeId")]
    public virtual ScopeModel Scope { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual UserModel? User { get; set; }
}
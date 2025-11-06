using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.Infrastructure.Persistence.Models;

[Table("User", Schema = "OAuth")]
public class UserModel
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [StringLength(255)]
    public string Username { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Password { get; set; }

    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }

    [StringLength(255)]
    public string? Firstname { get; set; }

    [StringLength(255)]
    public string? Lastname { get; set; }

    [StringLength(255)]
    public string? EmailAddress { get; set; }

    public int? AccessFailedCount { get; set; }
    public DateTime? LockoutEnd { get; set; }

    // Navigation properties
    public virtual ICollection<UserClientScopeModel> UserClientScopes { get; set; } = new List<UserClientScopeModel>();
    public virtual ICollection<RefreshTokenModel> RefreshTokens { get; set; } = new List<RefreshTokenModel>();
}
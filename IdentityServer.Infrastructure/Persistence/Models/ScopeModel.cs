using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.Infrastructure.Persistence.Models;

[Table("Scope", Schema = "OAuth")]
public class ScopeModel
{
    [Key]
    public int ScopeId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string ShortDescription { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<UserClientScopeModel> UserClientScopes { get; set; } = new List<UserClientScopeModel>();
}
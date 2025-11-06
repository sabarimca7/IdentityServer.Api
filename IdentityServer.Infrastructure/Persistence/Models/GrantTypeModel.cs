using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.Infrastructure.Persistence.Models;

[Table("GrantType", Schema = "OAuth")]
public class GrantTypeModel
{
    [Key]
    public int GrantTypeId { get; set; }

    [Required]
    [StringLength(300)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string ShortDescription { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<ClientModel> Clients { get; set; } = new List<ClientModel>();
}
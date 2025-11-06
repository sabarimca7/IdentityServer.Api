using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.Infrastructure.Persistence.Models;

[Table("AppSetting", Schema = "OAuth")]
public class AppSettingModel
{
    [Key]
    public int AppSettingId { get; set; }

    [StringLength(255)]
    public string? AppSettingKey { get; set; }

    public string? AppSettingValue { get; set; }
    public bool? DecryptionRequired { get; set; }
}
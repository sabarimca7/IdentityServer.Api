using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.Infrastructure.Persistence.Models;

[Table("ExceptionLog", Schema = "OAuth")]
public class ExceptionLogModel
{
    [Key]
    public int ExceptionLogId { get; set; }

    [StringLength(255)]
    public string? ExceptionSource { get; set; }

    [StringLength(255)]
    public string? ExceptionType { get; set; }

    [StringLength(2500)]
    public string? ExceptionMessage { get; set; }

    public string? StackTrace { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? UserId { get; set; }

    [StringLength(255)]
    public string? ClientId { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.Infrastructure.Persistence.Models;

[Table("RequestResponseLog", Schema = "OAuth")]
public class RequestResponseLogModel
{
    [Key]
    public int RequestResponseLogId { get; set; }

    [StringLength(255)]
    public string? MachineName { get; set; }

    [StringLength(255)]
    public string? RequestContentType { get; set; }

    [StringLength(255)]
    public string? RequestIpAddress { get; set; }

    public string? RequestContentBody { get; set; }

    [StringLength(1500)]
    public string? RequestUri { get; set; }

    [StringLength(255)]
    public string? RequestMethod { get; set; }

    [StringLength(255)]
    public string? RequestRouteTemplate { get; set; }

    public string? RequestRouteDate { get; set; }
    public string? RequestHeader { get; set; }
    public DateTime? RequestDateTime { get; set; }

    [StringLength(255)]
    public string? ResponseContentType { get; set; }

    public string? ResponseContentBody { get; set; }
    public int? ResponseStatusCode { get; set; }
    public string? ResponseHeader { get; set; }
    public DateTime? ResponseDateTime { get; set; }
    public DateTime? ClientTimeStamp { get; set; }
    public int? UserId { get; set; }

    [StringLength(255)]
    public string? ClientId { get; set; }
}
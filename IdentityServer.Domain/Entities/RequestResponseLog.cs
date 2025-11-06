using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Entities
{
    public class RequestResponseLog
    {
        public int RequestResponseLogId { get; set; }
        public string? MachineName { get; set; }
        public string? RequestContentType { get; set; }
        public string? RequestIpAddress { get; set; }
        public string? RequestContentBody { get; set; }
        public string? RequestUri { get; set; }
        public string? RequestMethod { get; set; }
        public string? RequestRouteTemplate { get; set; }
        public string? RequestRouteDate { get; set; }
        public string? RequestHeader { get; set; }
        public DateTime? RequestDateTime { get; set; }
        public string? ResponseContentType { get; set; }
        public string? ResponseContentBody { get; set; }
        public int? ResponseStatusCode { get; set; }
        public string? ResponseHeader { get; set; }
        public DateTime? ResponseDateTime { get; set; }
        public DateTime? ClientTimeStamp { get; set; }
        public int? UserId { get; set; }
        public string? ClientId { get; set; }
    }
}

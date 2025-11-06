using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Entities
{
    public class ExceptionLog
    {
        public int ExceptionLogId { get; set; }
        public string? ExceptionSource { get; set; }
        public string? ExceptionType { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? StackTrace { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UserId { get; set; }
        public string? ClientId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Application.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? EmailAddress { get; set; }
        public int? AccessFailedCount { get; set; }
        public DateTime? LockoutEnd { get; set; }
    }

}

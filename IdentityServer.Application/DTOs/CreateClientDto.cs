using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Application.DTOs
{
    public class CreateClientDto
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string? Name { get; set; }
        public int? GrantTypeId { get; set; }
        public int? ApplicationId { get; set; }
        public int? AccessTokenValidity { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

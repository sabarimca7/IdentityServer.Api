using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Application.DTOs
{
    public class UpdateClientDto
    {
        public int OAuthClientId { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public int? AccessTokenValidity { get; set; }
    }
}

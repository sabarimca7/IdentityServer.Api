using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IdentityServer.Application.DTOs
{
    public class CreateUserDto
    {
        //[JsonPropertyName("clientId")]
        //public string ClientId { get; set; } = string.Empty;
        public int OAuthClientId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
       
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? EmailAddress { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

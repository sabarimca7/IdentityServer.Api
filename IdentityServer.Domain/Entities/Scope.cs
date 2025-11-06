using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Entities
{
    public class Scope
    {
        public int ScopeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation properties
        public virtual ICollection<UserClientScope> UserClientScopes { get; set; } = new List<UserClientScope>();
    }
}

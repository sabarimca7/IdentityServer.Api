using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Entities
{
    public class UserClientScope
    {
        public int UserClientScopeId { get; set; }
        public int OAuthClientId { get; set; }
        public int ScopeId { get; set; }
        public int? UserId { get; set; }
        public bool? ScopeGranted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        // Navigation properties
        public virtual Client Client { get; set; } = null!;
        public virtual Scope Scope { get; set; } = null!;
        public virtual User? User { get; set; }
    }
}

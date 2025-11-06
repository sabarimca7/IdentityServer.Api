using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Entities
{
    public class GrantType
    {
        public int GrantTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
    }
}

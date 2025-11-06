using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Application.DTOs
{
    public class ScopeDto
    {

        /// <summary>
        /// Unique identifier for the scope
        /// </summary>
        public int ScopeId { get; set; }

        /// <summary>
        /// Name of the scope (e.g., "read", "write", "admin")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Short description of what the scope allows
        /// </summary>
        public string ShortDescription { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the scope's purpose and permissions
        /// </summary>
        public string? Description { get; set; }
    }
}

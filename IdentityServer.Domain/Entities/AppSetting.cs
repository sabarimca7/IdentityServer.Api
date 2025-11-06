using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Entities
{
    public class AppSetting
    {
        public int AppSettingId { get; set; }
        public string? AppSettingKey { get; set; }
        public string? AppSettingValue { get; set; }
        public bool? DecryptionRequired { get; set; }
    }
}

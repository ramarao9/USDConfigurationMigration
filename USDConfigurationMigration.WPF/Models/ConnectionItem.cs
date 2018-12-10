using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigration.WPF.Models
{
    public class ConnectionItem
    {
        public string OrganizationURL { get; set; }

        public string AuthType { get; set; }

        public string Domain { get; set; }

        public string UserName { get; set; }

        public SecureString Password { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigration.WPF.Models
{
    public class CrmConnectionInfo
    {

        public Guid OrganizationId { get; set; }

        public Guid UserId { get; set; }
        public string OrgDisplayName { get; set; }

        public string OrgUniqueName { get; set; }

        public string OrganizationURL { get; set; }

        public string AuthType { get; set; }

        public string Domain { get; set; }

    }
}

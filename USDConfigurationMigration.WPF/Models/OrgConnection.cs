using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigration.WPF.Models
{
    public class OrgConnection
    {
        public CrmServiceClient CrmServiceClient { get; set; }

        public CrmConnectionInfo CrmConnectionInfo { get; set; }

    }
}

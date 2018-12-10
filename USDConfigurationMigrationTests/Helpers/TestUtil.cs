using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigrationTests.Helpers
{
    public static class TestUtil
    {

        public static CrmServiceClient GetSourceCRMClient()
        {
            string connectionStr = ConfigurationManager.ConnectionStrings["CRMSource"].ConnectionString;
            CrmServiceClient crmServiceClient = new CrmServiceClient(connectionStr);
            return crmServiceClient;
        }

    }
}

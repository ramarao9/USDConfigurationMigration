using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDConfigurationMigration.Models;

namespace USDConfigurationMigration.Helpers
{
    public static  class USDConfigurationExtensions
    {
        public static CRMEntity GetCRMEntity(this USDConfiguration usdConfiguration,string entityLogicalName)
        {
            if (usdConfiguration == null || usdConfiguration.CRMEntities==null)
                return null;

            CRMEntity crmEntity = usdConfiguration.CRMEntities.FirstOrDefault(x => x.LogicalName == entityLogicalName);
            return crmEntity;
        }

    }
}

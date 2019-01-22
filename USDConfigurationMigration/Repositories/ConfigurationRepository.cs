using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using USDConfigurationMigration.Helpers;
using System.Collections.Generic;

using USDConfigurationMigration.Models;
using System;

namespace USDConfigurationMigration.Repositories
{
    public class ConfigurationRepository
    {

        public CRMEntity GetConfigurationEntity(IOrganizationService service, string configurationName)
        {

            QueryExpression qe = new QueryExpression("msdyusd_configuration");
            qe.ColumnSet = new ColumnSet(true);
            qe.Criteria.AddCondition("msdyusd_name", ConditionOperator.Equal, configurationName);


            EntityCollection results = service.RetrieveMultiple(qe);

            if (results == null || results.Entities == null || results.Entities.Count != 1)
                return null;

            Entity configuration = results.Entities[0];
            CRMRecord crmConfigurationRecord = configuration.ToCRMRecord();

            CRMEntity crmEntity = new CRMEntity
            {
                LogicalName = "msdyusd_configuration",
                CRMRecords = new List<CRMRecord> { crmConfigurationRecord },
                RecordCount = 1
            };

            return crmEntity;
        }


        public ImportResult ImportConfiguration(IOrganizationService targetCrmService, USDConfiguration sourceUsdConfiguration, USDConfiguration targetUSDConfiguration)
        {
            ImportResult importresult = new ImportResult();
            importresult.EntityLogicalName = "msdyusd_configuration";

            CRMEntity sourceConfigurationCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_configuration");
            CRMEntity targetConfigurationCE = targetUSDConfiguration.GetCRMEntity("msdyusd_configuration");

            Entity sourceConfigurationEnt = sourceConfigurationCE.CRMRecords[0].ToEntity(null);

            if (targetConfigurationCE == null)
            {
                targetCrmService.Create(sourceConfigurationEnt);
                importresult.CreateCount = 1;
            }
            else
            {
                Entity targetConfigurationEnt = targetConfigurationCE.CRMRecords[0].ToEntity(null);

                Entity modifiedEntity = sourceConfigurationEnt.GetModifiedEntity(targetConfigurationEnt);

                targetCrmService.Update(modifiedEntity);

                importresult.UpdateCount = 1;
            }

            importresult.TotalProcessed = 1;
            importresult.SuccessCount = 1;
            importresult.EndedOn = DateTime.Now;

            return importresult;
        }

    }
}

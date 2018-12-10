

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System.Collections.Generic;
using USDConfigurationMigration.WPF.Models;

namespace USDConfigurationMigration.WPF.Repositories
{
    public class ConfigurationRepository
    {

        public ConfigurationRepository()
        {

        }


        public List<ConfigurationItem> GetConfigurations(CrmServiceClient crmService)
        {

            List<ConfigurationItem> configurationItems = new List<ConfigurationItem>();

            QueryExpression configurationQuery = new QueryExpression("msdyusd_configuration");
            configurationQuery.ColumnSet = new ColumnSet("msdyusd_name");
            configurationQuery.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);



            EntityCollection results = crmService.RetrieveMultiple(configurationQuery);

            if (results != null && results.Entities != null && results.Entities.Count > 0)
            {
                foreach (Entity configuration in results.Entities)
                {

                    string name = configuration.GetAttributeValue<string>("msdyusd_name");
                    configurationItems.Add(new ConfigurationItem { Name = name, Id = configuration.Id });
                }
            }

            return configurationItems;
        }
    }
}

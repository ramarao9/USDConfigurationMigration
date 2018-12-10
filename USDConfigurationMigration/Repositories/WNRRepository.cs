using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using USDConfigurationMigration.Models;
using System;

namespace USDConfigurationMigration.Repositories
{
    public class WNRRepository : BaseRepository
    {

        public CRMEntity GetWindowNavigationRules(IOrganizationService service, EntityReference configuration)
        {
            string fetchXML = GetWindowNavigationRulesFetchXML(configuration);

            CRMEntity crmEntity = GetCRMEntity(service, fetchXML);

            return crmEntity;
        }


        public string GetWindowNavigationRulesFetchXML(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='msdyusd_windowroute'>
                        <all-attributes />
                        <link-entity name='msdyusd_configuration_windowroute' from='msdyusd_windowrouteid' to='msdyusd_windowrouteid' visible='false' intersect='true'>
                          <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='ap'>
                            <filter type='and'>
                              <condition attribute='msdyusd_configurationid' operator='eq' uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                            </filter>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
        }



        public ImportResult ImportWindowNavigationRules(IOrganizationService targetCrmService, CRMEntity sourceWindowNavigationRulesCE,
                                     CRMEntity targetWindowNavigationRulesCE, LookupMatchCriteria windowNavigationRulesMatchCriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceWindowNavigationRulesCE,
                                           targetWindowNavigationRulesCE,
                                           windowNavigationRulesMatchCriteria);
            return importResult;
        }


        public ImportResult ImportConfigurationWindowNavigationRules(IOrganizationService targetCrmService,
                                                     Guid configurationId,
                                                     CRMEntity sourceWindowNavigationRulesCE,
                                                     CRMEntity targetWindowNavigationRulesCE)
        {

            string configurationWindowNavigationRuleIntersectName = "msdyusd_configuration_windowroute";

            ImportResult configurationWindowNavigationRuleImportResult = ImportConfigurationIntersectEntity(targetCrmService,
                                                                                            configurationId,
                                                                                            sourceWindowNavigationRulesCE,
                                                                                            targetWindowNavigationRulesCE,
                                                                                            configurationWindowNavigationRuleIntersectName);
            return configurationWindowNavigationRuleImportResult;
        }



    }
}

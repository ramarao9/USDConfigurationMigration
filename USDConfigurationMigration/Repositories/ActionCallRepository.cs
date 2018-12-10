using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using USDConfigurationMigration.Models;
using System;

namespace USDConfigurationMigration.Repositories
{
    public class ActionCallRepository : BaseRepository
    {


        public CRMEntity GetActionCalls(IOrganizationService service, EntityReference configuration)
        {
            string fetchXML = GetActionCallsFetchXML(configuration);

            CRMEntity crmEntity = GetCRMEntity(service, fetchXML);

            return crmEntity;
        }


        public string GetActionCallsFetchXML(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='msdyusd_agentscriptaction'>
                        <all-attributes />
                        <link-entity name='msdyusd_configuration_actioncalls' from='msdyusd_agentscriptactionid' to='msdyusd_agentscriptactionid' visible='false' intersect='true'>
                          <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='ar'>
                            <filter type='and'>
                              <condition attribute='msdyusd_configurationid' operator='eq' uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                            </filter>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
        }



        public ImportResult ImportActionCalls(IOrganizationService targetCrmService, CRMEntity sourceActionCallsCE,
                                     CRMEntity targetActionCallsCE, LookupMatchCriteria actionCallsMatchCriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceActionCallsCE,
                                           targetActionCallsCE,
                                           actionCallsMatchCriteria);
            return importResult;
        }


        public ImportResult ImportConfigurationActionCalls(IOrganizationService targetCrmService,
                                                     Guid configurationId,
                                                     CRMEntity sourceActionCallsCE,
                                                     CRMEntity targetActionCallsCE)
        {

            string configurationActionCallIntersectName = "msdyusd_configuration_actioncalls";

            ImportResult configurationSessionLineImportResult = ImportConfigurationIntersectEntity(targetCrmService,
                                                                                            configurationId,
                                                                                            sourceActionCallsCE,
                                                                                            targetActionCallsCE,
                                                                                            configurationActionCallIntersectName);
            return configurationSessionLineImportResult;
        }



    }
}

using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System;

namespace USDConfigurationMigration.Repositories
{
    public class AgentScriptTaskRepository : BaseRepository
    {


        public CRMEntity GetAgentScriptTasks(IOrganizationService service, EntityReference configuration)
        {
            string fetchXML = GetAgentScriptTasksFetchXML(configuration);

            CRMEntity crmEntity = GetCRMEntity(service, fetchXML);

            return crmEntity;
        }


        public string GetAgentScriptTasksFetchXML(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='msdyusd_task'>
                        <all-attributes />
                        <link-entity name='msdyusd_configuration_agentscript' from='msdyusd_taskid' to='msdyusd_taskid' visible='false' intersect='true'>
                          <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='at'>
                            <filter type='and'>
                              <condition attribute='msdyusd_configurationid' operator='eq'  uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                            </filter>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
        }



        public ImportResult ImportAgentScriptTasks(IOrganizationService targetCrmService, CRMEntity sourceAgentScriptTasksCE,
                                     CRMEntity targetAgentScriptTasksCE, LookupMatchCriteria agentScriptTasksMatchCriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceAgentScriptTasksCE,
                                           targetAgentScriptTasksCE,
                                           agentScriptTasksMatchCriteria);
            return importResult;
        }


        public ImportResult ImportConfigurationAgentScriptTasks(IOrganizationService targetCrmService,
                                                     Guid configurationId,
                                                     CRMEntity sourceAgentScriptTasksCE,
                                                     CRMEntity targetAgentScriptTasksCE)
        {

            string configurationAgentScriptTaskIntersectName = "msdyusd_configuration_agentscript";

            ImportResult configurationAgentScriptTaskImportResult = ImportConfigurationIntersectEntity(targetCrmService,
                                                                                            configurationId,
                                                                                            sourceAgentScriptTasksCE,
                                                                                            targetAgentScriptTasksCE,
                                                                                            configurationAgentScriptTaskIntersectName);
            return configurationAgentScriptTaskImportResult;
        }


    }
}

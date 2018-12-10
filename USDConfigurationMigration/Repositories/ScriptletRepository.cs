using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using USDConfigurationMigration.Models;
using System;


namespace USDConfigurationMigration.Repositories
{
    public class ScriptletRepository : BaseRepository
    {




        public CRMEntity GetScriptlets(IOrganizationService service, EntityReference configuration)
        {
            string fetchXml = GetScriptletsFetchXML(configuration);

            CRMEntity crmEntity = GetCRMEntity(service, fetchXml);

            return crmEntity;
        }


        public string GetScriptletsFetchXML(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                        <entity name='msdyusd_scriptlet'>
                        <all-attributes />
                        <link-entity name='msdyusd_configuration_scriptlet' from='msdyusd_scriptletid' to='msdyusd_scriptletid' visible='false' intersect='true'>
                            <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='ad'>
                            <filter type='and'>
                                <condition attribute='msdyusd_configurationid' operator='eq'  uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                            </filter>
                            </link-entity>
                        </link-entity>
                        </entity>
                    </fetch>";
        }



        public ImportResult ImportScriptlets(IOrganizationService targetCrmService, CRMEntity sourceScriptletsCE,
                                     CRMEntity targetScriptletsCE, LookupMatchCriteria scriptletsMatchCriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceScriptletsCE,
                                           targetScriptletsCE,
                                           scriptletsMatchCriteria);
            return importResult;
        }


        public ImportResult ImportConfigurationScriptlet(IOrganizationService targetCrmService,
                                                     Guid configurationId,
                                                     CRMEntity sourceScriptletsCE,
                                                     CRMEntity targetScriptletsCE)
        {

            string configurationScriptletIntersectName = "msdyusd_configuration_scriptlet";

            ImportResult configurationScriptletsImportResult = ImportConfigurationIntersectEntity(targetCrmService,
                                                                                            configurationId,
                                                                                            sourceScriptletsCE,
                                                                                            targetScriptletsCE,
                                                                                            configurationScriptletIntersectName);
            return configurationScriptletsImportResult;
        }


    }
}

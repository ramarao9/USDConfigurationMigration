using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System;


namespace USDConfigurationMigration.Repositories
{
    public class SessionLineRepository : BaseRepository
    {
        public CRMEntity GetSessionLines(IOrganizationService service, EntityReference configuration)
        {
            string fetchXML = GetSessionLinesFetchXML(configuration);

            CRMEntity crmEntity = GetCRMEntity(service, fetchXML);

            return crmEntity;
        }


        public string GetSessionLinesFetchXML(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='msdyusd_sessioninformation'>
                        <all-attributes />
                        <link-entity name='msdyusd_configuration_sessionlines' from='msdyusd_sessioninformationid' to='msdyusd_sessioninformationid' visible='false' intersect='true'>
                          <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='ah'>
                            <filter type='and'>
                              <condition attribute='msdyusd_configurationid' operator='eq' uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                            </filter>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
        }



        public ImportResult ImportSessionLines(IOrganizationService targetCrmService, CRMEntity sourceSessionLinesCE,
                                     CRMEntity targetSessionLinesCE, LookupMatchCriteria sessionLinesMatchCriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceSessionLinesCE,
                                           targetSessionLinesCE,
                                           sessionLinesMatchCriteria);
            return importResult;
        }


        public ImportResult ImportConfigurationSessionLines(IOrganizationService targetCrmService,
                                                     Guid configurationId,
                                                     CRMEntity sourceSessionLinesCE,
                                                     CRMEntity targetSessionLinesCE)
        {

            string configurationSessionLineIntersectName = "msdyusd_configuration_sessionlines";

            ImportResult configurationSessionLineImportResult = ImportConfigurationIntersectEntity(targetCrmService,
                                                                                            configurationId,
                                                                                            sourceSessionLinesCE,
                                                                                            targetSessionLinesCE,
                                                                                            configurationSessionLineIntersectName);
            return configurationSessionLineImportResult;
        }


    }
}

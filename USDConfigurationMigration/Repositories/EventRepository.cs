using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System;

namespace USDConfigurationMigration.Repositories
{
    public class EventRepository : BaseRepository
    {


        public CRMEntity GetEvents(IOrganizationService service, EntityReference configuration)
        {
            string eventsfetchXML = GetEventsFetchXML(configuration);

            CRMEntity crmEntity = GetCRMEntity(service, eventsfetchXML);

            return crmEntity;
        }


        public string GetEventsFetchXML(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='msdyusd_uiievent'>
                        <all-attributes /> 
                        <link-entity name='msdyusd_configuration_event' from='msdyusd_uiieventid' to='msdyusd_uiieventid' visible='false' intersect='true'>
                            <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='ab'>
                                <filter type='and'>
                                    <condition attribute='msdyusd_configurationid' operator='eq'  uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                                </filter>
                            </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
        }



        public ImportResult ImportEvents(IOrganizationService targetCrmService, CRMEntity sourceEventsCE,
                                     CRMEntity targetEventsCE, LookupMatchCriteria eventMatchCriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceEventsCE,
                                           targetEventsCE,
                                           eventMatchCriteria);
            return importResult;
        }


        public ImportResult ImportConfigurationEvent(IOrganizationService targetCrmService,
                                                     Guid configurationId,
                                                     CRMEntity sourceEventsCE,
                                                     CRMEntity targetEventsCE)
        {

            string configurationEventIntersectName = "msdyusd_configuration_event";

            ImportResult configurationEventImportResult = ImportConfigurationIntersectEntity(targetCrmService,
                                                                                            configurationId,
                                                                                            sourceEventsCE,
                                                                                            targetEventsCE,
                                                                                            configurationEventIntersectName);
            return configurationEventImportResult;
        }


    }
}

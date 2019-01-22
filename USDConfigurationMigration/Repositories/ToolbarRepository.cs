using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System;

namespace USDConfigurationMigration.Repositories
{
    public class ToolbarRepository : BaseRepository
    {


        public CRMEntity GetToolbars(IOrganizationService service, EntityReference configuration)
        {
            string fetchXml = GetToolbarsFetchXML(configuration);

            CRMEntity crmEntity = GetCRMEntity(service, fetchXml);

            return crmEntity;
        }


        public string GetToolbarsFetchXML(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='msdyusd_toolbarstrip'>
                        <all-attributes />
                        <link-entity name='msdyusd_configuration_toolbar' from='msdyusd_toolbarstripid' to='msdyusd_toolbarstripid' visible='false' intersect='true'>
                          <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='aa'>
                            <filter type='and'>
                              <condition attribute='msdyusd_configurationid' operator='eq' uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                            </filter>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
        }



        public ImportResult ImportToolbars(IOrganizationService targetCrmService, CRMEntity sourceToolbarsCE,
                                     CRMEntity targetToolbarsCE, LookupMatchCriteria toolbarsMatchCriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceToolbarsCE,
                                           targetToolbarsCE,
                                           toolbarsMatchCriteria);
            return importResult;
        }


        public ImportResult ImportConfigurationToolbar(IOrganizationService targetCrmService,
                                                     Guid configurationId,
                                                     CRMEntity sourceToolbarsCE,
                                                     CRMEntity targetToolbarsCE)
        {

            string configurationToolbarIntersectName = "msdyusd_configuration_toolbar";

            ImportResult configurationToolbarsImportResult = ImportConfigurationIntersectEntity(targetCrmService,
                                                                                            configurationId,
                                                                                            sourceToolbarsCE,
                                                                                            targetToolbarsCE,
                                                                                            configurationToolbarIntersectName);
            return configurationToolbarsImportResult;
        }

    }
}

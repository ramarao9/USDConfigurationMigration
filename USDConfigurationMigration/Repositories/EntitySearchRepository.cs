using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System;

namespace USDConfigurationMigration.Repositories
{
    public class EntitySearchRepository : BaseRepository
    {


        public CRMEntity GetEntitySearches(IOrganizationService service, EntityReference configuration)
        {
            string fetchXML = GetEntitySearchesFetchXML(configuration);

            CRMEntity crmEntity = GetCRMEntity(service, fetchXML);

            return crmEntity;
        }


        public string GetEntitySearchesFetchXML(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='msdyusd_entitysearch'>
                        <all-attributes />
                        <link-entity name='msdyusd_configuration_entitysearch' from='msdyusd_entitysearchid' to='msdyusd_entitysearchid' visible='false' intersect='true'>
                          <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='af'>
                            <filter type='and'>
                              <condition attribute='msdyusd_configurationid' operator='eq'  uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                            </filter>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
        }



        public ImportResult ImportEntitySearches(IOrganizationService targetCrmService, CRMEntity sourceEntitySearchesCE,
                                     CRMEntity targetEntitySearchesCE, LookupMatchCriteria entitySearchesMatchCriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceEntitySearchesCE,
                                           targetEntitySearchesCE,
                                           entitySearchesMatchCriteria);
            return importResult;
        }


        public ImportResult ImportConfigurationEntitySearches(IOrganizationService targetCrmService,
                                                     Guid configurationId,
                                                     CRMEntity sourceEntitySearchesCE,
                                                     CRMEntity targetEntitySearchesCE)
        {

            string configurationEntitySearchIntersectName = "msdyusd_configuration_entitysearch";

            ImportResult configurationEntitySearchesImportResult = ImportConfigurationIntersectEntity(targetCrmService,
                                                                                            configurationId,
                                                                                            sourceEntitySearchesCE,
                                                                                            targetEntitySearchesCE,
                                                                                            configurationEntitySearchIntersectName);
            return configurationEntitySearchesImportResult;
        }


    }
}

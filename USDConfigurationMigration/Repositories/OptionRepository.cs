using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using USDConfigurationMigration.Models;
using System;


namespace USDConfigurationMigration.Repositories
{
    public class OptionRepository : BaseRepository
    {
        public CRMEntity GetOptions(IOrganizationService service, EntityReference configuration)
        {
            string fetchXML = GetOptionsFetchXML(configuration);

            CRMEntity crmEntity = GetCRMEntity(service, fetchXML);

            return crmEntity;
        }


        public string GetOptionsFetchXML(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='uii_option'>
                        <all-attributes /> 
                        <link-entity name='msdyusd_configuration_option' from='uii_optionid' to='uii_optionid' visible='false' intersect='true'>
                          <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='aj'>
                            <filter type='and'>
                              <condition attribute='msdyusd_configurationid' operator='eq' uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                            </filter>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
        }



        public ImportResult ImportOptions(IOrganizationService targetCrmService, CRMEntity sourceOptionsCE,
                                     CRMEntity targetOptionsCE, LookupMatchCriteria optionsMatchCriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceOptionsCE,
                                           targetOptionsCE,
                                           optionsMatchCriteria);
            return importResult;
        }


        public ImportResult ImportConfigurationEntitySearches(IOrganizationService targetCrmService,
                                                     Guid configurationId,
                                                     CRMEntity sourceOptionsCE,
                                                     CRMEntity targetOptionsCE)
        {

            string configurationOptionIntersectName = "msdyusd_configuration_option";

            ImportResult configurationOptionsImportResult = ImportConfigurationIntersectEntity(targetCrmService,
                                                                                            configurationId,
                                                                                            sourceOptionsCE,
                                                                                            targetOptionsCE,
                                                                                            configurationOptionIntersectName);
            return configurationOptionsImportResult;
        }


    }
}

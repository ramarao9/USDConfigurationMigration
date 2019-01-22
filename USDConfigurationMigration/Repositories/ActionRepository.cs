using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System;



namespace USDConfigurationMigration.Repositories
{
    public class ActionRepository : BaseRepository
    {


        public CRMEntity GetActions(IOrganizationService service, EntityReference configuration)
        {
            string fetchXML = GetActionsFetchXML(configuration);

            CRMEntity crmEntity = GetCRMEntity(service, fetchXML);

            return crmEntity;
        }


        public string GetActionsFetchXML(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='uii_action'>
                        <all-attributes />
                        <link-entity name='uii_hostedapplication' from='uii_hostedapplicationid' to='uii_hostedapplicationid' alias='am'>
                          <link-entity name='msdyusd_configuration_hostedcontrol' from='uii_hostedapplicationid' to='uii_hostedapplicationid' visible='false' intersect='true'>
                            <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='an'>
                              <filter type='and'>
                                <condition attribute='msdyusd_configurationid' operator='eq' uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                              </filter>
                            </link-entity>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
        }



        public ImportResult ImportActions(IOrganizationService targetCrmService, CRMEntity sourceActionsCE,
                         CRMEntity targetActionsCE, LookupMatchCriteria actionsMatchCriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceActionsCE,
                                           targetActionsCE,
                                           actionsMatchCriteria);
            return importResult;
        }

    }
}

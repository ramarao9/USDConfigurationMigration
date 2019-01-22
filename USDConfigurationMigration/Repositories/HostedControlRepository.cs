using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using USDConfigurationMigration.Models;
using System.Linq;
using MayoEnterprise.Libraries.Xrm;
using System;
using USDConfigurationMigration.Helpers;

using Microsoft.Xrm.Sdk.Messages;

namespace USDConfigurationMigration.Repositories
{
    public class HostedControlRepository : BaseRepository
    {


        public CRMEntity GetHostedControls(IOrganizationService service, EntityReference configuration)
        {
            string hostedControlsfetchXML = GetHostedControlsFetchXML(configuration);

            CRMEntity crmEntity = GetCRMEntity(service, hostedControlsfetchXML);

            return crmEntity;
        }


        public CRMEntity GetConfigurationHostedControls(IOrganizationService service, EntityReference configuration)
        {
            string hostedControlsfetchXML = GetConfigurationHostedControlsFetchXML(configuration);

            CRMEntity crmEntity = GetConfigurationHostedControlM2MEntity(service, hostedControlsfetchXML);

            return crmEntity;
        }



        public string GetHostedControlsFetchXML(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='uii_hostedapplication'>
                        <all-attributes /> 
                        <link-entity name='msdyusd_configuration_hostedcontrol' from='uii_hostedapplicationid' to='uii_hostedapplicationid' visible='false' intersect='true'>
                          <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='ab'>
                            <filter type='and'>
                              <condition attribute='msdyusd_configurationid' operator='eq'  uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                            </filter>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
        }


        public string GetConfigurationHostedControlsFetchXML(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='msdyusd_configuration_hostedcontrol' >
                        <attribute name='uii_hostedapplicationid' />
                        <attribute name='msdyusd_configuration_hostedcontrolid' />
                        <attribute name='msdyusd_configurationid' />
                        <filter type='and' >
                          <condition attribute='msdyusd_configurationid' operator='eq' value='" + configuration.Id + @"' />
                        </filter>
                      </entity>
                    </fetch>";


        }


        public CRMEntity GetConfigurationHostedControlM2MEntity(IOrganizationService service, string fetchXml)
        {
            EntityCollection results = service.RetrieveMultiple(new FetchExpression(fetchXml));

            if (results != null && results.Entities != null && results.Entities.Count > 0)
            {

                List<CRMM2MRecord> crmM2MRecords = new List<CRMM2MRecord>();
                foreach (Entity entity in results.Entities)
                {
                    Guid configurationId = entity.GetAttributeValue<Guid>("msdyusd_configurationid");
                    Guid hostedControlId = entity.GetAttributeValue<Guid>("uii_hostedapplicationid");

                    CRMM2MRecord m2mRecord = new CRMM2MRecord
                    {
                        Entity1AttributeId = configurationId,
                        Entity1LogicalName = "msdyusd_configuration",
                        Entity2AttributeId = hostedControlId,
                        Entity2LogicalName = "uii_hostedapplication"
                    };

                    crmM2MRecords.Add(m2mRecord);
                }

                CRMEntity crmEntity = new CRMEntity { CRMM2MRecords = crmM2MRecords, IsIntersect = true, LogicalName = results.EntityName, RecordCount = results.Entities.Count };
                return crmEntity;
            }

            return null;
        }

        public ImportResult ImportHostedControls(IOrganizationService targetCrmService, CRMEntity sourceHostedControlsCE,
                                             CRMEntity targetHostedControlsCE, LookupMatchCriteria hostedControlMatchCriteria)
        {

            ImportResult importResult = ImportEntity(targetCrmService,
                                                    sourceHostedControlsCE,
                                                    targetHostedControlsCE,
                                                    hostedControlMatchCriteria);
            return importResult;
        }




        public ImportResult ImportConfigurationHostedControl(IOrganizationService targetCrmService,
                                                             Guid configurationId,
                                                             CRMEntity sourceHostedControlsCE,
                                                             CRMEntity targetHostedControlsCE)
        {

            string configurationHostedControlIntersectName = "msdyusd_configuration_hostedcontrol";

            ImportResult configurationHostedControlsImportResult = ImportConfigurationIntersectEntity(targetCrmService,
                                                                                         configurationId,
                                                                                         sourceHostedControlsCE,
                                                                                         targetHostedControlsCE,
                                                                                         configurationHostedControlIntersectName);

            return configurationHostedControlsImportResult;



        }



    }
}

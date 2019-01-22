using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System;


namespace USDConfigurationMigration.Repositories
{
    public class ToolbarButtonRepository : BaseRepository
    {

        public CRMEntity GetToolbarButtons(IOrganizationService service, EntityReference configuration)
        {
            string toolbarButtonsFetchXML = GetToolbarButtonsfetchXML(service, configuration);

            string toolBarButtonNoToolbarFetchXML = GetToolbarButtonsNoToolbarFetchXml(configuration);



            CRMEntity crmEntity = GetCRMEntity(service, toolbarButtonsFetchXML);

            CRMEntity toolbarButtonsNoToolbar = GetCRMEntity(service, toolBarButtonNoToolbarFetchXML);


            if (toolbarButtonsNoToolbar != null && toolbarButtonsNoToolbar.CRMRecords != null)
            {
                crmEntity.CRMRecords.AddRange(toolbarButtonsNoToolbar.CRMRecords);

                crmEntity.RecordCount = crmEntity.CRMRecords.Count;
            }

            return crmEntity;
        }



        private string GetToolbarButtonsfetchXML(IOrganizationService service, EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='msdyusd_toolbarbutton'>
                        <all-attributes />
                        <order attribute='msdyusd_buttons' descending='false' />
                        <link-entity name='msdyusd_toolbarstrip' from='msdyusd_toolbarstripid' to='msdyusd_toolbarid' alias='af'>
                          <link-entity name='msdyusd_configuration_toolbar' from='msdyusd_toolbarstripid' to='msdyusd_toolbarstripid' visible='false' intersect='true'>
                            <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='ag'>
                              <filter type='and'>
                                <condition attribute='msdyusd_configurationid' operator='eq'  uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                              </filter>
                            </link-entity>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
        }


        private string GetToolbarButtonsNoToolbarFetchXml(EntityReference configuration)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='msdyusd_toolbarbutton'>
                        <all-attributes />
                        <order attribute='msdyusd_buttons' descending='false' />
                        <filter type='and'>
                          <condition attribute='msdyusd_toolbarid' operator='null' />
                        </filter>
                        <link-entity name='msdyusd_toolbarbutton' from='msdyusd_toolbarbuttonid' to='msdyusd_buttons' alias='ai'>
                          <link-entity name='msdyusd_toolbarstrip' from='msdyusd_toolbarstripid' to='msdyusd_toolbarid' alias='aj'>
                            <link-entity name='msdyusd_configuration_toolbar' from='msdyusd_toolbarstripid' to='msdyusd_toolbarstripid' visible='false' intersect='true'>
                              <link-entity name='msdyusd_configuration' from='msdyusd_configurationid' to='msdyusd_configurationid' alias='ak'>
                                <filter type='and'>
                                  <condition attribute='msdyusd_configurationid' operator='eq'  uitype='msdyusd_configuration' value='{" + configuration.Id + @"}' />
                                </filter>
                              </link-entity>
                            </link-entity>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
        }

        public ImportResult ImportToolbarButtons(IOrganizationService targetCrmService, CRMEntity sourceToolbarButtonsCE,
                             CRMEntity targetToolbarButtonsCE, LookupMatchCriteria toolbarButtonMatchcriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceToolbarButtonsCE,
                                           targetToolbarButtonsCE,
                                           toolbarButtonMatchcriteria);
            return importResult;
        }



    }
}

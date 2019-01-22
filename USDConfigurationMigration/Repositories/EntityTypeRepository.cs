using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System;


namespace USDConfigurationMigration.Repositories
{
    public class EntityTypeRepository : BaseRepository
    {


        public CRMEntity GetEntityTypes(IOrganizationService service)
        {
            string entityTypesfetchXML = GetEntityTypesfetchXML(service);

            CRMEntity crmEntity = GetCRMEntity(service, entityTypesfetchXML);

            return crmEntity;
        }



        private string GetEntityTypesfetchXML(IOrganizationService service)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                      <entity name='msdyusd_entityassignment'>
                        <all-attributes />
                      </entity>
                    </fetch>";
        }


        public ImportResult ImportEntityTypes(IOrganizationService targetCrmService, CRMEntity sourceEntityTypesCE,
                             CRMEntity targetEntityTypesCE, LookupMatchCriteria entityTypeMatchCriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceEntityTypesCE,
                                           targetEntityTypesCE,
                                           entityTypeMatchCriteria);
            return importResult;
        }

    }
}

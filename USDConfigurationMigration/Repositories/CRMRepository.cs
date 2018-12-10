using MayoEnterprise.Libraries.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDConfigurationMigration.Models;

namespace USDConfigurationMigration.Repositories
{
    public class CRMRepository
    {

        public Dictionary<string, List<Entity>> GetTargetEntitiesForLookupMatch(IOrganizationService crmService, List<LookupMatchCriteria> lookupMatchCriterias)
        {
            Dictionary<string, List<Entity>> entityLookups = new Dictionary<string, List<Entity>>();

            Dictionary<string, QueryExpression> queriesToBeProcessed = new Dictionary<string, QueryExpression>();
            foreach (LookupMatchCriteria lookupMatchCriteria in lookupMatchCriterias)
            {
                QueryExpression queryForLookupMatching = GetQueryForLookupMatching(lookupMatchCriteria);
                queriesToBeProcessed.Add(lookupMatchCriteria.EntityLogicalName, queryForLookupMatching);
            }

            BulkRequest bulkRequest = new BulkRequest(100);
            BulkResponse bulkResponse = bulkRequest.Read(crmService, queriesToBeProcessed);

            foreach (BulkResponseItem bulkResponseItem in bulkResponse.Responses)
            {
                if (bulkResponseItem.Results != null && bulkResponseItem.Results.Entities != null)
                {
                    entityLookups.Add(bulkResponseItem.BulkRequestId, bulkResponseItem.Results.Entities.ToList());
                }
            }


            return entityLookups;
        }

        private QueryExpression GetQueryForLookupMatching(LookupMatchCriteria lookupMatchCriteria)
        {
            string primaryAttributeName = lookupMatchCriteria.EntityLogicalName.StartsWith("uii") ? "uii_name" : "msdyusd_name";

            QueryExpression queryExpression = new QueryExpression(lookupMatchCriteria.EntityLogicalName);
            queryExpression.ColumnSet = new ColumnSet(primaryAttributeName);

            if (lookupMatchCriteria.AttributesToMatchOn != null && lookupMatchCriteria.AttributesToMatchOn.Count > 0)
            {
                queryExpression.ColumnSet.AddColumns(lookupMatchCriteria.AttributesToMatchOn.ToArray());
            }


            return queryExpression;
        }


        public List<LookupMatchCriteria> GetDefaultLookupMatchCriterias()
        {
            List<LookupMatchCriteria> lookupMatchCriterias = new List<LookupMatchCriteria>();
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "uii_option", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_form", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_search", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_scriptlet", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_toolbarstrip", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_usersettings", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_configuration", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "uii_hostedapplication", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_sessiontransfer", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_entityassignment", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_actioncallworkflow", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_agentscripttaskcategory", PrimaryIdMatchOnly = true });

            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_customizationfiles", PrimaryIdMatchOnly = true });


            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "uii_action", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_task", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_uiievent", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_entitysearch", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_toolbarbutton", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_languagemodule", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_sessioninformation", PrimaryIdMatchOnly = true });


            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_agentscriptaction", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_answer", PrimaryIdMatchOnly = true });
            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_scripttasktrigger", PrimaryIdMatchOnly = true });

            lookupMatchCriterias.Add(new LookupMatchCriteria { EntityLogicalName = "msdyusd_windowroute", PrimaryIdMatchOnly = true });


            return lookupMatchCriterias;
        }
    }
}

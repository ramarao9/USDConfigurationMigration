using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System.Linq;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using USDConfigurationMigration.Models;


namespace USDConfigurationMigration.Repositories
{
    public class ToolbarButtonActionCallRepository : BaseRepository
    {


        public CRMEntity GetToolbarButtonActionCalls(IOrganizationService service, CRMEntity toolbarButtonCE, CRMEntity actionCallCE)
        {
            List<Entity> toolbarButtons = GetAnswers(toolbarButtonCE, null);

            List<Guid> toolbarButtonIds = (toolbarButtons != null) ? toolbarButtons.Select(x => x.Id).ToList() : null;

            List<Entity> actionCalls = GetAnswers(actionCallCE, null);

            List<Guid> actionCallIds = (actionCalls != null) ? actionCalls.Select(x => x.Id).ToList() : null;

            if (toolbarButtonIds == null || toolbarButtonIds.Count == 0 || actionCallIds == null || actionCallIds.Count == 0)
                return null;

            CRMM2MEntityMapping toolbarButtonActionCallsEntityMap = GetToolbarButtonActionCallsEntityMap();

            QueryExpression queryExpression = GetToolbarButtonActionCallsQuery(toolbarButtonIds, actionCallIds);

            CRMEntity crmEntity = GetCRMEntityM2M(service, queryExpression, toolbarButtonActionCallsEntityMap);

            return crmEntity;
        }


        public QueryExpression GetToolbarButtonActionCallsQuery(List<Guid> toolbarButtonIds, List<Guid> actionCallIds)
        {
            QueryExpression query = new QueryExpression("msdyusd_toolbarbutton_agentscriptaction");
            query.ColumnSet = new ColumnSet(true);

            query.Criteria.AddCondition(new ConditionExpression("msdyusd_toolbarbuttonid", ConditionOperator.In, toolbarButtonIds.ToArray()));
            query.Criteria.AddCondition(new ConditionExpression("msdyusd_agentscriptactionid", ConditionOperator.In, actionCallIds.ToArray()));

            return query;
        }



        public ImportResult ImportToolbarButtonActionCalls(IOrganizationService targetCrmService, CRMEntity sourceToolbarButtonActionCallsCE,
                             CRMEntity targetToolbarButtonActionCallsCE)
        {
            CRMM2MEntityMapping toolbarButtonActionCallsEntityMap = GetToolbarButtonActionCallsEntityMap();

            ImportResult importResult = ImportM2MEntity(targetCrmService,
                                           sourceToolbarButtonActionCallsCE,
                                           targetToolbarButtonActionCallsCE,
                                           toolbarButtonActionCallsEntityMap);
            return importResult;
        }



        private CRMM2MEntityMapping GetToolbarButtonActionCallsEntityMap()
        {
            CRMM2MEntityMapping toolbarButtonActionCallsEntityMap = new CRMM2MEntityMapping
            {
                IntersectEntity = "msdyusd_toolbarbutton_agentscriptaction",
                Entity1 = "msdyusd_toolbarbutton",
                Entity1Attribute = "msdyusd_toolbarbuttonid",
                Entity2 = "msdyusd_agentscriptaction",
                Entity2Attribute = "msdyusd_agentscriptactionid"
            };

            return toolbarButtonActionCallsEntityMap;
        }


    }
}

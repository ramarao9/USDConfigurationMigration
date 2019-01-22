using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System.Linq;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;


namespace USDConfigurationMigration.Repositories
{
    public class WNRActionCallRepository : BaseRepository
    {

        public CRMEntity GetWNRActionCalls(IOrganizationService service, CRMEntity wnrCE, CRMEntity actionCallCE)
        {
            List<Entity> navigationRules = GetAnswers(wnrCE, null);

            List<Guid> navigationRuleIds = (navigationRules != null) ? navigationRules.Select(x => x.Id).ToList() : null;

            List<Entity> actionCalls = GetAnswers(actionCallCE, null);

            List<Guid> actionCallIds = (actionCalls != null) ? actionCalls.Select(x => x.Id).ToList() : null;

            if (navigationRuleIds == null || navigationRuleIds.Count == 0 || actionCallIds == null || actionCallIds.Count == 0)
                return null;

            CRMM2MEntityMapping wnrActionCallsM2MEntityMapping = GetWNRActionCallsEntityMap();

            QueryExpression queryExpression = GetWNRActionCallsQuery(navigationRuleIds, actionCallIds);

            CRMEntity crmEntity = GetCRMEntityM2M(service, queryExpression, wnrActionCallsM2MEntityMapping);

            return crmEntity;
        }


        public QueryExpression GetWNRActionCallsQuery(List<Guid> navigationRuleIds, List<Guid> actionCallIds)
        {
            QueryExpression query = new QueryExpression("msdyusd_windowroute_agentscriptaction");
            query.ColumnSet = new ColumnSet(true);

            query.Criteria.AddCondition(new ConditionExpression("msdyusd_windowrouteid", ConditionOperator.In, navigationRuleIds.ToArray()));
            query.Criteria.AddCondition(new ConditionExpression("msdyusd_agentscriptactionid", ConditionOperator.In, actionCallIds.ToArray()));

            return query;
        }



        public ImportResult ImportWNRActionCalls(IOrganizationService targetCrmService, CRMEntity sourceWNRActionCallsCE,
                             CRMEntity targetWNRActionCallsCE)
        {
            CRMM2MEntityMapping wnrActionCallsEntityMap = GetWNRActionCallsEntityMap();

            ImportResult importResult = ImportM2MEntity(targetCrmService,
                                           sourceWNRActionCallsCE,
                                           targetWNRActionCallsCE,
                                           wnrActionCallsEntityMap);
            return importResult;
        }



        private CRMM2MEntityMapping GetWNRActionCallsEntityMap()
        {
            CRMM2MEntityMapping wnrActionCallsM2MEntityMapping = new CRMM2MEntityMapping
            {
                IntersectEntity = "msdyusd_windowroute_agentscriptaction",
                Entity1 = "msdyusd_windowroute",
                Entity1Attribute = "msdyusd_windowrouteid",
                Entity2 = "msdyusd_agentscriptaction",
                Entity2Attribute = "msdyusd_agentscriptactionid"
            };

            return wnrActionCallsM2MEntityMapping;
        }

    }
}

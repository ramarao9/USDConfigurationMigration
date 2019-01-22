using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System.Linq;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;

namespace USDConfigurationMigration.Repositories
{
    public class SubActionCallsRepository : BaseRepository
    {
        public CRMEntity GetSubActionCalls(IOrganizationService service, CRMEntity actionCallCE)
        {

            List<Entity> actionCalls = GetAnswers(actionCallCE, null);

            List<Guid> actionCallIds = (actionCalls != null) ? actionCalls.Select(x => x.Id).ToList() : null;

            if (actionCallIds == null || actionCallIds.Count == 0)
                return null;

            CRMM2MEntityMapping subActionCallsM2MEntityMapping = GetSubActionCallsEntityMap();

            QueryExpression queryExpression = GetSubActionCallsQuery(actionCallIds);

            CRMEntity crmEntity = GetCRMEntityM2M(service, queryExpression, subActionCallsM2MEntityMapping);

            return crmEntity;
        }


        public QueryExpression GetSubActionCallsQuery(List<Guid> actionCallIds)
        {
            QueryExpression query = new QueryExpression("msdyusd_subactioncalls");
            query.ColumnSet = new ColumnSet(true);

            query.Criteria.AddCondition(new ConditionExpression("msdyusd_agentscriptactionidone", ConditionOperator.In, actionCallIds.ToArray()));
            query.Criteria.AddCondition(new ConditionExpression("msdyusd_agentscriptactionidtwo", ConditionOperator.In, actionCallIds.ToArray()));

            return query;
        }



        public ImportResult ImportSubActionCalls(IOrganizationService targetCrmService, CRMEntity sourceActionCallsCE,
                             CRMEntity targetActionCallsCE)
        {
            CRMM2MEntityMapping subActionCallsEntityMap = GetSubActionCallsEntityMap();

            ImportResult importResult = ImportM2MEntity(targetCrmService,
                                           sourceActionCallsCE,
                                           targetActionCallsCE,
                                           subActionCallsEntityMap);
            return importResult;
        }



        private CRMM2MEntityMapping GetSubActionCallsEntityMap()
        {
            CRMM2MEntityMapping subActionCallsM2MEntityMapping = new CRMM2MEntityMapping
            {
                IntersectEntity = "msdyusd_subactioncalls",
                Entity1 = "msdyusd_agentscriptaction",
                Entity1Attribute = "msdyusd_agentscriptactionidone",
                Entity2 = "msdyusd_agentscriptaction",
                Entity2Attribute = "msdyusd_agentscriptactionidtwo"
            };

            return subActionCallsM2MEntityMapping;
        }

    }
}

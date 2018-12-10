using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System.Linq;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using USDConfigurationMigration.Models;


namespace USDConfigurationMigration.Repositories
{
    public class EventActionCallRepository : BaseRepository
    {

        public CRMEntity GetEventActionCalls(IOrganizationService service, CRMEntity eventCE, CRMEntity actionCallCE)
        {
            List<Entity> events = GetAnswers(eventCE, null);

            List<Guid> eventIds = (events != null) ? events.Select(x => x.Id).ToList() : null;

            List<Entity> actionCalls = GetAnswers(actionCallCE, null);

            List<Guid> actionCallIds = (actionCalls != null) ? actionCalls.Select(x => x.Id).ToList() : null;

            if (eventIds == null || eventIds.Count == 0 || actionCallIds == null || actionCallIds.Count == 0)
                return null;

            CRMM2MEntityMapping eventActionCallsM2MEntityMapping = GetEventActionCallsEntityMap();

            QueryExpression queryExpression = GetEventActionCallsQuery(eventIds, actionCallIds);

            CRMEntity crmEntity = GetCRMEntityM2M(service, queryExpression, eventActionCallsM2MEntityMapping);

            return crmEntity;
        }


        public QueryExpression GetEventActionCallsQuery(List<Guid> eventIds, List<Guid> actionCallIds)
        {
            QueryExpression query = new QueryExpression("msdyusd_uiievent_agentscriptaction");
            query.ColumnSet = new ColumnSet(true);

            query.Criteria.AddCondition(new ConditionExpression("msdyusd_uiieventid", ConditionOperator.In, eventIds.ToArray()));
            query.Criteria.AddCondition(new ConditionExpression("msdyusd_agentscriptactionid", ConditionOperator.In, actionCallIds.ToArray()));

            return query;
        }



        public ImportResult ImportEventActionCalls(IOrganizationService targetCrmService, CRMEntity sourceEventActionCallsCE,
                             CRMEntity targetEventActionCallsCE)
        {
            CRMM2MEntityMapping eventActionCallsEntityMap = GetEventActionCallsEntityMap();

            ImportResult importResult = ImportM2MEntity(targetCrmService,
                                           sourceEventActionCallsCE,
                                           targetEventActionCallsCE,
                                           eventActionCallsEntityMap);
            return importResult;
        }



        private CRMM2MEntityMapping GetEventActionCallsEntityMap()
        {
            CRMM2MEntityMapping eventActionCallsM2MEntityMapping = new CRMM2MEntityMapping
            {
                IntersectEntity = "msdyusd_uiievent_agentscriptaction",
                Entity1 = "msdyusd_uiievent",
                Entity1Attribute = "msdyusd_uiieventid",
                Entity2 = "msdyusd_agentscriptaction",
                Entity2Attribute = "msdyusd_agentscriptactionid"
            };

            return eventActionCallsM2MEntityMapping;
        }


    }
}

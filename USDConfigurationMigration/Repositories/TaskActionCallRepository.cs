using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System.Linq;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;


namespace USDConfigurationMigration.Repositories
{
    public class TaskActionCallRepository : BaseRepository
    {


        public CRMEntity GetTaskActionCalls(IOrganizationService service, CRMEntity taskCE, CRMEntity actionCallCE)
        {
            List<Entity> tasks = GetAnswers(taskCE, null);

            List<Guid> taskIds = (tasks != null) ? tasks.Select(x => x.Id).ToList() : null;

            List<Entity> actionCalls = GetAnswers(actionCallCE, null);

            List<Guid> actionCallIds = (actionCalls != null) ? actionCalls.Select(x => x.Id).ToList() : null;

            if (taskIds == null || taskIds.Count == 0 || actionCallIds == null || actionCallIds.Count == 0)
                return null;

            CRMM2MEntityMapping taskActionCallsM2MEntityMapping = GetTaskActionCallsEntityMap();

            QueryExpression queryExpression = GetTaskActionCallsQuery(taskIds, actionCallIds);

            CRMEntity crmEntity = GetCRMEntityM2M(service, queryExpression, taskActionCallsM2MEntityMapping);

            return crmEntity;
        }


        public QueryExpression GetTaskActionCallsQuery(List<Guid> taskIds, List<Guid> actionCallIds)
        {
            QueryExpression query = new QueryExpression("msdyusd_task_agentscriptaction");
            query.ColumnSet = new ColumnSet(true);

            query.Criteria.AddCondition(new ConditionExpression("msdyusd_taskid", ConditionOperator.In, taskIds.ToArray()));
            query.Criteria.AddCondition(new ConditionExpression("msdyusd_agentscriptactionid", ConditionOperator.In, actionCallIds.ToArray()));

            return query;
        }



        public ImportResult ImportTaskActionCalls(IOrganizationService targetCrmService, CRMEntity sourceTaskActionCallsCE,
                             CRMEntity targetTaskActionCallsCE)
        {
            CRMM2MEntityMapping taskActionCallsEntityMap = GetTaskActionCallsEntityMap();

            ImportResult importResult = ImportM2MEntity(targetCrmService,
                                           sourceTaskActionCallsCE,
                                           targetTaskActionCallsCE,
                                           taskActionCallsEntityMap);
            return importResult;
        }



        private CRMM2MEntityMapping GetTaskActionCallsEntityMap()
        {
            CRMM2MEntityMapping taskActionCallsM2MEntityMapping = new CRMM2MEntityMapping
            {
                IntersectEntity = "msdyusd_task_agentscriptaction",
                Entity1 = "msdyusd_task",
                Entity1Attribute = "msdyusd_taskid",
                Entity2 = "msdyusd_agentscriptaction",
                Entity2Attribute = "msdyusd_agentscriptactionid"
            };

            return taskActionCallsM2MEntityMapping;
        }
    }
}

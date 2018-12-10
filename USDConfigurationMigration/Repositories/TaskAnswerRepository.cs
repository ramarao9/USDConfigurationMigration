using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System.Linq;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using USDConfigurationMigration.Models;

namespace USDConfigurationMigration.Repositories
{
    public class TaskAnswerRepository : BaseRepository
    {

        public CRMEntity GetTaskAnswers(IOrganizationService service, CRMEntity agentScriptTaskCE)
        {
            List<Entity> agentScriptTasks = GetAnswers(agentScriptTaskCE, null);

            List<Guid> agentScriptTaskIds = (agentScriptTasks != null) ? agentScriptTasks.Select(x => x.Id).ToList() : null;

            if (agentScriptTaskIds == null || agentScriptTaskIds.Count == 0)
                return null;

            CRMM2MEntityMapping taskAnswersM2MEntityMapping = GetTaskAnswersEntityMap();

            QueryExpression queryExpression = GetTaskAnswersQuery(agentScriptTaskIds);

            CRMEntity crmEntity = GetCRMEntityM2M(service, queryExpression, taskAnswersM2MEntityMapping);

            return crmEntity;
        }


        public QueryExpression GetTaskAnswersQuery(List<Guid> agentScriptTaskIds)
        {
            QueryExpression query = new QueryExpression("msdyusd_task_answer");
            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddCondition(new ConditionExpression("msdyusd_taskid", ConditionOperator.In, agentScriptTaskIds.ToArray()));

            return query;
        }



        public ImportResult ImportTaskAnswers(IOrganizationService targetCrmService, CRMEntity sourceTaskAnswersCE,
                             CRMEntity targetTaskAnswersCE)
        {
            CRMM2MEntityMapping taskAnswersEntityMap = GetTaskAnswersEntityMap();

            ImportResult importResult = ImportM2MEntity(targetCrmService,
                                           sourceTaskAnswersCE,
                                           targetTaskAnswersCE,
                                           taskAnswersEntityMap);
            return importResult;
        }



        private CRMM2MEntityMapping GetTaskAnswersEntityMap()
        {
            CRMM2MEntityMapping taskAnswersM2MEntityMapping = new CRMM2MEntityMapping
            {
                IntersectEntity = "msdyusd_task_answer",
                Entity1 = "msdyusd_task",
                Entity1Attribute = "msdyusd_taskid",
                Entity2 = "msdyusd_answer",
                Entity2Attribute = "msdyusd_answerid"
            };

            return taskAnswersM2MEntityMapping;
        }


    }
}

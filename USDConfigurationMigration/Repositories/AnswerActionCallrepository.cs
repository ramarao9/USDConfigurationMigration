using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System.Linq;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
namespace USDConfigurationMigration.Repositories
{
    public class AnswerActionCallrepository : BaseRepository
    {
        public CRMEntity GetAnswerActionCalls(IOrganizationService service, CRMEntity answersCE, CRMEntity actionCallCE)
        {
            List<Entity> answers = GetAnswers(answersCE, null);

            List<Guid> answerIds = (answers != null) ? answers.Select(x => x.Id).ToList() : null;

            List<Entity> actionCalls = GetAnswers(actionCallCE, null);

            List<Guid> actionCallIds = (actionCalls != null) ? actionCalls.Select(x => x.Id).ToList() : null;

            if (answerIds == null || answerIds.Count == 0 || actionCallIds == null || actionCallIds.Count == 0)
                return null;

            CRMM2MEntityMapping answerActionCallsM2MEntityMapping = GetAnswerActionCallsEntityMap();

            QueryExpression queryExpression = GetAnswerActionCallsQuery(answerIds, actionCallIds);

            CRMEntity crmEntity = GetCRMEntityM2M(service, queryExpression, answerActionCallsM2MEntityMapping);

            return crmEntity;
        }


        public QueryExpression GetAnswerActionCallsQuery(List<Guid> answersIds, List<Guid> actionCallIds)
        {
            QueryExpression query = new QueryExpression("msdyusd_answer_agentscriptaction");
            query.ColumnSet = new ColumnSet(true);

            query.Criteria.AddCondition(new ConditionExpression("msdyusd_answerid", ConditionOperator.In, answersIds.ToArray()));
            query.Criteria.AddCondition(new ConditionExpression("msdyusd_agentscriptactionid", ConditionOperator.In, actionCallIds.ToArray()));

            return query;
        }



        public ImportResult ImportAnswerActionCalls(IOrganizationService targetCrmService, CRMEntity sourceAnswerActionCallsCE,
                             CRMEntity targetAnswerActionCallsCE)
        {
            CRMM2MEntityMapping answerActionCallsEntityMap = GetAnswerActionCallsEntityMap();

            ImportResult importResult = ImportM2MEntity(targetCrmService,
                                           sourceAnswerActionCallsCE,
                                           targetAnswerActionCallsCE,
                                           answerActionCallsEntityMap);
            return importResult;
        }



        private CRMM2MEntityMapping GetAnswerActionCallsEntityMap()
        {
            CRMM2MEntityMapping answerActionCallsM2MEntityMapping = new CRMM2MEntityMapping
            {
                IntersectEntity = "msdyusd_answer_agentscriptaction",
                Entity1 = "msdyusd_answer",
                Entity1Attribute = "msdyusd_answerid",
                Entity2 = "msdyusd_agentscriptaction",
                Entity2Attribute = "msdyusd_agentscriptactionid"
            };

            return answerActionCallsM2MEntityMapping;
        }


    }
}

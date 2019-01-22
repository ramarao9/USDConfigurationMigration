using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.Xrm.Sdk.Query;

namespace USDConfigurationMigration.Repositories
{
    public class AgentScriptAnswerRepository : BaseRepository
    {

        public CRMEntity GetAgentScriptAnswers(IOrganizationService service, CRMEntity taskAnswersCE)
        {

            List<Guid> answerIds = (taskAnswersCE != null && taskAnswersCE.CRMM2MRecords != null) ?
                                  taskAnswersCE.CRMM2MRecords.Select(x => x.Entity2AttributeId).ToList() : null;

            if (answerIds == null)
                return null;

            QueryExpression agentScriptAnswersQuery = GetAgentScriptAnswersQuery(service, answerIds);

            CRMEntity crmEntity = GetCRMEntity(service, agentScriptAnswersQuery);

            return crmEntity;
        }


        private QueryExpression GetAgentScriptAnswersQuery(IOrganizationService service, List<Guid> answerIds)
        {
            QueryExpression query = new QueryExpression("msdyusd_answer");
            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddCondition(new ConditionExpression("msdyusd_answerid", ConditionOperator.In, answerIds.ToArray()));

            return query;
        }


        public ImportResult ImportAgentScriptAnswers(IOrganizationService targetCrmService, CRMEntity sourceAnswersCE,
                             CRMEntity targetAnswersCE, LookupMatchCriteria answersMatchCriteria)
        {
            ImportResult importResult = ImportEntity(targetCrmService,
                                           sourceAnswersCE,
                                           targetAnswersCE,
                                           answersMatchCriteria);
            return importResult;
        }

    }
}

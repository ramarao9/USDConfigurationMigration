using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigration.Helpers
{
    public static class QueryExpressionExtensions
    {
        public static EntityReference GetFirstMatch(this QueryExpression queryExpression, CrmServiceClient crmserviceClient,
            string entityLogicalName, string primaryAttributeName, string name)
        {
            if (queryExpression == null)
            {
                queryExpression = new QueryExpression(entityLogicalName);
            }
            queryExpression.EntityName = entityLogicalName;
            queryExpression.ColumnSet = new ColumnSet(primaryAttributeName);
            queryExpression.Criteria.AddCondition(primaryAttributeName, ConditionOperator.Equal, name);


            EntityCollection results = crmserviceClient.RetrieveMultiple(queryExpression);

            if (results != null && results.Entities != null && results.Entities.Count > 0)
                return results.Entities[0].ToEntityReference();


            return null;
        }

    }
}

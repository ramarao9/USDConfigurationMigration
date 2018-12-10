using Microsoft.Xrm.Sdk;
using USDConfigurationMigration.Models;
using System.Linq;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using USDConfigurationMigration.Models;

namespace USDConfigurationMigration.Repositories
{
    public class ToolbarHostedControlRepository : BaseRepository
    {

        public CRMEntity GetToolbarHostedControls(IOrganizationService service, CRMEntity toolbarCE, CRMEntity hostedControlsCE)
        {
            List<Entity> toolbars = GetAnswers(toolbarCE, null);

            List<Guid> toolbarIds = (toolbars != null) ? toolbars.Select(x => x.Id).ToList() : null;

            List<Entity> hostedControls = GetAnswers(hostedControlsCE, null);

            List<Guid> hostedControlIds = (hostedControls != null) ? hostedControls.Select(x => x.Id).ToList() : null;

            if (toolbarIds == null || toolbarIds.Count == 0 || hostedControlIds == null || hostedControlIds.Count == 0)
                return null;

            CRMM2MEntityMapping toolbarHostedControlsEntityMap = GetToolbarHostedControlsEntityMap();

            QueryExpression queryExpression = GetToolbarHostedControlsQuery(toolbarIds, hostedControlIds);

            CRMEntity crmEntity = GetCRMEntityM2M(service, queryExpression, toolbarHostedControlsEntityMap);

            return crmEntity;
        }


        public QueryExpression GetToolbarHostedControlsQuery(List<Guid> toolbarIds, List<Guid> hostedControlIds)
        {
            QueryExpression query = new QueryExpression("msdyusd_toolbarstrip_uii_hostedapplication");
            query.ColumnSet = new ColumnSet(true);

            query.Criteria.AddCondition(new ConditionExpression("msdyusd_toolbarstripid", ConditionOperator.In, toolbarIds.ToArray()));
            query.Criteria.AddCondition(new ConditionExpression("uii_hostedapplicationid", ConditionOperator.In, hostedControlIds.ToArray()));

            return query;
        }



        public ImportResult ImportToolbarHostedControls(IOrganizationService targetCrmService, CRMEntity sourceToolbarHostedControlsCE,
                             CRMEntity targetToolbarHostedControlsCE)
        {
            CRMM2MEntityMapping toolbarHostedControlsEntityMap = GetToolbarHostedControlsEntityMap();

            ImportResult importResult = ImportM2MEntity(targetCrmService,
                                           sourceToolbarHostedControlsCE,
                                           targetToolbarHostedControlsCE,
                                           toolbarHostedControlsEntityMap);
            return importResult;
        }



        private CRMM2MEntityMapping GetToolbarHostedControlsEntityMap()
        {
            CRMM2MEntityMapping toolbarButtonActionCallsEntityMap = new CRMM2MEntityMapping
            {
                IntersectEntity = "msdyusd_toolbarstrip_uii_hostedapplication",
                Entity1 = "msdyusd_toolbarstrip",
                Entity1Attribute = "msdyusd_toolbarstripid",
                Entity2 = "uii_hostedapplication",
                Entity2Attribute = "uii_hostedapplicationid"
            };

            return toolbarButtonActionCallsEntityMap;
        }

    }
}

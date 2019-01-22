using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using USDConfigurationMigration.Models;
using USDConfigurationMigration.Helpers;
using Microsoft.Xrm.Sdk.Messages;
using MayoEnterprise.Libraries.Xrm;

namespace USDConfigurationMigration.Repositories
{
    public class BaseRepository
    {


        protected BulkRequest _bulkRequest;

        MatchingHelper _matchingHelper;
        public BaseRepository()
        {
            _matchingHelper = new MatchingHelper();
            _bulkRequest = new BulkRequest(100);
        }

        public CRMEntity GetCRMEntityM2M(IOrganizationService service, QueryExpression query, CRMM2MEntityMapping crmM2MEntityMapping)
        {
            EntityCollection results = service.RetrieveMultiple(query);

            if (results != null && results.Entities != null && results.Entities.Count > 0)
            {

                List<CRMM2MRecord> records = new List<CRMM2MRecord>();
                foreach (Entity entity in results.Entities)
                {
                    CRMM2MRecord crmM2MRecord = entity.ToCRMM2MRecord(crmM2MEntityMapping);
                    records.Add(crmM2MRecord);
                }


                CRMEntity crmEntity = new CRMEntity { CRMM2MRecords = records, IsIntersect = true, LogicalName = results.EntityName, RecordCount = results.Entities.Count };
                return crmEntity;
            }


            return null;
        }

        public CRMEntity GetCRMEntity(IOrganizationService service, QueryExpression query)
        {
            EntityCollection results = service.RetrieveMultiple(query);

            if (results != null && results.Entities != null && results.Entities.Count > 0)
            {

                List<CRMRecord> records = new List<CRMRecord>();
                foreach (Entity entity in results.Entities)
                {
                    CRMRecord crmRecord = entity.ToCRMRecord();
                    records.Add(crmRecord);
                }


                CRMEntity crmEntity = new CRMEntity { CRMRecords = records, LogicalName = results.EntityName, RecordCount = results.Entities.Count };
                return crmEntity;
            }


            return null;
        }

        public CRMEntity GetCRMEntity(IOrganizationService service, string fetchXml)
        {
            EntityCollection results = service.RetrieveMultiple(new FetchExpression(fetchXml));

            if (results != null && results.Entities != null && results.Entities.Count > 0)
            {

                List<CRMRecord> records = new List<CRMRecord>();
                foreach (Entity entity in results.Entities)
                {
                    CRMRecord crmRecord = entity.ToCRMRecord();
                    records.Add(crmRecord);
                }


                CRMEntity crmEntity = new CRMEntity { CRMRecords = records, LogicalName = results.EntityName, RecordCount = results.Entities.Count };
                return crmEntity;
            }


            return null;
        }





        public List<Entity> GetAllRecordsWithMinimumColumns(IOrganizationService targetCrmService, LookupMatchCriteria lookupMatchCriteria)
        {
            string primaryAttributeName = lookupMatchCriteria.EntityLogicalName.StartsWith("uii") ? "uii_name" : "msdyusd_name";

            QueryExpression entityQuery = new QueryExpression(lookupMatchCriteria.EntityLogicalName);
            entityQuery.ColumnSet = new ColumnSet(primaryAttributeName);

            if (lookupMatchCriteria.AttributesToMatchOn != null && lookupMatchCriteria.AttributesToMatchOn.Count > 0)
            {
                entityQuery.ColumnSet.AddColumns(lookupMatchCriteria.AttributesToMatchOn.ToArray());
            }

            EntityCollection results = targetCrmService.RetrieveMultiple(entityQuery);

            if (results == null || results.Entities == null | results.Entities.Count == 0)
                return null;

            List<Entity> entities = new List<Entity>();

            foreach (Entity entity in results.Entities)
            {
                entities.Add(entity);
            }

            return entities;

        }

        public Dictionary<string, OrganizationRequest> GetCreateOrUpdateRequests(CRMEntity sourceCE, CRMEntity targetCE,
                                                                                 List<Entity> allRecordsInTargetSystemForLookup,
                                                                                 LookupMatchCriteria matchCriteriaForCurrentEntity)
        {

            Dictionary<string, OrganizationRequest> recordsForCreateOrUpdate = new Dictionary<string, OrganizationRequest>();


            if (sourceCE == null)
                return recordsForCreateOrUpdate;

            List<Entity> targetEntitiesInTargetConfiguration = GetAnswers(targetCE, null);

            foreach (CRMRecord crmRecord in sourceCE.CRMRecords)
            {
                Entity sourceEntity = crmRecord.ToEntity(null);
                Entity targetEntity = _matchingHelper.GetMatchedEntity(matchCriteriaForCurrentEntity, sourceEntity, allRecordsInTargetSystemForLookup);

                OrganizationRequest orgRequest = null;
                if (targetEntity == null)
                {
                    orgRequest = new CreateRequest { Target = sourceEntity };
                }
                else
                {

                    if (targetEntitiesInTargetConfiguration.Any(x => x.Id == targetEntity.Id))//Get the target Entity with data from the USD Configuration to only update the data that has been modified
                    {
                        targetEntity = targetEntitiesInTargetConfiguration.First(x => x.Id == targetEntity.Id);
                    }

                    Entity modifiedEntity = sourceEntity.GetModifiedEntity(targetEntity);
                    modifiedEntity.Id = targetEntity.Id;

                    if (modifiedEntity.Attributes != null && modifiedEntity.Attributes.Count > 0)
                    {
                        orgRequest = new UpdateRequest { Target = modifiedEntity };
                    }
                }

                if (orgRequest != null)
                {
                    recordsForCreateOrUpdate.Add(sourceEntity.Id.ToString(), orgRequest);
                }
            }

            return recordsForCreateOrUpdate;
        }


        private Entity GetTargetEntityFromTargetConfiguration(List<Entity> targetEntities, Entity sourceEntity, LookupMatchCriteria matchCriteria)
        {
            Entity matchedEntity = _matchingHelper.GetMatchedEntity(matchCriteria, sourceEntity, targetEntities);
            return matchedEntity;
        }






        public List<Entity> GetAnswers(CRMEntity crmEntity, Dictionary<Guid, Guid> sourceTargetMappings)
        {


            List<Entity> entities = new List<Entity>();

            if (crmEntity == null || crmEntity.CRMRecords == null || crmEntity.CRMRecords.Count == 0)
                return entities;

            foreach (CRMRecord crmRecord in crmEntity.CRMRecords)
            {
                Entity entity = crmRecord.ToEntity(sourceTargetMappings);
                entities.Add(entity);
            }

            return entities;
        }


        public List<Entity> GetM2MEntities(CRMEntity crmEntity, CRMM2MEntityMapping m2mEntityMap, Dictionary<Guid, Guid> sourceTargetMappings)
        {


            List<Entity> entities = new List<Entity>();

            if (crmEntity == null || crmEntity.CRMM2MRecords == null || crmEntity.CRMM2MRecords.Count == 0)
                return entities;

            foreach (CRMM2MRecord crmM2MRecord in crmEntity.CRMM2MRecords)
            {
                Entity entity = crmM2MRecord.ToEntity(m2mEntityMap, sourceTargetMappings);
                entities.Add(entity);
            }

            return entities;
        }




        protected ImportResult GetImportResult(BulkResponse bulkResponse, CRMEntity crmEntity)
        {

            ImportResult importResult = new ImportResult();
            List<String> errorMessages = new List<string>();


            if (crmEntity == null)
                return importResult;

            List<CRMRecord> crmRecords = crmEntity.CRMRecords;
            foreach (BulkResponseItem bulkResponseItem in bulkResponse.Responses)
            {

                string requestId = bulkResponseItem.BulkRequestId;
                if (!bulkResponseItem.Success)
                {
                    string errorMessage = null;
                    if (crmEntity.IsIntersect)
                    {
                        errorMessage = crmEntity.LogicalName + "," + requestId + "," + bulkResponseItem.Action + "," + bulkResponseItem.Error;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(requestId))
                            continue;

                        Guid id = Guid.Parse(requestId);
                        CRMRecord failedCrmRecord = crmRecords.FirstOrDefault(x => x.Id == id);


                        string failedId = (failedCrmRecord.Id != Guid.Empty) ? failedCrmRecord.Id.ToString() : "";
                        errorMessage = crmEntity.LogicalName + "," + failedId + "," + bulkResponseItem.Action + "," + bulkResponseItem.Error;
                    }

                    errorMessages.Add(errorMessage);

                }
            }



            importResult.EntityLogicalName = crmEntity.LogicalName;
            importResult.Errors = errorMessages;
            importResult.FailureCount = errorMessages.Count;
            importResult.TotalProcessed = bulkResponse.Responses.Count;
            importResult.SuccessCount = importResult.TotalProcessed - importResult.FailureCount;
            importResult.CreateCount = bulkResponse.Responses.Where(x => x.Action != null && x.Action.ToLower() == "create" && x.Success).Count();
            importResult.UpdateCount = bulkResponse.Responses.Where(x => x.Action != null && x.Action.ToLower() == "update" && x.Success).Count();
            importResult.AssociateCount = bulkResponse.Responses.Where(x => x.Action != null && x.Action.ToLower() == "associaterequest" && x.Success).Count();
            importResult.DisassociateCount = bulkResponse.Responses.Where(x => x.Action != null && x.Action.ToLower() == "disassociaterequest" && x.Success).Count();
            return importResult;
        }






        protected ImportResult ImportEntity(IOrganizationService targetCrmService, CRMEntity sourceEntitiesCE,
                                            CRMEntity targetEntitiesCE, LookupMatchCriteria entityLookupMatchCriteria)
        {
            DateTime startedOn = DateTime.Now;


            List<Entity> allEntityRecordsInTargetForMatching = GetAllRecordsWithMinimumColumns(targetCrmService, entityLookupMatchCriteria);

            Dictionary<string, OrganizationRequest> createOrUpdateRequests = GetCreateOrUpdateRequests(sourceEntitiesCE,
                                                                                                        targetEntitiesCE,
                                                                                                        allEntityRecordsInTargetForMatching,
                                                                                                        entityLookupMatchCriteria);

            BulkResponse bulkResponse = _bulkRequest.Write(targetCrmService, createOrUpdateRequests);
            DateTime endedOn = DateTime.Now;


            ImportResult importResult = GetImportResult(bulkResponse, sourceEntitiesCE);
            if (string.IsNullOrWhiteSpace(importResult.EntityLogicalName) && entityLookupMatchCriteria != null)
            {
                importResult.EntityLogicalName = entityLookupMatchCriteria.EntityLogicalName;
            }

            importResult.StartedOn = startedOn;
            importResult.EndedOn = endedOn;

            return importResult;
        }





        protected ImportResult ImportConfigurationIntersectEntity(IOrganizationService targetCrmService,
                                                 Guid configurationId,
                                                 CRMEntity sourceEntitiesCE,
                                                 CRMEntity targetEntitiesCE,
                                                 string configurationIntersectEntityName)
        {
            DateTime startedOn = DateTime.Now;


            List<Entity> souceEntities = GetAnswers(sourceEntitiesCE, null);
            List<Entity> targetEntities = GetAnswers(targetEntitiesCE, null);



            Dictionary<string, OrganizationRequest> associateOrDisassociateRequests = GetAssociateAndDisassociateRequests(souceEntities,
                                                                                                                            targetEntities,
                                                                                                                            configurationId,
                                                                                                                            configurationIntersectEntityName);

            BulkResponse bulkResponse = _bulkRequest.Write(targetCrmService, associateOrDisassociateRequests);
            DateTime endedOn = DateTime.Now;


            ImportResult importResult = GetImportResult(bulkResponse, sourceEntitiesCE);
            importResult.EntityLogicalName = configurationIntersectEntityName;
            importResult.StartedOn = startedOn;
            importResult.EndedOn = endedOn;

            return importResult;
        }


        protected ImportResult ImportM2MEntity(IOrganizationService targetCrmService, CRMEntity sourceEntitiesCE,
                                       CRMEntity targetEntitiesCE,
                                       CRMM2MEntityMapping m2mEntityMap)
        {
            DateTime startedOn = DateTime.Now;

            List<Entity> souceEntities = GetM2MEntities(sourceEntitiesCE, m2mEntityMap, null);
            List<Entity> targetEntities = GetM2MEntities(targetEntitiesCE, m2mEntityMap, null);


            Dictionary<string, OrganizationRequest> m2mAssociateOrDisassociateRequests = GetM2MAssociateAndDisassociateRequests(souceEntities,
                                                                                                                        targetEntities,
                                                                                                                        m2mEntityMap);

            BulkResponse bulkResponse = _bulkRequest.Write(targetCrmService, m2mAssociateOrDisassociateRequests);

            DateTime endedOn = DateTime.Now;

            ImportResult importResult = GetImportResult(bulkResponse, sourceEntitiesCE);
            if (string.IsNullOrWhiteSpace(importResult.EntityLogicalName) && m2mEntityMap != null)
            {
                importResult.EntityLogicalName = m2mEntityMap.IntersectEntity;
            }

            importResult.StartedOn = startedOn;
            importResult.EndedOn = endedOn;

            return importResult;
        }


        private Dictionary<string, OrganizationRequest> GetAssociateAndDisassociateRequests(List<Entity> sourceEntities,
                                                           List<Entity> targetEntities,
                                                           Guid configurationId,
                                                           string intersectEntityName)
        {

            //Associate M2M ConfigurationIntersect Records
            Dictionary<string, OrganizationRequest> associateRequests = GetAssociateRequests(sourceEntities,
                                                                                            targetEntities,
                                                                                            configurationId,
                                                                                            intersectEntityName);




            //Disassociate M2M ConfigurationIntersect Records
            Dictionary<string, OrganizationRequest> disassociateRequests = GetDisassociateRequests(sourceEntities,
                                                                                                   targetEntities,
                                                                                                   configurationId,
                                                                                                   intersectEntityName);

            Dictionary<string, OrganizationRequest> associateOrDisassociateRequests = associateRequests.Union(disassociateRequests).ToDictionary(x => x.Key, y => y.Value);
            return associateOrDisassociateRequests;
        }

        //Since the Source and target Entities are already filtered based on a particular configuration just searching on the Ids would be sufficent for the M2M matching
        private Dictionary<string, OrganizationRequest> GetAssociateRequests(List<Entity> sourceEntities,
                                                                               List<Entity> targetEntities,
                                                                               Guid configurationId,
                                                                               string intersectEntityName)
        {
            Dictionary<string, OrganizationRequest> associateRequests = new Dictionary<string, OrganizationRequest>();


            foreach (Entity sourceEntity in sourceEntities)
            {
                if (!targetEntities.Any(x => x.Id == sourceEntity.Id))
                {
                    AssociateRequest associateRequest = new AssociateRequest
                    {
                        Target = new EntityReference("msdyusd_configuration", configurationId),
                        RelatedEntities = new EntityReferenceCollection{
                                new EntityReference(sourceEntity.LogicalName, sourceEntity.Id)
                        },
                        Relationship = new Relationship(intersectEntityName)
                    };

                    associateRequests.Add(sourceEntity.Id.ToString(), associateRequest);
                }
            }



            return associateRequests;

        }

        //Since the Source and target Entities are already filtered based on a particular configuration just searching on the Ids would be sufficent for the M2M matching
        private Dictionary<string, OrganizationRequest> GetDisassociateRequests(
                                                               List<Entity> sourceEntities,
                                                               List<Entity> targetEntities,
                                                               Guid configurationId,
                                                               string intersectEntityName)
        {
            Dictionary<string, OrganizationRequest> disassociateRequests = new Dictionary<string, OrganizationRequest>();

            foreach (Entity targetEntity in targetEntities)
            {
                if (!sourceEntities.Any(x => x.Id == targetEntity.Id))
                {
                    DisassociateRequest disassociateRequest = new DisassociateRequest
                    {
                        Target = new EntityReference("msdyusd_configuration", configurationId),
                        RelatedEntities = new EntityReferenceCollection
                            {
                                new EntityReference(targetEntity.LogicalName, targetEntity.Id)
                            },
                        Relationship = new Relationship(intersectEntityName)
                    };

                    disassociateRequests.Add(targetEntity.Id.ToString(), disassociateRequest);
                }
            }



            return disassociateRequests;

        }








        private Dictionary<string, OrganizationRequest> GetM2MAssociateAndDisassociateRequests(List<Entity> sourceEntities,
                                                         List<Entity> targetEntities,
                                                          CRMM2MEntityMapping m2mEntityMapping)
        {

            //Associate M2M Non Configuration Entity Intersect Records : Ex- msdyusd_subactioncalls, msdyusd_answer_agentscriptaction etc.
            Dictionary<string, OrganizationRequest> associateRequests = GetM2MAssociateRequests(sourceEntities,
                                                                                            targetEntities,
                                                                                            m2mEntityMapping);




            //Disassociate M2M Non Configuration Entity Intersect Records : Ex- msdyusd_subactioncalls, msdyusd_answer_agentscriptaction etc.
            Dictionary<string, OrganizationRequest> disassociateRequests = GetM2MDisassociateRequests(sourceEntities,
                                                                                                   targetEntities,
                                                                                                   m2mEntityMapping);

            Dictionary<string, OrganizationRequest> associateOrDisassociateRequests = associateRequests.Union(disassociateRequests).ToDictionary(x => x.Key, y => y.Value);
            return associateOrDisassociateRequests;
        }


        private Dictionary<string, OrganizationRequest> GetM2MAssociateRequests(List<Entity> sourceEntities,
                                                                       List<Entity> targetEntities,
                                                                       CRMM2MEntityMapping m2mEntityMapping)
        {
            Dictionary<string, OrganizationRequest> associateRequests = new Dictionary<string, OrganizationRequest>();


            foreach (Entity sourceEntity in sourceEntities)
            {
                Guid sourceEntity1Id = sourceEntity.GetAttributeValue<Guid>(m2mEntityMapping.Entity1Attribute);
                Guid sourceEntity2Id = sourceEntity.GetAttributeValue<Guid>(m2mEntityMapping.Entity2Attribute);

                string requestId = GetRequestId(sourceEntity1Id, sourceEntity2Id);
                bool associate = false;
                if (m2mEntityMapping.Entity1 == m2mEntityMapping.Entity2)//Ex;- SubActionCalls
                {

                    if (!targetEntities.Any(x => (x.GetAttributeValue<Guid>(m2mEntityMapping.Entity1Attribute) == sourceEntity1Id &&
                                                  x.GetAttributeValue<Guid>(m2mEntityMapping.Entity2Attribute) == sourceEntity2Id) ||
                                                 (x.GetAttributeValue<Guid>(m2mEntityMapping.Entity2Attribute) == sourceEntity1Id &&
                                                  x.GetAttributeValue<Guid>(m2mEntityMapping.Entity1Attribute) == sourceEntity2Id)))
                    {
                        associate = true;
                    }

                }
                else if (!targetEntities.Any(x => x.GetAttributeValue<Guid>(m2mEntityMapping.Entity1Attribute) == sourceEntity1Id &&
                                             x.GetAttributeValue<Guid>(m2mEntityMapping.Entity2Attribute) == sourceEntity2Id))
                {

                    associate = true;
                }

                if (associate)
                {
                    AssociateRequest associateRequest = GetAssociateRequestM2M(m2mEntityMapping, sourceEntity1Id, sourceEntity2Id);
                    associateRequests.Add(requestId, associateRequest);
                }
            }



            return associateRequests;

        }


        private Dictionary<string, OrganizationRequest> GetM2MDisassociateRequests(List<Entity> sourceEntities,
                                                                List<Entity> targetEntities,
                                                                CRMM2MEntityMapping m2mEntityMapping)
        {
            Dictionary<string, OrganizationRequest> disassociateRequests = new Dictionary<string, OrganizationRequest>();


            foreach (Entity targetEntity in targetEntities)
            {
                Guid targetEntity1Id = targetEntity.GetAttributeValue<Guid>(m2mEntityMapping.Entity1Attribute);
                Guid targetEntity2Id = targetEntity.GetAttributeValue<Guid>(m2mEntityMapping.Entity2Attribute);
                string requestId = GetRequestId(targetEntity1Id, targetEntity2Id);

                bool disassociate = false;

                if (m2mEntityMapping.Entity1 == m2mEntityMapping.Entity2)//Ex;- SubActionCalls
                {

                    if (!sourceEntities.Any(x => (x.GetAttributeValue<Guid>(m2mEntityMapping.Entity1Attribute) == targetEntity1Id &&
                                                  x.GetAttributeValue<Guid>(m2mEntityMapping.Entity2Attribute) == targetEntity2Id) ||
                                                 (x.GetAttributeValue<Guid>(m2mEntityMapping.Entity2Attribute) == targetEntity1Id &&
                                                  x.GetAttributeValue<Guid>(m2mEntityMapping.Entity1Attribute) == targetEntity2Id)))
                    {
                        disassociate = true;
                    }
                }
                else if (!sourceEntities.Any(x => x.GetAttributeValue<Guid>(m2mEntityMapping.Entity1Attribute) == targetEntity1Id &&
                                             x.GetAttributeValue<Guid>(m2mEntityMapping.Entity2Attribute) == targetEntity2Id))
                {
                    disassociate = true;
                }

                if (disassociate)
                {
                    DisassociateRequest disassociateRequest = GetDisassociateRequestM2M(m2mEntityMapping, targetEntity1Id, targetEntity2Id);
                    disassociateRequests.Add(requestId, disassociateRequest);
                }
            }



            return disassociateRequests;

        }


        private AssociateRequest GetAssociateRequestM2M(CRMM2MEntityMapping m2mEntityMapping, Guid entity1Id, Guid entity2Id)
        {
            AssociateRequest associateRequest = new AssociateRequest
            {
                Target = new EntityReference(m2mEntityMapping.Entity1, entity1Id),
                RelatedEntities = new EntityReferenceCollection{
                                new EntityReference(m2mEntityMapping.Entity2, entity2Id)
                        },
                Relationship = new Relationship(m2mEntityMapping.IntersectEntity)
            };

            if (m2mEntityMapping.Entity1 == m2mEntityMapping.Entity2)
            {
                associateRequest.Relationship.PrimaryEntityRole = EntityRole.Referencing;
            }

            return associateRequest;
        }


        private DisassociateRequest GetDisassociateRequestM2M(CRMM2MEntityMapping m2mEntityMapping, Guid entity1Id, Guid entity2Id)
        {
            DisassociateRequest disassociateRequest = new DisassociateRequest
            {
                Target = new EntityReference(m2mEntityMapping.Entity1, entity1Id),
                RelatedEntities = new EntityReferenceCollection{
                                new EntityReference(m2mEntityMapping.Entity2, entity2Id)
                        },
                Relationship = new Relationship(m2mEntityMapping.IntersectEntity)
            };

            if (m2mEntityMapping.Entity1 == m2mEntityMapping.Entity2)
            {
                disassociateRequest.Relationship.PrimaryEntityRole = EntityRole.Referencing;
            }

            return disassociateRequest;
        }


        private string GetRequestId(Guid entity1Id, Guid entity2Id)
        {


            return GetCleanedGuid(entity1Id) + "|" + GetCleanedGuid(entity2Id);
        }

        public string GetCleanedGuid(Guid id)
        {
            return id.ToString().Replace("-", "");
        }
    }
}

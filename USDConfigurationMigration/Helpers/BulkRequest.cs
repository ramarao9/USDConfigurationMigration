using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MayoEnterprise.Libraries.Xrm
{

    public enum Operation
    {
        Create = 1,
        Update = 2
    }


    public class BulkResponseItem
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string BulkRequestId { get; set; }
        public Guid CreatedRecordId { get; set; }

        public string Action { get; set; }
        public EntityCollection Results { get; set; }
    }

    public class BulkResponse
    {
        public bool HasErrors { get; set; }
        public List<BulkResponseItem> Responses { get; set; }
    }


    public class BulkRequest
    {
        private int _batchSize;

        public BulkRequest(int batchSize)
        {
            _batchSize = batchSize;
        }

        public BulkResponse Write(IOrganizationService service, Dictionary<string, OrganizationRequest> requestsToBeProcessed)
        {
            BulkResponse bulkWriteResponse = new BulkResponse();
            bulkWriteResponse.Responses = new List<BulkResponseItem>();

            int batchNumber = 0;
            int recordsToProcessCount = requestsToBeProcessed.Count;
            while (recordsToProcessCount > _batchSize || recordsToProcessCount > 0)
            {
                Dictionary<string, OrganizationRequest> currentBatchOfRequests = requestsToBeProcessed.Skip(batchNumber * _batchSize).Take(_batchSize).ToDictionary(x => x.Key, x => x.Value);

                OrganizationRequestCollection orgReqs = new OrganizationRequestCollection();
                orgReqs.AddRange(currentBatchOfRequests.Values.ToList());

                ExecuteMultipleResponse response = ExecuteMultiple(service, orgReqs);

                List<BulkResponseItem> bulkResponses = ReadExecuteMultipleResponse(response, currentBatchOfRequests);
                bulkWriteResponse.Responses.AddRange(bulkResponses);
                bulkWriteResponse.HasErrors = response.IsFaulted;


                recordsToProcessCount = recordsToProcessCount - currentBatchOfRequests.Count;
                batchNumber++;
            }




            return bulkWriteResponse;
        }

        public BulkResponse Write(IOrganizationService service, Dictionary<string, Entity> recordsToBeProcessed, Operation operation)
        {
            Dictionary<string, OrganizationRequest> orgReqs = GetOrgRequests(recordsToBeProcessed, operation);
            BulkResponse bulkWriteResponse = Write(service, orgReqs);

            return bulkWriteResponse;
        }

        public BulkResponse Read(IOrganizationService service, Dictionary<string, QueryExpression> queriesToBeProcessed)
        {
            BulkResponse bulkWriteResponse = new BulkResponse();
            bulkWriteResponse.Responses = new List<BulkResponseItem>();


            Dictionary<string, OrganizationRequest> orgRequests = GetQueryOrgRequests(queriesToBeProcessed);
            OrganizationRequestCollection orgReqs = new OrganizationRequestCollection();
            orgReqs.AddRange(orgRequests.Values.ToList());

            ExecuteMultipleResponse response = ExecuteMultiple(service, orgReqs);

            List<BulkResponseItem> bulkResponses = ReadExecuteMultipleResponse(response, orgRequests);
            bulkWriteResponse.Responses.AddRange(bulkResponses);
            bulkWriteResponse.HasErrors = response.IsFaulted;


            return bulkWriteResponse;
        }

        private Dictionary<string, OrganizationRequest> GetQueryOrgRequests(Dictionary<string, QueryExpression> queriesToBeProcessed)
        {
            Dictionary<string, OrganizationRequest> orgRequests = new Dictionary<string, OrganizationRequest>();


            foreach (var queryToBeProcessed in queriesToBeProcessed)
            {

                RetrieveMultipleRequest retrieveMultipleRequest = new RetrieveMultipleRequest { Query = queryToBeProcessed.Value };
                orgRequests.Add(queryToBeProcessed.Key, retrieveMultipleRequest);
            }

            return orgRequests;
        }




        private Dictionary<string, OrganizationRequest> GetOrgRequests(Dictionary<string, Entity> recordsToBeProcessed, Operation operation)
        {
            Dictionary<string, OrganizationRequest> orgReqs = new Dictionary<string, OrganizationRequest>();

            foreach (var recordKvp in recordsToBeProcessed)
            {

                if (operation == Operation.Create)
                {
                    CreateRequest createRequest = new CreateRequest { Target = recordKvp.Value };
                    orgReqs.Add(recordKvp.Key, createRequest);
                }
                else if (operation == Operation.Update)
                {
                    UpdateRequest updateRequest = new UpdateRequest { Target = recordKvp.Value };
                    orgReqs.Add(recordKvp.Key, updateRequest);
                }

            }

            return orgReqs;
        }

        private OrganizationRequestCollection GetOrgRequestsForCreateOrUpdate(Dictionary<string, Entity> recordsToBeProcessed, Operation operation)
        {
            OrganizationRequestCollection orgReqs = new OrganizationRequestCollection();

            foreach (var recordKvp in recordsToBeProcessed)
            {
                if (operation == Operation.Create)
                {
                    CreateRequest createRequest = new CreateRequest { Target = recordKvp.Value };
                    orgReqs.Add(createRequest);
                }
                else if (operation == Operation.Update)
                {
                    UpdateRequest updateRequest = new UpdateRequest { Target = recordKvp.Value };
                    orgReqs.Add(updateRequest);
                }

            }

            return orgReqs;
        }

        private ExecuteMultipleResponse ExecuteMultiple(IOrganizationService service, OrganizationRequestCollection orgReqs)
        {
            ExecuteMultipleRequest executeMultipleReq = new ExecuteMultipleRequest();
            executeMultipleReq.Settings = new ExecuteMultipleSettings
            {
                ContinueOnError = true,
                ReturnResponses = true
            };

            executeMultipleReq.Requests = orgReqs;



            ExecuteMultipleResponse execMultipleResponse = (ExecuteMultipleResponse)service.Execute(executeMultipleReq);
            return execMultipleResponse;
        }

        private List<BulkResponseItem> ReadExecuteMultipleResponse(ExecuteMultipleResponse executeMultipleResponse, Dictionary<string, OrganizationRequest> requests)
        {

            List<string> requestIds = requests.Keys.ToList();

            List<BulkResponseItem> responses = new List<BulkResponseItem>();

            if (executeMultipleResponse != null && executeMultipleResponse.Responses != null && executeMultipleResponse.Responses.Count > 0)
            {
                ExecuteMultipleResponseItemCollection responseItems = executeMultipleResponse.Responses;

                foreach (ExecuteMultipleResponseItem responseItem in responseItems)
                {

                    string requestKey = requestIds[responseItem.RequestIndex];

                    OrganizationRequest orgRequest = requests[requestKey];


                    BulkResponseItem bulkResponseItem = new BulkResponseItem();
                    bulkResponseItem.BulkRequestId = requestKey;
                    bulkResponseItem.Action = GetActionName(orgRequest);

                    if (responseItem.Fault != null)
                    {
                        string message = GetFaultedMessage(responseItem);
                        bulkResponseItem.Error = message;
                    }
                    else
                    {
                        OrganizationResponse response = responseItem.Response;
                        Guid recordId = Guid.Empty;
                        if (response is CreateResponse)
                        {
                            bulkResponseItem.CreatedRecordId = ((CreateResponse)response).id;

                        }
                        else if (response is RetrieveMultipleResponse)
                        {
                            RetrieveMultipleResponse retrieveMultipleResponse = (RetrieveMultipleResponse)response;
                            bulkResponseItem.Results = (retrieveMultipleResponse != null) ? retrieveMultipleResponse.EntityCollection : null;

                        }

                        bulkResponseItem.Success = true;

                    }

                    responses.Add(bulkResponseItem);

                }
            }
            return responses;
        }


        private string GetActionName(OrganizationRequest request)
        {
            string actionName = "";
            if (request is CreateRequest)
            {
                actionName = "Create";
            }
            else if (request is RetrieveMultipleRequest)
            {
                actionName = "RetrieveMultiple";
            }
            else if (request is UpdateRequest)
            {
                actionName = "Update";
            }
            else if (request is DeleteRequest)
            {
                actionName = "Delete";
            }
            else if (request is SetStateRequest)
            {
                actionName = "SetState";
            }
            else if (request is AssociateRequest)
            {
                actionName = "AssociateRequest";
            }
            else if (request is DisassociateRequest)
            {
                actionName = "DisassociateRequest";
            }

            return actionName;
        }
        private string GetFaultedMessage(ExecuteMultipleResponseItem responseItem)
        {

            string message = string.Empty;
            if (responseItem.Fault == null)
                return message;

            if (responseItem.Fault.Timestamp != null)
            {
                message = responseItem.Fault.Timestamp.ToString() + " ";
            }


            if (responseItem.Fault.Message != null)
            {
                message = message + responseItem.Fault.Message;
            }


            if (responseItem.Fault.InnerFault != null && responseItem.Fault.InnerFault.Message != null)
            {
                message = message + " Inner Fault : " + responseItem.Fault.InnerFault.Message;
            }

            return message;
        }
    }
}

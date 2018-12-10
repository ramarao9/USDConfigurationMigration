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

        public EntityCollection Results { get; set; }
    }

    public class BulkResponse
    {
        public List<BulkResponseItem> Responses { get; set; }
    }


    public class BulkDataManager
    {
        public BulkResponse Write(IOrganizationService service, Dictionary<string, Entity> recordsToBeProcessed, Operation operation)
        {

            OrganizationRequestCollection orgReqs = GetOrgRequestsForCreateOrUpdate(recordsToBeProcessed, operation);

            ExecuteMultipleResponse response = ExecuteMultiple(service, orgReqs);



            BulkResponse bulkWriteResponse = ReadExecuteMultipleResponse(response, recordsToBeProcessed.Keys.ToList());
            return bulkWriteResponse;
        }



        public BulkResponse Read(IOrganizationService service, Dictionary<string, QueryExpression> queriesToBeProcessed)
        {
            OrganizationRequestCollection orgReqs = GetOrgRequestsForRetrieveMultiple(queriesToBeProcessed);
            ExecuteMultipleResponse response = ExecuteMultiple(service, orgReqs);


            BulkResponse bulkWriteResponse = ReadExecuteMultipleResponse(response, queriesToBeProcessed.Keys.ToList());
            return bulkWriteResponse;
        }

        private OrganizationRequestCollection GetOrgRequestsForRetrieveMultiple(Dictionary<string, QueryExpression> queriesToBeProcessed)
        {
            OrganizationRequestCollection orgReqs = new OrganizationRequestCollection();

            foreach (var queryRecord in queriesToBeProcessed)
            {
                RetrieveMultipleRequest retrieveMultipleRequest = new RetrieveMultipleRequest { Query = queryRecord.Value };
                orgReqs.Add(retrieveMultipleRequest);
            }

            return orgReqs;
        }


        private OrganizationRequestCollection GetOrgRequestsForCreateOrUpdate(Dictionary<string, Entity> recordsToBeProcessed, Operation operation)
        {
            OrganizationRequestCollection orgReqs = new OrganizationRequestCollection();

            foreach (var recordKvp in recordsToBeProcessed)
            {
                // Guid requestId = Guid.NewGuid();
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


        private BulkResponse ReadExecuteMultipleResponse(ExecuteMultipleResponse executeMultipleResponse, List<string> recordIds)
        {

            List<BulkResponseItem> responses = new List<BulkResponseItem>();

            if (executeMultipleResponse != null && executeMultipleResponse.Responses != null && executeMultipleResponse.Responses.Count > 0)
            {
                ExecuteMultipleResponseItemCollection responseItems = executeMultipleResponse.Responses;

                foreach (ExecuteMultipleResponseItem responseItem in responseItems)
                {

                    string recordKey = recordIds[responseItem.RequestIndex];


                    BulkResponseItem bulkResponseItem = new BulkResponseItem();
                    bulkResponseItem.BulkRequestId = recordKey;


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
            return new BulkResponse { Responses = responses };
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

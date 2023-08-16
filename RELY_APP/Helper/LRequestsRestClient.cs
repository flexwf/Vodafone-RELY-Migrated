using Newtonsoft.Json;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Helper
{
    public class LRequestsRestClient : ILRequestsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        public LRequestsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public RWorkflowViewModel GetWFDetails(int WorkFlowId, string CompanyCode)
        {
            var request = new RestRequest("api/LRequests/GetWFDetails?WorkFlowId={WorkFlowId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json, };
            request.AddParameter("WorkFlowId", WorkFlowId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<RWorkflowViewModel>(request);
            return response.Data;
        }
        public int GetSurveyCompletionStatus(int RequestId, string CompanyCode)
        {
            var request = new RestRequest("api/LRequests/GetSurveyCompletionStatus?RequestId={RequestId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json, };
            request.AddParameter("RequestId", RequestId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        public IEnumerable<LRequestViewModel> GetLRequest()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRequests/GetLRequest?UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json, };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LRequestViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public IEnumerable<LRequestViewModel> GetByCompanyCode(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRequests/GetByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LRequestViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        public LRequestViewModel GetById(int Id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRequests/GetById?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LRequestViewModel>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public List<string> GetRequestNamesByOpco(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRequests/GetRequestNamesByOpco?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);

            return response.Data;
        }

        public List<dynamic> GetSurveySummary(int? SurveyId, int EntityId, string EntityType)
        {
            var request = new RestRequest("api/LRequests/GetSurveySummary?SurveyId={SurveyId}&EntityId={EntityId}&EntityType={EntityType}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SurveyId", SurveyId, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);

            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;

        }



        public int Add(LRequestViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRequests/PostLRequest?UserName={UserName}&WorkFlow={WorkFlow}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("FileList", FileList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("OriginalFileNameList", OriginalFileNameList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocumentsDescription", SupportingDocumentsDescription, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }
        public int Update(LRequestViewModel model, int Id, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRequests/PutLRequest?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("FileList", FileList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("OriginalFileNameList", OriginalFileNameList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocumentsDescription", SupportingDocumentsDescription, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }

        //This Function will check that Accounting Memo for given RequestId is possible or not
        public string CheckFeasibilityOfAccMemo(int RequestId)
        {
            var request = new RestRequest("api/LRequests/CheckFeasibilityOfAccMemo?RequestId={RequestId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RequestId", RequestId, ParameterType.UrlSegment);

            var response = _client.Execute(request);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return null;
            }
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;

        }

        public string GetRequestLevelAccMemo(int RequestId, int LoggedInUserId, string CompanyCode)
        {
            var request = new RestRequest("api/LRequests/GetRequestLevelAccMemo?RequestId={RequestId}&LoggedInUserId={LoggedInUserId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RequestId", RequestId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return null;
            }
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;

        }

        public DataTable UploadPPM(string FileName, string LoggedInRoleId, string iCompanyCode, string UpdatedBy, string RedirectToUrl)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(WorkflowName))
            {
                WorkflowName = "No Workflow";
            }
          
            var request = new RestRequest("api/LRequests/GetUploadPPM?FileName={FileName}&UserName={UserName}&LoggedInRoleId={LoggedInRoleId}&iCompanyCode={iCompanyCode}&WorkflowName={WorkflowName}&UpdatedBy={UpdatedBy}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("FileName", FileName, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("iCompanyCode", iCompanyCode, ParameterType.UrlSegment);
            request.AddParameter("WorkflowName", WorkflowName, ParameterType.UrlSegment);
            request.AddParameter("UpdatedBy", UpdatedBy, ParameterType.UrlSegment);
            var response =_client.Execute<dynamic>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                if (string.IsNullOrEmpty(RedirectToUrl))
                {
                    RedirectToUrl = "/Home/ErrorPage";
                }
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", (int)response.StatusCode);
                ex.Data.Add("RedirectToUrl", RedirectToUrl);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;

            }
            var res = JsonConvert.DeserializeObject<DataTable>(response.Content);
            return res;
        }


        public IEnumerable<LRequestsUploadViewModelForReviewGrid> GetGridDataFields(int CompanyId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRequests/GetGridDataFields?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LRequestsUploadViewModelForReviewGrid>>(request);
            //var res = response.Data.ToString();
            return response.Data;
        }

        public int GetXUploadLRequestCountByBatchNumber(string CompanyCode, int BatchNumber)
        {
            var request = new RestRequest("api/LRequests/GetXUploadLRequestCountByBatchNumber?CompanyId={CompanyCode}&BatchNumber={BatchNumber}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }

        public dynamic GetXUploadLRequestByBatchNumber(int CompanyId, int BatchNumber, string sortdatafield, string sortorder, int? pagesize, int? pagenum, string FilterQuery)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRequests/GetXUploadLRequestByBatchNumber?CompanyId={CompanyId}&BatchNumber={BatchNumber}&sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            return response.Data;
        }

        public IEnumerable<LBatchViewModelGrid> GetByUserForRequestUploadGrid(string CompanyCode, string LoggedInUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRequests/GetByUserForLRequestUploadGrid?CompanyCode={CompanyCode}&AspnetUserid={LoggedInUserId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LBatchViewModelGrid>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void DeleteRequestUploadBatch(int Id)
        {
            var request = new RestRequest("api/LRequests/DeleteLRequestUploadBatch?Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            return;
        }

        public LBatchViewModelForRequestGrid GetDetailsById(string CompanyCode, int Id)
        {
            var request = new RestRequest("api/LRequests/GetById?CompanyCode={CompanyCode}&Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<LBatchViewModelForRequestGrid>(request);
            return response.Data;
        }

        public void UploadValidatedRequestBatch(string CompanyCode, int BatchNumber, string AspNetUserId, int LoggedinRoleId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            var request = new RestRequest("api/LRequests/UploadValidatedRequestBatch?CompanyCode={CompanyCode}&BatchNumber={BatchNumber}&AspNetUserId={AspNetUserId}&LoggedinRoleId={LoggedinRoleId}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            request.AddParameter("AspNetUserId", AspNetUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedinRoleId", LoggedinRoleId, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            return;
        }

        public string DownloadRequestUploadErrors(string CompanyCode, int BatchNumber)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRequests/DownloadRequestUploadErrors?CompanyCode={CompanyCode}&BatchNumber={BatchNumber}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            var res = response.Data.ToString();
            return res;
        }
    }
    interface ILRequestsRestClient
    {
        RWorkflowViewModel GetWFDetails(int WorkFlowId, string CompanyCode);
        int GetSurveyCompletionStatus(int RequestId, string CompanyCode);
        IEnumerable<LRequestViewModel> GetByCompanyCode(string CompanyCode);
        int Add(LRequestViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList);
        LRequestViewModel GetById(int Id);
        List<string> GetRequestNamesByOpco(string CompanyCode);
        int Update(LRequestViewModel model, int Id, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList);
        List<dynamic> GetSurveySummary(int? SurveyId, int RequestId, string EntityType);
        string CheckFeasibilityOfAccMemo(int RequestId);
        string GetRequestLevelAccMemo(int RequestId, int LoggedInUserId, string CompanyCode);
        DataTable UploadPPM(string FileName, string LoggedInRoleId, string iCompanyId, string UpdatedBy, string RedirectToUrl);
        IEnumerable<LRequestsUploadViewModelForReviewGrid> GetGridDataFields(int CompanyId);
        int GetXUploadLRequestCountByBatchNumber(string CompanyCode, int BatchNumber);

        dynamic GetXUploadLRequestByBatchNumber(int CompanyId, int BatchNumber, string sortdatafield, string sortorder, int? pagesize, int? pagenum, string FilterQuery);

        IEnumerable<LBatchViewModelGrid> GetByUserForRequestUploadGrid(string CompanyCode, string LoggedInUserId);
        //string DownloadLRequestUploadErrors(int CompanyId, int BatchNumber);
        void DeleteRequestUploadBatch(int Id);

        LBatchViewModelForRequestGrid GetDetailsById(string CompanyCode, int Id);

        void UploadValidatedRequestBatch(string CompanyCode, int BatchNumber, string AspNetUserId, int LoggedinRoleId);

        string DownloadRequestUploadErrors(string CompanyCode, int BatchNumber);
    }

}
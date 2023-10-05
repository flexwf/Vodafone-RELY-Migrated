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

namespace RELY_APP.Helper
{
    public class SSPDimensionsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public SSPDimensionsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public SSPDimensionViewModel GetById(int Id)
        {
            var request = new RestRequest("api/SSPDimensions/GetById?Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<SSPDimensionViewModel>(request);
            return response.Data;
        }

        public IEnumerable<SSPDimensionViewModel> GetBySSPId(int SspId, string CompanyCode)
        {
            var request = new RestRequest("api/SSPDimensions/GetBySspId?SspId={SspId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SspId", SspId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<SSPDimensionViewModel>>(request);
            return response.Data;
        }

        //GetSSPIdForNew
        public int GetSSPIdForNew(string CompanyCode)
        {
            var request = new RestRequest("api/SSPDimensions/GetSSPIdForNew?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        public void Add(SSPDimensionViewModel serverData, string Source, string EntityId, string RedirectToUrl)
        {
            var request = new RestRequest("api/SSPDimensions/Post?Source={Source}&EntityId={EntityId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("Source", Source, ParameterType.UrlSegment);
            request.AddParameter("EntityId", string.IsNullOrEmpty(EntityId) ? "" : EntityId, ParameterType.UrlSegment);
            request.AddBody(serverData);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
        public void Update(SSPDimensionViewModel serverData, string RedirectToUrl)
        {
            var request = new RestRequest("api/SSPDimensions/Put?Id={Id}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);
            var response = _client.Execute<SSPDimensionViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
        public void Delete(int id, string RedirectToUrl)
        {

            var request = new RestRequest("api/SSPDimensions/Delete?id={id}", Method.DELETE);

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }
        public List<dynamic> GetDataForGrid(string EntityType, int EntityId,string CompanyCode)
        {
            var request = new RestRequest("api/SSPDimensions/GetDataForGrid?EntityType={EntityType}&EntityId={EntityId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }
        public int GetExistingSspsCount(string CompanyCode)
        {
            var request = new RestRequest("api/SSPDimensions/GetExistingSspsCount?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        public List<dynamic> GetExistingSsps(string CompanyCode, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery)
        {
            var request = new RestRequest("api/SSPDimensions/GetExistingSsps?CompanyCode={CompanyCode}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&PageNumber={PageNumber}&PageSize={PageSize}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }
        public void DetachSSP(int EntityId,string EntityType, string RedirectToUrl)
        {
            var request = new RestRequest("api/SSPDimensions/DetachSSP?EntityId={EntityId}&EntityType={EntityType}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
        public void AttachSSP(int EntityId, string EntityType, int SspId, string RedirectToUrl)
        {
            var request = new RestRequest("api/SSPDimensions/AttachSSP?EntityId={EntityId}&SspId={SspId}&EntityType={EntityType}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("SspId", SspId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LoggedInUserId"></param>
        /// <param name="Workflow"></param>
        /// <param name="LoggedInRoleId"></param>
        /// <param name="CompanyCode"></param>
        /// <returns></returns>
        public GenericNameAndIdViewModel DownloadGetGridDataByWorkflowId(int LoggedInUserId, string Workflow, int LoggedInRoleId, string CompanyCode)
        {
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/SSPDimensions/DownloadGenericDataGrid?Workflow={Workflow}&UserName={UserName}&LoggedInUserId={LoggedInUserId}&LoggedInRoleId={LoggedInRoleId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<GenericNameAndIdViewModel>(request);
            return response.Data;
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CompanyCode"></param>
        /// <param name="LoggedInUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<LBatchViewModelGrid> GetByUserForRequestUploadGrid(string CompanyCode, string LoggedInUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/SSPDimensions/GetByUserForLRequestUploadGrid?CompanyCode={CompanyCode}&AspnetUserid={LoggedInUserId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LBatchViewModelGrid>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="LoggedInRoleId"></param>
        /// <param name="iCompanyCode"></param>
        /// <param name="UpdatedBy"></param>
        /// <param name="RedirectToUrl"></param>
        /// <returns></returns>
        public DataTable UploadSSPs(string FileName, string LoggedInRoleId, string iCompanyCode, string UpdatedBy, string RedirectToUrl)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(WorkflowName))
            {
                WorkflowName = "No Workflow";
            }

            var request = new RestRequest("api/SSPDimensions/GetUploadSSPs?FileName={FileName}&UserName={UserName}&LoggedInRoleId={LoggedInRoleId}&iCompanyCode={iCompanyCode}&WorkflowName={WorkflowName}&UpdatedBy={UpdatedBy}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("FileName", FileName, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("iCompanyCode", iCompanyCode, ParameterType.UrlSegment);
            request.AddParameter("WorkflowName", WorkflowName, ParameterType.UrlSegment);
            request.AddParameter("UpdatedBy", UpdatedBy, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CompanyCode"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public LBatchViewModelForRequestGrid GetDetailsById(string CompanyCode, int Id)
        {
            var request = new RestRequest("api/SSPDimensions/GetById?CompanyCode={CompanyCode}&Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<LBatchViewModelForRequestGrid>(request);
            return response.Data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CompanyCode"></param>
        /// <param name="BatchNumber"></param>
        /// <returns></returns>
        public string DownloadRequestUploadErrors(string CompanyCode, int BatchNumber)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/SSPDimensions/DownloadRequestUploadErrors?CompanyCode={CompanyCode}&BatchNumber={BatchNumber}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            var res = response.Data.ToString();
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CompanyCode"></param>
        /// <param name="BatchNumber"></param>
        /// <param name="AspNetUserId"></param>
        /// <param name="LoggedinRoleId"></param>
        public void UploadValidatedRequestBatch(string CompanyCode, int BatchNumber, string AspNetUserId, int LoggedinRoleId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            var request = new RestRequest("api/SSPDimensions/UploadValidatedRequestBatch?CompanyCode={CompanyCode}&BatchNumber={BatchNumber}&AspNetUserId={AspNetUserId}&LoggedinRoleId={LoggedinRoleId}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            request.AddParameter("AspNetUserId", AspNetUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedinRoleId", LoggedinRoleId, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            return;
        }
    }
}
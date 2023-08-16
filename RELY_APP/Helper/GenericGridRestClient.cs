using System;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;

namespace RELY_APP.Helper
{
    public class GenericGridRestClient:IGenericGridRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
       
        //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
        public GenericGridRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public RWorkflowViewModel GetWorkflowDetails(string WorkFlow, string CompanyCode)
        {
            var request = new RestRequest("api/GenericGrid/GetWorkFlowDetails?Workflow={Workflow}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Workflow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<RWorkflowViewModel>(request);

            return response.Data;
        }

        public IEnumerable<GenericGridColumnsViewModel> GetGenericGridColumnsByWorkflow(string CompanyCode,string WorkFlow)
        {
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/GenericGrid/GetWorkflowGridColumnsByWorkflow?WorkFlow={WorkFlow}&UserName={UserName}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<GenericGridColumnsViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<WStepsViewModel> GetTabsByWorkflow(string CompanyCode, string WorkFlow, int LoggedInRoleId, int LoggedInUserId)
        {
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/GenericGrid/GetStepsByWorkflow?WorkFlow={WorkFlow}&UserName={UserName}&CompanyCode={CompanyCode}&LoggedInUserId={LoggedInUserId}&LoggedInRoleId={LoggedInRoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            var response = _client.Execute<List<WStepsViewModel>>(request);

            return response.Data;
        }
        //
        public int GetGenericGridCounts(int StepId, int LoggedInUserId, string Workflow)
        {
            string CompanyCode = Globals.GetSessionData("CompanyCode");
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/GenericGrid/GetGenericGridCounts?Workflow={Workflow}&UserName={UserName}&StepId={StepId}&LoggedInUserId={LoggedInUserId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("StepId", StepId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            return response.Data;
        }
        //
        public string GetGenericGridTopLinks(int LoggedInRoleId, int LoggedInUserId, string Workflow,string CompanyCode)
        {
            var request = new RestRequest("api/GenericGrid/GetTopActionLinks?Workflow={Workflow}&CompanyCode={CompanyCode}&LoggedInUserId={LoggedInUserId}&LoggedInRoleId={LoggedInRoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.Content == null)
                return null;
            dynamic data = JsonConvert.DeserializeObject(response.Content);
            return data;
        }

        public IEnumerable<dynamic> GetGridDataByWorkflowId(int StepId, int LoggedInUserId, string Workflow, int PageSize, int PageNumber, string sortdatafield, string sortorder, string FilterQuery,int LoggedInRoleId)
        {
            string CompanyCode = Globals.GetSessionData("CompanyCode");
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/GenericGrid/GetGenericGridData?Workflow={Workflow}&UserName={UserName}&StepId={StepId}&LoggedInUserId={LoggedInUserId}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&PageNumber={PageNumber}&PageSize={PageSize}&LoggedInRoleId={LoggedInRoleId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("StepId", StepId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            //send data
            return response.Data;
        }
        
        public void UpdateActionStatus(string WorkFlowName, string TransactionId, string CompanyCode, string Action, int LoggedInUserId, string Comments, int LoggedInRoleId, string AssigneeId)
        {
            if (string.IsNullOrEmpty(Comments))
                Comments = "Empty";//"SS is passing text to avoid error then will replace it from empty string in SP
            //var request = new RestRequest("api/GenericGrid/UpdateActionStatus?Action={Action}&WorkFlowName={WorkFlowName}&TransactionId={TransactionId}&CompanyCode={CompanyCode}&LoggedInUserId={LoggedInUserId}&Comments={Comments}&LoggedInRoleId={LoggedInRoleId}&AssigneeId={AssigneeId}", Method.GET) { RequestFormat = DataFormat.Json };
            var request = new RestRequest("api/GenericGrid/UpdateActionStatus?Action={Action}&WorkFlowName={WorkFlowName}&CompanyCode={CompanyCode}&LoggedInUserId={LoggedInUserId}&Comments={Comments}&LoggedInRoleId={LoggedInRoleId}&AssigneeId={AssigneeId}", Method.POST) { RequestFormat = DataFormat.Json };
            OtherAPIData objTrans = new OtherAPIData();
            objTrans.TransactionID = TransactionId;
            request.AddBody(objTrans);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("Action", Action, ParameterType.UrlSegment);
            request.AddParameter("WorkFlowName", WorkFlowName, ParameterType.UrlSegment);
            //request.AddParameter("TransactionId", TransactionId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("AssigneeId", string.IsNullOrEmpty(AssigneeId) ? "" : AssigneeId, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
            }

        }

        public List<string> GetGridBottomButtons(string Workflow, string CompanyCode, int LoggedInRoleId, int LoggedInUserId)
        {
            var request = new RestRequest("api/GenericGrid/GetGridBottomActionItems?Workflow={Workflow}&CompanyCode={CompanyCode}&LoggedInRoleId={LoggedInRoleId}&LoggedInUserId={LoggedInUserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return null;
            }
            return response.Data;
        }
        public string GetFormBottomButtons(string Workflow, string CompanyCode, int LoggedInRoleId, int LoggedInUserId,int StepId,string FormType)
        {
            var request = new RestRequest("api/GenericGrid/GetFormBottomActionItems?Workflow={Workflow}&CompanyCode={CompanyCode}&LoggedInRoleId={LoggedInRoleId}&LoggedInUserId={LoggedInUserId}&StepId={StepId}&FormType={FormType}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("StepId", StepId, ParameterType.UrlSegment);
            request.AddParameter("FormType", FormType, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return null;
            }
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }





        public string ValidateActionResult(int StepParticipantActionId, int EntityId, string CompanyCode)
        {
            var request = new RestRequest("api/GenericGrid/ValidateActionResult?StepParticipantActionId={StepParticipantActionId}&EntityId={EntityId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("StepParticipantActionId", StepParticipantActionId, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return null;
            }
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
            //return response.Data;
        }
        

        public GenericNameAndIdViewModel DownloadGetGridDataByWorkflowId(int StepId, int LoggedInUserId, string Workflow,int LoggedInRoleId, string CompanyCode, string TabName)
        {

           
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/GenericGrid/DownloadGenericDataGrid?Workflow={Workflow}&UserName={UserName}&StepId={StepId}&LoggedInUserId={LoggedInUserId}&LoggedInRoleId={LoggedInRoleId}&CompanyCode={CompanyCode}&TabName={TabName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("StepId", StepId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("TabName", TabName, ParameterType.UrlSegment);
            //TabName
            var response = _client.Execute<GenericNameAndIdViewModel>(request);            
            return response.Data;
        }



    }

    interface IGenericGridRestClient
        {
        List<string> GetGridBottomButtons(string Workflow, string CompanyCode, int LoggedInRoleId, int LoggedInUserId);
        string GetGenericGridTopLinks(int LoggedInRoleId, int LoggedInUserId, string Workflow, string CompanyCode);
        IEnumerable<GenericGridColumnsViewModel> GetGenericGridColumnsByWorkflow(string CompanyCode, string WorkFlow);
        IEnumerable<WStepsViewModel> GetTabsByWorkflow(string CompanyCode, string WorkFlow, int LoggedInRoleId, int LoggedInUserId);
        IEnumerable<dynamic> GetGridDataByWorkflowId(int StepId, int LoggedInUserId, string Workflow, int PageSize, int PageNumber, string sortdatafield, string sortorder, string FilterQuery, int LoggedInRoleId);
        int GetGenericGridCounts(int StepId, int LoggedInUserId, string Workflow);
        RWorkflowViewModel GetWorkflowDetails(string WorkFlow, string CompanyCode);
        void UpdateActionStatus(string WorkFlowName, string TransactionId, string CompanyCode, string Action, int LoggedInUserId, string Comments, int LoggedInRoleId, string AssigneeId);
        string GetFormBottomButtons(string Workflow, string CompanyCode, int LoggedInRoleId, int LoggedInUserId, int StepId, string FormType);
        string ValidateActionResult(int StepParticipantActionId, int EntityId, string CompanyCode);

        GenericNameAndIdViewModel DownloadGetGridDataByWorkflowId(int StepId, int LoggedInUserId, string Workflow, int LoggedInRoleId, string CompanyCode, string TabName);
    }
}
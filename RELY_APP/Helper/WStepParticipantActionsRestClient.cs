using Newtonsoft.Json;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;


namespace RELY_APP.Helper
{
    public class WStepParticipantActionsRestClient : IWStepParticipantActionsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

        public WStepParticipantActionsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<WStepParticipantActionViewModel> GetAll()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/WStepParticipantActions/GetWStepParticipantActions?UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<WStepParticipantActionViewModel>>(request);
            return response.Data;
        }
        
        public IEnumerable<WActionViewModel> GetAllActions()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

            var request = new RestRequest("api/WStepParticipantActions/GetAllActions?UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<WActionViewModel>>(request);
            return response.Data;
        }
        public IEnumerable<WStepParticipantActionViewModel> GetByParticipantIdStepId(int ParticipantId, int StepId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/WStepParticipantActions/GetByParticipantIdStepId?ParticipantId={ParticipantId}&StepId={StepId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ParticipantId", ParticipantId, ParameterType.UrlSegment);
            request.AddParameter("StepId", StepId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<WStepParticipantActionViewModel>>(request);
            return response.Data;
        }

        public StepParticipantActionForWorkflowViewModel GetWFIdStepIdById(int Id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/WStepParticipantActions/GetWFIdStepIdById?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<StepParticipantActionForWorkflowViewModel>(request);
            return response.Data;
        }

        public int GetCount(string ActionName,int LoggedInRoleId, int LoggedInUserId,int WorkFlowId, int StepId)
        {

            var request = new RestRequest("api/WStepParticipantActions/GetCount?ActionName={ActionName}&LoggedInRoleId={LoggedInRoleId}&LoggedInUserId={LoggedInUserId}&WorkFlowId={WorkFlowId}&StepId={StepId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ActionName", ActionName, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("WorkFlowId", WorkFlowId, ParameterType.UrlSegment);
            request.AddParameter("StepId", StepId, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            
            return response.Data;
        }

        public void Add(WStepParticipantActionViewModel model, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/WStepParticipantActions/PostWStepParticipantAction?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);


            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }
        public void Delete(int Id, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/WStepParticipantActions/DeleteById?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.DELETE);

            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            var response = _client.Execute<WStepParticipantActionViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

    }

    interface IWStepParticipantActionsRestClient
    {
        StepParticipantActionForWorkflowViewModel GetWFIdStepIdById(int Id);
        IEnumerable<WStepParticipantActionViewModel> GetAll();
        IEnumerable<WStepParticipantActionViewModel> GetByParticipantIdStepId(int ParticipantId, int StepId);
        int GetCount(string ActionName, int LoggedInRoleId, int LoggedInUserId, int WorkFlowId, int StepId); 
        void Add(WStepParticipantActionViewModel model, string RedirectToUrl);
        void Delete(int Id, string RedirectToUrl);
        IEnumerable<WActionViewModel> GetAllActions();
    }
}
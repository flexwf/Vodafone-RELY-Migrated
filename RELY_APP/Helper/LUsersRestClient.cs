using Newtonsoft.Json;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace RELY_APP.Helper
{
    class LUsersRestClient : ILUsersRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;

        public LUsersRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        //GetLUserByEmail
        public LUserViewModel GetByEmail(string Email)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LUser/GetLUserByEmail?Email={Email}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LUserViewModel>(request);
            return response.Data;
        }

        public LUserViewModel GetById(int Id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LUser/GetLUser/{id}?UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LUserViewModel>(request);
            return response.Data;
        }
        //Add
        public void Add(LUserViewModel serverData, string RedirectToUrl,string RolesList,string Workflow)
        {
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LUser/PostLUser?RolesList={RolesList}&UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("RolesList", RolesList, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", Workflow, ParameterType.UrlSegment);
            request.AddBody(serverData);
            var response = _client.Execute<LUserViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
        //update 
        public void Update(LUserViewModel serverData, string RedirectToUrl,string RolesList,string FormType,int LoggedInUserId,int LoggedInRoleId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LUser/PutLUser/{id}?RolesList={RolesList}&UserName={UserName}&WorkFlow={WorkFlow}&FormType={FormType}&LoggedInUserId={LoggedInUserId}&LoggedInRoleId={LoggedInRoleId}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("RolesList", RolesList, ParameterType.UrlSegment);
            request.AddParameter("FormType", FormType, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LUserViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }


        }
        public void TerminateUser(string Status, int Id, int LoggedInUserId)
        {
             var request = new RestRequest("api/LUser/TerminateUser?Status={Status}&Id={Id}&LoggedInUserId={LoggedInUserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            var response = _client.Execute<LUserViewModel>(request);
            
        }
    }
    interface ILUsersRestClient
    {
        LUserViewModel GetByEmail(string Email);
        LUserViewModel GetById(int Id);
        void Add(LUserViewModel serverData, string RedirectToUrl, string RolesList,string Workflow);
        void Update(LUserViewModel serverData, string RedirectToUrl, string RolesList, string FormType, int LoggedInUserId, int LoggedInRoleId);
        void TerminateUser(string Status, int Id, int LoggedInUserId);
    }
}
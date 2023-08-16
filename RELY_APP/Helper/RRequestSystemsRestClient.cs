using RestSharp;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using RELY_APP.Utilities;

namespace RELY_APP.Helper
{
    public class RRequestSystemsRestClient : IRRequestSystemsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        
        public RRequestSystemsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<RRequestSystemViewModel> GetByCompanyCode(string CompanyCode)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/RRequestSystems/GetByCompanyCode?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<RRequestSystemViewModel>>(request);
            return response.Data;
        }

        public RRequestSystemViewModel GetByName(string Name)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/RRequestSystems/GetByName?Name={Name}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Name", Name, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<RRequestSystemViewModel>(request);
            return response.Data;
        }

        public void Add(RRequestSystemViewModel serverData, string RedirectToUrl)
        {
            var request = new RestRequest("api/RRequestSystems/Post", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
        public void Update(RRequestSystemViewModel model, string RedirectToUrl)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            // string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            //var request = new RestRequest("api/RLocalPobTypes/Put?UserName={UserName}&WorkFlow={WorkFlow}&id={id}", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var request = new RestRequest("api/RRequestSystems/Put?id={id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", model.Id, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
        public RRequestSystemViewModel GetByRSystemId(int Id)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/RRequestSystems/GetByRSystemId?Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<RRequestSystemViewModel>(request);
            return response.Data;
        }
        public void Delete(int Id,string RedirectToUrl)
        {
            var request = new RestRequest("api/RRequestSystems/Delete?Id={Id}", Method.DELETE);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<RLocalPobTypeViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            //var request = new RestRequest("api/RRequestSystems/DeleteRRequestSystem?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.DELETE);
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            //request.AddParameter("Id", Id, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            //var response = _client.Execute<RRequestSystemViewModel>(request);
            //if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            //{
            //    //call globals method to generate exception based on response
            //    Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            //}
        }
    }

    interface IRRequestSystemsRestClient
    {
        IEnumerable<RRequestSystemViewModel> GetByCompanyCode(string CompanyCode);      
        void Add(RRequestSystemViewModel serverData, string RedirectToUrl);       
        RRequestSystemViewModel GetByName(string Name);
        void Update(RRequestSystemViewModel model, string RedirectToUrl);
        RRequestSystemViewModel GetByRSystemId(int Id);
        void Delete(int Id,string RedirectToUrl);
    }
}
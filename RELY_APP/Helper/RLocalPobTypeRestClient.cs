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
    public class RLocalPobTypeRestClient : IRLocalPobTypeRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public RLocalPobTypeRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public RLocalPobTypeViewModel GetById(int Id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/RLocalPobTypes/GetRLocalPobType?id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<RLocalPobTypeViewModel>(request);
            return response.Data;
        }
        public RLocalPobTypeViewModel GetByName(string Name)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/RLocalPobTypes/GetRLocalPobTypeByName?Name={Name}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Name", Name, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<RLocalPobTypeViewModel>(request);
            return response.Data;
        }
        public IEnumerable<RLocalPobTypeViewModel> GetByCompanyCode(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/RLocalPobTypes/GetByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<RLocalPobTypeViewModel>>(request);

            return response.Data;
        }       

        public void Add(RLocalPobTypeViewModel model, string RedirectToUrl)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
           // var request = new RestRequest("api/RLocalPobTypes/Post?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            var request = new RestRequest("api/RLocalPobTypes/Post", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(model);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }
        //Edit
        public void Update(RLocalPobTypeViewModel model, string RedirectToUrl)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
           // string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            //var request = new RestRequest("api/RLocalPobTypes/Put?UserName={UserName}&WorkFlow={WorkFlow}&id={id}", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var request = new RestRequest("api/RLocalPobTypes/Put?id={id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id",model.Id, ParameterType.UrlSegment);
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
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            //var request = new RestRequest("api/RLocalPobTypes/Delete?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.DELETE);
            var request = new RestRequest("api/RLocalPobTypes/Delete?Id={Id}", Method.DELETE);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<RLocalPobTypeViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

    }
    interface IRLocalPobTypeRestClient
    {
        RLocalPobTypeViewModel GetByName(string Name);
        RLocalPobTypeViewModel GetById(int Id);
        IEnumerable<RLocalPobTypeViewModel> GetByCompanyCode(string CompanyCode);
        void Add(RLocalPobTypeViewModel model, string RedirectToUrl);
        void Delete(int Id, string RedirectToUrl);
        void Update(RLocalPobTypeViewModel model, string RedirectToUrl);

    }
}
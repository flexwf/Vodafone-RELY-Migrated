using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
//using System.Linq;
using System.Net;
//using System.Web;

namespace RELY_APP.Helper
{
    public class LReconFileFormatsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
         
        public LReconFileFormatsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        //GetLReconFileFormat/ByCompanyCode
        public IEnumerable<LReconFileFormatsViewModel> GetByCompanyCode(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LReconFileFormats/GetByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LReconFileFormatsViewModel>>(request);

            return response.Data;
        }

        public void Add(LReconFileFormatsViewModel model, string RedirectToUrl)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LReconFileFormats/POST", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }

        public LReconFileFormatsViewModel GetById(int Id)
        {            
            var request = new RestRequest("api/LReconFileFormats/GetById?id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);            
            var response = _client.Execute<LReconFileFormatsViewModel>(request);
            return response.Data;
        }

        //DELETE LReconFileFormat
        public void Delete(int Id, string CompanyCode, string RedirectToUrl)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LReconFileFormats/Delete?Id={Id}&CompanyCode={CompanyCode}", Method.DELETE);

            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }

        public void Update(LReconFileFormatsViewModel model, string RedirectToUrl)
        {
            var request = new RestRequest("api/LReconFileFormats/Put?id={id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", model.Id, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
    }
    interface ILReconFileFormatsRestClient
    {
        IEnumerable<LReconFileFormatsViewModel> GetByCompanyCode(string CompanyCode);
        void Delete(int Id, string Name, string RedirectToUrl);
        void Add(LReconFileFormatsViewModel model, string RedirectToUrl);
    }
}

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
    class GCompaniesRestClient : IGCompaniesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;

        public GCompaniesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<GCompanyViewModel> GetAll()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/GCompanies/GetGCompanies?&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName,ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow,ParameterType.UrlSegment);
            var response = _client.Execute<List<GCompanyViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public GCompanyViewModel GetByComapnyCode(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/GCompanies/GetGCompany?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<GCompanyViewModel>(request);
            
            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }


        public IEnumerable<CreateCompanyViewModel> GetCompanyInfo(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/GCompanies/GetCompanyInfo?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<CreateCompanyViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }


        public void SaveData(CreateCompanyViewModel Data, string RedirectToUrl)
        {

            var request = new RestRequest("api/GCompanies/CreateCompany", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(Data);

            var response = _client.Execute<CreateCompanyViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }


        public void UpdateData(CreateCompanyViewModel Data)
        {
            var request = new RestRequest("api/GCompanies/UpdateData", Method.PUT) { RequestFormat = DataFormat.Json };
            
            request.AddBody(Data);
            var response = _client.Execute<CreateCompanyViewModel>(request);

        }

        public void DeleteCompany(string CompanyCode, string RedirectToUrl)
        {

            var request = new RestRequest("api/GCompanies/DeleteCompany?CompanyCode={CompanyCode}", Method.DELETE) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);

            var response = _client.Execute<LPasswordPolicyViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        public void Add(GCompanyViewModel serverData, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/GCompanies/PostGCompany?&UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<GCompanyViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

          
            
        }

       public string GetCompanyCodeByCompanyId(int id, string RedirectToUrl)
        {
            var request = new RestRequest("api/GCompanies/GetCompanyCodeByCompanyId?id={id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<GCompanyViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Content;
        }

    }
    interface IGCompaniesRestClient
    {
        IEnumerable<GCompanyViewModel> GetAll();
        GCompanyViewModel GetByComapnyCode(string ComapnyCode);
        void Add(GCompanyViewModel serverData, string RedirectToUrl);
        IEnumerable<CreateCompanyViewModel> GetCompanyInfo(string CompanyCode);
        void SaveData(CreateCompanyViewModel Data, string RedirectToUrl);
        void UpdateData(CreateCompanyViewModel Data);
        void DeleteCompany(string CompanyCode, string RedirectToUrl);
        // string GetCompanyCodeByCompanyId(int id);
        string GetCompanyCodeByCompanyId(int id, string RedirectToUrl);

    }
}
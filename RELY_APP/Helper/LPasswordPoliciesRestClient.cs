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
    class LPasswordPoliciesRestClient : ILPasswordPoliciesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        // string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
        public LPasswordPoliciesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public LPasswordPolicyViewModel GetByCompanyCode(string CompanyCode, int UserId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LPasswordPolicies/GetLPasswordPolicies?CompanyCode={CompanyCode}&UserId={UserId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute <LPasswordPolicyViewModel>(request);

            return response.Data;
        }

        public void SaveData(LPasswordPolicyViewModel Data, string RedirectToUrl)
        {
            
            var request = new RestRequest("api/LPasswordPolicies/SaveData", Method.POST) { RequestFormat = DataFormat.Json };
             request.AddBody(Data);

            var response = _client.Execute<LPasswordPolicyViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        public LPasswordPolicyViewModel GetPasswordPolicyData(int id, string CompanyCode)
        {
            var request = new RestRequest("api/LPasswordPolicies/GetPasswordPolicyById?id={id}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
             var response = _client.Execute<LPasswordPolicyViewModel>(request);

            return response.Data;
        }

        public void Delete(int id,string RedirectToUrl)
        {

            var request = new RestRequest("api/LPasswordPolicies/DeletePasswordPolicy?id={id}", Method.DELETE) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<LPasswordPolicyViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        public void PutPasswordPolicy(int id, LPasswordPolicyViewModel Data)
        {
            var request = new RestRequest("api/LPasswordPolicies/PutPasswordPolicy?id={id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddBody(Data);
            var response = _client.Execute<LEmailTemplateViewModel>(request);
            
        }
    }

    interface ILPasswordPoliciesRestClient
    {
        LPasswordPolicyViewModel GetByCompanyCode(string CompanyCode, int UserId);
        void SaveData(LPasswordPolicyViewModel Data, string RedirectToUrl);
        LPasswordPolicyViewModel GetPasswordPolicyData(int id, string CompanyCode);
        void Delete(int id, string RedirectToUrl);
        void PutPasswordPolicy(int id, LPasswordPolicyViewModel Data);

    }
}
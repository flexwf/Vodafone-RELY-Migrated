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
    public class LFinancialSurveysRestClient : ILFinancialSurveysRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;

        public LFinancialSurveysRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        
            public IEnumerable<SurveyDropDownViewModel> GetSurveyforDropDown(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LFinancialSurveys/GetSurveyforDropDown?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<SurveyDropDownViewModel>>(request);

            return response.Data;
        }
        public IEnumerable<LFinancialSurveysViewModel> GetByCompanyCode(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LFinancialSurveys/GetByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LFinancialSurveysViewModel>>(request);

            return response.Data;
        }

        public void Add(LFinancialSurveysViewModel model, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LFinancialSurveys/Post?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
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
        public void Update(LFinancialSurveysViewModel model, string RedirectToUrl)
        {
            var request = new RestRequest("api/LFinancialSurveys/Put?id={id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", model.Id, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }


        public void Delete(int Id, string Name, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LFinancialSurveys/Delete?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.DELETE);

            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            var response = _client.Execute<LFinancialSurveysViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }
        public int GetSurveyIdbyName(string SurveyName)
        {
            var request = new RestRequest("api/LFinancialSurveys/GetSurveyIdByName?SurveyName={SurveyName}", Method.GET);

            request.AddParameter("SurveyName", SurveyName, ParameterType.UrlSegment);


            var response = _client.Execute<int>(request);

            return response.Data;
        }
        
        public LFinancialSurveysViewModel GetLFinancialSurveyById(int? Id)
        {
            var request = new RestRequest("api/LFinancialSurveys/GetLFinancialSurveyById?Id={Id}", Method.GET);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<LFinancialSurveysViewModel>(request);
            return response.Data;
        }

        public LFinancialSurveysViewModel GetById(int Id)
        {           
            var request = new RestRequest("api/LFinancialSurveys/GetById?id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);           
            var response = _client.Execute<LFinancialSurveysViewModel>(request);
            return response.Data;
        }
    }

    interface ILFinancialSurveysRestClient
    {
        IEnumerable<LFinancialSurveysViewModel> GetByCompanyCode(string CompanyCode);
        void Delete(int Id, string Name, string RedirectToUrl);
        int GetSurveyIdbyName(string SurveyName);
        void Add(LFinancialSurveysViewModel model, string RedirectToUrl);
        LFinancialSurveysViewModel GetLFinancialSurveyById(int? Id);
        LFinancialSurveysViewModel GetById(int Id);
        void Update(LFinancialSurveysViewModel model, string RedirectToUrl);
         IEnumerable<SurveyDropDownViewModel> GetSurveyforDropDown(string CompanyCode);
    }

    }
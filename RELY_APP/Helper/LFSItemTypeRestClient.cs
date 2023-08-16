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
    public class LFSItemTypeRestClient: ILFSItemTypeRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

        public LFSItemTypeRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LFSItemTypeViewModel> GetByCompanyCode(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSItemTypes/GetLFSItemTypesByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LFSItemTypeViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void Add(LFSItemTypeViewModel serverData, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSItemTypes/PostLFSItemTypes?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LFSItemTypeViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        public void Update(LFSItemTypeViewModel serverData, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSItemTypes/PutLFSItemTypes?CompanyCode={CompanyCode}&Name={Name}&UserName={UserName}&WorkFlow={WorkFlow}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", serverData.CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Name", serverData.Name, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LFSItemTypeViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        public LFSItemTypeViewModel GetByCode(string CompanyCode, string Name)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSItemTypes/GetLFSItemType?CompanyCode={CompanyCode}&Name={Name}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Name", Name, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            var response = _client.Execute<LFSItemTypeViewModel>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
            //string source = response.Content;
            //dynamic data = JsonConvert.DeserializeObject(source);
            //return data;
        }

        public void Delete(int Id, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSItemTypes/DeleteLFSItemTypes?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.DELETE);

            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            var response = _client.Execute<LFSItemTypeViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
        public int GetItemTypeIdbyName(string Name)
        {
            var request = new RestRequest("api/LFSItemTypes/GetItemTypeIdByName?Name={Name}", Method.GET);

            request.AddParameter("Name", Name, ParameterType.UrlSegment);


            var response = _client.Execute<int>(request);

            return response.Data;
        }

    }

    interface ILFSItemTypeRestClient
    {
        IEnumerable<LFSItemTypeViewModel> GetByCompanyCode(string CompanyCode);
        void Add(LFSItemTypeViewModel serverData, string RedirectToUrl);
        LFSItemTypeViewModel GetByCode(string CompanyCode, string Name);
        void Update(LFSItemTypeViewModel serverData, string RedirectToUrl);
        void Delete(int Id, string RedirectToUrl);
        int GetItemTypeIdbyName(string SurveyName);

    }
}
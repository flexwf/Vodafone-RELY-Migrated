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
    class GMenusRestClient : IGMenusRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        public GMenusRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<GMenuViewModel> GetAll(string CompanyCode)
        {
            var request = new RestRequest("api/GMenus/GetGMenus?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            
            var response = _client.Execute<List<GMenuViewModel>>(request);

            return response.Data;
        }

        public void Add(GMenuViewModel serverData, string RedirectToUrl)
        {
            var request = new RestRequest("api/GMenus", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<GMenuViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            
        }
        public IEnumerable<MMenuRoleViewModel> GetByUserRole(string UserRole, string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GMenus/GetGMenusRolesByUserRole?UserRole={UserRole}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserRole", UserRole, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<MMenuRoleViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void ProcessMenuItems(GMenuViewModel GMenu, string MenuLabel, int MenuId, int ParentMenuId, string CompanyCode,string RedirectToUrl)
        {
            var request = new RestRequest("api/GMenus/ProcessMenuItems?MenuLabel={MenuLabel}&MenuId={MenuId}&ParentMenuId={ParentMenuId}&CompanyCode={CompanyCode}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(GMenu);
            request.AddParameter("MenuLabel", MenuLabel, ParameterType.UrlSegment);
            request.AddParameter("MenuId", MenuId, ParameterType.UrlSegment);
            request.AddParameter("ParentMenuId", ParentMenuId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }

        public IEnumerable<GMenuViewModel> GetMenuById(int id)
        {
            var request = new RestRequest("api/GMenus/GetMenuById?id={id}", Method.GET) { RequestFormat = DataFormat.Json };
           
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<List<GMenuViewModel>>(request);
            return response.Data;
        }

        public void Update(GMenuViewModel GMenu, int id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GMenus/PutGMenu?id={id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddBody(GMenu);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<GMenuViewModel>>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, null);
            }
        }
    }

    interface IGMenusRestClient
    {
        IEnumerable<GMenuViewModel> GetAll(string CompanyCode);
        void Add(GMenuViewModel serverData, string RedirectToUrl);
        IEnumerable<MMenuRoleViewModel> GetByUserRole(string UserRole, string CompanyCode);
        void ProcessMenuItems(GMenuViewModel GMenu, string MenuLabel, int MenuId, int ParentMenuId, string CompanyCode, string RedirectToUrl);
        IEnumerable<GMenuViewModel> GetMenuById(int id);
        void Update(GMenuViewModel GMenu, int id);
    }
}
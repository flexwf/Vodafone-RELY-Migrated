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
    class MMenuRolesRestClient : IMMenuRolesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        public MMenuRolesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<MMenuRoleViewModel> GetAll()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/MMenuRoles?UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName",UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<MMenuRoleViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        public MMenuRoleViewModel GetById(int Id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/MMenuRoles/Id?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName,ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow,ParameterType.UrlSegment);
            var response = _client.Execute<MMenuRoleViewModel>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<MMenuRoleViewModel> GetByUserRole(string UserRole, string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/MMenuRoles/GetByUserRole?UserRole={UserRole}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserRole", UserRole, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName",UserName,ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow,ParameterType.UrlSegment);
            var response = _client.Execute<List<MMenuRoleViewModel>>(request);
            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        public List<dynamic> GetDataForMenuRoleMapping(string CompanyCode)
        {


            var request = new RestRequest("api/MMenuRoles/GetDataForMenuRoleMapping?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);

            return response.Data;

        }
        public List<LRoleViewModel> GetColumnForMenuRoles(string CompanyCode)
        {


            var request = new RestRequest("api/MMenuRoles/GetColumnForMenuRoles?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LRoleViewModel>>(request);

            return response.Data;

        }

        public void SaveDataForMenuRole(List<UpdateMappingMenuRoleViewModel> model, string CompanyCode, string RedirectToUrl)
        {
            var request = new RestRequest("api/MMenuRoles/SaveDataForMenuRole?CompanyCode={CompanyCode}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddBody(model);

            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

    }
    interface IMMenuRolesRestClient
    {
        IEnumerable<MMenuRoleViewModel> GetAll();
        MMenuRoleViewModel GetById(int Id);
        IEnumerable<MMenuRoleViewModel> GetByUserRole(string UserRole, string CompanyCode);
        List<dynamic> GetDataForMenuRoleMapping(string CompanyCode);
        List<LRoleViewModel> GetColumnForMenuRoles(string CompanyCode);
        void SaveDataForMenuRole(List<UpdateMappingMenuRoleViewModel> model, string CompanyCode, string RedirectToUrl);
    }
}
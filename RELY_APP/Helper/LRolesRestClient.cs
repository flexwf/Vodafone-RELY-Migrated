using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace RELY_APP.Helper
{
    public class LRolesRestClient : ILRolesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        
        public LRolesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<PageAccessRoleViewModel> GetRoleAccessListByControllerAction(string ControllerName, string MethodName, string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRoles/GetRoleAccessListByControllerAction?ControllerName={ControllerName}&MethodName={MethodName}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ControllerName", ControllerName, ParameterType.UrlSegment);
            request.AddParameter("MethodName", MethodName, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<PageAccessRoleViewModel>>(request);
            return response.Data;
        }
        public IEnumerable<LRoleViewModel> GetByCompanyCode(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRoles/GetLRolesByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LRoleViewModel>>(request);
            return response.Data;
        }
       
        public LRoleViewModel GetByRoleName(string RoleName, string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRoles/GetByRoleName?RoleName={RoleName}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("RoleName", RoleName, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LRoleViewModel>(request);
            return response.Data;
        }
        public LRoleViewModel GetById(int Id, string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["UserName"] as string)) ? "No UserName" : System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRoles/GetById?Id={Id}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LRoleViewModel>(request);
            return response.Data;
        }

        public IEnumerable<LRoleViewModel> GetUserRolesForEditPage(string CompanyCode,int UserId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            var request = new RestRequest("api/LRoles/GetUserRolesForEditPage?CompanyCode={CompanyCode}&UserId={UserId}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LRoleViewModel>>(request);

            return response.Data;
        }

        public void Add(LRoleViewModel serverData, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRoles/PostLRole?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(serverData);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }           
        }

        //Below code is commented by Rakhi Singh on 27th july 2018 during code review by Vikas.
        //public void Update(LRoleViewModel serverData, string RedirectToUrl)
        //{
        //    string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //    string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        //    var request = new RestRequest("api/LRoles/PutLRole?CompanyCode={CompanyCode}&Name={Name}&UserName={UserName}&WorkFlow={WorkFlow}", Method.PUT) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyCode", serverData.CompanyCode, ParameterType.UrlSegment);
        //    request.AddParameter("Name", serverData.RoleName, ParameterType.UrlSegment);
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
        //    request.AddBody(serverData);

        //    var response = _client.Execute<LRoleViewModel>(request);
        //    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
        //    {
        //        //call globals method to generate exception based on response
        //        Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
        //    }

        //}

        //public void Delete(int Id, string RedirectToUrl)
        //{
        //    string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //    string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        //    var request = new RestRequest("api/LRoles/DeleteLRole?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.DELETE);

        //    request.AddParameter("Id", Id, ParameterType.UrlSegment);
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

        //    var response = _client.Execute<LRoleViewModel>(request);
        //    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
        //    {
        //        //call globals method to generate exception based on response
        //        Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
        //    }
        //}
        //public IEnumerable<LRoleViewModel> GetAll()
        //{
        //    string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //    string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        //    var request = new RestRequest("api/LRoles/GetAll?UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LRoleViewModel>>(request);

        //    //if (response.Data == null)
        //    //    throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}
        //public LRoleViewModel GetByCode(string CompanyCode, string Name)
        //{
        //    string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //    string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        //    var request = new RestRequest("api/LRoles/GetLRoles?CompanyCode={CompanyCode}&Name={Name}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };

        //    request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
        //    request.AddParameter("Name", Name, ParameterType.UrlSegment);
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

        //    var response = _client.Execute<LRoleViewModel>(request);

        //    //if (response.Data == null)
        //    //    throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //    //string source = response.Content;
        //    //dynamic data = JsonConvert.DeserializeObject(source);
        //    //return data;
        //}

    }
    interface ILRolesRestClient
        {
        IEnumerable<PageAccessRoleViewModel> GetRoleAccessListByControllerAction(string ControllerName, string MethodName, string CompanyCode);
        IEnumerable<LRoleViewModel> GetByCompanyCode(string CompanyCode);     
        LRoleViewModel GetByRoleName(string RoleName, string CompanyCode);
        void Add(LRoleViewModel serverData, string RedirectToUrl);     
        IEnumerable<LRoleViewModel> GetUserRolesForEditPage(string CompanyCode, int UserId);
        LRoleViewModel GetById(int Id, string CompanyCode);

        //  LRoleViewModel GetByCode(string CompanyCode, string Name);
        //   IEnumerable<LRoleViewModel> GetAll();
        // void Update(LRoleViewModel serverData, string RedirectToUrl);
        //  void Delete(int Id, string RedirectToUrl);

    }

}
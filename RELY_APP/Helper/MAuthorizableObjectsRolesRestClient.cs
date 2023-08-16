using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Helper
{
    public class MAuthorizableObjectsRolesRestClient : IMAuthorizableObjectsRolesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

        public MAuthorizableObjectsRolesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        
        //public IEnumerable<MAuthorizableObjectsRoleViewModel> GetMAuthorizableObjectsRoles()
        //{
        //    string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //    string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        //    var request = new RestRequest("api/MAuthorizableObjects/GetMAuthorizableObjectsRoles?UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json, };
        //    //request.AddParameter("Id",Id , ParameterType.UrlSegment);
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<MAuthorizableObjectsRoleViewModel>>(request);

        //    //if (response.Data == null)
        //    //    throw new Exception(response.ErrorMessage);
        //    return response.Data;
        //}
        //public int GetCount(string UserRoleId, string CurrentActionKey)
        //{

        //    var request = new RestRequest("api/MAuthorizableObjectsRoles/GetCount?UserRoleId={UserRoleId}&CurrentActionKey={CurrentActionKey}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("UserRoleId", UserRoleId, ParameterType.UrlSegment);
        //    request.AddParameter("CurrentActionKey", CurrentActionKey, ParameterType.UrlSegment);
        //    var response = _client.Execute<int>(request);
        //    if (response.StatusCode == HttpStatusCode.InternalServerError)
        //    {
        //        throw new Exception(response.ErrorMessage);
        //    }
        //    return response.Data;
        //}

        public List<string> GetRolesListbyControllerAction( string CurrentActionKey, string CompanyCode)
        {
            var request = new RestRequest("api/MAuthorizableObjectsRoles/GetRolesListbyControllerAction?CurrentActionKey={CurrentActionKey}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CurrentActionKey", CurrentActionKey, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new Exception(response.ErrorMessage);
            }
            return response.Data;
        }

        public List<dynamic> GetDataForAuthorizableObjectRole()
        {


            var request = new RestRequest("api/MAuthorizableObjectsRoles/GetDataForAuthorizableObjectRole", Method.GET) { RequestFormat = DataFormat.Json };
          
            var response = _client.Execute<List<dynamic>>(request);

            return response.Data;

        }
        public List<LRoleViewModel> GetColumnForAuthorizableObjectRole()
        {


            var request = new RestRequest("api/MAuthorizableObjectsRoles/GetColumnForAuthorizableObjectRole", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<LRoleViewModel>>(request);

            return response.Data;

        }



        public void SaveDataForAuthorizableObjectRole(List<UpdateMappingAuthorizableObjectRoleViewModel> model,string RedirectToUrl)
        {
            var request = new RestRequest("api/MAuthorizableObjectsRoles/SaveDataForAuthorizableObjectRole", Method.POST) { RequestFormat = DataFormat.Json };
            
            request.AddBody(model);

            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        //public void TestMultiThreading(int RequestId)
        //{
        //    var request = new RestRequest("api/MAuthorizableObjectsRoles/TestMultiThreading?RequestId={RequestId}", Method.POST) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("RequestId", RequestId, ParameterType.UrlSegment);
        //    var response = _client.Execute(request);
           

        //}
    }

    interface IMAuthorizableObjectsRolesRestClient
    {
        //IEnumerable<MAuthorizableObjectsRoleViewModel> GetMAuthorizableObjectsRoles();
        //int GetCount(string UserRoleId, string CurrentActionKey);
        List<string> GetRolesListbyControllerAction(string CurrentActionKey, string CompanyCode);
        List<dynamic> GetDataForAuthorizableObjectRole();
        List<LRoleViewModel> GetColumnForAuthorizableObjectRole();
        void SaveDataForAuthorizableObjectRole(List<UpdateMappingAuthorizableObjectRoleViewModel> model, string RedirectToUrl);
       // void TestMultiThreading(int RequestId);
    }
}
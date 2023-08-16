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
    public class LAuditRestClient : ILAuditRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;

        public LAuditRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public int Add(LAuditViewModel model, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LAudit/PostLAudit?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

            return response.Data;
        }
        public IEnumerable<LAuditViewModel> GetByEntityName(string EntityName, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LAudit/GetByEntityName?EntityName={EntityName}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("EntityName", EntityName, ParameterType.UrlSegment);
            var response = _client.Execute<List<LAuditViewModel>>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

            return response.Data;
        }

        public IEnumerable<LAuditViewModel> GetByTypeEntityId(string EntityType, int EntityId, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LAudit/GetByTypeEntityId?UserName={UserName}&WorkFlow={WorkFlow}&EntityType={EntityType}&EntityId={EntityId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LAuditViewModel>>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

            return response.Data;
        }
        public IEnumerable<LAuditViewModel> GetByEntityTypeEntityId(string EntityType, string EntityIdList, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LAudit/GetByEntityTypeEntityId?UserName={UserName}&WorkFlow={WorkFlow}&EntityType={EntityType}&EntityIdList={EntityIdList}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityIdList", EntityIdList, ParameterType.UrlSegment);
            var response = _client.Execute<List<LAuditViewModel>>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

            return response.Data;
        }

        public string DownloadGetByTypeEntityId(string EntityType, int EntityId, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LAudit/GetByTypeEntityId?UserName={UserName}&WorkFlow={WorkFlow}&EntityType={EntityType}&EntityId={EntityId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

            return response.Content;
        }

        public int GetCountsForNewItems()
        {
            var request = new RestRequest("api/LAudit/GetCountsForNewItems?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}&Intervalid={Intervalid}", Method.GET) { RequestFormat = DataFormat.Json };
          
            var response = _client.Execute<int>(request);

            return response.Data;
        }

        public List<dynamic> GetNewItemscolumnlist(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, int Intervalid)
        {
            var request = new RestRequest("api/LAudit/GetNewItemscolumnlist?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}&Intervalid={Intervalid}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("Intervalid", Intervalid, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }

        public List<dynamic> GetDataForNewItemColumns(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, int Intervalid)
        {


            var request = new RestRequest("api/LAudit/GetDataForNewItemColumns?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}&Intervalid={Intervalid}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            //@Intervalid
            request.AddParameter("Intervalid", Intervalid, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);

            return response.Data;

        }

    }

    interface ILAuditRestClient
    {
        int Add(LAuditViewModel model, string RedirectToUrl);
        IEnumerable<LAuditViewModel> GetByEntityTypeEntityId(string EntityType, string EntityIdList, string RedirectToUrl);
        IEnumerable<LAuditViewModel> GetByEntityName(string EntityName, string RedirectToUrl);
        IEnumerable<LAuditViewModel> GetByTypeEntityId(string EntityType, int EntityId, string RedirectToUrl);
        string DownloadGetByTypeEntityId(string EntityType, int EntityId, string RedirectToUrl);
        int GetCountsForNewItems();
        List<dynamic> GetNewItemscolumnlist(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, int Intervalid);
        List<dynamic> GetDataForNewItemColumns(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, int Intervalid);
    }
}
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace RELY_APP.Helper
{
    public class LSupportingDocumentsRestClient:ILSupportingDocumentsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;

        public LSupportingDocumentsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LSupportingDocumentViewModel> GetByEntityType(string EntityType,int EntityId)
        {
            var request = new RestRequest("api/LSupportingDocuments/GetSupportingDocumentsByEntityType?EntityType={EntityType}&EntityId={EntityId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LSupportingDocumentViewModel>>(request);

            return response.Data;
        }

        public LSupportingDocumentViewModel GetById(int id)
        {
            var request = new RestRequest("api/LSupportingDocuments/GetLSupportingDocumentById/{id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LSupportingDocumentViewModel>(request);

            return response.Data;
        }

        public void Delete(int id,int LoggedInUserId,int LoggedInRoleId,string WorkFlow,string CompanyCode)
        {
            var request = new RestRequest("api/LSupportingDocuments/DeleteSupportingDocument/{id}?WorkFlow={WorkFlow}&LoggedInUserId={LoggedInUserId}&LoggedInRoleId={LoggedInRoleId}&CompanyCode={CompanyCode}", Method.DELETE) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute(request);

        }
    }

    interface ILSupportingDocumentsRestClient
    {
        IEnumerable<LSupportingDocumentViewModel> GetByEntityType(string EntityType, int EntityId);
        LSupportingDocumentViewModel GetById(int id);
        void Delete(int id, int LoggedInUserId, int LoggedInRoleId, string WorkFlow, string CompanyCode);
    }
}
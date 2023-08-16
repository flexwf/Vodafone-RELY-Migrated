using Newtonsoft.Json;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace RELY_APP.Helper
{
    class LLocalPobSSPRestClient : ILLocalPobSSPRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        public LLocalPobSSPRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LLocalPobSspViewModel> GetByLocalPobId(int LocalPobId)
        {
            var request = new RestRequest("api/LLocalPobSSP/GetByLocalPobId?LocalPobId={LocalPobId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("LocalPobId", LocalPobId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LLocalPobSspViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public LLocalPobSspViewModel GetById(int id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LLocalPobSSP/GetSSP?id={id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow,ParameterType.UrlSegment);
            var response = _client.Execute<LLocalPobSspViewModel>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }

    }
    interface ILLocalPobSSPRestClient
    {
        IEnumerable<LLocalPobSspViewModel> GetByLocalPobId(int LocalPObId);
        LLocalPobSspViewModel GetById(int LocalPobId);
    }
}
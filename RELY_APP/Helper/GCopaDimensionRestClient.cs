using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net;
using RELY_APP.Utilities;

namespace RELY_APP.Helper
{
    public class GCopaDimensionRestClient : IGCopaDimensionRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        public GCopaDimensionRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<GCopaDimensionViewModel> GetGCopaDimensions()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCopaDimensions/GetGCopaDimensions?&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json, };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<GCopaDimensionViewModel>>(request);
            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public IEnumerable<GCopaDimensionViewModel> GetGCopaDimensionsByClass(int Class,int PobCatalogueId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCopaDimensions/GetGCopaDimensionsByClass?Class={Class}&PobCatalogueId={PobCatalogueId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json, };
            request.AddParameter("Class", Class, ParameterType.UrlSegment);
            request.AddParameter("PobCatalogueId", PobCatalogueId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<GCopaDimensionViewModel>>(request);
            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public IEnumerable<CopaDimentionDropDownViewModel> GetGCopaDimensionsforDropDown(int Class)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCopaDimensions/GetGCopaDimensionsforDropDown?Class={Class}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json, };
            request.AddParameter("Class", Class, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<CopaDimentionDropDownViewModel>>(request);
            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }


        public int Add(GCopaDimensionViewModel model, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCopaDimensions/PostGCopaDimension?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
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
        public string DownloadGCopaDimensions()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCopaDimensions/GetGCopaDimensions?&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json, };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<GCopaDimensionViewModel>>(request);
            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Content;
        }
    }
    interface IGCopaDimensionRestClient
    {
        IEnumerable<GCopaDimensionViewModel> GetGCopaDimensions();
        IEnumerable<GCopaDimensionViewModel> GetGCopaDimensionsByClass(int Class,int PobCatalogueId);
        IEnumerable<CopaDimentionDropDownViewModel> GetGCopaDimensionsforDropDown(int Class);
        int Add(GCopaDimensionViewModel model, string RedirectToUrl);
        string DownloadGCopaDimensions();
    }
}
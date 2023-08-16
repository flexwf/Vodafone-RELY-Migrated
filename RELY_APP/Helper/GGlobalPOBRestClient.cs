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
    public class GGlobalPOBRestClient : IGGlobalPOBRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

        public GGlobalPOBRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<GGlobalPOBViewModel> GetAllGGlobalPOB(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GGlobalPOB/GetAllGGlobalPOB?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<GGlobalPOBViewModel>>(request);
            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public IEnumerable<GGlobalPOBViewModel> GetGGlobalPOB(int PobCatalogueId,string GpobType)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GGlobalPOB/GetGGlobalPOB?PobCatalogueId={PobCatalogueId}&GpobType={GpobType}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PobCatalogueId", PobCatalogueId, ParameterType.UrlSegment);
            request.AddParameter("GpobType", GpobType, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<GGlobalPOBViewModel>>(request);
            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public IEnumerable<GlobalPobDropDownViewModel> GetGGlobalPOBforDropDown(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GGlobalPOB/GetGGlobalPOBforDropDown?CompanyCode=CompanyCode&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<GlobalPobDropDownViewModel>>(request);
            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        /// <summary>
        /// Date:29 june 2018
        /// Created By: Rakhi Singh
        /// Description: This method is used in download method of controller 
        /// to download & export to excel GlobalPOB data 
        /// </summary>
        /// <returns></returns>
        public String DownloadGGlobalPOB(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GGlobalPOB/GetAllGGlobalPOB?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);
            return response.Content;
        }
        public int Add(List<GGlobalPOBViewModel> model, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GGlobalPOB/POSTGGlobalPOB?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
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


    }

    interface IGGlobalPOBRestClient
    {
        IEnumerable<GGlobalPOBViewModel> GetAllGGlobalPOB(string CompanyCode);
        IEnumerable<GGlobalPOBViewModel> GetGGlobalPOB(int PobCatalogueId, string GpobType);
        IEnumerable<GlobalPobDropDownViewModel> GetGGlobalPOBforDropDown(string CompanyCode);
        int Add(List<GGlobalPOBViewModel> model, string RedirectToUrl);
        String DownloadGGlobalPOB(string CompanyCode);
    }
}
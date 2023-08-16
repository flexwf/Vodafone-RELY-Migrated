using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace RELY_APP.Helper
{
    public class RelyDataDownloadRestClient : IRelyDataDownloadRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public RelyDataDownloadRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        /// <summary>
        /// Method gets all the company name to fill dropdown
        /// </summary>
        public IEnumerable<RelyDataDownload> GetAllCompanyName()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCompanies/GetGCompanies?", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);            
            var response = _client.Execute<List<RelyDataDownload>>(request);
            return response.Data;
        }
        /// <summary>
        /// Method gets company code on the basis of id selected from dropdown
        /// </summary>
        public RelyDataDownload GetCompanyCodeById(int? id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/RelyDataDownload/GetCompanyCodeById?id={id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<RelyDataDownload>(request);
            return response.Data;
        }

        /// <summary>
        /// Method generate calls API method to upload CSVs and ZIP file to S3 path and in return it gets Zip file Name
        /// </summary>
        public GenericNameAndIdViewModel DownloadRelyDataDump(string companycode)
        {
            var request = new RestRequest("api/RelyDataDownload/GenerateRelyDataDump?companycode={companycode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("companycode", companycode, ParameterType.UrlSegment);
            var response = _client.Execute<GenericNameAndIdViewModel>(request); //using this model as function is returning string value which needs to be sent back to APP
            return response.Data;
        }
    }

    interface IRelyDataDownloadRestClient
    {
        IEnumerable<RelyDataDownload> GetAllCompanyName();
        RelyDataDownload GetCompanyCodeById(int? id);
        GenericNameAndIdViewModel DownloadRelyDataDump(string companycode);
    }
}
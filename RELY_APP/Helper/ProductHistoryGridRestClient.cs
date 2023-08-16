using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;
using System.Net;
using RELY_APP.Utilities;
using System.Data;

namespace RELY_APP.Helper
{
    public class ProductHistoryGridRestClient : IProductHistoryGridRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        // GET: ProductHistoryGridRestClient
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ProductHistoryGridRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public List<dynamic> GetDataForProductHistory(int ProductId)
        {


            var request = new RestRequest("api/ProductHistoryGrid/GetDataForProductHistory?ProductId={ProductId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);

            var response = _client.Execute<List<dynamic>>(request);

            return response.Data;

        }

        public string DownloadGetDataForProductHistory(int ProductId)
        {
            var request = new RestRequest("api/ProductHistoryGrid/GetDataForProductHistory?ProductId={ProductId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);

            var response = _client.Execute<List<string>>(request);

            return response.Content;

            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            //var request = new RestRequest("api/LAudit/GetByTypeEntityId?UserName={UserName}&WorkFlow={WorkFlow}&EntityType={EntityType}&EntityId={EntityId}", Method.GET) { RequestFormat = DataFormat.Json };
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            //request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            //request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            //var response = _client.Execute<List<string>>(request);

            //if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            //{
            //    //call globals method to generate exception based on response
            //    Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            //}

            //return response.Content;
        }
        public int GetColumnsCountForProductHistory(int ProductId)
        {
            var request = new RestRequest("api/ProductHistoryGrid/GetColumnsForProductHistory?ProductId={ProductId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);

            // var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);
          

                var response = _client.Execute<int>(request);

                return response.Data;
            
            }
        public string DownloadGetColumnsCountForProductHistory(int ProductId)
        {
            var request = new RestRequest("api/ProductHistoryGrid/GetColumnsForProductHistory?ProductId={ProductId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);

            // var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);


            var response = _client.Execute<List<string>>(request);

            return response.Content;

        }

    }




    

    interface IProductHistoryGridRestClient
    {

        List<dynamic> GetDataForProductHistory(int ProductId);

        string DownloadGetDataForProductHistory(int ProductId);

        int GetColumnsCountForProductHistory(int ProductId);
        string DownloadGetColumnsCountForProductHistory(int ProductId);




    }
}
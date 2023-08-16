using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;
using System.Net;
using RELY_APP.Utilities;

namespace RELY_APP.Helper
{
    class LProductPobsRestClient : ILProductPobsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;

        public LProductPobsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public LProductPobViewModel GetById(int Id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProductPobs/GetById?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LProductPobViewModel>(request);
            return response.Data;
        }

        public LProductPobViewModel GetByProductId(int? ProductId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProductPobs/GetByProductId?ProductId={ProductId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LProductPobViewModel>(request);
            return response.Data;
        }
        public IEnumerable<ProductObligationsViewModel> GetByProductIdForProductGrid(int ProductId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProductPobs/GetByProductIdForProductGrid?ProductId={ProductId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<ProductObligationsViewModel>>(request);
            return response.Data;
        }
        public int Add(List<LProductPobViewModel> model, string TypeOfProduct,string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProductPobs/PostLProductPob?UserName={UserName}&WorkFlow={WorkFlow}&TypeOfProduct={TypeOfProduct}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("TypeOfProduct", TypeOfProduct==null?"": TypeOfProduct, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            
            return response.Data;
        }

        public void Delete(int Id,string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProductPobs/DeleteLProductPob?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }
    }
    interface ILProductPobsRestClient
    {
        LProductPobViewModel GetById(int Id);
        LProductPobViewModel GetByProductId(int? ProductId);
        int Add(List<LProductPobViewModel> model, string TypeOfProduct, string RedirectToUrl);
        IEnumerable<ProductObligationsViewModel> GetByProductIdForProductGrid(int ProductId);
        void Delete(int Id,  string RedirectToUrl);
    }
}


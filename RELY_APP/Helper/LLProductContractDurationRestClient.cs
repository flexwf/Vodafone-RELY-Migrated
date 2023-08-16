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
    class LProductContractDurationRestClient : ILProductContractDurationRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

        public LProductContractDurationRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LProductContractDurationViewModel> GetByProductId(string ProductId)
        {
            var request = new RestRequest("api/LProductContractDuration/GetByProductId?ProductId={ProductId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LProductContractDurationViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }


        public void Add(LProductContractDurationViewModel model, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LProductContractDuration/PostLProductContractDuration", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            //Exception Handling
            //if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            //{
            //    if (string.IsNullOrEmpty(RedirectToUrl))
            //    {
            //        RedirectToUrl = "/Home/ErrorPage";
            //    }
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    ex.Data.Add("RedirectToUrl", RedirectToUrl);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}
        }

        public void Add(List<LProductContractDurationViewModel> model, int ProductId, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LProductContractDuration/PostLProductContractDuration?ProductId={ProductId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(model);
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

            //Exception Handling
            //if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            //{
            //    if (string.IsNullOrEmpty(RedirectToUrl))
            //    {
            //        RedirectToUrl = "/Home/ErrorPage";
            //    }
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    ex.Data.Add("RedirectToUrl", RedirectToUrl);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}
        }

    }

    interface ILProductContractDurationRestClient
    {
        IEnumerable<LProductContractDurationViewModel> GetByProductId(string CompanyCode);
        void Add(LProductContractDurationViewModel ProductSSP, string RedirectToUrl);
        void Add(List<LProductContractDurationViewModel> model, int ProductId, string RedirectToUrl);
    }
}
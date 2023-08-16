using Newtonsoft.Json;
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
    public class GAuthorizableObjectsRestClient : IGAuthorizableObjectsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

        public GAuthorizableObjectsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<GAuthorizableObjectViewModel> GetAll()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GAuthorizableObjects/GetGAuthorizableObjects?UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<GAuthorizableObjectViewModel>>(request);
            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }


        public void Add(string[] DataArray, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GAuthorizableObjects/PostGAuthorizableObject?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(DataArray);
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
        public void Delete(int Id, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GAuthorizableObjects/DeleteGAuthorizableObject?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.DELETE);

            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            var response = _client.Execute<GAuthorizableObjectViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }


        }

    }

    interface IGAuthorizableObjectsRestClient
    {
        IEnumerable<GAuthorizableObjectViewModel> GetAll();

        void Add(string[] DataArray, string RedirectToUrl);
        void Delete(int Id, string RedirectToUrl);
    }
}
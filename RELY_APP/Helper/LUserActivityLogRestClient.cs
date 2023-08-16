using Newtonsoft.Json;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Configuration;
using System.Net;
namespace RestSharp.Helper
{
    public class LUserActivityLogRestClient : ILUserActivityLogRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["LoginEmail"] as string)) ? "No User Logged In" : System.Web.HttpContext.Current.Session["LoginEmail"] as string;
        public LUserActivityLogRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public void Add(LUserActivityLogViewModel  serverData, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["LoginEmail"] as string)) ? "No User Logged In" : System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LUserActivityLog/PostLUserActivityLog?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(serverData);
            var response = _client.Execute<LUserActivityLogViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

            //if (response.StatusCode == HttpStatusCode.InternalServerError)
            //{
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}
            //if (response.StatusCode == HttpStatusCode.BadRequest)
            //{
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}
        }
    }
    interface ILUserActivityLogRestClient
    {

        void Add(LUserActivityLogViewModel LUserActivityLog, string RedirectToUrl);

    }
}

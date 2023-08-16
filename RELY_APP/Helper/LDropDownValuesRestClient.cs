using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace RELY_APP.Helper
{
    public class LDropDownValuesRestClient : ILDropDownValuesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LDropDownValuesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public int GetCountsByDropDownId(int DropDownId)
        {
            var request = new RestRequest("api/LDropDownValues/GetLDropDownValueCountsByDropDownId?DropDownId={DropDownId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("DropDownId", DropDownId, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
            //string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            //if (string.IsNullOrEmpty(Workflow))
            //{
            //    Workflow = "No Workflow";
            //}
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

        }

        public IEnumerable<LDropDownValueViewModel> GetByDropDownId(int DropDownId)
        {
            var request = new RestRequest("api/LDropDownValues/GetLDropDownValuesByDropDownId?DropDownId={DropDownId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("DropDownId", DropDownId, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LDropDownValueViewModel>>(request);
            return response.Data;
            //string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            //if (string.IsNullOrEmpty(Workflow))
          //  { 
            //    Workflow = "No Workflow";
            //}
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

        }
        public LDropDownValueViewModel GetById(int id)
        {
            //string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            //if (string.IsNullOrEmpty(Workflow))
            //{
            //    Workflow = "No Workflow";
            //}
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LDropDownValues/GetLDropDownValue/{id}", Method.GET) { RequestFormat = DataFormat.Json };
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LDropDownValueViewModel>(request);
            return response.Data;
        }


        public void Add(LDropDownValueViewModel serverData, string RedirectToUrl)
        {
            var request = new RestRequest("api/LDropDownValues/Post", Method.POST) { RequestFormat = DataFormat.Json };
           
            request.AddBody(serverData);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

            //string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            //if (string.IsNullOrEmpty(Workflow))
            //{
            //    Workflow = "No Workflow";
            //}
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            //var request = new RestRequest("api/LDropDownValues/PostLDropDownValue?UserName={UserName}&Workflow={Workflow}", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddBody(serverData);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            //var response = _client.Execute<LDropDownValueViewModel>(request);

            //if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            //{
            //    if (string.IsNullOrEmpty(RedirectToUrl))
            //    {
            //        RedirectToUrl = "/Home/ErrorPage";
            //    }
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", (int)response.StatusCode);
            //    ex.Data.Add("RedirectToUrl", RedirectToUrl);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;

            //}
        }

        public void Update(LDropDownValueViewModel model, string RedirectToUrl)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            // string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            //var request = new RestRequest("api/RLocalPobTypes/Put?UserName={UserName}&WorkFlow={WorkFlow}&id={id}", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var request = new RestRequest("api/LDropDownValues/Put?id={id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", model.Id, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        public void Delete(int id, string RedirectToUrl, int DropdownId)
        {
            var request = new RestRequest("api/LDropDownValues/Delete/{id}?DropdownId={DropdownId}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("DropdownId", DropdownId, ParameterType.UrlSegment);
            var response = _client.Execute<LDropDownValueViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            //string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            //if (string.IsNullOrEmpty(Workflow))
            //{
            //    Workflow = "No Workflow";
            //}
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            //var request = new RestRequest("api/LDropDownValues/Delete/{id}?DropdownId={DropdownId}", Method.DELETE);
            //request.AddParameter("id", id, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            //request.AddParameter("DropdownId", DropdownId, ParameterType.UrlSegment);
            //var response = _client.Execute<LDropDownValueViewModel>(request);
            //if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            //{
            //    if (string.IsNullOrEmpty(RedirectToUrl))
            //    {
            //        RedirectToUrl = "/Home/ErrorPage";
            //    }
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", (int)response.StatusCode);
            //    ex.Data.Add("RedirectToUrl", RedirectToUrl);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;

            //}


        }

    }


    interface ILDropDownValuesRestClient
    {
        int GetCountsByDropDownId(int DropDownId);
        LDropDownValueViewModel GetById(int id);
        IEnumerable<LDropDownValueViewModel> GetByDropDownId(int DropDownId);
        void Add(LDropDownValueViewModel serverData, string RedirectToUrl);
        void Delete(int id, string RedirectToUrl, int DropdownId);
        void Update(LDropDownValueViewModel model, string RedirectToUrl);
       
    }
}
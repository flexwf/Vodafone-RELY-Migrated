using Newtonsoft.Json;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace RELY_APP.Helper
{
    public class RProductSystemsRestClient: IRProductSystemsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
       
        public RProductSystemsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<RProductSystemsViewModel> GetByCompanyCode(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/RProductSystems/GetRProductSystemsByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<RProductSystemsViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public void Add(RProductSystemsViewModel serverData, string RedirectToUrl)
        {
           
            var request = new RestRequest("api/RProductSystems/Post", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);            
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            //var request = new RestRequest("api/RProductSystems/PostRProductSystems?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            //request.AddBody(serverData);
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

        public void Update(RProductSystemsViewModel model, string RedirectToUrl)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            // string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            //var request = new RestRequest("api/RLocalPobTypes/Put?UserName={UserName}&WorkFlow={WorkFlow}&id={id}", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var request = new RestRequest("api/RProductSystems/Put?id={id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", model.Id, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        /// <summary>
        /// Created by Rakhi Singh on  31st july
        /// Description: this method is used to get all Product system on the basis of Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public RProductSystemsViewModel GetById(int Id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/RProductSystems/GetRProductSystemById?id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<RProductSystemsViewModel>(request);
            return response.Data;
        }       

        /// <summary>
        /// Created by Rakhi Singh on 31st july 2018
        /// Description: Method to delete product system on the basis of Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        /// <param name="RedirectToUrl"></param>
        public void Delete(int Id, string RedirectToUrl)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            //var request = new RestRequest("api/RProductSystems/Delete?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.DELETE);

            //request.AddParameter("Id", Id, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            var request = new RestRequest("api/RProductSystems/Delete?Id={Id}", Method.DELETE);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<RProductSystemsViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        /*Below code is commented by Rakhi on 30 july 2018*/
        //public void Update(RProductSystemsViewModel serverData, string RedirectToUrl)
        //{
        //    string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //    string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        //    var request = new RestRequest("api/RProductSystems/PutRProductSystems?CompanyCode={CompanyCode}&Name={Name}&UserName={UserName}&WorkFlow={WorkFlow}", Method.PUT) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyCode", serverData.CompanyCode, ParameterType.UrlSegment);
        //    request.AddParameter("Name", serverData.Name, ParameterType.UrlSegment);
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
        //    request.AddBody(serverData);

        //    var response = _client.Execute<RProductSystemsViewModel>(request);
        //    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
        //    {
        //        //call globals method to generate exception based on response
        //        Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
        //    }

        //    //if (response.StatusCode == HttpStatusCode.NotFound)
        //    //{
        //    //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //    //    ex.Data.Add("ErrorCode", response.StatusCode);
        //    //    string source = response.Content;
        //    //    dynamic data = JsonConvert.DeserializeObject(source);
        //    //    string xx = data.Message;
        //    //    ex.Data.Add("ErrorMessage", xx);
        //    //    throw ex;
        //    //}



        //    //if (response.StatusCode == HttpStatusCode.InternalServerError)
        //    //{
        //    //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //    //    ex.Data.Add("ErrorCode", response.StatusCode);
        //    //    string source = response.Content;
        //    //    dynamic data = JsonConvert.DeserializeObject(source);
        //    //    string xx = data.Message;
        //    //    ex.Data.Add("ErrorMessage", xx);
        //    //    throw ex;
        //    ////}
        //    //if (response.StatusCode == HttpStatusCode.BadRequest)
        //    //{
        //    //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //    //    ex.Data.Add("ErrorCode", response.StatusCode);
        //    //    string source = response.Content;
        //    //    dynamic data = JsonConvert.DeserializeObject(source);
        //    //    string xx = data.Message;
        //    //    ex.Data.Add("ErrorMessage", xx);
        //    //    throw ex;
        //    //}

        //}

        //public void Delete(string CompanyCode)
        //{
        //    var request = new RestRequest("api/RSystems/DeleteRSystems?CompanyCode={CompanyCode}", Method.DELETE);
        //    request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);

        //    var response = _client.Execute<RProductSystemsViewModel>(request);

        //    if (response.StatusCode != HttpStatusCode.OK)
        //    {

        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.Message;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }
        //}
       
        //public RProductSystemsViewModel GetByCode(string CompanyCode, string Name)
        //{
        //    string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //    string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        //    var request = new RestRequest("api/RProductSystems/GetRProductSystems?CompanyCode={CompanyCode}&Name={Name}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };

        //    request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
        //    request.AddParameter("Name", Name, ParameterType.UrlSegment);
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

        //    var response = _client.Execute<RProductSystemsViewModel>(request);

        //    //if (response.Data == null)
        //    //    throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //    //string source = response.Content;
        //    //dynamic data = JsonConvert.DeserializeObject(source);
        //    //return data;
        //}
    }
    interface IRProductSystemsRestClient
    {
        IEnumerable<RProductSystemsViewModel> GetByCompanyCode(string CompanyCode);
        void Add(RProductSystemsViewModel serverData, string RedirectToUrl);       
        RProductSystemsViewModel GetById(int Id);
        void Delete(int Id, string RedirectToUrl);
        void Update(RProductSystemsViewModel model, string RedirectToUrl);
        /*Below code is commented by Rakhi on 30 july 2018*/       
       
        //void Delete(string CompanyCode);
        //RProductSystemsViewModel GetByCode(string CompanyCode, string Name);
    }
}
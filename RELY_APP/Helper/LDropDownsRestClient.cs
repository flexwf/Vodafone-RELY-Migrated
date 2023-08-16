using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace RELY_APP.Helper
{
    public class LDropDownsRestClient:ILDropDownsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        
        public LDropDownsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LDropDownViewModel> GetByComapnyCode(string CompanyCode)
        {
            var request = new RestRequest("api/LDropDowns/GetByCompanyCode?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LDropDownViewModel>>(request);

            return response.Data;
        }

        public LDropDownViewModel GetByName(string Name,string CompanyCode)
        {
            var request = new RestRequest("api/LDropDowns/GetLDropDownByNameCompanyCode?Name={Name}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Name", Name, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<LDropDownViewModel>(request);

            return response.Data;
        }
        public LDropDownViewModel GetById(int Id)
        {
            var request = new RestRequest("api/LDropDowns/GetById?Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<LDropDownViewModel>(request);

            return response.Data;
        }

        public void Add(LDropDownViewModel model, string RedirectToUrl)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            // var request = new RestRequest("api/RLocalPobTypes/Post?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            var request = new RestRequest("api/LDropDowns/Post", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(model);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }

        public void Update(LDropDownViewModel model, string RedirectToUrl)
        {           
            var request = new RestRequest("api/LDropDowns/Put?id={id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", model.Id, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }


        public void Delete(int Id,string RedirectToUrl)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LDropDowns/Delete?Id={Id}", Method.DELETE);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }


        }

    }

    interface ILDropDownsRestClient
        {
        LDropDownViewModel GetByName(string Name,string CompanyCode);
        IEnumerable<LDropDownViewModel> GetByComapnyCode(string CompanyCode);
        void Delete(int Id, string RedirectToUrl);
        void Add(LDropDownViewModel model, string RedirectToUrl);
        LDropDownViewModel GetById(int Id);
        void Update(LDropDownViewModel model, string RedirectToUrl);
        }
}
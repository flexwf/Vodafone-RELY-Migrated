using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using RELY_APP.Utilities;
using System.Web;
using Newtonsoft.Json;

namespace RELY_APP.Helper
{
    public class LReferenceDataRestClient : ILReferenceDataRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;

        public LReferenceDataRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LReferenceDataViewModel> GetByReferenceId(int ReferenceId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LReferenceData/GetByReferenceId?ReferenceId={ReferenceId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ReferenceId", ReferenceId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment); 
             var response = _client.Execute<List<LReferenceDataViewModel>>(request);
            return response.Data;
        }
        public int GetReferenceDataGridCounts(int ReferenceId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LReferenceData/GetReferenceDataGridCounts?ReferenceId={ReferenceId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ReferenceId", ReferenceId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        public IEnumerable<LReferenceDataViewModel> GenerateReferenceDataGrid(int ReferenceId, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LReferenceData/GenerateReferenceDataGrid?ReferenceId={ReferenceId}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&PageNumber={PageNumber}&PageSize={PageSize}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("ReferenceId", ReferenceId, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<LReferenceDataViewModel>>(request);
            return response.Data;
        }
        public IEnumerable<dynamic> GetLReferenceDataForGrid(string CompanyCode, string TableName, int ReferenceTypeId, int ReferenceId, int PageNumber, int PageSize)
        {
            var request = new RestRequest("api/LReferenceData/GetLReferenceDataForGrid?CompanyCode={CompanyCode}&TableName={TableName}&ReferenceTypeId={ReferenceTypeId}&ReferenceId={ReferenceId}&PageNumber={PageNumber}&PageSize={PageSize}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("TableName", TableName, ParameterType.UrlSegment);
            request.AddParameter("ReferenceTypeId", ReferenceTypeId, ParameterType.UrlSegment);
            request.AddParameter("ReferenceId", ReferenceId, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);

            var response = _client.Execute<List<dynamic>>(request);
            // var response1 = JObject.Parse(response.Content);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                ex.Data.Add("ErrorMessage", data.Message);
                throw ex;
            }

            return response.Data;
        }


        public IEnumerable<LReferenceDataViewModel> GetLReferenceDataCounts(string CompanyCode, string TableName)
        {
            var request = new RestRequest("api/LReferenceData/GetLReferenceDataCounts?CompanyCode={CompanyCode}&TableName={TableName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Tablename", TableName, ParameterType.UrlSegment);

            var response = _client.Execute<List<LReferenceDataViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public void Add(List<LReferenceDataViewModel> model, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LReferenceData/PostLReferenceData?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(model);
            request.AddParameter("Id", 0, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }
        public void Update(LReferenceDataViewModel model, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LReferenceData/PutLReferenceData?id={id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(model);

            request.AddParameter("id", model.Id, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }


        }

        public void Delete(int Id, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LReferenceData/DeleteLReferenceData?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.DELETE);

            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }

        public GenericNameAndIdViewModel DownloadReferenceDataGrid(int ReferenceId, string LRefType, string Tablename,string CompanyCode, string OutputFilename)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LReferenceData/DownloadReferenceDataGrid?Tablename={Tablename}&LRefType={LRefType}&ReferenceId={ReferenceId}&CompanyCode={CompanyCode}&OutputFilename={OutputFilename}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Tablename", Tablename, ParameterType.UrlSegment);
            request.AddParameter("LRefType", LRefType, ParameterType.UrlSegment);
            request.AddParameter("ReferenceId", ReferenceId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("OutputFilename", OutputFilename, ParameterType.UrlSegment);            
            var response = _client.Execute<GenericNameAndIdViewModel>(request);
            return response.Data;
        }
    }
    interface ILReferenceDataRestClient
    {
        IEnumerable<LReferenceDataViewModel> GetByReferenceId(int ReferenceId);
        //  void Add(string GridArray,int id, string RedirectToUrl);
        void Add(List<LReferenceDataViewModel> model, string RedirectToUrl);
        void Update(LReferenceDataViewModel model, string RedirectToUrl);
        void Delete(int Id, string RedirectToUrl);

        //To get Dynamic Grid
        IEnumerable<dynamic> GetLReferenceDataForGrid(string CompanyCode, string TableName, int ReferenceTypeId, int ReferenceId, int PageNumber, int PageSize);
        IEnumerable<LReferenceDataViewModel> GetLReferenceDataCounts(string CompanyCode, string TableName);
        // LReferenceDataViewModel GetRefDataByReferenceID(int ReferenceId);
        int GetReferenceDataGridCounts(int ReferenceId);
        IEnumerable<LReferenceDataViewModel> GenerateReferenceDataGrid(int ReferenceId, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery);
        GenericNameAndIdViewModel DownloadReferenceDataGrid(int ReferenceId, string LRefType, string Tablename, string CompanyCode, string OutputFilename);
    }

}
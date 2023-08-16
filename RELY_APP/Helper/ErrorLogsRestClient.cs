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
    class ErrorLogsRestClient : IErrorLogsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        public ErrorLogsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public void Add(GErrorLogViewModel serverData, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GErrorLogs/PostGErrorLog?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
                request.AddParameter("UserName",UserName, ParameterType.UrlSegment);
                request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
                request.AddBody(serverData);

                var response = _client.Execute<GErrorLogViewModel>(request);

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

        public IEnumerable<GErrorLogViewModel> GetGErrorlogGrid(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            var request = new RestRequest("api/GErrorLogs/GetGErrorlogGridData?pagesize={pagesize}&pagenum={pagenum}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);

            var response = _client.Execute<List<GErrorLogViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public int GetGErrorLogscounts()
        {
            var request = new RestRequest("api/GErrorLogs/GetErrorLogCount", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        /// <summary>
        /// Method to get the total counts of Exception records for Summary tab on L2Admin Page
        /// </summary>        
        public int GetExceptionSummaryCounts()
        {
            var request = new RestRequest("api/GErrorLogs/GetExceptionSummaryCounts", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        /// <summary>
        ///  Method to get the data for summary tab of exception on L2Admin Page
        /// </summary>
        public IEnumerable<GErrorLogViewModel> GetExceptionSummary(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {
            var request = new RestRequest("api/GErrorLogs/GetExceptionSummary?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<GErrorLogViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<GErrorLogViewModel> GetExceptionChart()
        {
            var request = new RestRequest("api/GErrorLogs/GetExceptionChart", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<GErrorLogViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
    }


    interface IErrorLogsRestClient
    {
        void Add(GErrorLogViewModel serverData, string RedirectToUrl);
        int GetExceptionSummaryCounts();
        IEnumerable<GErrorLogViewModel> GetExceptionSummary(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery);
        int GetGErrorLogscounts();
        IEnumerable<GErrorLogViewModel> GetGErrorlogGrid(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery);
        IEnumerable<GErrorLogViewModel> GetExceptionChart();

    }
}
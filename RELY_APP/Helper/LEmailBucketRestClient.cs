using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Helper
{
    public class LEmailBucketRestClient : ILEmailBucketRestClient
    {
        private readonly RestClient _client;

        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LEmailBucketRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        //method to get the data's count for summary tab for L2Admin Page
        public int GetEmailBucketSummaryCounts()
        {
            var request = new RestRequest("api/LEmailBucket/GetEmailBucketSummaryCount", Method.GET) { RequestFormat = DataFormat.Json };
            var response = _client.Execute<int>(request);
            return response.Data;
        }

        //method to get the data for detail tab for L2Admin Page
        public IEnumerable<LEmailBucketViewModel> GetEmailBucketDetail(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            var request = new RestRequest("api/LEmailBucket/GetEmailBucketDetail?pagesize={pagesize}&pagenum={pagenum}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<LEmailBucketViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        //method to get the data's count for the detail tab for L2Admin Page
        public int GetEmailBucketDetailCounts()
        {
            var request = new RestRequest("api/LEmailBucket/GetEmailBucketDetailCount", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        //method to get the data for summary tab for L2Admin Page
        public IEnumerable<LEmailBucketViewModel> GetEmailBucketSummary(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {
            var request = new RestRequest("api/LEmailBucket/GetEmailBucketSummary?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<LEmailBucketViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        //method to get the data for Email Bucket Chart
        public IEnumerable<LEmailBucketViewModel> GetEmailBucketChart()
        {
            var request = new RestRequest("api/LEmailBucket/GetEmailBucketChart", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<LEmailBucketViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
    }
    interface ILEmailBucketRestClient
    {
        int GetEmailBucketSummaryCounts();
        IEnumerable<LEmailBucketViewModel> GetEmailBucketDetail(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery);
        int GetEmailBucketDetailCounts();
        IEnumerable<LEmailBucketViewModel> GetEmailBucketSummary(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery);
        IEnumerable<LEmailBucketViewModel> GetEmailBucketChart();
    }
}
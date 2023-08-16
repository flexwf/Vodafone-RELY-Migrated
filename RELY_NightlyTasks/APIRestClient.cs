using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;
using System.Configuration;
using Newtonsoft.Json;

namespace RELY_ScheduledTasks
{
    class APIRestClient :IAPIRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public APIRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        //to generate s15 extracts
        //public IEnumerable<string> GetS15Extracts(List<S15ExtractsViewModel> model, DateTime StartDate, DateTime EndDate, string ExtractName, string CompanyCode, string ExportType, string ExtractFileNameList, string FileFormat)
        public IEnumerable<string> GetS15Extracts(List<S15ExtractsViewModel> model, DateTime StartDate, DateTime EndDate, string CompanyCode, string ExportType, string FileFormat)
        {
            bool IsAutomatic = true;
            //As the target method signature and HTTP method is updated,so updating to POST call
            var request = new RestRequest("api/S15Extracts/GetS15Extracts?StartDate={StartDate}&EndDate={EndDate}&CompanyCode={CompanyCode}&ExportType={ExportType}&FileFormat={FileFormat}&IsAutomatic={IsAutomatic}", Method.POST) { RequestFormat = DataFormat.Json };
            //var request = new RestRequest("api/S15Extracts/GetS15Extracts?StartDate={StartDate}&EndDate={EndDate}&ExtractName={ExtractName}&CompanyCode={CompanyCode}&ExportType={ExportType}&ExtractFileNameList={ExtractFileNameList}&FileFormat={FileFormat}&IsAutomatic={IsAutomatic}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddBody(model);
            request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);
            //request.AddParameter("ExtractName", ExtractName, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("ExportType", ExportType, ParameterType.UrlSegment);
            //request.AddParameter("ExtractFileNameList", ExtractFileNameList, ParameterType.UrlSegment);
            request.AddParameter("FileFormat", FileFormat, ParameterType.UrlSegment);
            request.AddParameter("IsAutomatic", IsAutomatic, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);
            return response.Data;
        }

        //Getting Value of a key from GKeyValues Table
        public GKeyValueViewModel GetKeyValue(string Key, string CompanyCode)
        {
            var request = new RestRequest("api/GKeyValues/GetKeyValue?Key={Key}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Key", Key, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<GKeyValueViewModel>(request);
            return response.Data;
        }
        public List<S15ExtractsViewModel> GetS15GridData(string CompanyCode)
        {
            var request = new RestRequest("api/S15Extracts/GetS15GridData?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<S15ExtractsViewModel>>(request);
            return response.Data;
        }

        public void Add(GErrorLogViewModel serverData, string RedirectToUrl)
        {
            var request = new RestRequest("api/GErrorLogs/PostGErrorLog", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);
            var response = _client.Execute<GErrorLogViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
        private static void GenerateExceptionFromResponse(IRestResponse response, string RedirectToUrl)
        {
            if (string.IsNullOrEmpty(RedirectToUrl))
            {
                RedirectToUrl = "/Home/ErrorPage";
            }
            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            ex.Data.Add("ErrorCode", (int)response.StatusCode);
            ex.Data.Add("RedirectToUrl", RedirectToUrl);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            string xx = data.Message;
            ex.Data.Add("ErrorMessage", xx);
            throw ex;
        }


    }

    interface IAPIRestClient
    {
        //IEnumerable<string> GetS15Extracts(List<S15ExtractsViewModel> model, DateTime StartDate, DateTime EndDate, string ExtractName, string CompanyCode, string ExportType, string ExtractFileNameList, string FileFormat);
        IEnumerable<string> GetS15Extracts(List<S15ExtractsViewModel> model, DateTime StartDate, DateTime EndDate, string CompanyCode, string ExportType, string FileFormat);
        GKeyValueViewModel GetKeyValue(string Key, string CompanyCode);
        List<S15ExtractsViewModel> GetS15GridData(string CompanyCode);
        void Add(GErrorLogViewModel serverData, string RedirectToUrl);
    }


}

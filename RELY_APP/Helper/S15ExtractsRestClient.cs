using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RELY_APP.Helper
{
    public class S15ExtractsRestClient : IS15ExtractsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        public S15ExtractsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
   
        //the target method of API does not exist, need to rechek the references.
        public string GetGGlobalPOB()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/S15Extracts/GetGGlobalPOB?&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<GGlobalPOBViewModel>>(request);
            // return response.Data;
            return response.Content;
        }
        //the target method of API does not exist, need to rechek the references.
        public string GetGCopaDimensions()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/S15Extracts/GetGCopaDimensions?&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<GGlobalPOBViewModel>>(request);
            return response.Content;
        }

        //to generate s15 extracts////
        public IEnumerable<string> GetS15Extracts(List<S15ExtractsViewModel> model, DateTime StartDate, DateTime EndDate, string CompanyCode, string ExportType, string FileFormat)
        {
            bool IsAutomatic = false;
           // var request = new RestRequest("api/S15Extracts/GetS15Extracts?StartDate={StartDate}&EndDate={EndDate}&ExtractName={ExtractName}&CompanyCode={CompanyCode}&ExportType={ExportType}&ExtractFileNameList={ExtractFileNameList}&FileFormat={FileFormat}&IsAutomatic={IsAutomatic}", Method.GET) { RequestFormat = DataFormat.Json };
            var request = new RestRequest("api/S15Extracts/GetS15Extracts?StartDate={StartDate}&EndDate={EndDate}&CompanyCode={CompanyCode}&ExportType={ExportType}&FileFormat={FileFormat}&IsAutomatic={IsAutomatic}", Method.POST) { RequestFormat = DataFormat.Json };
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

        public List<dynamic> GetS15GridData(string CompanyCode)
        {
            var request = new RestRequest("api/S15Extracts/GetS15GridData?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
           
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }
    }

    interface IS15ExtractsRestClient
    {
        //IEnumerable<GGlobalPOBViewModel> GetGGlobalPOB();
        string GetGGlobalPOB();
        string GetGCopaDimensions();

        //IEnumerable<string> GetS15Extracts(DateTime StartDate,DateTime EndDate,string ExtractName,string CompanyCode,string ExportType, string ExtractFileNameList,string FileFormat);
        IEnumerable<string> GetS15Extracts(List<S15ExtractsViewModel> model,DateTime StartDate, DateTime EndDate,string CompanyCode, string ExportType,string FileFormat);
        List<dynamic> GetS15GridData(string CompanyCode);
    }
}
using Newtonsoft.Json;
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
    public class ReportsRestClient : IReportRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

        public ReportsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<RWorkflowViewModel> GetWftypeByBaseTableName(string BaseTableName,string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/Reports/GetWftype?BaseTableName={BaseTableName}&UserName={UserName}&WorkFlow={WorkFlow}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("BaseTableName", BaseTableName, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<RWorkflowViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public IEnumerable<RWorkflowViewModel> GetRequestPerStatus(string CompanyCode, string WFType)
        {
            var request = new RestRequest("api/Reports/GetRequestPerStatus?CompanyCode={CompanyCode}&WFType={WFType}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("WFType", WFType, ParameterType.UrlSegment);
            var response = _client.Execute<List<RWorkflowViewModel>>(request);
            return response.Data;
            //var response = _client.Execute(request);
            //if (response.StatusCode == HttpStatusCode.InternalServerError)
            //{
            //    return null;
            //}
            //string source = response.Content;
            //dynamic data = JsonConvert.DeserializeObject(source);
            //return data;
        }

        public string DownloadRequestPerStatus(string CompanyCode, string WFType)
        {
            var request = new RestRequest("api/Reports/GetRequestPerStatus?CompanyCode={CompanyCode}&WFType={WFType}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("WFType", WFType, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            return response.Content;
        }

        public IEnumerable<RWorkflowViewModel> GetRequestsPerDateInterval(string CompanyCode)
        {
            var request = new RestRequest("api/Reports/GetRequestsPerDateInterval?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<RWorkflowViewModel>>(request);
            return response.Data;
            //var response = _client.Execute(request);
            //if (response.StatusCode == HttpStatusCode.InternalServerError)
            //{
            //    return null;
            //}
            //string source = response.Content;
            //dynamic data = JsonConvert.DeserializeObject(source);
            //return data;
        }
       
        public string DownloadReqPerDateInt(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/Reports/GetRequestsPerDateInterval?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute(request);           
            return response.Content;
        }

        public string GetRequestDetails(string CompanyCode, int Id)
        {
            var request = new RestRequest("api/Reports/GetRequestDetails?CompanyCode={CompanyCode}&Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);
            return response.Content;
        }
       
        public List<dynamic> GetReportofproduct(string CompanyCode,string FilterType, string FilterValue)
        {
            var request = new RestRequest("api/Reports/GetReportOfProducts?CompanyCode={CompanyCode}&FilterType={FilterType}&FilterValue={FilterValue}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("FilterType", FilterType, ParameterType.UrlSegment);
            request.AddParameter("FilterValue", FilterValue, ParameterType.UrlSegment);

            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }

        public DownloadFileNameViewModel DownloadReportofProducts(string CompanyCode, string FilterType, string FilterValue)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/Reports/DownloadReportOfProducts?CompanyCode={CompanyCode}&FilterType={FilterType}&FilterValue={FilterValue}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("FilterType", FilterType, ParameterType.UrlSegment);
            request.AddParameter("FilterValue", FilterValue, ParameterType.UrlSegment);           
            var response = _client.Execute<DownloadFileNameViewModel>(request);  /*As Restclient throws error while returning string to return filename only,so returning filename in new viewmodel */
            return response.Data;
        }

        public IEnumerable<LRequestViewModel> GetRequestName(string CompanyCode)
        {
            var request = new RestRequest("api/Reports/GetRequestName?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LRequestViewModel>>(request);
            return response.Data;
        }

        public IEnumerable<LRequestViewModel> GetRequestsInProgress(string CompanyCode, int Interval, int NumberofBuckets)
        {
            var request = new RestRequest("api/Reports/GetRequestsInProgress?CompanyCode={CompanyCode}&Interval={Interval}&NumberofBuckets={NumberofBuckets}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Interval", Interval, ParameterType.UrlSegment);
            request.AddParameter("NumberofBuckets", NumberofBuckets, ParameterType.UrlSegment);
            var response = _client.Execute<List<LRequestViewModel>>(request);
            return response.Data;
        }
        
        public string DownloadRequestInProg(string CompanyCode, int Interval, int NumberofBuckets)
        {           
            var request = new RestRequest("api/Reports/GetRequestsInProgress?CompanyCode={CompanyCode}&Interval={Interval}&NumberofBuckets={NumberofBuckets}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Interval", Interval, ParameterType.UrlSegment);
            request.AddParameter("NumberofBuckets", NumberofBuckets, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            return response.Content;
        }

        public IEnumerable<WStepsViewModel> GetStatusByBaseTableName(string BaseTableName, string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/Reports/GetStatus?BaseTableName={BaseTableName}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("BaseTableName", BaseTableName, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<WStepsViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<RPTAccountingScenariosViewModel> GetAccountingScenarios(string CompanyCode, string StartDate, string EndDate)
        {
            var request = new RestRequest("api/Reports/GetAccountingScenarios?CompanyCode={CompanyCode}&StartDate={StartDate}&EndDate={EndDate}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);
            var response = _client.Execute<List<RPTAccountingScenariosViewModel>>(request);
            return response.Data;
        }
        
        public string DownloadAccountingSenario(string CompanyCode, string StartDate, string EndDate)
        {
            var request = new RestRequest("api/Reports/GetAccountingScenarios?CompanyCode={CompanyCode}&StartDate={StartDate}&EndDate={EndDate}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            return response.Content;
        }
        public IEnumerable<LAccountingScenarioViewModel> GetAccountingScenarioList(string CompanyCode)
        {
            var request = new RestRequest("api/Reports/GetAccountingScenarioList?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);

            var response = _client.Execute<List<LAccountingScenarioViewModel>>(request);
            return response.Data;
        }

        public IEnumerable<LScenarioDemandViewModel> GetNewScenarioDemand(string CompanyCode, int Interval, int NumberofBuckets)
        {
            var request = new RestRequest("api/Reports/GetNewScenarioDemand?CompanyCode={CompanyCode}&Interval={Interval}&NumberofBuckets={NumberofBuckets}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Interval", Interval, ParameterType.UrlSegment);
            request.AddParameter("NumberofBuckets", NumberofBuckets, ParameterType.UrlSegment);
            var response = _client.Execute<List<LScenarioDemandViewModel>>(request);
            return response.Data;
        }
        
        public string DownloadScenarioDemands(string CompanyCode, int Interval, int NumberofBuckets)
        {
            var request = new RestRequest("api/Reports/GetNewScenarioDemand?CompanyCode={CompanyCode}&Interval={Interval}&NumberofBuckets={NumberofBuckets}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Interval", Interval, ParameterType.UrlSegment);
            request.AddParameter("NumberofBuckets", NumberofBuckets, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            return response.Content;
        }
        public GenericNameAndIdViewModel DownloadAccountingScenariosList(string CompanyCode)
        {
            var request = new RestRequest("api/Reports/DownloadAccountingScenariosList?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<GenericNameAndIdViewModel>(request);
            return response.Data;
        }

        public string DownloadAuditReports(string CompanyCode)
        {
            var request = new RestRequest("api/Reports/DownloadAuditReports?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<GenericNameAndIdViewModel>(request);
            string source = response.Content;
            string data = Convert.ToString(JsonConvert.DeserializeObject(source)); 
            return data;
        }
    }
   
    interface IReportRestClient
    {
        IEnumerable<RWorkflowViewModel> GetWftypeByBaseTableName(string BaseTableName, string CompanyCode);
        IEnumerable<RWorkflowViewModel> GetRequestPerStatus(string CompanyCode, string WFType);
        IEnumerable<RWorkflowViewModel> GetRequestsPerDateInterval(string CompanyCode);
        string GetRequestDetails(string CompanyCode, int Id);
        IEnumerable<LRequestViewModel> GetRequestName(string CompanyCode);
        IEnumerable<LRequestViewModel> GetRequestsInProgress(string CompanyCode, int Interval, int NumberofBuckets);
        List<dynamic> GetReportofproduct(string CompanyCode, string FilterType, string FilterValue);
        IEnumerable<WStepsViewModel> GetStatusByBaseTableName(string BaseTableName, string CompanyCodes);
        IEnumerable<RPTAccountingScenariosViewModel> GetAccountingScenarios(string CompanyCode, string StartDate, string EndDate);
        IEnumerable<LAccountingScenarioViewModel> GetAccountingScenarioList(string CompanyCode);
        IEnumerable<LScenarioDemandViewModel> GetNewScenarioDemand(string CompanyCode, int Interval, int NumberofBuckets);
        string DownloadAccountingSenario(string CompanyCode, string StartDate, string EndDate);
        GenericNameAndIdViewModel DownloadAccountingScenariosList(string CompanyCode);
        string DownloadRequestPerStatus(string CompanyCode, string WFType);
        string DownloadAuditReports(string CompanyCode);
        string DownloadReqPerDateInt(string CompanyCode);
        string DownloadRequestInProg(string CompanyCode, int Interval, int NumberofBuckets);
        string DownloadScenarioDemands(string CompanyCode, int Interval, int NumberofBuckets);
        DownloadFileNameViewModel DownloadReportofProducts(string CompanyCode, string FilterType, string FilterValue);
    }
}

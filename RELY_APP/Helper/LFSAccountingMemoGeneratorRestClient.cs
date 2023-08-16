using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RELY_APP.Helper
{
    public class LFSAccountingMemoGeneratorRestClient : IRLFSAccountingMemoGeneratorRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        public LFSAccountingMemoGeneratorRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public string DownloadAccountingMemo(string EntityType, int EntityId,int SurveyId,int UserId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSAccountingMemoGenerator/DownloadAccountingMemo?EntityType={EntityType}&EntityId={EntityId}&SurveyId={SurveyId}&UserId={UserId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("SurveyId", SurveyId, ParameterType.UrlSegment);
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LFSAccountingMemoGeneratorViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Content;
        }

        public string DownloadNextSteps(string EntityType, int EntityId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSNextSteps/GetNextStepActionsByEntityIdType?EntityId={EntityId}&EntityType={EntityType}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LFSNextStepActionViewModel>>(request);
            return response.Content;
        }

        public string DownloadAccountingScenarioMatrix(int EntityId, string EntityType)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var request = new RestRequest("api/LFSAccountingMemoGenerator/DownloadAccountingScenarioMatrix?EntityId={EntityId}&EntityType={EntityType}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LFSAccountingMemoGeneratorViewModel>>(request);
            return response.Content;
        }
        //public IEnumerable<LFSAccountingMemoGeneratorViewModel> DownloadAccountingMemo(string EntityType, int EntityId)
        //{
        //    var request = new RestRequest("api/LFSAccountingMemoGenerator/DownloadAccountingMemo?EntityType={EntityType}&EntityId={EntityId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
        //    request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LFSAccountingMemoGeneratorViewModel>>(request);

        //    //if (response.Data == null)
        //    //    throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}
    }

    interface IRLFSAccountingMemoGeneratorRestClient
    {
        string DownloadAccountingMemo(string EntityType, int EntityId, int SurveyId, int UserId);
        string DownloadNextSteps(string EntityType, int EntityId);
        string DownloadAccountingScenarioMatrix(int EntityId, string EntityType);
        //string DownloadAccountingScenarioMatrix(string EntityType, int EntityId);
        //IEnumerable<LFSAccountingMemoGeneratorViewModel> DownloadAccountingMemo(string EntityType,int EntityId);
    }
}
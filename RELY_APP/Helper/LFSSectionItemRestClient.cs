using RELY_APP.Utilities;
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
    public class LFSSectionItemRestClient : ILFSSectionItemRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

        public LFSSectionItemRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LFSSectionItemViewModel> GetById(int Id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSSectionItems/GetLFSSectionItemsById?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LFSSectionItemViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        
        public int GetTableItemsCountForSection(string SectionCode, string ChapterCode, int SurveyId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSSectionItems/GetTableItemsCountForSection?SectionCode={SectionCode}&ChapterCode={ChapterCode}&SurveyId={SurveyId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SectionCode", SectionCode, ParameterType.UrlSegment);
            request.AddParameter("ChapterCode", ChapterCode, ParameterType.UrlSegment);
            request.AddParameter("SurveyId", SurveyId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        

        public int GetItemsCountExcludingTable(string SectionCode, string ChapterCode, int SurveyId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSSectionItems/GetItemsCountExcludingTable?SectionCode={SectionCode}&ChapterCode={ChapterCode}&SurveyId={SurveyId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SectionCode", SectionCode, ParameterType.UrlSegment);
            request.AddParameter("ChapterCode", ChapterCode, ParameterType.UrlSegment);
            request.AddParameter("SurveyId", SurveyId, ParameterType.UrlSegment);

            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }

        public IEnumerable<LFSSectionItemViewModel> GetSectionItem()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSSectionItems/GetLFSSectionItem?&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LFSSectionItemViewModel>>(request);
            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        public string DownloadSectionItems()
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSSectionItems/GetLFSSectionItem?&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            return response.Content;
        }

        public IEnumerable<LFSSectionItemViewModel> GetBySectionCode(string SectionCode, string ChapterCode, int EntityId,string EntityType,string CompanyCode,int SurveyId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSSectionItems/GetLFSSectionItemsBySectionCode?SectionCode={SectionCode}&ChapterCode={ChapterCode}&EntityId={EntityId}&EntityType={EntityType}&SurveyId={SurveyId}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SectionCode", SectionCode, ParameterType.UrlSegment);
            request.AddParameter("ChapterCode", ChapterCode, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("SurveyId", SurveyId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LFSSectionItemViewModel>>(request);

            return response.Data;
        }
        public void Add(LFSSectionItemViewModel serverData, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSSectionItems/PostLFSSectionItems?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LFSSectionItemViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        public int AddData(List<LFSSectionItemViewModel> model, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSSectionItems/PostLFSSectionItem?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            request.AddBody(model);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }


        public void Update(LFSSectionItemViewModel serverData, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSSectionItems/PutLFSSectionItems?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", serverData.Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LFSSectionItemViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        public void Delete(int Id, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LFSSectionItems/DeleteLFSSectionItems?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.DELETE);

            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            var response = _client.Execute<LFSSectionItemViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }

        public int GetQuestionCodeCountForSurvey(string QuestionCode, int SurveyId)
        {
           var request = new RestRequest("api/LFSSectionItems/GetQuestionCodeCountForSurvey?QuestionCode={QuestionCode}&SurveyId={SurveyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("QuestionCode", QuestionCode, ParameterType.UrlSegment);
            request.AddParameter("SurveyId", SurveyId, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }




        public IEnumerable<LFinancialSurveysViewModel> GetSurveyName(string CompanyCode)
        {
            var request = new RestRequest("api/LFSSectionItems/GetSurveyName?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LFinancialSurveysViewModel>>(request);
            return response.Data;
        }

       

    }

    interface ILFSSectionItemRestClient
    {
        IEnumerable<LFSSectionItemViewModel> GetById(int Id);
        int GetTableItemsCountForSection(string SectionCode, string ChapterCode, int SurveyId);
        int GetItemsCountExcludingTable(string SectionCode, string ChapterCode, int SurveyId);
        int GetQuestionCodeCountForSurvey(string QuestionCode,int SurveyId);
        IEnumerable<LFSSectionItemViewModel> GetSectionItem();
        IEnumerable<LFinancialSurveysViewModel> GetSurveyName(string CompanyCode);
        IEnumerable<LFSSectionItemViewModel> GetBySectionCode(string SectionCode, string ChapterCode, int EntityId, string EntityType, string CompanyCode,int SurveyId);
        void Add(LFSSectionItemViewModel serverData, string RedirectToUrl);
        int AddData(List<LFSSectionItemViewModel> model, string RedirectToUrl);
        void Update(LFSSectionItemViewModel serverData, string RedirectToUrl);
        void Delete(int Id, string RedirectToUrl);
        string DownloadSectionItems();
    }
}
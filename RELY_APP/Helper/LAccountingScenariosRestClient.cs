using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace RELY_APP.Helper
{
    public class LAccountingScenariosRestClient: ILAccountingScenariosRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        public LAccountingScenariosRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LAccountingScenarioViewModel> GetByQuestionCodeCompanyCode(string QuestionCode,string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LAccountingScenarios/GetByQuestionCodeCompanyCode?QuestionCode={QuestionCode}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("QuestionCode", QuestionCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LAccountingScenarioViewModel>>(request);

            return response.Data;
        }
        public IEnumerable<LAccountingScenarioViewModel> GetByCompanyCode(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LAccountingScenarios/GetByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LAccountingScenarioViewModel>>(request);

            return response.Data;
        }
        public IEnumerable<LAccountingScenarioViewModel> GetCompletedList(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LAccountingScenarios/GetCompletedList?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LAccountingScenarioViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<GetAccountingScenarioMatrixViewModel> GetAccountingScenarioMatrix(int EntityId,string EntityType, string TabName,string CompanyCode)
        {
            var request = new RestRequest("api/LAccountingScenarios/GetAccountingScenarioMatrix?EntityId={EntityId}&EntityType={EntityType}&TabName={TabName}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("TabName", TabName, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            var response = _client.Execute<List<GetAccountingScenarioMatrixViewModel>>(request);

            return response.Data;
        }

        public string DownloadGAccountScenerioMatrix(int EntityId, string EntityType, string TabName, string CompanyCode)
        {
            var request = new RestRequest("api/LAccountingScenarios/GetAccountingScenarioMatrix?EntityId={EntityId}&EntityType={EntityType}&TabName={TabName}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("TabName", TabName, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            var response = _client.Execute<List<GetAccountingScenarioMatrixViewModel>>(request);

            return response.Content;

        }
        //GetAccountingScenarioMatrix
        public GetAccountingScenarioMatrixViewModel GetAccountingScenarioByResponseId(int ResponseId, string CompanyCode)
        {
            var request = new RestRequest("api/LAccountingScenarios/GetAccountingScenarioByResponseId?ResponseId={ResponseId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("ResponseId", ResponseId, ParameterType.UrlSegment);
            var response = _client.Execute<GetAccountingScenarioMatrixViewModel>(request);

            return response.Data;
        }

        public IEnumerable<LFSResponsViewModel> GetRecommendedAccountingScenarioByResponseId(int ResponseId,string CompanyCode)
        {
            var request = new RestRequest("api/LAccountingScenarios/GetRecommendedAccountingScenarioByResponseId?ResponseId={ResponseId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("ResponseId", ResponseId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LFSResponsViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<LFSManualAccountingScenarioViewModel> GetManualAccountingScenario(int EntityId, string EntityType)
        {
            var request = new RestRequest("api/LAccountingScenarios/GetManualAccountingScenario?EntityId={EntityId}&EntityType={EntityType}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            var response = _client.Execute<List<LFSManualAccountingScenarioViewModel>>(request);

            return response.Data;
        }

        //public IEnumerable<GetAccountingScenarioMatrixViewModel> GetOverviewAccountingScenario(int EntityId, string EntityType, int LoggedInUserId, string CompanyCode)
        //{
        //    var request = new RestRequest("api/LAccountingScenarios/GetOverviewAccountingScenario?EntityId={EntityId}&EntityType={EntityType}&LoggedInUserId={LoggedInUserId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
        //    request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
        //    request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
        //    request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<GetAccountingScenarioMatrixViewModel>>(request);

        //    return response.Data;
        //}
        public void SaveManualAccountingScenario(Object[] ServerData, string CompanyCode)
        {
            var request = new RestRequest("api/LAccountingScenarios/SaveManualAccountingScenario?CompanyCode={CompanyCode}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddBody(ServerData);
            var response = _client.Execute(request);
            
        }

        public IEnumerable<LAccountingScenarioViewModel> GetAccountingScenarioByCompanyCode(string CompanyCode)
        {
            var request = new RestRequest("api/LAccountingScenarios/GetAccountingScenarioByCompanyCode?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LAccountingScenarioViewModel>>(request);

            return response.Data;
        }
        public void SaveAccountingScenario(int AccountingScenarioId, int ResponseId, string CompanyCode, int LoggedinUserId)
        {
            var request = new RestRequest("api/LAccountingScenarios/SaveAccountingScenario?CompanyCode={CompanyCode}&AccountingScenarioId={AccountingScenarioId}&ResponseId={ResponseId}&LoggedinUserId={LoggedinUserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("AccountingScenarioId", AccountingScenarioId, ParameterType.UrlSegment);
            request.AddParameter("ResponseId", ResponseId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("LoggedinUserId", LoggedinUserId, ParameterType.UrlSegment);
            var response = _client.Execute(request);
        }
        public void SaveRecommendedAccountingScenario(int AnswerBankId, int ResponseId, string CompanyCode, int LoggedinUserId)
        {
            var request = new RestRequest("api/LAccountingScenarios/SaveRecommendedAccountingScenario?AnswerBankId={AnswerBankId}&ResponseId={ResponseId}&CompanyCode={CompanyCode}&LoggedinUserId={LoggedinUserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("AnswerBankId", AnswerBankId, ParameterType.UrlSegment);
            request.AddParameter("ResponseId", ResponseId, ParameterType.UrlSegment);
            request.AddParameter("LoggedinUserId", LoggedinUserId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LAccountingScenarioViewModel>>(request);
        }

        public void UpdateResponseComments(int ResponseId,string Comments)
        {
            var request = new RestRequest("api/LAccountingScenarios/SaveResponseComments?ResponseId={ResponseId}&Comments={Comments}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ResponseId", ResponseId, ParameterType.UrlSegment);
            request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
            var response = _client.Execute<List<GetAccountingScenarioMatrixViewModel>>(request);
        }

        public void DeleteResponse(int ResponseId)
        {
            var request = new RestRequest("api/LAccountingScenarios/DeleteResponse?ResponseId={ResponseId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ResponseId", ResponseId, ParameterType.UrlSegment);
            var response = _client.Execute<List<GetAccountingScenarioMatrixViewModel>>(request);
        }

        public LAccountingScenarioViewModel GetByReference(string Reference)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LAccountingScenarios/GetByReference?Reference={Reference}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Reference", Reference, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LAccountingScenarioViewModel>(request);

            return response.Data;
        }
        public LAccountingScenarioViewModel GetById(int Id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LAccountingScenarios/GetById?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LAccountingScenarioViewModel>(request);

            return response.Data;
        }
        public int Add(LAccountingScenarioViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LAccountingScenarios/PostData?UserName={UserName}&WorkFlow={WorkFlow}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("FileList", FileList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("OriginalFileNameList", OriginalFileNameList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocumentsDescription", SupportingDocumentsDescription, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

            return response.Data;
        }
        public void Update(LAccountingScenarioViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

            var request = new RestRequest("api/LAccountingScenarios/PutData?id={id}&UserName={UserName}&WorkFlow={WorkFlow}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("FileList", FileList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("OriginalFileNameList", OriginalFileNameList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocumentsDescription", SupportingDocumentsDescription, ParameterType.UrlSegment);
            request.AddBody(model);
            request.AddParameter("id", model.Id, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }

    }

    interface ILAccountingScenariosRestClient
    {
        IEnumerable<LAccountingScenarioViewModel> GetByCompanyCode(string CompanyCode);
        IEnumerable<LAccountingScenarioViewModel> GetCompletedList(string CompanyCode);
        IEnumerable<LAccountingScenarioViewModel> GetByQuestionCodeCompanyCode(string QuestionCode, string CompanyCode);
        int Add(LAccountingScenarioViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList);
        void Update(LAccountingScenarioViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList);
        LAccountingScenarioViewModel GetById(int Id);
        LAccountingScenarioViewModel GetByReference(string Reference);
        IEnumerable<GetAccountingScenarioMatrixViewModel> GetAccountingScenarioMatrix(int EntityId,string EntityType, string TabName, string CompanyCode);
        void UpdateResponseComments(int ResponseId,string Comments);
        IEnumerable<LFSResponsViewModel> GetRecommendedAccountingScenarioByResponseId(int ResponseId, string CompanyCode);
        IEnumerable<LAccountingScenarioViewModel> GetAccountingScenarioByCompanyCode(string CompanyCode);
        void SaveAccountingScenario(int AccountingScenarioId, int ResponseId, string CompanyCode, int LoggedinUserId);
        void SaveRecommendedAccountingScenario(int AnswerBankId, int ResponseId, string CompanyCode, int LoggedinUserId);
        void DeleteResponse(int ResponseId);
        GetAccountingScenarioMatrixViewModel GetAccountingScenarioByResponseId(int ResponseId, string CompanyCode);
        void SaveManualAccountingScenario(Object[] ServerData, string CompanyCode);
        IEnumerable<LFSManualAccountingScenarioViewModel> GetManualAccountingScenario(int EntityId, string EntityType);
        //IEnumerable<GetAccountingScenarioMatrixViewModel> GetOverviewAccountingScenario(int EntityId, string EntityType, int LoggedInUserId, string CompanyCode);
        string DownloadGAccountScenerioMatrix(int EntityId, string EntityType, string TabName, string CompanyCode);
    }
}
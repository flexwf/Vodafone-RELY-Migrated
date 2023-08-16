using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace RELY_APP.Helper
{
    public class LScenarioDemandRestClient : ILScenarioDemandRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        string WorkFlow = "AccountingScenario";
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        public LScenarioDemandRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LScenarioDemandViewModel> GetByCompanyCode(string CompanyCode)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LScenarioDemand/GetByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LScenarioDemandViewModel>>(request);

            return response.Data;
        }
      
        public LScenarioDemandViewModel GetById(int Id)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LScenarioDemand/GetById?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LScenarioDemandViewModel>(request);

            return response.Data;
        }
        public int Add(LScenarioDemandViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LScenarioDemand/PostData?UserName={UserName}&WorkFlow={WorkFlow}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}", Method.POST) { RequestFormat = DataFormat.Json };
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
        public void Update(LScenarioDemandViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

            var request = new RestRequest("api/LScenarioDemand/PutData?id={id}&UserName={UserName}&WorkFlow={WorkFlow}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}", Method.PUT) { RequestFormat = DataFormat.Json };
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

    interface ILScenarioDemandRestClient
    {
        IEnumerable<LScenarioDemandViewModel> GetByCompanyCode(string CompanyCode);
        int Add(LScenarioDemandViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList);
        void Update(LScenarioDemandViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList);
        LScenarioDemandViewModel GetById(int Id);
    }
}
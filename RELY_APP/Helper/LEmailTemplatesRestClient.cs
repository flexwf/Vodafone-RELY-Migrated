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
    public class LEmailTemplatesRestClient: ILEmailTemplatesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LEmailTemplatesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public void SaveTemplate(LEmailTemplateViewModel Data, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            //  var request = new RestRequest("api/LEmailTemplates/PostLEmailTemplate", Method.POST) { RequestFormat = DataFormat.Json };
            var request = new RestRequest("api/LEmailTemplates/PostLEmailTemplate?FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}&LoggedInUserId={LoggedInUserId}&UserRoleId={UserRoleId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("FileList", FileList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("OriginalFileNameList", OriginalFileNameList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocumentsDescription", SupportingDocumentsDescription, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("UserRoleId", UserRoleId, ParameterType.UrlSegment);

            request.AddBody(Data);

            var response = _client.Execute<LEmailTemplateViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        public void UpdateTemplate(int id,LEmailTemplateViewModel Data, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {

            //  var request = new RestRequest("api/LEmailTemplates/PostLEmailTemplate", Method.POST) { RequestFormat = DataFormat.Json };
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());

            var request = new RestRequest("api/LEmailTemplates/PutLEmailTemplate?id={id}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}&LoggedInUserId={LoggedInUserId}&UserRoleId={UserRoleId}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("FileList", FileList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("OriginalFileNameList", OriginalFileNameList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocumentsDescription", SupportingDocumentsDescription, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("UserRoleId", UserRoleId, ParameterType.UrlSegment);

            request.AddBody(Data);

            var response = _client.Execute<LEmailTemplateViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

       
        public IEnumerable<LEmailTemplateViewModel> GetLEmailTemplates(string CompanyCode)
        {


            var request = new RestRequest("api/LEmailTemplates/GetLEmailTemplates?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);

            var response = _client.Execute<List<LEmailTemplateViewModel>>(request);

            return response.Data;

        }

        public LEmailTemplateViewModel GetLEmailTemplateById(int id)
        {
             var request = new RestRequest("api/LEmailTemplates/GetLEmailTemplateById?id={id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", id, ParameterType.UrlSegment);
           
            var response = _client.Execute<LEmailTemplateViewModel>(request);
            return response.Data;
        }

        public void DeleteLEmailTemplate(int id, string CompanyCode, string RedirectToUrl)
        {
           
            var request = new RestRequest("api/LEmailTemplates/DeleteLEmailTemplate?id={id}&CompanyCode={CompanyCode}", Method.DELETE);

            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }

    }

    interface ILEmailTemplatesRestClient
    {

        void SaveTemplate(LEmailTemplateViewModel Data, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList);
        IEnumerable<LEmailTemplateViewModel> GetLEmailTemplates(string CompanyCode);
        LEmailTemplateViewModel GetLEmailTemplateById(int id);
        void UpdateTemplate(int id, LEmailTemplateViewModel Data, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList);
        void DeleteLEmailTemplate(int id, string CompanyCode, string RedirectToUrl);
    }
}
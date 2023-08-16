using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace RELY_APP.Helper
{
    public class LReferencesRestClient: ILReferencesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LReferencesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public GenericNameAndIdViewModel ValidateColumnValue(string TableName, string ColumnName, string Value, string FieldLabel, string SelecterType, string CompanyCode,string RedirectToUrl)
        {
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            var request = new RestRequest("api/LReferences/ValidateColumnValue?UserName={UserName}&WorkFlow={WorkFlow}&TableName={TableName}&ColumnName={ColumnName}&FieldLabel={FieldLabel}&SelecterType={SelecterType}&CompanyCode={CompanyCode}&Value={Value}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("TableName", TableName, ParameterType.UrlSegment);
            request.AddParameter("ColumnName", ColumnName, ParameterType.UrlSegment);
            request.AddParameter("Value", Value, ParameterType.UrlSegment);
            request.AddParameter("FieldLabel", FieldLabel, ParameterType.UrlSegment);
            request.AddParameter("SelecterType", SelecterType, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<GenericNameAndIdViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }

        public LReferencesViewModel GetLReferencesById(int ReferenceId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LReferences/GetLReferencesById?ReferenceId={ReferenceId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ReferenceId", ReferenceId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LReferencesViewModel>(request);
            return response.Data;
        }

        //public InformationSchemaViewModel GetInformationSchema(string TableName, string ColumnName)
        //{//To get Schema
        //    var request = new RestRequest("api/LReferences/GetInformationSchema?TableName={TableName}&ColumnName={ColumnName}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("TableName", TableName, ParameterType.UrlSegment);
        //    request.AddParameter("ColumnName", ColumnName, ParameterType.UrlSegment);
        //   // request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
        //    var response = _client.Execute<InformationSchemaViewModel>(request);
        //    return response.Data;
        //}
        
        ////this is not used any where, Created by SG for getting schema details of the given table. Keeping it for future reference.
        //public IEnumerable<InformationSchemaViewModel> GetInformationSchemaForTable(string TableName)
        //{
        //    var request = new RestRequest("api/LReferences/GetInformationSchemaForTable?TableName={TableName}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("TableName", TableName, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<InformationSchemaViewModel>>(request);
        //    return response.Data;
        //}

        //Add method

        public int Add(LReferencesViewModel model, string RedirectToUrl, string CompanyCode, int LoggedInUserId, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList,string DataUploadFileName, bool IsDataUploadedByFile)
        {
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            //string WorkFlow = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            var request = new RestRequest("api/LReferences/PostLReference?UserName={UserName}&WorkFlow={WorkFlow}&CompanyCode={CompanyCode}&LoggedInUserId={LoggedInUserId}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}&DataUploadFileName={DataUploadFileName}&IsDataUploadedByFile={IsDataUploadedByFile}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("FileList", FileList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("OriginalFileNameList", OriginalFileNameList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocumentsDescription", SupportingDocumentsDescription, ParameterType.UrlSegment);
            request.AddParameter("DataUploadFileName", DataUploadFileName, ParameterType.UrlSegment);
            request.AddParameter("IsDataUploadedByFile", IsDataUploadedByFile, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }

        public void Update(LReferencesViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList,string CompanyCode, int LoggedInUserId,bool OverwriteExistingData,string ActionName,string DataUploadFileName, bool IsDataUploadedByFile)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            //update
            var request = new RestRequest("api/LReferences/PutLReference/{id}?UserName={UserName}&WorkFlow={WorkFlow}&CompanyCode={CompanyCode}&LoggedInUserId={LoggedInUserId}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}&DataUploadFileName={DataUploadFileName}&IsDataUploadedByFile={IsDataUploadedByFile}&OverwriteExistingData={OverwriteExistingData}&ActionName={ActionName}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("FileList", FileList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("OriginalFileNameList", OriginalFileNameList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocumentsDescription", SupportingDocumentsDescription, ParameterType.UrlSegment);
            request.AddParameter("DataUploadFileName", DataUploadFileName, ParameterType.UrlSegment);
            request.AddParameter("IsDataUploadedByFile", IsDataUploadedByFile, ParameterType.UrlSegment);
            request.AddParameter("OverwriteExistingData", OverwriteExistingData, ParameterType.UrlSegment);
            request.AddParameter("ActionName", ActionName, ParameterType.UrlSegment);
            request.AddBody(model);
            request.AddParameter("id", model.Id, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        //public void Delete(int Id, string Name, string RedirectToUrl)
        //{
        //    string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //    string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string; 
        //    var request = new RestRequest("api/LReferences/DeleteLReference?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.DELETE);

        //    request.AddParameter("Id", Id, ParameterType.UrlSegment);
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

        //    var response = _client.Execute<int>(request);
        //    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
        //    {
        //        //call globals method to generate exception based on response
        //        Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
        //    }


        //}

        public int ReadAndValidateExcelData(int LoggedInUserId, string filename, string CompanyCode, string SelecterType, string RedirectToUrl)
        {
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            var request = new RestRequest("api/LReferences/ReadAndValidateExcelData?UserName={UserName}&WorkFlow={WorkFlow}&LoggedInUserId={LoggedInUserId}&filename={filename}&CompanyCode={CompanyCode}&SelecterType={SelecterType}", Method.GET);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("filename", filename, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("SelecterType", SelecterType, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }

        public List<dynamic> GetInvalidRecords(int LoggedInUserId, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LReferences/GetInValidDataRecords?UserName={UserName}&WorkFlow={WorkFlow}&LoggedInUserId={LoggedInUserId}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&PageNumber={PageNumber}&PageSize={PageSize}", Method.GET);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }

        public int CheckExistenceOfExtractFileName(string CompanyCode, string ExtractFileName)
        {
            var request = new RestRequest("api/LReferences/CheckExistenceOfExtractFileName?ExtractFileName={ExtractFileName}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ExtractFileName", ExtractFileName, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            return response.Data;

        }

        public List<string> GetExtractFileNames(string CompanyCode, int Id)
        {
            var request = new RestRequest("api/LReferences/GetExtractFileNames?CompanyCode={CompanyCode}&Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);

            return response.Data;

        }
    }

    interface ILReferencesRestClient
    {
        GenericNameAndIdViewModel ValidateColumnValue(string TableName, string ColumnName, string Value, string FieldLabel, string SelecterType, string CompanyCode, string RedirectToUrl);
        //InformationSchemaViewModel GetInformationSchema(string TableName, string ColumnName);
        int Add(LReferencesViewModel model, string RedirectToUrl,string CompanyCode, int LoggedInUserId, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList,string DataUploadFileName, bool IsDataUploadedByFile);
        void Update(LReferencesViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList, string CompanyCode, int LoggedInUserId, bool OverwriteExistingData, string ActionName, string DataUploadFileName, bool IsDataUploadedByFile);
        //void Delete(int Id, string Name, string RedirectToUrl);
        LReferencesViewModel GetLReferencesById(int ReferenceId);

        //IEnumerable<InformationSchemaViewModel> GetInformationSchemaForTable(string TableName);
        int ReadAndValidateExcelData(int LoggedInUserId, string filename, string CompanyCode,string SelecterType, string RedirectToUrl);
        List<dynamic> GetInvalidRecords(int LoggedInUserId, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery);
        int CheckExistenceOfExtractFileName(string CompanyCode, string ExtractFileName);
        List<string> GetExtractFileNames(string CompanyCode, int Id);
    }
}
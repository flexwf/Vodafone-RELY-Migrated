using Newtonsoft.Json;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace RELY_APP.Helper
{
    public class LCompanySpecificColumnsRestClient : ILCompanySpecificColumnsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        public LCompanySpecificColumnsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public List<string> GetSelecterTypeByTableName(string TableName, string CompanyCode)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/GetSelecterTypeByTableName?TableName={TableName}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("TableName", TableName, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);

            return response.Data;
        }

        public IEnumerable<GetTableNamesForFormConfigViewModel> GetTableNamesByCompanyCode(string CompanyCode)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/GetTableNamesByCompanyCode/?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<GetTableNamesForFormConfigViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<LCompanySpecificColumnViewModel> GetLCompanySpecificColumnsByTableName(string TableName, string SelecterType)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/GetCompanySpecificColumnsByTableName?TableName={TableName}&SelecterType={SelecterType}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("TableName", TableName, ParameterType.UrlSegment);
            request.AddParameter("SelecterType", SelecterType, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

            return response.Data;
        }
        public IEnumerable<LCompanySpecificColumnViewModel> GetColumnsForProductHistory(string CompanyCode, string SelecterType)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCompanySpecificColumns/GetColumnsForProductHistory?CompanyCode={CompanyCode}&SelecterType={SelecterType}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("SelecterType", SelecterType, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<LCompanySpecificColumnViewModel> GetColumnsForProductReport(string CompanyCode, string SelecterType1, string SelecterType2)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/GetColumnsForProductReprt?CompanyCode={CompanyCode}&SelecterType1={SelecterType1}&SelecterType2={SelecterType2}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("SelecterType1", SelecterType1, ParameterType.UrlSegment);
            request.AddParameter("SelecterType2", SelecterType2, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);
            return response.Data;
        }
        public IEnumerable<LCompanySpecificColumnViewModel> GetLCompanySpecificColumnsByCompanyCode(string CompanyCode, string TableName, string SelecterType)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCompanySpecificColumns/GetLCompanySpecificColumnsByCompanyCode?CompanyCode={CompanyCode}&TableName={TableName}&SelecterType={SelecterType}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("TableName", TableName, ParameterType.UrlSegment);
            request.AddParameter("SelecterType", SelecterType, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<CompanYSpecificColumnForGridViewModel> GetAttributesForGrid(string CompanyCode, string TableName, string SelecterType)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCompanySpecificColumns/GetAttributesForGrid?CompanyCode={CompanyCode}&TableName={TableName}&SelecterType={SelecterType}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("TableName", TableName, ParameterType.UrlSegment);
            request.AddParameter("SelecterType", SelecterType, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<CompanYSpecificColumnForGridViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public LCompanySpecificColumnViewModel GetColumnDetails(string CompanyCode, string TableName, string SelecterType, string ColumnName)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCompanySpecificColumns/GetColumnDetails?CompanyCode={CompanyCode}&TableName={TableName}&SelecterType={SelecterType}&ColumnName={ColumnName}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("TableName", TableName, ParameterType.UrlSegment);
            request.AddParameter("SelecterType", SelecterType, ParameterType.UrlSegment);
            request.AddParameter("ColumnName", ColumnName, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LCompanySpecificColumnViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        //SG Commented on - 09/07/2018 - not being usd any where
        //public IEnumerable<LCompanySpecificColumnViewModel> GetLocalPobsLCompanySpecificColumnsByCompanyCode(string CompanyCode)
        //{
        //    string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //    string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        //    var request = new RestRequest("api/LCompanySpecificColumns/GetLocalPobsLCompanySpecificColumnsByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}
        //
        public void Add(string GridData, string TableName, string SelectorType, string CompanyCode)
        {
            var ModelData = new PostLCompanySpecificFormViewModel { GridData = GridData, CompanyCode = CompanyCode, TableName = TableName, SelecterType = SelectorType };
            var request = new RestRequest("api/LCompanySpecificColumns/PostLCompanySpecificColumns", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(ModelData);
            //request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            //request.AddParameter("GridData", GridData, ParameterType.UrlSegment);
            //request.AddParameter("TableName", TableName, ParameterType.UrlSegment);
            //request.AddParameter("SelecterType", SelectorType, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, null);
            }
        }

        public string GetColumnNameByLabel(string CompanyCode, string TableName, int SysCatId, string Label)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/GetColumnNameByLabel?CompanyCode={CompanyCode}&TableName={TableName}&SysCatId={SysCatId}&Label={Label}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("TableName", TableName, ParameterType.UrlSegment);
            request.AddParameter("SysCatId", SysCatId, ParameterType.UrlSegment);
            request.AddParameter("Label", Label, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            dynamic data = JsonConvert.DeserializeObject(response.Content);
            return data;
        }

        public IEnumerable<LCompanySpecificColumnViewModel> GetLRequestColumnsByCompanyCodeForGrid(string CompanyCode)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/GetLRequestLCompanySpecificColumnsByCompanyCodeForGrid?CompanyId={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
    }



interface ILCompanySpecificColumnsRestClient
{
    string GetColumnNameByLabel(string CompanyCode, string TableName, int SysCatId, string Label);
    IEnumerable<LCompanySpecificColumnViewModel> GetLCompanySpecificColumnsByTableName(string TableName, string SelecterType);
    IEnumerable<LCompanySpecificColumnViewModel> GetLCompanySpecificColumnsByCompanyCode(string CompanyCode, string TableName, string SelecterType);
    //IEnumerable<LCompanySpecificColumnViewModel> GetLocalPobsLCompanySpecificColumnsByCompanyCode(string CompanyCode);
    void Add(string GridData, string TableName, string SelectorType, string CompanyCode);
    List<string> GetSelecterTypeByTableName(string TableName, string CompanyCode);
    IEnumerable<LCompanySpecificColumnViewModel> GetColumnsForProductReport(string CompanyCode, string SelecterType1, string SelecterType2);
    IEnumerable<LCompanySpecificColumnViewModel> GetColumnsForProductHistory(string CompanyCode, string SelecterType);
    LCompanySpecificColumnViewModel GetColumnDetails(string CompanyCode, string TableName, string SelecterType, string ColumnName);
    IEnumerable<GetTableNamesForFormConfigViewModel> GetTableNamesByCompanyCode(string CompanyCode);
    IEnumerable<CompanYSpecificColumnForGridViewModel> GetAttributesForGrid(string CompanyCode, string TableName, string SelecterType);
    IEnumerable<LCompanySpecificColumnViewModel> GetLRequestColumnsByCompanyCodeForGrid(string CompanyId);
    }

}
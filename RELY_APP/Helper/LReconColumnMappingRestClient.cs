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
    public class LReconColumnMappingRestClient : ILReconColumnMappingRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LReconColumnMappingRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LReconColumnMappingViewModel> GetLReconColumnMappingBySysCat(string CompanyCode,int FileFormatId, int SysCatId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LReconColumnMapping/GetLReconColumnMappingBySysCat?CompanyCode={CompanyCode}&FileFormatId={FileFormatId}&SysCatId={SysCatId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("FileFormatId", FileFormatId, ParameterType.UrlSegment);
            request.AddParameter("SysCatId", SysCatId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LReconColumnMappingViewModel>>(request);
            return response.Data;
        }

        public List<LReconColumnMappingViewModel> GetFileFormatBySysCat(int SysCatId, string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LReconColumnMapping/GetFileFormatBySysCat?SysCatId={SysCatId}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SysCatId", SysCatId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LReconColumnMappingViewModel>>(request);
            return response.Data;
        }

        public void Add(string GridData, int FileFormatId, int SysCatId, string CompanyCode)
        {
            var ModelData = new PostLReconColumnMappingViewModel { GridData = GridData, CompanyCode = CompanyCode, FileFormatId=FileFormatId, SysCatId = SysCatId };
            var request = new RestRequest("api/LReconColumnMapping/POSTLReconColumnMapping", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(ModelData);
            //request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            //request.AddParameter("GridData", GridData, ParameterType.UrlSegment);
            //request.AddParameter("TableName", TableName, ParameterType.UrlSegment);
            //request.AddParameter("SelecterType", SelectorType, ParameterType.UrlSegment);
            var response = _client.Execute<List<LReconColumnMappingViewModel>>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, null);
            }
        }



        public IEnumerable<LReconColumnMappingViewModel> GetLReconColumnsByFormatId(string CompanyCode, int FileFormatId)
        {
           // string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LReconColumnMapping/GetLReconColumnsByFormatId?CompanyCode={CompanyCode}&FileFormatId={FileFormatId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("FileFormatId", FileFormatId, ParameterType.UrlSegment);
            //request.AddParameter("SysCatId", SysCatId, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LReconColumnMappingViewModel>>(request);
            return response.Data;
        }
        public int CheckExistenceOfProductCode(string CompanyCode, string ProductCode)
        {
            var request = new RestRequest("api/LProducts/CheckExistenceOfProductCode?ProductCode={ProductCode}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductCode", ProductCode, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            return response.Data;

        }
    }






    interface ILReconColumnMappingRestClient
    {
        IEnumerable<LReconColumnMappingViewModel> GetLReconColumnMappingBySysCat(string CompanyCode, int FileFormatId, int SysCatId);
        List<LReconColumnMappingViewModel> GetFileFormatBySysCat(int SysCatId, string CompanyCode);
        void Add(string GridData, int FileFormatId, int SysCatId, string CompanyCode);
        IEnumerable<LReconColumnMappingViewModel> GetLReconColumnsByFormatId(string CompanyCode, int FileFormatId);
        int CheckExistenceOfProductCode(string CompanyCode, string ProductCode);
    }
}
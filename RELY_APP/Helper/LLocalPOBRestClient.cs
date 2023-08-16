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
    class LLocalPOBRestClient : ILLocalPOBRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        public LLocalPOBRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public void DeleteMappingRow(int RowId, string Type)
        {
            var request = new RestRequest("api/LLocalPOB/DeleteMappingRow?RowId={RowId}&Type={Type}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RowId", RowId, ParameterType.UrlSegment);
            request.AddParameter("Type", Type, ParameterType.UrlSegment);
            var response = _client.Execute(request);
        }
        public IEnumerable<LLocalPOBViewModel> GetByCompanyCode(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LLocalPOB/GetByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName,ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LLocalPOBViewModel>>(request);

            return response.Data;
        }
        public int GetCompletedListCount(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LLocalPOB/GetCompletedListCount?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            return response.Data;
        }

        public IEnumerable<dynamic> GetCompletedList(string CompanyCode,string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery,string UnderlyingProductId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LLocalPOB/GetCompletedList?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&PageNumber={PageNumber}&PageSize={PageSize}&UnderlyingProductId={UnderlyingProductId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("UnderlyingProductId", string.IsNullOrEmpty(UnderlyingProductId) ? "" : UnderlyingProductId, ParameterType.UrlSegment);

            var response = _client.Execute<List<dynamic>>(request);

            return response.Data;
        }

        public IEnumerable<LLocalPOBViewModel> GetVersions(string Name,string CompanyCode,int TypeId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LLocalPOB/GetLLocalPOBVersions?Name={Name}&CompanyCode={CompanyCode}&TypeId={TypeId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Name", Name, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("TypeId", TypeId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LLocalPOBViewModel>>(request);
            
            return response.Data;
        }

        //public LLocalPOBViewModel GetByIdWithCurrentSSP(int LocalPobId)
        //{
        //    var request = new RestRequest("api/LLocalPOB/GetLLocalPOBWithCurrentSSP?id={LocalPobId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("LocalPobId", LocalPobId, ParameterType.UrlSegment);
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
        //    var response = _client.Execute<LLocalPOBViewModel>(request);
        //    return response.Data;
        //}
        public DateTime? GetPreviousVersionStartDate(int Id)
        {
            var request = new RestRequest("api/LLocalPOB/GetPreviousVersionStartDate?Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<DateTime>(request);
            return response.Data;
        }

        public LLocalPOBViewModel GetById(int LocalPobId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LLocalPOB/GetLLocalPOB?id={LocalPobId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("LocalPobId", LocalPobId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LLocalPOBViewModel>(request);
            return response.Data;
        }
        public int GetMaxPobCatelogueId(string CompanyCode)
        {
            var request = new RestRequest("api/LLocalPOB/GetMaxPobCatelogueId?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        public LocalPobForProductViewModel GetLLocalPOBForProduct(int PobCatalogueId,int ProductId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LLocalPOB/GetLLocalPOBForProduct?PobCatalogueId={PobCatalogueId}&ProductId={ProductId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PobCatalogueId", PobCatalogueId, ParameterType.UrlSegment);
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LocalPobForProductViewModel>(request);
            return response.Data;
        }


        public int Add(LLocalPOBViewModel model,string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList,int? ProductId,string PobStDt,string PobEnDt)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LLocalPOB/PostLLocalPOB?UserName={UserName}&WorkFlow={WorkFlow}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}&ProductId={ProductId}&PobStDt={PobStDt}&PobEnDt={PobEnDt}", Method.POST) { RequestFormat = DataFormat.Json };
            //as ProductId is nullable int, cannot send null for REST request. Therefore, marking it to -9999 in case of null.
            request.AddParameter("ProductId", ProductId == null ? -9999 : ProductId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("FileList", FileList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("OriginalFileNameList", OriginalFileNameList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocumentsDescription", SupportingDocumentsDescription, ParameterType.UrlSegment);
            request.AddParameter("PobStDt", PobStDt, ParameterType.UrlSegment);
            request.AddParameter("PobEnDt", PobEnDt, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }

        public void Update(LLocalPOBViewModel model,int Id, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList , string ActionName,int? ProductId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LLocalPOB/PutLLocalPOB?id={Id}&UserName={UserName}&WorkFlow={WorkFlow}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}&ActionName={ActionName}&ProductId={ProductId}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddBody(model);
             request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("FileList", FileList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("OriginalFileNameList", OriginalFileNameList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocumentsDescription", SupportingDocumentsDescription, ParameterType.UrlSegment);
            request.AddParameter("ActionName", ActionName, ParameterType.UrlSegment);
            //as ProductId is nullable int, cannot send null for REST request. Therefore, marking it to -9999 in case of null.
            request.AddParameter("ProductId", ProductId == null ? -9999 : ProductId, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        //public IEnumerable<LocalPobForProductViewModel> GetAllLPobsDetails(string CompanyCode)
        //{
        //    string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //    string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
        //    var request = new RestRequest("api/LLocalPOB/GetAllLPobsByCompanyCode?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LocalPobForProductViewModel>>(request);
        //    return response.Data;
        //}

        public int GetAllLocalPOBCounts(string CompanyCode)
        {
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LLocalPOB/GetAllLocalPOBCountsByCompanyCode?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        public LLocalPOBViewModel CloneLPob(int LPobId, int loggedInUserId, int LoggedInUserRoleId, string CompanyCode, string Source)
        {
            var request = new RestRequest("api/LLocalPOB/CloneLPOB?LPobId={LPobId}&loggedInUserId={loggedInUserId}&LoggedInUserRoleId={LoggedInUserRoleId}&CompanyCode={CompanyCode}&Source={Source}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("LPobId", LPobId, ParameterType.UrlSegment);
            request.AddParameter("loggedInUserId", loggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserRoleId", LoggedInUserRoleId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Source", Source, ParameterType.UrlSegment);
            var response = _client.Execute<LLocalPOBViewModel>(request);
            return response.Data;
        }

        public IEnumerable<dynamic> GetAllLPobsDetails(string CompanyCode, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LLocalPOB/GetAllLPobsByCompanyCode?CompanyCode={CompanyCode}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&PageNumber={PageNumber}&PageSize={PageSize}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);

            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }
        public DateTime AttachGpob(int PobCatalogueId, string GpobMappingStartDate, int GpobId,string CompanyCode,string GpobType)
        {
            var request = new RestRequest("api/LLocalPOB/AttachGpob?PobCatalogueId={PobCatalogueId}&GpobMappingStartDate={GpobMappingStartDate}&GpobId={GpobId}&CompanyCode={CompanyCode}&GpobType={GpobType}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PobCatalogueId", PobCatalogueId, ParameterType.UrlSegment);
            request.AddParameter("GpobMappingStartDate", GpobMappingStartDate, ParameterType.UrlSegment);
            request.AddParameter("GpobId", GpobId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("GpobType", GpobType, ParameterType.UrlSegment);
            var response = _client.Execute<DateTime>(request);
            return response.Data;
        }
        public List<dynamic> GetGPOBDataForGrid(int PobCatalogueId,string CompanyCode,string GpobType)
        {
            var request = new RestRequest("api/LLocalPOB/GetGPOBDataForGrid?PobCatalogueId={PobCatalogueId}&CompanyCode={CompanyCode}&GpobType={GpobType}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PobCatalogueId", PobCatalogueId, ParameterType.UrlSegment);
            request.AddParameter("GpobType", GpobType, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }
        public DateTime AttachCopa(int PobCatalogueId, string CopaMappingStartDate, int CopaId, string CompanyCode, int CopaClass)
        {
            var request = new RestRequest("api/LLocalPOB/AttachCopa?PobCatalogueId={PobCatalogueId}&CopaMappingStartDate={CopaMappingStartDate}&CopaId={CopaId}&CompanyCode={CompanyCode}&CopaClass={CopaClass}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PobCatalogueId", PobCatalogueId, ParameterType.UrlSegment);
            request.AddParameter("CopaMappingStartDate", CopaMappingStartDate, ParameterType.UrlSegment);
            request.AddParameter("CopaId", CopaId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("CopaClass", CopaClass, ParameterType.UrlSegment);
            var response = _client.Execute<DateTime>(request);
            return response.Data;
        }
        public List<dynamic> GetCopaDataForGrid(int PobCatalogueId, string CompanyCode, int CopaClass)
        {
            var request = new RestRequest("api/LLocalPOB/GetCopaDataForGrid?PobCatalogueId={PobCatalogueId}&CompanyCode={CompanyCode}&CopaClass={CopaClass}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PobCatalogueId", PobCatalogueId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("CopaClass", CopaClass, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }
        public DateTime GetLatestMappedGpobStartDate(int PobCatalogueId, string GpobType)
        {
            var request = new RestRequest("api/LLocalPOB/GetLatestMappedGpobStartDate?PobCatalogueId={PobCatalogueId}&GpobType={GpobType}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PobCatalogueId", PobCatalogueId, ParameterType.UrlSegment);
            request.AddParameter("GpobType", GpobType, ParameterType.UrlSegment);
            var response = _client.Execute<DateTime>(request);
            return response.Data;
        }
        public DateTime GetLatestMappedCopaStartDate(int PobCatalogueId, int CopaClass)
        {
            var request = new RestRequest("api/LLocalPOB/GetLatestMappedCopaStartDate?PobCatalogueId={PobCatalogueId}&CopaClass={CopaClass}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PobCatalogueId", PobCatalogueId, ParameterType.UrlSegment);
            request.AddParameter("CopaClass", CopaClass, ParameterType.UrlSegment);
            var response = _client.Execute<DateTime>(request);
            return response.Data;
        }
    }

    interface ILLocalPOBRestClient
    {
        DateTime GetLatestMappedGpobStartDate(int PobCatalogueId, string GpobType);
        DateTime GetLatestMappedCopaStartDate(int PobCatalogueId, int CopaClass);
        List<dynamic> GetGPOBDataForGrid(int PobCatalogueId, string CompanyCode, string GpobType);
        List<dynamic> GetCopaDataForGrid(int PobCatalogueId, string CompanyCode,int CopaClass);
        DateTime AttachGpob(int PobCatalogueId, string GpobMappingStartDate, int GpobId, string CompanyCode, string GpobType);
        DateTime AttachCopa(int PobCatalogueId, string CopaMappingStartDate, int GpobId, string CompanyCode, int CopaClass);
        int GetMaxPobCatelogueId(string CompanyCode);
        IEnumerable<LLocalPOBViewModel> GetByCompanyCode(string CompanyCode);
        int Add(LLocalPOBViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList, int? ProductId, string PobStDt, string PobEnDt);
        LLocalPOBViewModel GetById(int LocalPobId);
        // LLocalPOBViewModel GetByIdWithCurrentSSP(int LocalPobId);
        void Update(LLocalPOBViewModel model, int Id, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList, string ActionName,int? ProductId);
        IEnumerable<LLocalPOBViewModel> GetVersions(string Name, string CompanyCode, int TypeId);
        LocalPobForProductViewModel GetLLocalPOBForProduct(int PobCatalogueId,int ProductId);
        int GetCompletedListCount(string CompanyCode);
        IEnumerable<dynamic> GetCompletedList(string CompanyCode, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery,string UnderlyingProductId);
      //  IEnumerable<LocalPobForProductViewModel> GetAllLPobsDetails(string CompanyCode);
        int GetAllLocalPOBCounts(string CompanyCode);
        IEnumerable<dynamic> GetAllLPobsDetails(string CompanyCode, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery);
        LLocalPOBViewModel CloneLPob(int LPobId, int loggedInUserId, int LoggedInUserRoleId, string CompanyCode, string Source);
        DateTime? GetPreviousVersionStartDate(int Id);
        void DeleteMappingRow(int RowId, string Type);
    }
}
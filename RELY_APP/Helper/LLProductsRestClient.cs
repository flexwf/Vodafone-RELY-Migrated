using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;
using System.Net;
using RELY_APP.Utilities;
using System.Data;

namespace RELY_APP.Helper
{
    class LProductsRestClient : ILProductsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;

        public LProductsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public LProductViewModel GetById(int ProductId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/GetLProduct?id={ProductId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LProductViewModel>(request);
            return response.Data;
        }

        public IEnumerable<LProductViewModel> GetByCompanyCode(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/GetByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LProductViewModel>>(request);
            return response.Data;
        }
        public ProductForRequestDetailViewModel GetProductCountAttachedToRequest(int RequestId, string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/GetProductCountAttachedToRequest?RequestId={RequestId}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RequestId", RequestId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<ProductForRequestDetailViewModel>(request);
            return response.Data;
        }

        public int GetProductsListForRequestCount(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/GetProductsListForRequestCount?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        public IEnumerable<LProductViewModel> GetProductsListForRequest(string CompanyCode, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/GetProductsListForRequest?CompanyCode={CompanyCode}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&PageNumber={PageNumber}&PageSize={PageSize}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<LProductViewModel>>(request);
            return response.Data;
        }
        public int GetByCompanyCodeForChangeSurveyCount(string CompanyCode, int CurrentProductId)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/GetByCompanyCodeForChangeSurveyCount?CompanyCode={CompanyCode}&CurrentProductId={CurrentProductId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("CurrentProductId", CurrentProductId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        public DateTime? GetPreviousVersionStartDate(int ProductId)
        {
            var request = new RestRequest("api/LProducts/GetPreviousVersionStartDate?ProductId={ProductId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);
            var response = _client.Execute<DateTime>(request);
            return response.Data;
        }
        public IEnumerable<LProductViewModel> GetByCompanyCodeForChangeSurvey(string CompanyCode,int CurrentProductId, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/GetByCompanyCodeForChangeSurvey?CompanyCode={CompanyCode}&CurrentProductId={CurrentProductId}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&PageNumber={PageNumber}&PageSize={PageSize}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("CurrentProductId", CurrentProductId, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<LProductViewModel>>(request);
            return response.Data;
        }
        public LocalPobForProductViewModel GetLatestPOBByProductId(string CompanyCode,int ProductId)
        {
            var request = new RestRequest("api/LLocalPOB/GetLatestPOBCreatedOnFlyForProduct?CompanyCode={CompanyCode}&ProductId={ProductId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);
            var response = _client.Execute<LocalPobForProductViewModel>(request);
            return response.Data;
        }

        public LProductViewModel GetLatestProductByRequestId(int RequestId)
        {
            var request = new RestRequest("api/LProducts/GetLatestProcuctCreatedOnFlyForRequest?RequestId={RequestId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RequestId", RequestId, ParameterType.UrlSegment);
            var response = _client.Execute<LProductViewModel>(request);
            return response.Data;
        }
       

        public LProductViewModel CloneProduct(int ProductId, int loggedInUserId,int LoggedInUserRoleId, string CompanyCode , string Source,int RequestId)
        {
            var request = new RestRequest("api/LProducts/CloneProduct?ProductId={ProductId}&loggedInUserId={loggedInUserId}&LoggedInUserRoleId={LoggedInUserRoleId}&CompanyCode={CompanyCode}&Source={Source}&RequestId={RequestId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);
            request.AddParameter("loggedInUserId", loggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserRoleId", LoggedInUserRoleId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Source", Source, ParameterType.UrlSegment);
            request.AddParameter("RequestId", RequestId, ParameterType.UrlSegment);
            var response = _client.Execute<LProductViewModel>(request);
            return response.Data;
        }
        public LProductViewModel UpdateSurvey(int EntityId, int SurveyId , bool IsCopySurvey, int? SourceProductId,int LoggedInUserId,string CompanyCode, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/UpdateSurvey?EntityId={EntityId}&SurveyId={SurveyId}&IsCopySurvey={IsCopySurvey}&SourceProductId={SourceProductId}&LoggedInUserId={LoggedInUserId}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("SurveyId", SurveyId, ParameterType.UrlSegment);
            request.AddParameter("IsCopySurvey", IsCopySurvey, ParameterType.UrlSegment);
            request.AddParameter("SourceProductId", SourceProductId == null ? 0 : SourceProductId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LProductViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

            return response.Data;
        }

        public LProductViewModel UpdateSurveyNew(int EntityId, int SurveyId, bool IsCopySurvey, int? SourceProductId, int LoggedInUserId, string CompanyCode, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/UpdateSurveyNew?TargetEntityId={TargetEntityId}&SurveyId={SurveyId}&IsCopySurvey={IsCopySurvey}&SourceProductId={SourceProductId}&LoggedInUserId={LoggedInUserId}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("TargetEntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("SurveyId", SurveyId, ParameterType.UrlSegment);
            request.AddParameter("IsCopySurvey", IsCopySurvey, ParameterType.UrlSegment);
            request.AddParameter("SourceProductId", SourceProductId == null ? 0 : SourceProductId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LProductViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

            return response.Data;
        }

        public IEnumerable<LProductViewModel> GetByRequestId(int EntityId, string EntityType)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/GetByRequestId?EntityId={EntityId}&EntityType={EntityType}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LProductViewModel>>(request);
            return response.Data;
        }

        //Add by Ankit
        public IEnumerable<LProductRequestHistoryViewModel> GetProductRequestHistory(int ProductId)
        {
            var request = new RestRequest("api/LProducts/GetProductRequestHistory?ProductId={ProductId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);
            // request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LProductRequestHistoryViewModel>>(request);
            return response.Data;
        }

        public List<dynamic> GetProductHistory(string SelecterType, string CompanyCode, int ProductId)
        {
            var request = new RestRequest("api/LProducts/GetProductHistory?SelecterType={SelecterType}&CompanyCode={CompanyCode}&ProductId={ProductId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductId", ProductId, ParameterType.UrlSegment);
            request.AddParameter("SelecterType", SelecterType, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);

            var response = _client.Execute<List<dynamic>>(request);
            //send data
            return response.Data;
           // var response = _client.Execute<DataTable>(request);
            //DataTable dt = (DataTable)JsonConvert.DeserializeObject(response.Content, (typeof(DataTable)));
            //return dt;
        }


        public int Add(LProductViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/PostLProduct?UserName={UserName}&WorkFlow={WorkFlow}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}", Method.POST) { RequestFormat = DataFormat.Json };
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
        public List<string> GetProductCodesByOpcoSysCat(string CompanyCode,int SysCatId,int? Id)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/GetProductCodesByOpcoSysCat?CompanyCode={CompanyCode}&SysCatId={SysCatId}&Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("SysCatId", SysCatId, ParameterType.UrlSegment);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);

            return response.Data;
        }
        public string CheckProductDuplicacy(string CompanyCode, int SysCatId, string Id, string ProductCode, string BusinessCategory)
        {
            var request = new RestRequest("api/LProducts/CheckProductDuplicacy?CompanyCode={CompanyCode}&SysCatId={SysCatId}&Id={Id}&ProductCode={ProductCode}&BusinessCategory={BusinessCategory}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("SysCatId", SysCatId, ParameterType.UrlSegment);
            request.AddParameter("ProductCode", ProductCode, ParameterType.UrlSegment);
            request.AddParameter("BusinessCategory", BusinessCategory, ParameterType.UrlSegment);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            dynamic data = JsonConvert.DeserializeObject(response.Content);
            return data;
        }
        public void Update(LProductViewModel model, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/PutLProduct?id={id}&UserName={UserName}&WorkFlow={WorkFlow}&FileList={FileList}&SupportingDocumentsDescription={SupportingDocumentsDescription}&FilePath={FilePath}&OriginalFileNameList={OriginalFileNameList}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("FileList", FileList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("OriginalFileNameList", OriginalFileNameList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocumentsDescription", SupportingDocumentsDescription, ParameterType.UrlSegment);
            request.AddBody(model);
            request.AddParameter("id", model.Id, ParameterType.UrlSegment);
            var response = _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            
        }
        public void UpdateForRequest(List<LProductViewModel> model, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/UpdateForRequest?UserName={UserName}&WorkFlow={WorkFlow}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(model);
            
            var response = _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
           
        }
        public void DetachProductFromRequest(int Id, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/DetachProductFromRequest?id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }
        public int GetAllProductsCounts(string CompanyCode)
        {
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/GetAllProductsCountsByCompanyCode?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        public IEnumerable<dynamic> GetAllProductsDetails(string CompanyCode, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LProducts/GetAllProductsByCompanyCode?CompanyCode={CompanyCode}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&PageNumber={PageNumber}&PageSize={PageSize}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);

            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }

        public string GetProductHistoryDetails(string CompanyCode, int Id)
        {
            var request = new RestRequest("api/LProducts/GetProductHistory?CompanyCode={CompanyCode}&Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);
            return response.Content;
        }
    }

    interface ILProductsRestClient
    {
        string CheckProductDuplicacy(string CompanyCode, int SysCatId, string Id, string ProductCode, string BusinessCategory);
        LProductViewModel GetById(int ProductId);
        IEnumerable<LProductViewModel> GetByCompanyCode(string CompanyCode);
        ProductForRequestDetailViewModel GetProductCountAttachedToRequest(int RequestId, string CompanyCode);
        int GetByCompanyCodeForChangeSurveyCount(string CompanyCode, int CurrentProductId);
        IEnumerable<LProductViewModel> GetByCompanyCodeForChangeSurvey(string CompanyCode, int CurrentProductId, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery);
        IEnumerable<LProductViewModel> GetByRequestId(int EntityId,string EntityType);
        IEnumerable<LProductRequestHistoryViewModel> GetProductRequestHistory(int ProductId);
        int Add(LProductViewModel Product, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList);
        void Update(LProductViewModel Product, string RedirectToUrl, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList);
        void UpdateForRequest(List<LProductViewModel> model, string RedirectToUrl);
        void DetachProductFromRequest(int Id, string RedirectToUrl);
        LProductViewModel GetLatestProductByRequestId(int RequestId);
        LocalPobForProductViewModel GetLatestPOBByProductId(string CompanyCode,int ProductId);
        LProductViewModel CloneProduct(int ProductId, int loggedInUserId, int LoggedInUserRoleId, string CompanyCode, string Source, int RequestId);
        List<string> GetProductCodesByOpcoSysCat(string CompanyCode, int SysCatId,int? Id);
        //dynamic GetProductHistory(string SelecterType, string CompanyCode, int ProductId);
        LProductViewModel UpdateSurvey(int EntityId, int SurveyId, bool IsCopySurvey, int? SourceProductId,int LoggedInUserId, string CompanyCode,string UpdateSurvey);
        LProductViewModel UpdateSurveyNew(int EntityId, int SurveyId, bool IsCopySurvey, int? SourceProductId, int LoggedInUserId, string CompanyCode, string RedirectToUrl);
        List<dynamic> GetProductHistory(string SelecterType, string CompanyCode, int ProductId);
        int GetAllProductsCounts(string CompanyCode);
        IEnumerable<dynamic> GetAllProductsDetails(string CompanyCode, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery);
        IEnumerable<LProductViewModel> GetProductsListForRequest(string CompanyCode, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery);
        int GetProductsListForRequestCount(string CompanyCode);

        string GetProductHistoryDetails(string CompanyCode, int Id);
        DateTime? GetPreviousVersionStartDate(int ProductId);
    }
}
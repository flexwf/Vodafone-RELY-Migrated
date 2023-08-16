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
    public class LReconBucketRestclient : ILReconBucketRestclient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public LReconBucketRestclient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        /// <summary>
        /// Created by Rakhi Singh
        /// Method to get columns for ReconGrid which is use to call API method
        /// </summary>
        /// <param name="ProductCode"></param>
        /// <param name="SysCatId"></param>
        /// <param name="CompanyCode"></param>
        /// <returns>List of columns</returns>
        public List<string> GetColumnsRecon(string ProductCode, int SysCatId, string CompanyCode)
        {
            var request = new RestRequest("api/LReconBucket/GetDataForReconColumns?ProductCode={ProductCode}&SysCatId={SysCatId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductCode", ProductCode, ParameterType.UrlSegment);
            request.AddParameter("SysCatId", SysCatId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);
            return response.Data;
        }
        /// <summary>
        /// Created by Rakhi Singh
        /// Method to get data for the Grid into the columns received from above method
        /// </summary>
        /// <param name="ProductCode"></param>
        /// <param name="SysCatId"></param>
        /// <param name="CompanyCode"></param>
        /// <returns></returns>
        public List<dynamic> GetDataForRecon(string ProductCode, int SysCatId, string CompanyCode)
        {
            var request = new RestRequest("api/LReconBucket/GetDataForRecon?ProductCode={ProductCode}&SysCatId={SysCatId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ProductCode", ProductCode, ParameterType.UrlSegment);
            request.AddParameter("SysCatId", SysCatId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
            //GetDataForRecon
        }
        public List<dynamic> GetMissingProducts(string CompanyCode)
        {
            var request = new RestRequest("api/LReconBucket/GetMissingProducts?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }

        public int Add(string CompanyCode,int SysCatId,int FileFormatId, int LoggedInUserId,string filename,string RedirectToUrl)
        {
            var request = new RestRequest("api/LReconBucket/PostLReconBucket?CompanyCode={CompanyCode}&SysCatId={SysCatId}&FileFormatId={FileFormatId}&LoggedInUserId={LoggedInUserId}&filename={filename}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("filename", filename, ParameterType.UrlSegment);
            request.AddParameter("SysCatId", SysCatId, ParameterType.UrlSegment);
            request.AddParameter("FileFormatId", FileFormatId, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }

        public int ReadAndValidateCSVData(int LoggedInUserId, string filename, string CompanyCode, int FileFormatId, int SysCatId, string RedirectToUrl)
        {
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            var request = new RestRequest("api/LReconBucket/ReadAndValidateCSVData?UserName={UserName}&WorkFlow={WorkFlow}&LoggedInUserId={LoggedInUserId}&filename={filename}&FileFormatId={FileFormatId}&CompanyCode={CompanyCode}&SysCatId={SysCatId}", Method.GET);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("filename", filename, ParameterType.UrlSegment);
            request.AddParameter("FileFormatId", FileFormatId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            
            request.AddParameter("SysCatId", SysCatId, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }

        public List<dynamic> GetInvalidRecords(int SysCatId, int FileFormatId, string CompanyCode,string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/LReconBucket/GetInValidDataRecords?SysCatId={SysCatId}&FileFormatId={FileFormatId}&CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&PageNumber={PageNumber}&PageSize={PageSize}", Method.GET);
            request.AddParameter("SysCatId", SysCatId, ParameterType.UrlSegment);
            request.AddParameter("FileFormatId", FileFormatId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            
            
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }




    }

    interface ILReconBucketRestclient
    {
        List<dynamic> GetDataForRecon(string ProductCode, Int32 SysCatId, string CompanyCode);
        List<string> GetColumnsRecon(string ProductCode, int SysCatId, string CompanyCode);
        List<dynamic> GetMissingProducts(string CompanyCode);
        int Add(string CompanyCode, int SysCatId, int FileFormatId, int LoggedInUserId, string DataUploadFileName, string RedirectToUrl);
        int ReadAndValidateCSVData(int LoggedInUserId, string filename, string CompanyCode, int FileFormatId, int SysCatId, string RedirectToUrl);
        List<dynamic> GetInvalidRecords(int SysCatId, int FileFormatId, string CompanyCode,string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery);
    }

}
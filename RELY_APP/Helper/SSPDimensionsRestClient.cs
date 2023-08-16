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
    public class SSPDimensionsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public SSPDimensionsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public SSPDimensionViewModel GetById(int Id)
        {
            var request = new RestRequest("api/SSPDimensions/GetById?Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<SSPDimensionViewModel>(request);
            return response.Data;
        }

        public IEnumerable<SSPDimensionViewModel> GetBySSPId(int SspId, string CompanyCode)
        {
            var request = new RestRequest("api/SSPDimensions/GetBySspId?SspId={SspId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SspId", SspId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<SSPDimensionViewModel>>(request);
            return response.Data;
        }

        //GetSSPIdForNew
        public int GetSSPIdForNew(string CompanyCode)
        {
            var request = new RestRequest("api/SSPDimensions/GetSSPIdForNew?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        public void Add(SSPDimensionViewModel serverData, string Source, string EntityId, string RedirectToUrl)
        {
            var request = new RestRequest("api/SSPDimensions/Post?Source={Source}&EntityId={EntityId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("Source", Source, ParameterType.UrlSegment);
            request.AddParameter("EntityId", string.IsNullOrEmpty(EntityId) ? "" : EntityId, ParameterType.UrlSegment);
            request.AddBody(serverData);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
        public void Update(SSPDimensionViewModel serverData, string RedirectToUrl)
        {
            var request = new RestRequest("api/SSPDimensions/Put?Id={Id}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);
            var response = _client.Execute<SSPDimensionViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
        public void Delete(int id, string RedirectToUrl)
        {

            var request = new RestRequest("api/SSPDimensions/Delete?id={id}", Method.DELETE);

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }
        public List<dynamic> GetDataForGrid(string EntityType, int EntityId,string CompanyCode)
        {
            var request = new RestRequest("api/SSPDimensions/GetDataForGrid?EntityType={EntityType}&EntityId={EntityId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }
        public int GetExistingSspsCount(string CompanyCode)
        {
            var request = new RestRequest("api/SSPDimensions/GetExistingSspsCount?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        public List<dynamic> GetExistingSsps(string CompanyCode, string sortdatafield, string sortorder, int PageSize, int PageNumber, string FilterQuery)
        {
            var request = new RestRequest("api/SSPDimensions/GetExistingSsps?CompanyCode={CompanyCode}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&PageNumber={PageNumber}&PageSize={PageSize}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }
        public void DetachSSP(int EntityId,string EntityType, string RedirectToUrl)
        {
            var request = new RestRequest("api/SSPDimensions/DetachSSP?EntityId={EntityId}&EntityType={EntityType}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
        public void AttachSSP(int EntityId, string EntityType, int SspId, string RedirectToUrl)
        {
            var request = new RestRequest("api/SSPDimensions/AttachSSP?EntityId={EntityId}&SspId={SspId}&EntityType={EntityType}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("SspId", SspId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
    }
}
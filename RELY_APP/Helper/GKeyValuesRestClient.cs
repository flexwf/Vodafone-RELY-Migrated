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
    public class GKeyValuesRestClient: IGKeyValuesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public GKeyValuesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        } 

        //Getting Value of a key from GKeyValues Table
        public GKeyValueViewModel GetKeyValue(string Key, string CompanyCode)
        {
            var request = new RestRequest("api/GKeyValues/GetKeyValue?Key={Key}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Key", Key, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);

            // var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);


            var response = _client.Execute<GKeyValueViewModel>(request);

            return response.Data;
        }

       
        public void Add(GKeyValueViewModel serverData, string RedirectToUrl)
        {
            var request = new RestRequest("api/GKeyValues/PostGKeyValue", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<GKeyValueViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        public void Update(GKeyValueViewModel serverData, string RedirectToUrl)
        {
            var request = new RestRequest("api/GKeyValues/PutGKeyValue/{Id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);
            var response = _client.Execute<GKeyValueViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }

        //method used to delete the selected config for deletion 
        public void Delete(int id, string RedirectToUrl)
        {
            var request = new RestRequest("api/GKeyValues/DeleteGKeyVAlue/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        public GKeyValueViewModel GetById(int id)
        {
            var request = new RestRequest("api/GKeyValues/GetGKeyValue/{Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", id, ParameterType.UrlSegment);
            var response = _client.Execute<GKeyValueViewModel>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        /// <summary>
        /// Created by Rakhi Singh
        /// Method to get the counts for Key Values from the GKeyValues for L2Admin Page
        /// </summary>
        /// <returns></returns>
        public int CountsForGKeyValueForConfiguration()
        {
            var request = new RestRequest("api/GKeyValues/GetGKeyValueCountForConfiguration", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        /// <summary>
        /// Created by Rakhi Singh 
        /// Method to get the Key Values from the GKeyValues for L2Admin Page
        /// </summary>       
        public IEnumerable<GKeyValueViewModel> GetGKeyValueForConfiguration(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            var request = new RestRequest("api/GKeyValues/GetGKeyValueForConfiguration?pagesize={pagesize}&pagenum={pagenum}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);

            var response = _client.Execute<List<GKeyValueViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public GKeyValueViewModel GetByName(string KeyName)
        {
            var request = new RestRequest("api/GKeyValues/GetGKeyValuesByName?KeyName={KeyName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("KeyName", KeyName, ParameterType.UrlSegment);
            //request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<GKeyValueViewModel>(request);
            return response.Data;
        }

    }

    interface IGKeyValuesRestClient
    {

        GKeyValueViewModel GetKeyValue(string Key, string CompanyCode);

        void Add(GKeyValueViewModel serverData, string RedirectToUrl);
        IEnumerable<GKeyValueViewModel> GetGKeyValueForConfiguration(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery);
        int CountsForGKeyValueForConfiguration();
        GKeyValueViewModel GetById(int id);

        void Delete(int id, string RedirectToUrl);
        void Update(GKeyValueViewModel serverData, string RedirectToUrl);

        GKeyValueViewModel GetByName(string KeyName);


    }
}
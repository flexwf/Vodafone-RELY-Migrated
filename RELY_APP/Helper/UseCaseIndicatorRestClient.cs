using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RELY_APP.Helper
{
    public class UseCaseIndicatorRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public UseCaseIndicatorRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<UseCaseIndicatorViewModel> GetAll()
        {
            var request = new RestRequest("api/UseCaseIndicator/GetAll", Method.GET) { RequestFormat = DataFormat.Json };
            var response = _client.Execute<List<UseCaseIndicatorViewModel>>(request);
            return response.Data;
        }

    }
}
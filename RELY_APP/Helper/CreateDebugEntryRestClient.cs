using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RELY_APP.Helper
{
    public class CreateDebugEntryRestClient : IRCreateDebugEntryRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];


        public CreateDebugEntryRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }


        public string CreateDebugEntry(string Message)
        {
            var request = new RestRequest("api/CreateDebugEntry/CreateDebugEntry?Message=" + Message, Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(Message);
            var response = _client.Execute(request);
            return response.Content;
        }

    }
    interface IRCreateDebugEntryRestClient
    {
        string CreateDebugEntry(string Message);
    }


}
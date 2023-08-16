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
    public class DashBoardToDoRestClient : IDashBoardRestClient
    {
        // GET: DashBoardToDoRestClient
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        // GET: ProductHistoryGridRestClient
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public DashBoardToDoRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<GDashBoardToDoViewModel> GetDataForDashBoardToDo(int RoleId, int UserId)
        {


            var request = new RestRequest("api/GDashBoardToDo/GetDataForDashBoardToDo?RoleId={RoleId}&UserId={UserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);

            var response = _client.Execute<List<GDashBoardToDoViewModel>>(request);

            return response.Data;

        }


    }

    interface IDashBoardRestClient
    {

        IEnumerable<GDashBoardToDoViewModel> GetDataForDashBoardToDo(int RoleId, int UserId);



    }
}
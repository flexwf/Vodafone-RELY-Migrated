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
    class GSecurityQuestionsRestClient : IGSecurityQuestionsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        public GSecurityQuestionsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        //GET: Method to Get Entered Question Answer Detail by User from db 
        public IEnumerable<MLUsersGSecurityQuestionViewModel> GetQuestionAnswersByUserId(string userid)
        {
            var request = new RestRequest("api/GSecurityQuestions/GetQuestionAnswersByUser?userid={userid}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("userid", userid, ParameterType.UrlSegment);
            var response = _client.Execute<List<MLUsersGSecurityQuestionViewModel>>(request);
            return response.Data;
        }
        //GET: Method to Get Question Answer in Reset Password from Api
        public IEnumerable<ChangePasswordBindingModel> GetSecurityQuestions()
        {
            var request = new RestRequest("api/GSecurityQuestions/GetGSecurityQuestions", Method.GET) { RequestFormat = DataFormat.Json };
            var response = _client.Execute<List<ChangePasswordBindingModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        //POST Method to Post Question Answer in Reset Password From  Api to Db 
        public IEnumerable<MLUsersGSecurityQuestionViewModel> AddQuestionAnswers(ChangePasswordBindingModel serverData,string RedirectToUrl)
        {

            var request = new RestRequest("api/GSecurityQuestions/PostQuestionAnswers", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddBody(serverData);

            var response = _client.Execute<List<MLUsersGSecurityQuestionViewModel>>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }

        //PUT: Method to update Question Answer by user 
        public void PutQuestionAnswer(string userid, ChangePasswordBindingModel model, string RedirectToUrl)
        {
            var request = new RestRequest("api/GSecurityQuestions/PutQuestionAnswers?userid={userid}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("userid", userid, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<List<MLUsersGSecurityQuestionViewModel>>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

        }

    }



    interface IGSecurityQuestionsRestClient
    {
        IEnumerable<MLUsersGSecurityQuestionViewModel> GetQuestionAnswersByUserId(string userid);
        IEnumerable<ChangePasswordBindingModel> GetSecurityQuestions();
        IEnumerable<MLUsersGSecurityQuestionViewModel> AddQuestionAnswers(ChangePasswordBindingModel serverData,string RedirectToUrl);
        void PutQuestionAnswer(string userid, ChangePasswordBindingModel model, string RedirectToUrl);
    }
}
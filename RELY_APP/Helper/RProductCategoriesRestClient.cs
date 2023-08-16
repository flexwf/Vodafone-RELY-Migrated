using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace RELY_APP.Helper
{
    public class RProductCategoriesRestClient : IRProductCategoriesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];       

        public RProductCategoriesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<RProductCategoriesViewModel> GetByCompanyCode(string CompanyCode)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/RProductCategories/GetRProductCategoriesByCompanyCode?CompanyCode={CompanyCode}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<List<RProductCategoriesViewModel>>(request);
            return response.Data;
        }
        public void Add(RProductCategoriesViewModel serverData, string RedirectToUrl)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            //var request = new RestRequest("api/RProductCategories/Post?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var request = new RestRequest("api/RProductCategories/Post", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }            
        }

        /// <summary>
        /// Created by Rakhi Singh on  31st july
        /// Description: this method is used to get all Productcat on the basis of Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public RProductCategoriesViewModel GetById(int Id)
        {
            //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/RProductCategories/GetRProductCateById?id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<RProductCategoriesViewModel>(request);
            return response.Data;
        }

        /// <summary>
        /// Created by Rakhi Singh on 31st july 2018
        /// Description: Method to delete product cat on the basis of Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        /// <param name="RedirectToUrl"></param>
        public void Delete(int Id,string RedirectToUrl)
        {           
            var request = new RestRequest("api/RProductCategories/Delete?Id={Id}", Method.DELETE);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<RLocalPobTypeViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }

        public void Update(RProductCategoriesViewModel model, string RedirectToUrl)
        {           
            var request = new RestRequest("api/RProductCategories/Put?id={id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", model.Id, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<int>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
        }
        /*Below code is commented by Rakhi Singh on 30th july 2018 as per requirement.*/
        //public void Add(RProductCategoriesViewModel model, string RedirectToUrl)
        //{
        //    var request = new RestRequest("api/RProductCategories/PostRProductCategories?Name={Name}&Description={Description}&UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
        //    request.AddParameter("Name", model.Name, ParameterType.UrlSegment);
        //    request.AddParameter("Description", model.Description, ParameterType.UrlSegment);
        //    var response = _client.Execute<RProductCategoriesViewModel>(request);

        //    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
        //    {
        //        //call globals method to generate exception based on response
        //        Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
        //    }

        //    //if (response.StatusCode == HttpStatusCode.InternalServerError)
        //    //{
        //    //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //    //    ex.Data.Add("ErrorCode", response.StatusCode);
        //    //    string source = response.Content;
        //    //    dynamic data = JsonConvert.DeserializeObject(source);
        //    //    string xx = data.Message;
        //    //    ex.Data.Add("ErrorMessage", xx);
        //    //    throw ex;
        //    //}
        //    //if (response.StatusCode == HttpStatusCode.BadRequest)
        //    //{
        //    //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //    //    ex.Data.Add("ErrorCode", response.StatusCode);
        //    //    string source = response.Content;
        //    //    dynamic data = JsonConvert.DeserializeObject(source);
        //    //    string xx = data.Message;
        //    //    ex.Data.Add("ErrorMessage", xx);
        //    //    throw ex;
        //    //}
        //}

        //public void Update(RProductCategoriesViewModel model, string RedirectToUrl)
        //{
        //    string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //    string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

        //    var request = new RestRequest("api/RProductCategories/PutRProductCategories?Id={Id}&CompanyCode={CompanyCode}&Name={Name}&Description={Description}&UserName={UserName}&WorkFlow={WorkFlow}", Method.PUT) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("UserName", UserName,ParameterType.UrlSegment);
        //    request.AddParameter("WorkFlow", WorkFlow,ParameterType.UrlSegment);
        //    request.AddBody(model);
        //    var response = _client.Execute<RProductCategoriesViewModel>(request);

        //    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
        //    {
        //        //call globals method to generate exception based on response
        //        Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
        //    }

        //    //if (response.StatusCode == HttpStatusCode.NotFound)
        //    //{
        //    //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //    //    ex.Data.Add("ErrorCode", response.StatusCode);
        //    //    string source = response.Content;
        //    //    dynamic data = JsonConvert.DeserializeObject(source);
        //    //    string xx = data.Message;
        //    //    ex.Data.Add("ErrorMessage", xx);
        //    //    throw ex;
        //    //}
        //    //if (response.StatusCode == HttpStatusCode.InternalServerError)
        //    //{
        //    //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //    //    ex.Data.Add("ErrorCode", response.StatusCode);
        //    //    string source = response.Content;
        //    //    dynamic data = JsonConvert.DeserializeObject(source);
        //    //    string xx = data.Message;
        //    //    ex.Data.Add("ErrorMessage", xx);
        //    //    throw ex;
        //    //}
        //    //if (response.StatusCode == HttpStatusCode.BadRequest)
        //    //{
        //    //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //    //    ex.Data.Add("ErrorCode", response.StatusCode);
        //    //    string source = response.Content;
        //    //    dynamic data = JsonConvert.DeserializeObject(source);
        //    //    string xx = data.Message;
        //    //    ex.Data.Add("ErrorMessage", xx);
        //    //    throw ex;
        //    //}

        //}
        //public RProductCategoriesViewModel GetByCode(string CompanyCode, string Name)
        //{
        //    var request = new RestRequest("api/RProductCategories/GetRProductCategories?CompanyCode={CompanyCode}&Name={Name}", Method.GET) { RequestFormat = DataFormat.Json };

        //    request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
        //    request.AddParameter("Name", Name, ParameterType.UrlSegment);

        //    var response = _client.Execute<RProductCategoriesViewModel>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}
        //public int GetCountsByCompanyCode(string CompanyCode)
        //{
        //    var request = new RestRequest("api/RProductCategories/GetRProductCategoriesCountsByCompanyCode?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
        //    var response = _client.Execute<int>(request);

        //    return response.Data;
        //}

        //public void Delete(string CompanyCode, string Name, string RedirectToUrl)
        //{
        //    var request = new RestRequest("api/RProductCategories/DeleteRProductCategories?Id={Id}&CompanyCode={CompanyCode}&Name={Name}", Method.DELETE);

        //    request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
        //    request.AddParameter("Name", Name, ParameterType.UrlSegment);

        //    var response = _client.Execute<RProductCategoriesViewModel>(request);
        //    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
        //    {
        //        //call globals method to generate exception based on response
        //        Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
        //    }

        //    //if (response.StatusCode != HttpStatusCode.OK)
        //    //{

        //    //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //    //    ex.Data.Add("ErrorCode", response.StatusCode);
        //    //    string source = response.Content;
        //    //    dynamic data = JsonConvert.DeserializeObject(source);
        //    //    string xx = data.Message;
        //    //    ex.Data.Add("ErrorMessage", xx);
        //    //    throw ex;
        //    //}
        //}

        //Following Delete Method is Added By Vijay
        //public void Delete(int Id, string Name, string RedirectToUrl)
        //   {
        //       string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //       string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

        //       var request = new RestRequest("api/RProductCategories/DeleteRProductCat?Id={Id}&UserName={UserName}&WorkFlow={WorkFlow}", Method.DELETE);

        //       request.AddParameter("Id", Id, ParameterType.UrlSegment);
        //       request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //       request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

        //       var response = _client.Execute<RProductCategoriesViewModel>(request);
        //       if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
        //       {
        //           //call globals method to generate exception based on response
        //           Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
        //       }
        //   }

    }
    interface IRProductCategoriesRestClient
    {
        IEnumerable<RProductCategoriesViewModel> GetByCompanyCode(string CompanyCode);
        void Add(RProductCategoriesViewModel serverData, string RedirectToUrl);
        RProductCategoriesViewModel GetById(int Id);
        void Delete(int Id,string RedirectToUrl);
        void Update(RProductCategoriesViewModel model, string RedirectToUrl);
        /*Below code is commented by Rakhi Singh on 30th july 2018 as per requirement.*/
        //int GetCountsByCompanyCode(string CompanyCode);
        // int  GetByCode(string CompanyCode, string Name);
        // void Add(RProductCategoriesViewModel model, string RedirectToUrl);
        // RProductCategoriesViewModel GetByCode(string CompanyCode, string Name, string RedirectToUrl);
        //void Update(RProductCategoriesViewModel RProductCategoriesViewModel, string RedirectToUrl);
        //void Delete(string CompanyCode,string Name, string RedirectToUrl);
        //void Delete(int Id, string Name, string RedirectToUrl);
    }
}
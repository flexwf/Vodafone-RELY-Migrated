
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
    class AccountsRestClient : IAccountsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
        public AccountsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public bool Login()
        {
            if (System.Web.HttpContext.Current.Session["LoginEmail"].ToString().ToLower() == "accountant@vodafone.com")
            {
                if (System.Web.HttpContext.Current.Session["Password"].ToString() == "Rely#123")//hardcoding password, it will be updated later
                {
                    return true;
                }
                else
                {
                    var xx = "Username/Password is incorrect";
                    var ex = new Exception(String.Format("{0},{1}", xx, Globals.ExceptionType.Type4));
                    ex.Data.Add("ErrorCode", Globals.ExceptionType.Type4);
                    ex.Data.Add("RedirectToUrl", "");
                    //string source = response.Content;
                    //dynamic data = JsonConvert.DeserializeObject(source);
                    // string xx = data.Message;
                    ex.Data.Add("ErrorMessage", xx);
                    throw ex;
                }
                // return RedirectToAction("Index", "Home");
            }
            else
            {
                var xx = "Username/Password is incorrect";
                var ex = new Exception(String.Format("{0},{1}", xx, Globals.ExceptionType.Type4));
                ex.Data.Add("ErrorCode", Globals.ExceptionType.Type4);
                ex.Data.Add("RedirectToUrl", "");
                //string source = response.Content;
                //dynamic data = JsonConvert.DeserializeObject(source);
                // string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
               // return false; ;
            }

        }
        //VerifyMFAToken
        public LoginViewModel VerifyMFAToken(string  LoginEmail, string MFAOtp,string RedirectToUrl)
        {
            var request = new RestRequest("api/Account/VerifyMFAToken?LoginEmail={LoginEmail}&MFAOtp={MFAOtp}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("LoginEmail", LoginEmail, ParameterType.UrlSegment);
            request.AddParameter("MFAOtp", MFAOtp, ParameterType.UrlSegment);
            var response = _client.Execute<LoginViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }
        public string GetSelectedLandingPage(string Userid)
        {
            var text = "";
            var request = new RestRequest("api/Account/GetSelectedLandingPage?Userid={Userid}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Userid", Userid, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            text = response.Content.Replace('"', ' ').Trim();

            return text;
        }

        public void UpdateSelectedLandingPage(string UserId, string RoleName, string CompanyCode)
        {
            var request = new RestRequest("api/Account/UpdateSelectedLandingPage?Userid={Userid}&RoleName={RoleName}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Userid", UserId, ParameterType.UrlSegment);
            request.AddParameter("RoleName", RoleName, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute(request);

        }
        public LoginViewModel  GetUser(LoginViewModel model, string RedirectToUrl)
        {
            var request = new RestRequest("api/Account/GetUserInfo?HostBrowserDetails={HostBrowserDetails}&HostIP={HostIP}&HostTimeZone={HostTimeZone}&Email={Email}&Password={Password}&MFAOtp={MFAOtp}", Method.GET) { RequestFormat = DataFormat.Json };
            //var request = new RestRequest("api/Account/GetUserInfo?HostBrowserDetails={HostBrowserDetails}&HostIP={HostIP}&HostTimeZone={HostTimeZone}&Email={Email}&Password={Password}", Method.GET) { RequestFormat = DataFormat.Json };
            var LogUserEventModel = Globals.GetUserEvents();
            request.AddParameter("Email",model.Email, ParameterType.UrlSegment);
            request.AddParameter("Password",model.Password ,ParameterType.UrlSegment);
            request.AddParameter("HostBrowserDetails", LogUserEventModel.HostBrowserDetails, ParameterType.UrlSegment);
            request.AddParameter("HostIP", LogUserEventModel.HostIP, ParameterType.UrlSegment);
            request.AddParameter("HostTimeZone", LogUserEventModel.HostTimeZone, ParameterType.UrlSegment);
            request.AddParameter("MFAOtp", (string.IsNullOrEmpty(model.MFAOTP)) ? string.Empty : model.MFAOTP, ParameterType.UrlSegment);
            // request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            var response = _client.Execute<LoginViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //if (string.IsNullOrEmpty(RedirectToUrl))
                //{
                //    RedirectToUrl = "/Home/ErrorPage";
                //}
                //var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                //ex.Data.Add("ErrorCode", response.StatusCode);
                //ex.Data.Add("RedirectToUrl", RedirectToUrl);
                //string source = response.Content;
                //dynamic data = JsonConvert.DeserializeObject(source);
                //string xx = data.Message;
                //ex.Data.Add("ErrorMessage", xx);
                //throw ex;

                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);



            }

            return response.Data;

        }

        public LUserViewModel GetIdByEmail(string Email,string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/Account/GetIdByEmailId?Email={Email}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            var response = _client.Execute<LUserViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }

            return response.Data;
        }
        public bool CreateNewUser(LoginViewModel serverData, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/Account/CreateNewUser?UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<bool>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }

        public string GetHelpUrl(int RoleId, int MenuId, string RedirectToUrl)
        {
            var request = new RestRequest("api/Account/GetHelpUrl?RoleId={RoleId}&MenuId={MenuId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            request.AddParameter("MenuId", MenuId, ParameterType.UrlSegment);           
            var response = _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Content;
        }

        public bool ChangeUserPassword(ChangePasswordBindingModel serverData, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/Account/ChangeUserPassword?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<bool>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }
        public bool SetPasswordViaAdmin(ChangePasswordBindingModel serverData,int LoggedInUserId,  int LoggedInRoleId, string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/Account/SetPasswordViaAdmin?LoggedInUserId={LoggedInUserId}&LoggedInRoleId={LoggedInRoleId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddBody(serverData);
            var response = _client.Execute<bool>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }
        public ChangePasswordViewModel GenerateOTP(ChangePasswordBindingModel model,string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/Account/GenerateOTPnSendMail?UserName={UserName}&WorkFlow={WorkFlow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(model);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            var response = _client.Execute<ChangePasswordViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }
        public ChangePasswordViewModel VerifyOTP(string OTP, string Email, string UserId,string RedirectToUrl)
        {
            string WorkFlow = (string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["WorkFlow"] as string)) ? "No Workflow" : System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string;
            var request = new RestRequest("api/Account/VerifyOTP?OTP={OTP}&Email={Email}&UserId={UserId}&UserName={UserName}&WorkFlow={WorkFlow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("OTP", OTP, ParameterType.UrlSegment);
            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);

            var response = _client.Execute<ChangePasswordViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateExceptionFromResponse(response, RedirectToUrl);
            }
            return response.Data;
        }
      

    }

    interface IAccountsRestClient
    {
        string GetSelectedLandingPage(string Userid);
        void UpdateSelectedLandingPage(string UserId, string RoleName, string CompanyCode);
        LoginViewModel VerifyMFAToken(string LoginEmail, string MFAOtp, string RedirectToUrl);
        LoginViewModel GetUser(LoginViewModel model, string RedirectToUrl);
        bool Login();
        LUserViewModel GetIdByEmail(string Email,string RedirectToUrl);
        bool ChangeUserPassword(ChangePasswordBindingModel serverData,string RedirectToUrl);
        bool SetPasswordViaAdmin(ChangePasswordBindingModel serverData, int LoggedInUserId, int LoggedInRoleId, string RedirectToUrl);
        ChangePasswordViewModel GenerateOTP(ChangePasswordBindingModel model, string RedirectToUrl);
        ChangePasswordViewModel VerifyOTP(string OTP, string Email, string UserId, string RedirectToUrl);
        bool CreateNewUser(LoginViewModel serverData, string RedirectToUrl);
        string GetHelpUrl(int RoleId, int MenuId, string RedirectToUrl);
    }
}
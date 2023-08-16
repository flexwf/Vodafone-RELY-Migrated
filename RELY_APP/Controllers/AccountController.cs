using RELY_APP.Helper;
using RELY_APP.ViewModel;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using RELY_APP.Utilities;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

namespace RELY_APP.Controllers
{
    //[SessionExpire]
    //[HandleCustomError]
    public class AccountController : Controller
    {
        IAccountsRestClient ARC = new AccountsRestClient();
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {   //set session variable for no user logged in for use in exception handling
            System.Web.HttpContext.Current.Session["LoginEmail"] = "No User Logged in";
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string ClientIPAddress,string LastActiveUrl)
        {
            try
            {//set session variable for use in exception handling 
                System.Web.HttpContext.Current.Session["LoginEmail"] = (model.Email != null) ? model.Email : "No User Logged in";//Login Email Id
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Home");
                }
                
              //  IAccountsRestClient ACRC = new AccountsRestClient();
                var result = ARC.GetUser(model, "ResetPassword");
                TempData["LastLoginMessage"] = result.LastLoginMessage;

                //Last Active Url is maintained in case of session time out. This URL will redirects directly to the page which was desired by user before timeout occured
                if (!String.IsNullOrEmpty(LastActiveUrl))
                {
                    LastActiveUrl = LastActiveUrl.Replace("amp;", "");
                    TempData["LastActiveUrl"] = LastActiveUrl;
                    //Check whether RoleId exists in URL. If so, then Set RoleId and RoleName in session.
                    //Otherwise, Find out the default role for the Url page and set in session
                    if (LastActiveUrl.Contains("RoleId"))
                    {
                        Globals.GetRoleIdStepIdFromUrlAndUpdateSession(LastActiveUrl, result.CompanyCode);
                    }
                    else
                    {
                        //calculating Controller and Method Name from LastActiveUrl
                        var urlParts = LastActiveUrl.Split('/');
                        string ControllerPart = urlParts[3];
                        string MethodPart = urlParts[4].Split('?')[0];
                        //nullify LastActiveUrl when LastActiveUrl is like /Account/LogOff 
                        if ("logoff".Equals(MethodPart, StringComparison.OrdinalIgnoreCase) 
                            && "account".Equals(ControllerPart, StringComparison.OrdinalIgnoreCase))
                            TempData["LastActiveUrl"] = null;

                        CheckRoleAccessAndSetInSession(ControllerPart, MethodPart, result.CompanyCode, result.Roles);
                    }
                }
                if (result != null)
                {
                    Globals.SetSessionVariable(result.CompanyCode, result.Roles, result.Id, result.Email, result.FirstName, result.LastName, result.Phone);
                    Globals.LogUserEvent(result.Id, "LoggedIn", "Self Login", true, result.Id, result.CompanyCode, ClientIPAddress);
                    return RedirectToAction("DecideDashboard", "Home");
                }
                return RedirectToAction("Index", "Home");

            }
            catch(Exception ex)
            {
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)Globals.ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)Globals.ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller name and method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        // return View(GCVM);
                        return RedirectToAction("Index", "Home");
                    case (int)Globals.ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        //MFA screen added
                        if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"].ToString()) )
                        {
                            if (ex.Data["ErrorMessage"].ToString().Contains("MFA"))
                            {
                                model.ClientIPAddress = ClientIPAddress;
                                model.LastActiveUrl = LastActiveUrl;
                                if (ex.Data["ErrorMessage"].ToString().Equals("MFAScreen"))
                                {//clearing the message, so that it does not show on screen
                                    TempData["Error"] = "";
                                }
                                TempData["Login"] = model;

                                return RedirectToAction("MFAScreen");
                            }
                        }
                        
                        string redirectTo = ex.Data["RedirectToUrl"] as string;
                        return RedirectToAction(redirectTo, new { Email = model.Email, ResetPwdMsg = "Change Pwd" });
                    case (int)Globals.ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        // return View(GCVM);
                        return RedirectToAction("Index", "Home");
                    default:
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                        //throw ex;
                }

            }
        }
        private void CheckRoleAccessAndSetInSession(string Controller, string Method, string CompanyCode,List<LRoleViewModel> AssignedRolesListToUser)
        {
            /**Get the role list who has access to the page - /Controller/Action
             * Traverse through the Assigned Role List for the user. And Check whether the user has access to the page.
             * If yes, Store the Role variables in session
             */
            ILRolesRestClient RRC = new LRolesRestClient();
            var AssignesRolesListToPage = RRC.GetRoleAccessListByControllerAction(Controller,Method,CompanyCode);
            bool flag = false;

            foreach(var UserAccessRole in AssignedRolesListToUser)
            {
                foreach(var PageAccessRole in AssignesRolesListToPage)
                {
                    if (PageAccessRole.RoleName.Equals(UserAccessRole.RoleName))
                    {
                        System.Web.HttpContext.Current.Session["CurrentRoleName"] = UserAccessRole.RoleName;
                        System.Web.HttpContext.Current.Session["CurrentRoleId"] = UserAccessRole.Id;
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    break;
            }
            if (!flag)
            {
                System.Web.HttpContext.Current.Session["CurrentRoleName"] = (AssignedRolesListToUser.Count != 0) ? AssignedRolesListToUser.ElementAt(0).RoleName : ""; ;
                System.Web.HttpContext.Current.Session["CurrentRoleId"] = (AssignedRolesListToUser.Count > 0) ? Convert.ToString(AssignedRolesListToUser.ElementAt(0).Id) : "0";
            }

        }


        // POST: /Account/LogOff
        [HttpPost]
        [CustomAuthorize]
        // [ValidateAntiForgeryToken]
        public ActionResult LogOff(string ClientIPAddress)
        {
            string CompanyCode = "";
            string currentUserId = "";
            try
            {
                CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
                currentUserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
            }
            catch (Exception ex)
            {
            }
            //Killing session and clearing sessionId and Cache
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            Response.AppendHeader("Cache-Control", "no-store");
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (CompanyCode != "" && currentUserId != "")
            {
                Globals.LogUserEvent(currentUserId, "LoggedOut", "Self LogOff", true, currentUserId, CompanyCode, ClientIPAddress);
            }
            
            return RedirectToAction("Index", "Home");

        }

       // Description : Method to get helpurl for specific page.
        [HttpGet]
        public string GetHelpUrl(int RoleId, int MenuId)
        {   
            var url = ARC.GetHelpUrl(RoleId, MenuId, null);
            url = url.Replace('"', ' ').Trim();
            return url;
        }

        //Commenting due to build issue after model update
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        [CustomAuthorize]
        public ActionResult ResetPassword(string Email, string ResetPwdMsg)
        {
            //Changes by RG for Reset paswword Question and Answer
            IGSecurityQuestionsRestClient GSQRC = new GSecurityQuestionsRestClient();
           // IAccountsRestClient ARC = new AccountsRestClient();
            ChangePasswordBindingModel model = new ChangePasswordBindingModel();

            var user = ARC.GetIdByEmail(Email,null);
            var CPBM = GSQRC.GetQuestionAnswersByUserId(user.Id.ToString());
            //It Is assumed that there will be three records from db
            if (CPBM.Count() > 0)
            {

                model.Question1 = CPBM.ElementAt(0).QuestionId;
                model.Question2 = CPBM.ElementAt(1).QuestionId;
                model.Question3 = CPBM.ElementAt(2).QuestionId;
                model.Answer1 = CPBM.ElementAt(0).Answer;
                model.Answer2 = CPBM.ElementAt(1).Answer;
                model.Answer3 = CPBM.ElementAt(2).Answer;
            }
            ViewBag.Email = Email;
            ViewBag.ResetPwdMsg = ResetPwdMsg;
            var GSQuestionsDropdown = GSQRC.GetSecurityQuestions();
            ViewBag.Question1 = new SelectList(GSQuestionsDropdown, "Id", "Question", model.Question1);
            ViewBag.Question2 = new SelectList(GSQuestionsDropdown, "Id", "Question", model.Question2);
            ViewBag.Question3 = new SelectList(GSQuestionsDropdown, "Id", "Question", model.Question3);

            //Get Password validation rules as per current opco
            ILPasswordPoliciesRestClient LPPRC = new LPasswordPoliciesRestClient();
            var CompanyCode = user.CompanyCode;
            ViewBag.PasswordPolicies = LPPRC.GetByCompanyCode(CompanyCode, user.Id);
            return Email == null ? View("Error") : View(model);

        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        // [ControllerActionFilter]
        public ActionResult ResetPassword(ChangePasswordBindingModel model, string ResetPwdMsg)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            IGSecurityQuestionsRestClient GSARC = new GSecurityQuestionsRestClient();
            try
            {
                model.UserId = ARC.GetIdByEmail(model.Email,null).Id.ToString();
                //update securityQuestions only after Password is updated.
                 ARC.ChangeUserPassword(model, null);
                TempData["Message"] = "Password updated successfully.";

                var CPBM = GSARC.GetQuestionAnswersByUserId(model.UserId);
                if (CPBM.Count() == 0)
                {
                    var resultq = GSARC.AddQuestionAnswers(model,null);

                }
                else
                {
                    UpdateQuestionAnswers(model.UserId, model);
                }
                
                return RedirectToAction("ResetPasswordConfirmation", "Account");

            }
            catch (Exception ex)
            {
                ViewBag.Email = model.Email;
                ViewBag.Question1 = model.Question1;
                ViewBag.Question2 = model.Question2;
                ViewBag.Question3 = model.Question3;
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                TempData["ErrorMessage"] = ex.Data["ErrorMessage"];
                return RedirectToAction("ResetPassword", "Account", new { @Email = model.Email, ResetPwdMsg = ResetPwdMsg });
            }
           
        }

        //PUT :Method to overwite saved db data
        [CustomAuthorize]
        public void UpdateQuestionAnswers(string Email, ChangePasswordBindingModel model)
        {
            IGSecurityQuestionsRestClient GSARC = new GSecurityQuestionsRestClient();
             GSARC.PutQuestionAnswer(Email, model,null);

        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        [CustomAuthorize]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //[CustomAuthorize]
        public ActionResult ForgotPassword(string Email)
        {
          //  IAccountsRestClient ACRC = new AccountsRestClient();
            try
            {
                System.Web.HttpContext.Current.Session["LoginEmail"] = (Email != null) ? Email : "No User Logged in";//Login Email Id
                int UserId = ARC.GetIdByEmail(Email,null).Id;
                return RedirectToAction("GetQuestions", "Account", new { @UserId = UserId, @Email = Email, @Retry = false });
            }
            catch (Exception ex)
            {
                TempData["Message"] = "This User does not exist in the system. Please Provide valid Email in order to reset password";
                return RedirectToAction("Index", "Home");
            }
        }


       // [CustomAuthorize]
        public ActionResult GetQuestions(string UserId, string Email, bool Retry)
        {
            //user can try to answer/change the question maximum of 5 times. So maintaining a counter
            if (System.Web.HttpContext.Current.Session["OTPRetryCounter"] == null)
            {
                System.Web.HttpContext.Current.Session["OTPRetryCounter"] = 0;
            }
            else
            {
                int OTPRetryCounter = (int)System.Web.HttpContext.Current.Session["OTPRetryCounter"];
                if (OTPRetryCounter == 5)
                {
                    TempData["Message"] = "You did not answer the question. Please contact to L2 Support.";
                    return RedirectToAction("Index", "Home");
                }
                System.Web.HttpContext.Current.Session["OTPRetryCounter"] = OTPRetryCounter + 1;
            }

            IGSecurityQuestionsRestClient GSQRC = new GSecurityQuestionsRestClient();
            ChangePasswordBindingModel GSQ = new ChangePasswordBindingModel();
            var CPBM = GSQRC.GetQuestionAnswersByUserId(UserId);
            var GSQuestionsDropdown = GSQRC.GetQuestionAnswersByUserId(UserId);
            ViewBag.UserId = UserId;
            ViewBag.Email = Email;
            GSQ.Email = Email;
            GSQ.UserId = UserId;
            //when user provides incorrect answer, display the same question again.
            if (TempData["Model"] != null && !Retry)
            {
                GSQ = (ChangePasswordBindingModel)TempData["Model"];
                //NOW dropdown of question is replaced with Html Text in view
                ViewBag.Question1 = new SelectList(GSQuestionsDropdown, "QuestionId", "Question", GSQ.Question1);
                return View(GSQ);
            }
            List<MLUsersGSecurityQuestionViewModel> GSQList = new List<MLUsersGSecurityQuestionViewModel>();
            int index = 0;
            //Genetrateing a random number to get a random question
            if (CPBM.Count() > 0)
            {
                foreach (var item in CPBM)
                {
                    item.Id = index;
                    GSQList.Add(item);
                    index++;
                }
                Random rnd = new Random();
                int rndValue = rnd.Next(0, CPBM.Count());
                GSQ.Question1 = GSQList.ElementAt(rndValue).QuestionId;
                
                ViewBag.Question1 = new SelectList(GSQuestionsDropdown, "QuestionId", "Question", GSQ.Question1);
                return View(GSQ);
            }
            else
            {
                TempData["Message"] = "There are no security questions for the UserId. Please contact to the Support.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [ControllerActionFilter]--commenting the filer as Its not working even after mapping is inserted into DB. 
        public ActionResult GetQuestions(ChangePasswordBindingModel model)
        {
            try
            {
                var result = ARC.GenerateOTP(model,null);
                return RedirectToAction("GenerateOTP", "Account", new { @Email = model.Email, @UserId = model.UserId });
            }
            catch (Exception ex)
            {
                int OTPRetryCounter = (int)System.Web.HttpContext.Current.Session["OTPRetryCounter"];
                if (OTPRetryCounter > 5)
                {
                    TempData["Message"] = "You did not answer the question. Please contact to L2 Support.";
                    return RedirectToAction("Index", "Home");
                }

                TempData["ErrorMessage"] = ex.Data["ErrorMessage"];
                TempData["Model"] = model;
                return RedirectToAction("GetQuestions", "Account", new { @Email = model.Email, @UserId = model.UserId, @Retry = false });
                //return View(model);
            }
        }

        [HttpGet]
        //[CustomAuthorize]
        public ActionResult GenerateOTP(string Email, string UserId)
        {
            var OTPValidity = ConfigurationManager.AppSettings["OTPValidity"];
            ViewBag.Email = Email;
            ViewBag.UserId = UserId;
            ViewBag.OTPValidity = OTPValidity;
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateOTP(string OTP, string UserId, string Email)
        {
            try
            {
                var result = ARC.VerifyOTP(OTP, Email, UserId,null);
                ViewBag.Email = Email;
                ViewBag.UserId = UserId;
                return RedirectToAction("SetPassword", "Account", new { @UserId = UserId, @Email = Email });

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Data["ErrorMessage"];
                return RedirectToAction("GenerateOTP", "Account", new { @Email = Email, @UserId = UserId });

            }
        }

        [HttpGet]
       // [CustomAuthorize]
        public ActionResult SetPassword(int UserId, string Email)
        {
            ViewBag.UserId = UserId;
            ViewBag.Email = Email;
            //Get Password validation rules as per current opco
            ILPasswordPoliciesRestClient LPPRC = new LPasswordPoliciesRestClient();
            IAccountsRestClient ARC = new AccountsRestClient();
            var user = ARC.GetIdByEmail(Email,null);
            var CompanyCode = user.CompanyCode;
            ViewBag.PasswordPolicies = LPPRC.GetByCompanyCode(CompanyCode, UserId);
            return View();
        }

        // POST: /Account/SetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SetPassword(ChangePasswordBindingModel model, string UserId, string Email)
        {
            try
            {
                var result = ARC.ChangeUserPassword(model,null);

                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Data["ErrorMessage"];
                ViewBag.Email = Email;
                return RedirectToAction("SetPassword", "Account", new { @UserId = UserId, @Email = Email });
            }

        }

        //This method will Reset Password for User WorkFlow
        [CustomAuthorize]
        //public ActionResult ResetPasswordViaAdmin(int WorkflowId, int StepId, string TransactionId, string ActionName, string Comments, int StepParticipantActionId)

        public ActionResult ResetPasswordViaAdmin(string TransactionId, string Comments, int StepParticipantActionId)
        {
            int LoggedInUserId = Convert.ToInt32(Globals.GetSessionData("UserId"));
            int LoggedInRoleId = Convert.ToInt32(Globals.GetSessionData("CurrentRoleId"));
            //Calculating WrkFlowId,StepId,ActionName from StepParticipantId
            IWStepParticipantActionsRestClient WPARC = new WStepParticipantActionsRestClient();
            var ParticipantActionData = WPARC.GetWFIdStepIdById(StepParticipantActionId);
            //if (string.IsNullOrEmpty(ParticipantActionData.ActionName))
            //    ActionName = "Reset Password ";
            var isAuthorized = Globals.CheckActionAuthorization(ParticipantActionData.ActionName, LoggedInRoleId, LoggedInUserId, ParticipantActionData.WorkFlowId, ParticipantActionData.StepId);
            if (isAuthorized)
            {
                ILUsersRestClient URC = new LUsersRestClient();
                var user = URC.GetById(Convert.ToInt32(TransactionId));
                string LoginEmail = user.LoginEmail;
                ChangePasswordBindingModel model = new ChangePasswordBindingModel { Email = LoginEmail };
                var result = ARC.SetPasswordViaAdmin(model, LoggedInUserId, LoggedInRoleId, null);
                @TempData["Message"] = "New password has been sent to the user : " + LoginEmail;
                return RedirectToAction("Index", "GenericGrid", new { @WorkFlow = "Users" });
            }
            else
            {
                TempData["Error"] = "Sorry , You are not authorised to perform this action .Please contact L2 Admin";
                return RedirectToAction("Index", "GenericGrid", new { @WorkFlow = "Users" });
            }

        }




        //MFA OTP Screen
        public ActionResult MFAScreen()
        {
            LoginViewModel model = new LoginViewModel();
            model = (LoginViewModel)TempData["Login"];
            if (model != null)
            {
                ViewBag.LastActiveUrl = model.LastActiveUrl;
                ViewBag.ClientIPAddress = model.ClientIPAddress;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult PostMFAScreen(LoginViewModel model)
        {
            if(string.IsNullOrEmpty(model.MFAOTP))
            {
                TempData["Error"] = "Please provide MFA Code";
                TempData["Login"] = model;
                return RedirectToAction("MFAScreen");
            }
            try
            {//set session variable for use in exception handling 
                System.Web.HttpContext.Current.Session["LoginEmail"] = (model.Email != null) ? model.Email : "No User Logged in";//Login Email Id
                
                var result = ARC.VerifyMFAToken(model.Email, model.MFAOTP, "ResetPassword");
                TempData["LastLoginMessage"] = result.LastLoginMessage;
                string LastActiveUrl = model.LastActiveUrl;
                //Last Active Url is maintained in case of session time out. This URL will redirects directly to the page which was desired by user before timeout occured
                if (!String.IsNullOrEmpty(LastActiveUrl))
                {
                    LastActiveUrl = LastActiveUrl.Replace("amp;", "");
                    TempData["LastActiveUrl"] = LastActiveUrl;
                    //Check whether RoleId exists in URL. If so, then Set RoleId and RoleName in session.
                    //Otherwise, Find out the default role for the Url page and set in session
                    if (LastActiveUrl.Contains("RoleId"))
                    {
                        Globals.GetRoleIdStepIdFromUrlAndUpdateSession(LastActiveUrl, result.CompanyCode);
                    }
                    else
                    {
                        //calculating Controller and Method Name from LastActiveUrl
                        var urlParts = LastActiveUrl.Split('/');
                        string ControllerPart = urlParts[3];
                        string MethodPart = urlParts[4].Split('?')[0];
                        CheckRoleAccessAndSetInSession(ControllerPart, MethodPart, result.CompanyCode, result.Roles);
                    }
                }
                if (result != null)
                {
                    Globals.SetSessionVariable(result.CompanyCode, result.Roles, result.Id, result.Email, result.FirstName, result.LastName, result.Phone);
                    Globals.LogUserEvent(result.Id, "LoggedIn", "Self Login", true, result.Id, result.CompanyCode, model.ClientIPAddress);
                    return RedirectToAction("DecideDashboard", "Home");
                }
                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)Globals.ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)Globals.ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller name and method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        // return View(GCVM);
                        return RedirectToAction("Index", "Home");
                    case (int)Globals.ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        //MFA screen added
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"].ToString()))
                        {
                            if (ex.Data["ErrorMessage"].ToString().Contains("MFA"))
                            {
                                TempData["Login"] = model;
                                return RedirectToAction("MFAScreen");
                            }
                        }
                        string redirectTo = ex.Data["RedirectToUrl"] as string;
                        return RedirectToAction(redirectTo, new { Email = model.Email, ResetPwdMsg = "Change Pwd" });
                    case (int)Globals.ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        // return View(GCVM);
                        return RedirectToAction("Index", "Home");
                    default:
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                        //throw ex;
                }

            }
        }

        public ActionResult UserSettings()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Settings";
            string LoginEmail = Convert.ToString(System.Web.HttpContext.Current.Session["LoginEmail"]);
            ViewBag.Email = LoginEmail;
            return View();
        }
        
        [AllowAnonymous]
        public ActionResult ResetUserPreference(string Email)
        {
            ViewBag.Email = Email;
            return View();

        }
        public JsonResult GetSelectedLandingPage()
        {
            string UserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
            string data = string.Empty;
            IAccountsRestClient ARC = new AccountsRestClient();
            data = ARC.GetSelectedLandingPage(UserId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatSelectedLandingPage(string SelectedRole)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            string LoginEmail = Convert.ToString(System.Web.HttpContext.Current.Session["LoginEmail"]);
            string UserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
            IAccountsRestClient ARC = new AccountsRestClient();
            ARC.UpdateSelectedLandingPage(UserId, SelectedRole, CompanyCode);
            ViewBag.Email = LoginEmail;
            return Json("", JsonRequestBehavior.AllowGet);
        }


    }
}
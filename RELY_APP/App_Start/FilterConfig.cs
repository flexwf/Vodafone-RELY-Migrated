using RELY_APP.Utilities;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RELY_APP.ViewModel;
using RELY_APP.Helper;
using System.Linq;

namespace RELY_APP
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SessionExpireAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Gets the controller name
            var Controller = filterContext.RouteData.Values["controller"].ToString();
            //gets the action name
            string actionName = filterContext.RouteData.Values["action"].ToString();
            Uri CurrentURL = filterContext.HttpContext.Request.Url;
           CreateDebugEntryRestClient CDERC = new CreateDebugEntryRestClient();
            HttpContext context = HttpContext.Current;
            try
            {

                if (context.Session != null)
                {
                    if (context.Session.IsNewSession)
                    {
                        string sessionCookie = context.Request.Headers["Cookie"];

                        if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                        {
                            CDERC.CreateDebugEntry("OnActionExecuting:- session ID Details:" + sessionCookie.IndexOf("ASP.NET_SessionId").ToString());
                            FormsAuthentication.SignOut();
                            //string redirectTo = string.Format("~/Home/Index");
                            string redirectTo = string.Format("~/Home/SessionTimeOutLogin");
                            filterContext.Result = new RedirectResult(redirectTo);
                            filterContext.Controller.TempData["LastActiveUrl"] = CurrentURL;
                            return;
                        }
                    }
                    Boolean blnFound = false;
                    for (int i = 0; i < Globals.LstSessionIDs.Count; i++)
                    {
                        if (context.Session.SessionID == Globals.LstSessionIDs[i]._UserSessionID)
                        {
                            blnFound = true;
                            
                        }
                    }
                    if (!blnFound)
                    {
                        CDERC.CreateDebugEntry("OnActionExecuting:- session ID Details Not found");
                        FormsAuthentication.SignOut();
                        //string redirectTo = string.Format("~/Home/Index");
                        string redirectTo = string.Format("~/Home/SessionTimeOutLogin");
                        filterContext.Result = new RedirectResult(redirectTo);
                        filterContext.Controller.TempData["LastActiveUrl"] = CurrentURL;
                        return;
                    }
                }
                else
                {
                    //If session is null, user is reditrected to Login Page
                    CDERC.CreateDebugEntry("OnActionExecuting:- session is null");
                    // string redirectTo = string.Format("~/Home/Index");
                    string redirectTo = string.Format("~/Home/SessionTimeOutLogin");
                    filterContext.Controller.TempData["Error"] = "Session TimedOut";
                    filterContext.Controller.TempData["LastActiveUrl"] = CurrentURL;
                    filterContext.Result = new RedirectResult(redirectTo);
                    return;
                }
            }
            catch (Exception ex)
            {
                // Add Error Log to database Table
                CDERC.CreateDebugEntry("OnActionExecuting:- Catch Block");
                var model = new GErrorLogViewModel { UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string, Controller = Controller, Method = actionName,ErrorDateTime = DateTime.UtcNow,StackTrace = ex.StackTrace.ToString(),ErrorDescription = "", SourceProject = "[RELY WebApp]", Status = "New" };
                IErrorLogsRestClient ELRC = new ErrorLogsRestClient();
                ELRC.Add(model,null);
            }
            
            base.OnActionExecuting(filterContext);

        }


    }


    public class HandleCustomError : System.Web.Mvc.HandleErrorAttribute
    {
        public override void OnException(ExceptionContext FilterContext)
        {
            string Controller = "";
            string actionName = "";
            Type ControllerType;
            string statusMessage = "";
            try
            {
                //RK Moved following piece of code in try section to track the timeout error screen
                Controller = FilterContext.RouteData.Values["controller"].ToString();
                actionName = FilterContext.RouteData.Values["action"].ToString();
                ControllerType = FilterContext.Controller.GetType();
                statusMessage = FilterContext.Exception.ToString();
                // Add Error Log to database Table
                var model = new GErrorLogViewModel { UserName = System.Web.HttpContext.Current.Session["LoginEmail"].ToString(), Controller = Controller, Method = actionName, ErrorDateTime = DateTime.UtcNow, StackTrace = statusMessage.ToString(), SourceProject = "[RELY WebApp]", Status = "New" };
                IErrorLogsRestClient ELRC = new ErrorLogsRestClient();
                //////////////////////////////////////
                ELRC.Add(model, null);
            }
            catch (Exception ex)
            {
                //Send mail to  l2 admin
                var Subject = ConfigurationManager.AppSettings["ExceptionEmailSubject"];
                string Body;
                var UserName = (System.Web.HttpContext.Current.Session["LoginEmail"] != null) ? System.Web.HttpContext.Current.Session["LoginEmail"].ToString() : "";
                Body = "<table border='1'><tr><td>Application Name</td><td>" + ConfigurationManager.AppSettings["ExceptionEmailProjectName"] + "</td></tr><tr><td>Controller</td><td>" + Controller + "</td></tr><tr><td>Method Name</td><td>" + actionName + "</td></tr><tr><td>Exception Date/Time(Utc)</td><td>" + DateTime.UtcNow.ToString() + "</td></tr><tr><td>User Name</td><td>" + UserName + "</td></tr><tr><td>Stack Trace</td><td>" + statusMessage + "</td></tr></table>";
                Globals.SendExceptionEmail(Body);
            }
            FilterContext.ExceptionHandled = true;
            FilterContext.Result = new ViewResult
            {
                ViewName = "Error"
            };
        }
    }

    public class ControllerActionFilterAttribute : ActionFilterAttribute
    {
        private string CurrentActionKey;
        
        IMAuthorizableObjectsRolesRestClient client = new MAuthorizableObjectsRolesRestClient();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CreateDebugEntryRestClient CDERC = new CreateDebugEntryRestClient();
            CurrentActionKey = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName +
                              "-" + filterContext.ActionDescriptor.ActionName;
            var LoggedinRoleId = System.Web.HttpContext.Current.Session["CurrentRoleId"];//getting the current UserRoleId
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            
            Uri CurrentURL = filterContext.HttpContext.Request.Url;
            string LastActiveUrl = null;
            if (CurrentURL != null)
            {
                LastActiveUrl = CurrentURL.ToString();
            }
            //if URL contains RoleId, setting it in session. Thus, changing role on direct url hit.
            if (!String.IsNullOrEmpty(LastActiveUrl))
            {
                if (LastActiveUrl.Contains("RoleId"))
                {
                    Globals.GetRoleIdStepIdFromUrlAndUpdateSession(LastActiveUrl, CompanyCode);
                }
            }
            LoggedinRoleId = System.Web.HttpContext.Current.Session["CurrentRoleId"];
            if (LoggedinRoleId != null)
            {
                string UserRoleId = LoggedinRoleId.ToString();
                //getting list of Role Ids Who has  access to the Controller/Action
                var AuthorizedRolesListForMenu = client.GetRolesListbyControllerAction(CurrentActionKey, CompanyCode);
                //If LoggedInRoleId is found in Authorized RoleId List, then access granted. Otherwise, display unauthorized access page
                if(AuthorizedRolesListForMenu.Contains(UserRoleId))
                {
                    return;
                }
                //int matchFound = client.GetCount(UserRoleId, CurrentActionKey);
                //if (matchFound != 0)
                //{
                //    return;
                //}
                
                filterContext.Result = new ViewResult
                {
                    ViewName = "UnAuthorized"
                };
            }
            else
            {
                
                filterContext.Result = new RedirectResult("/Home/Index");
            }

        }
    }

    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedroles;
        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException("httpContext");
            // Make sure the user is authenticated. When there is value in Session variable, that means User is authenticated.
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["CurrentRoleName"] as string))
                return true;
            else
                return false ;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var currentRole = System.Web.HttpContext.Current.Session["CurrentRoleName"] as string;
            if (currentRole != null)
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "UnAuthorized"
                };
            }
            else
            {
                Uri CurrentURL = filterContext.HttpContext.Request.Url;
                filterContext.Controller.TempData["LastActiveUrl"] = CurrentURL;
                filterContext.Result = new RedirectResult("/Home/Index");
            }

        }
    }

}
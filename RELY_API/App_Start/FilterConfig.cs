using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace RELY_API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }

    //This attribute class is defined to log the unhandled errors in project to GError Logs
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        public override void OnException(HttpActionExecutedContext Context)
        {
            string exceptionMessage = Context.Exception.Message + Environment.NewLine + Context.Exception.StackTrace;
            int errorid = 0;
            if (Context.Exception.InnerException != null)
            {
                exceptionMessage += Environment.NewLine + Context.Exception.InnerException.Message + Environment.NewLine + Context.Exception.InnerException.StackTrace; ;

                if (Context.Exception.InnerException.InnerException != null)
                {
                    exceptionMessage += Environment.NewLine + Context.Exception.InnerException.InnerException.Message + Environment.NewLine + Context.Exception.InnerException.InnerException.StackTrace;
                }
            }
            string[] s = Context.Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at second and action name at 3rd index position
                                                                            // Add Error Log to database Table
            var model = new GErrorLog { UserName = "RELY", Controller = s[2], Method = s[3], ErrorDateTime = DateTime.UtcNow, StackTrace = exceptionMessage, SourceProject = "[RELY WebApi]" };

            try
            {
                //db.SpLogError("RELY-API", s[2], s[3], exceptionMessage, "RELY", "Type1", exceptionMessage, "resolution", "L2Admin", "field", 0, "New");
                //db.GErrorLogs.Add(model);//call db.spLogError here instead
                ObjectParameter Result = new ObjectParameter("Result", typeof(int)); //return parameter is declared
                db.SpLogError("RELY-API", s[2], s[3], exceptionMessage, "RELY", "Type1", exceptionMessage, "resolution", "L2Admin", "field", 0, "New", Result);
               
                db.SaveChanges();
                errorid = (int)Result.Value; //getting value of output parameter
            }
            catch (Exception ex)
            {
                //if something went wrong while adding error in db,generate email
                var Body = "<table border='1'><tr><td>Application Name</td><td>" + ConfigurationManager.AppSettings["ExceptionEmailProjectName"] + 
                    "</td></tr><tr><td>Controller</td><td>" + s[2] + "</td></tr><tr><td>Method Name</td><td>" + s[3] + "</td></tr><tr><td>Exception Date/Time(Utc)</td><td>" + 
                    DateTime.UtcNow.ToString() + "</td></tr><tr><td>User Name</td><td>" + "" + "</td></tr><tr><td>Stack Trace</td><td>" + exceptionMessage + "</td></tr></table>";
                //because system is unable to add row to error log, SendMail through SMTP details in web.config 
                Globals.SendExceptionEmail(Body);
            }
            throw new HttpResponseException(Context.Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, (string.Format(Globals.SomethingElseFailedInDBErrorMessage, errorid))));//new HttpResponseMessage(HttpStatusCode.InternalServerError)
           
        }
    }



}

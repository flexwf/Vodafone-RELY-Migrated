using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity.Validation;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LUserActivityLogController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/LUserActivityLogs
        public IHttpActionResult GetLUserActivityLogs()
        {
            var xx = (from aa in db.LUserActivityLogs
                      select new
                      {
                          aa.Id,
                          aa.Activity,
                          aa.Remarks,
                          aa.IsActivitySucceeded,
                          aa.HostIP,
                          aa.HostBrowserDetails,
                          aa.HostTimeZone,
                          aa.ActivityDateTime,
                          aa.CompanyCode,
                          aa.ActionById,
                          aa.ActionForId
                      }).OrderBy(p => p.Activity);
            return Ok(xx);
        }


        // GET: api/LUserActivityLogs/5
        [ResponseType(typeof(LUserActivityLog))]
        public async Task<IHttpActionResult> GetLUserActivityLog(Nullable<int> id)
        {
            var LUserActivityLog = db.LUserActivityLogs.Where(p => p.Id == id).Select(aa => new {
                aa.Id,
                aa.Activity,
                aa.Remarks,
                aa.IsActivitySucceeded,
                aa.HostIP,
                aa.HostBrowserDetails,
                aa.HostTimeZone,
                aa.ActivityDateTime,
                aa.CompanyCode,
                aa.ActionById,
                aa.ActionForId
            }).FirstOrDefault();
            if (LUserActivityLog == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "USER ACTIVITY")));
            }
            return Ok(LUserActivityLog);
        }

        // PUT: api/LUserActivityLogs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLUserActivityLog(int id, LUserActivityLog LUserActivityLog)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "USER ACTIVITY")));
            }

            if (!LUserActivityLogExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "USER ACTIVITY")));
            }

            if (id != LUserActivityLog.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "USER ACTIVITY")));
            }
            try
            {
                db.Entry(LUserActivityLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbex)
            {
                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                {
                    //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                }
                else
                {
                    throw ex;//This exception will be handled in FilterConfig's CustomHandler
                }
            }
            // return StatusCode(HttpStatusCode.NoContent);
            return Ok(LUserActivityLog);
        }

        // POST: api/LUserActivityLogs
        [ResponseType(typeof(LUserActivityLog))]
        public async Task<IHttpActionResult> PostLUserActivityLog(LUserActivityLog LUserActivityLog, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "USER ACTIVITY")));
            }

            try
            {
                db.LUserActivityLogs.Add(LUserActivityLog);
                await db.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbex)
            {
                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                {
                    //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                }
                else
                {
                    throw ex;//This exception will be handled in FilterConfig's CustomHandler
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = LUserActivityLog.Id }, LUserActivityLog);
        }

        // DELETE: api/LUserActivityLogs/5
        [ResponseType(typeof(LUserActivityLog))]
        public async Task<IHttpActionResult> DeleteLUserActivityLog(int id)
        {
            LUserActivityLog LUserActivityLog = await db.LUserActivityLogs.FindAsync(id);
            if (LUserActivityLog == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "USER ACTIVITY")));
            }
            try
            {
                db.LUserActivityLogs.Remove(LUserActivityLog);
                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                {
                    //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry. 
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                }
                else
                {
                    throw ex;
                }
            }
            return Ok(LUserActivityLog);
        }

        private bool LUserActivityLogExists(int id)
        {
            return db.LUserActivityLogs.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            //var SqEx = ex.GetBaseException() as SqlException;
            ////Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("FK_GCompanies_LUserActivityLog_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USER ACTIVITY", "COMPANY CODE"));
            //else if (SqEx.Message.IndexOf("FK_LUsers_LUserActivityLog_ActionBy", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USER ACTIVITY", "ACTIVITY BY"));
            //else if (SqEx.Message.IndexOf("FK_LUsers_LUserActivityLog_ActionFor", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USER ACTIVITY", "ACTION FOR"));
            ////UNIQUE KEY MUST ALSO COME
            //else
            //{
            //    //Something else failed return original error message as retrieved from database
            //    //Add complete Url in description
            //    var UserName = "";//System.Web.HttpContext.Current.Session["UserName"] as string;
            //    string UrlString = Convert.ToString(Request.RequestUri.AbsolutePath);
            //    var ErrorDesc = "";
            //    var Desc = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
            //    if (Desc.Count() > 0)
            //        ErrorDesc = string.Join(",", Desc);
            //    string[] s = Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at 2nd and action name at 3 rd index position
            //    db.SpLogError("RELY-API", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New");
            //    return Globals.SomethingElseFailedInDBErrorMessage;
            //}
            return null;
        }
    }
}

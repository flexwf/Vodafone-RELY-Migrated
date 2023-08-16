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
using System.Data.Entity.Core.Objects;
using System.Collections.Generic;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class GErrorLogsController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/GErrorLogs
        public IHttpActionResult GetGErrorLogs()
        {
            var xx = (from aa in db.GErrorLogs
                      select new
                      {
                          aa.Id,
                          aa.ErrorDateTime,
                          aa.SourceProject,
                          aa.Controller,
                          aa.Method,
                          aa.StackTrace,
                          aa.UserName,
                          aa.ErrorType,
                          aa.ErrorDescription,
                          aa.Resolution,
                          aa.ErrorOwner,
                          aa.FieldName,
                          aa.BatchNumber,
                          aa.Status
                      }).OrderBy(p => p.ErrorDateTime);
            return Ok(xx);
        }


        // GET: api/GErrorLogs/5
        [ResponseType(typeof(GErrorLog))]
        public async Task<IHttpActionResult> GetGErrorLog(Nullable<int> id)
        {
            var GErrorLog = db.GErrorLogs.Where(p => p.Id == id).Select(aa => new {
                aa.Id,
                aa.ErrorDateTime,
                aa.SourceProject,
                aa.Controller,
                aa.Method,
                aa.StackTrace,
                aa.UserName,
                aa.ErrorType,
                aa.ErrorDescription,
                aa.Resolution,
                aa.ErrorOwner,
                aa.FieldName,
                aa.BatchNumber,
                aa.Status
            }).FirstOrDefault();
            if (GErrorLog == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ERRORLOG")));
            }
            return Ok(GErrorLog);
        }

        // PUT: api/GErrorLogs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGErrorLog(int id, GErrorLog GErrorLog)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ERRORLOG")));
            }
                if (!GErrorLogExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ERRORLOG")));
                }

                if (id != GErrorLog.Id)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ERRORLOG")));
                }
                try
                {
                    db.Entry(GErrorLog).State = EntityState.Modified;
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
            return Ok(GErrorLog);
        }

        // POST: api/GErrorLogs
        [ResponseType(typeof(GErrorLog))]
        public async Task<IHttpActionResult> PostGErrorLog(GErrorLog GErrorLog)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ERRORLOG")));
            }
            
                try
                {
                    db.GErrorLogs.Add(GErrorLog);
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
              
            return CreatedAtRoute("DefaultApi", new { id = GErrorLog.Id }, GErrorLog);
        }

        // DELETE: api/GErrorLogs/5
        [ResponseType(typeof(GErrorLog))]
        public async Task<IHttpActionResult> DeleteGErrorLog(int id)
        {
            GErrorLog GErrorLog = await db.GErrorLogs.FindAsync(id);
            if (GErrorLog == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ERRORLOG")));
            }
            
                try
                {
                    db.GErrorLogs.Remove(GErrorLog);
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
               
            return Ok(GErrorLog);
        }

        public IHttpActionResult GetGErrorlogGridData(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
           
            var SortQuery = "";
            if (!string.IsNullOrEmpty(sortdatafield))
            {
                SortQuery = " order by " + sortdatafield + " " + sortorder;
            }
            else
            {
                SortQuery = " ORDER BY ErrorDateTime desc";
            }
            string Qry = string.Empty;
            if (FilterQuery == null)
            {
                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as row " +
                      "FROM(Select Id, ErrorDateTime, SourceProject, Controller, Method, StackTrace, UserName,[status], " +
                      "ErrorType,ErrorDescription,Resolution,ErrorOwner,FieldName,BatchNumber from GErrorLogs where ErrorDateTime >= dateadd(dd, -30, getdate()))A " +
                      " )B WHERE B.row > @P1 AND B.row <= @P2";
            }
            else
            {
                FilterQuery = "WHERE 1=1" + FilterQuery;
                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as row " +
                    "FROM(Select Id, ErrorDateTime, SourceProject, Controller, Method, StackTrace, UserName,[status], " +
                    "ErrorType,ErrorDescription,Resolution,ErrorOwner,FieldName,BatchNumber from GErrorLogs where ErrorDateTime >= dateadd(dd, -30, getdate()))A " +
                    FilterQuery + " )B WHERE B.row > @P1 AND B.row <= @P2";

            }
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<ErrorLogViewModel>(Qry, parameters).ToList();

            return Ok(xx);
        }

        //Method Added by RG To count Error into GErrorLog
        public IHttpActionResult GetErrorLogCount()
        {           
            string Qry = "SELECT ErrorDateTime, SourceProject,Controller, Method, StackTrace, UserName,[status],ErrorType, ErrorDescription, Resolution, ErrorOwner, FieldName, BatchNumber from GErrorLogs  where ErrorDateTime >= dateadd(dd, -30, getdate()) order by ErrorDateTime desc";
            var xx = db.Database.SqlQuery<ErrorLogViewModel>(Qry).Count();
            return Ok(xx);
        }

        //method to get the counts for summary tab on L2Admin Page
        public IHttpActionResult GetExceptionSummaryCounts()
        {
            string Qry = "SELECT Controller,Method,count(Method) as Exceptions from GErrorLogs where ErrorDateTime >= dateadd(dd, -30, getdate()) group by Controller, Method";
            var xx = db.Database.SqlQuery<ErrorLogViewModel>(Qry).Count();
            return Ok(xx);
        }

        [HttpGet]
        public IHttpActionResult GetExceptionSummary(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {
            var SortQuery = "";
            if (!string.IsNullOrEmpty(sortdatafield))
            {
                SortQuery = " order by " + sortdatafield + " " + sortorder;
            }
            else
            {
                SortQuery = " ORDER BY Controller desc";
            }

            string Qry = string.Empty;
            if (FilterQuery == null)
            {
                Qry = "SELECT * FROM (SELECT Controller,Method,count(Method) as Exceptions,ROW_NUMBER() OVER (" + SortQuery + ") " +
                      "as datacount FROM (SELECT * from GErrorLogs where ErrorDateTime >= dateadd(dd, -30, getdate()))a " +
                      "group by Controller, Method)b  where b.datacount > @P1 and b.datacount <= @P2";// Order by @P3";
            }
            else
            {
                FilterQuery = "WHERE 1=1" + FilterQuery;
                Qry = "SELECT * FROM (SELECT Controller,Method,count(Method) as Exceptions,ROW_NUMBER() OVER (" + SortQuery + ") " +
                     "as datacount FROM(SELECT * from GErrorLogs where ErrorDateTime >= dateadd(dd, -30, getdate()))a " +
                     FilterQuery + " group by Controller, Method)b  where b.datacount > @P1 and b.datacount <= @P2";// Order by @P3";
            }

            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<ErrorLogViewModel>(Qry, parameters).ToList();
            return Ok(xx);
        }

        public IHttpActionResult GetExceptionChart()
        {

            string Qry = string.Empty;
            Qry = "select Controller,Method,count(Method) as Exceptions from GErrorLogs where ErrorDateTime >= dateadd(dd, -30, getdate()) group by Controller,Method";
            var xx = db.Database.SqlQuery<ErrorLogViewModel>(Qry).ToList();

            return Ok(xx);
        }

        private bool GErrorLogExists(int id)
        {
            return db.GErrorLogs.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("SpLogEmail", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ERROR", "DATABASE OBJECTS(S)"));
            //UNIQUE KEY MUST ALSO COME
           else
            {
                //Something else failed return original error message as retrieved from database
                //Add complete Url in description
                var UserName = "";//System.Web.HttpContext.Current.Session["UserName"] as string;
                string UrlString = Convert.ToString(Request.RequestUri.AbsolutePath);
                var ErrorDesc = "";
                var Desc = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                if (Desc.Count() > 0)
                    ErrorDesc = string.Join(",", Desc);
                string[] s = Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at 2nd and action name at 3 rd index position
                //db.SpLogError("RELY-API", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New");
                //return Globals.SomethingElseFailedInDBErrorMessage;

             ObjectParameter Result = new ObjectParameter("Result", typeof(int)); //return parameter is declared
                db.SpLogError("RELY-API", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New", Result).FirstOrDefault();
                int errorid = (int)Result.Value; //getting value of output parameter
                //return Globals.SomethingElseFailedInDBErrorMessage;
                return (string.Format(Globals.SomethingElseFailedInDBErrorMessage, errorid));
            }
        }
    }
}

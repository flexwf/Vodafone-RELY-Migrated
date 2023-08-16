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
    public class LEmailBucketController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/LEmailBuckets
        public IHttpActionResult GetLEmailBuckets()
        {
            var xx = (from aa in db.LEmailBuckets
                      select new
                      {
                          aa.Id,
                          aa.RecipientList,
                          aa.CCList,
                          aa.BCCList,
                          aa.ReplyToList,
                          aa.Subject,
                          aa.Body,
                          aa.IsHTML,
                          aa.EmailType,
                          aa.Priority,
                          aa.AttachmentList,
                          aa.Status,
                          aa.Comments,
                          aa.CreatedDateTime,
                          aa.UpdatedDateTime,
                          aa.CreatedById,
                          aa.UpdatedById,
                          aa.SenderConfigId
                      }).OrderBy(p => p.UpdatedDateTime);
            return Ok(xx);
        }


        // GET: api/LEmailBuckets/5
        [ResponseType(typeof(LEmailBucket))]
        public async Task<IHttpActionResult> GetLEmailBucket(Nullable<int> id)
        {
            var LEmailBucket = db.LEmailBuckets.Where(p => p.Id == id).Select(aa => new {
                aa.Id,
                aa.RecipientList,
                aa.CCList,
                aa.BCCList,
                aa.ReplyToList,
                aa.Subject,
                aa.Body,
                aa.IsHTML,
                aa.EmailType,
                aa.Priority,
                aa.AttachmentList,
                aa.Status,
                aa.Comments,
                aa.CreatedDateTime,
                aa.UpdatedDateTime,
                aa.CreatedById,
                aa.UpdatedById,
                aa.SenderConfigId
            }).FirstOrDefault();
            if (LEmailBucket == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "EMAIL BUCKET")));
            }
            return Ok(LEmailBucket);
        }

        // PUT: api/LEmailBuckets/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLEmailBucket(int id, LEmailBucket LEmailBucket)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "EMAIL BUCKET")));
            }
            
                if (!LEmailBucketExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "EMAIL BUCKET")));
                }

                if (id != LEmailBucket.Id)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "EMAIL BUCKET")));
                }
                try
                {
                    db.Entry(LEmailBucket).State = EntityState.Modified;
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
            return Ok(LEmailBucket);
        }

        // POST: api/LEmailBuckets
        [ResponseType(typeof(LEmailBucket))]
        public async Task<IHttpActionResult> PostLEmailBucket(LEmailBucket LEmailBucket)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "EMAIL BUCKET")));
            }
                try
                {
                    db.LEmailBuckets.Add(LEmailBucket);
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
            return CreatedAtRoute("DefaultApi", new { id = LEmailBucket.Id }, LEmailBucket);
        }

        // DELETE: api/LEmailBuckets/5
        [ResponseType(typeof(LEmailBucket))]
        public async Task<IHttpActionResult> DeleteLEmailBucket(int id)
        {
            LEmailBucket LEmailBucket = await db.LEmailBuckets.FindAsync(id);
            if (LEmailBucket == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "EMAIL BUCKET")));
            }
                try
                {
                    db.LEmailBuckets.Remove(LEmailBucket);
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
            return Ok(LEmailBucket);
        }

        public IHttpActionResult GetEmailBucketSummaryCount()
        {
            string Qry = "select gc.CompanyCode, count(leb.[Status]) as EmailSent FROM LEmailBucket leb "+
                         "INNER JOIN LUsers u on u.Id = leb.CreatedById inner join GCompanies gc on gc.CompanyCode = u.CompanyCode "+
                         "where leb.[Status] = 'sent' and leb.CreatedDateTime>= dateadd(dd, -365, getdate()) GROUP BY gc.CompanyCode,leb.[Status]";
            var xx = db.Database.SqlQuery<LEmailBucketViewModel>(Qry).Count();
            return Ok(xx);
        }


        public IHttpActionResult GetEmailBucketChart()
        {
            string Qry = "select gc.CompanyCode, count(leb.[Status]) as EmailSent FROM LEmailBucket leb " +
                          "INNER JOIN LUsers u on u.Id = leb.CreatedById inner join GCompanies gc on gc.CompanyCode = u.CompanyCode " +
                          "where leb.[Status] = 'sent' and leb.CreatedDateTime>= dateadd(dd, -365, getdate()) GROUP BY gc.CompanyCode,leb.[Status]";
            var xx = db.Database.SqlQuery<LEmailBucketViewModel>(Qry).ToList();
            return Ok(xx);
        }

        public IHttpActionResult GetEmailBucketSummary(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {           
            var SortQuery = "";
            if (!string.IsNullOrEmpty(sortdatafield))
            {
                SortQuery = " order by " + sortdatafield + " " + sortorder;
            }
            else
            {
                SortQuery = " ORDER BY EmailSent desc";
            }
            string Qry = string.Empty;

            if (FilterQuery == null)
            {
                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM(select gc.CompanyCode, count(leb.[Status]) as EmailSent "+
                      "FROM LEmailBucket leb INNER JOIN LUsers u on u.Id = leb.CreatedById inner join GCompanies gc on gc.CompanyCode = u.CompanyCode " +
                      "where leb.[Status] = 'sent' and leb.CreatedDateTime>= dateadd(dd, -365, getdate()) GROUP BY gc.CompanyCode,leb.[Status])A " +
                      ") B WHERE B.datacount > @P1 AND B.datacount <= @P2";
            }
            else
            {
               
                FilterQuery = "WHERE 1=1 " + FilterQuery;
                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM(select gc.CompanyCode, count(leb.[Status]) as EmailSent " +
                     "FROM LEmailBucket leb INNER JOIN LUsers u on u.Id = leb.CreatedById inner join GCompanies gc on gc.CompanyCode = u.CompanyCode " +
                     "where leb.[Status] = 'sent' and leb.CreatedDateTime>= dateadd(dd, -365, getdate()) GROUP BY gc.CompanyCode,leb.[Status])A " +
                     FilterQuery + ") B WHERE B.datacount > @P1 AND B.datacount <= @P2";

            }
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            //parameterList.Add(new SqlParameter("@P3", qq));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<LEmailBucketViewModel>(Qry, parameters).ToList();

            return Ok(xx);
        }

        public IHttpActionResult GetEmailBucketDetail(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            string Qry = string.Empty;
            var SortQuery = "";
            if (!string.IsNullOrEmpty(sortdatafield))
            {
                SortQuery = " order by " + sortdatafield + " " + sortorder;
            }
            else
            {
                SortQuery = " ORDER BY CreatedDateTime desc";
            }

            if (FilterQuery == null)
            {
                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount " +
                      "FROM(select gc.CompanyCode, Body, RecipientList, [Subject], EmailType, leb.CreatedDateTime, leb.CreatedById, leb.[Status], "+
                      "leb.UpdatedDateTime FROM LEmailBucket leb INNER JOIN LUsers u on u.Id = leb.CreatedById inner join GCompanies gc "+
                      "on gc.CompanyCode = u.CompanyCode where leb.[Status] = 'sent' and leb.CreatedDateTime >= dateadd(dd, -365, getdate()))A "+
                      ") B WHERE B.datacount > @P1 AND B.datacount <= @P2";
            }
            else
            {
                FilterQuery = "WHERE 1=1 " + FilterQuery;
               
                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount " +
                     "FROM(select gc.CompanyCode, Body, RecipientList, [Subject], EmailType, leb.CreatedDateTime, leb.CreatedById, leb.[Status], " +
                     "leb.UpdatedDateTime FROM LEmailBucket leb INNER JOIN LUsers u on u.Id = leb.CreatedById inner join GCompanies gc " +
                     "on gc.CompanyCode = u.CompanyCode where leb.[Status] = 'sent' and leb.CreatedDateTime >= dateadd(dd, -365, getdate()))A " +
                     FilterQuery + ") B WHERE B.datacount > @P1 AND B.datacount <= @P2";
            }

            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<LEmailBucketViewModel>(Qry, parameters).ToList();
            return Ok(xx);

        }


        public IHttpActionResult GetEmailBucketDetailCount()
        {
            string Qry = "select gc.CompanyCode,Body,RecipientList, [Subject], EmailType, leb.CreatedDateTime, leb.CreatedById, leb.[Status],leb.UpdatedDateTime "+
                         "FROM LEmailBucket leb INNER JOIN LUsers u on u.Id = leb.CreatedById inner join GCompanies gc on gc.CompanyCode = u.CompanyCode "+
                         "where leb.[Status] = 'sent' and leb.CreatedDateTime>= dateadd(dd, -365, getdate())";
            var xx = db.Database.SqlQuery<LEmailBucketViewModel>(Qry).Count();
            return Ok(xx);
        }


        private bool LEmailBucketExists(int id)
        {
            return db.LEmailBuckets.Count(e => e.Id == id) > 0;
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

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
    public class GKeyValuesController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/GKeyValues
        public IHttpActionResult GetGKeyValues()
        {
            var xx = (from aa in db.GKeyValues
                      select new
                      {
                          aa.Id,
                          aa.Key,
                          aa.Value,
                          aa.Description,
                          aa.CompanyCode
                      }).OrderBy(p => p.Key);
            return Ok(xx);
        }


        // GET: api/GKeyValues/5
        [ResponseType(typeof(GKeyValue))]
        public async Task<IHttpActionResult> GetGKeyValue(Nullable<int> id)
        {
            var GKeyValue = db.GKeyValues.Where(p => p.Id == id).Select(aa => new {
                aa.Id,
                aa.Key,
                aa.Value,
                aa.Description,
                aa.CompanyCode
            }).FirstOrDefault();
            if (GKeyValue == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "KEYVALUE")));
            }
            return Ok(GKeyValue);
        }

        // PUT: api/GKeyValues/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGKeyValue(int id, GKeyValue GKeyValue)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "KEYVALUE")));
            }
            
                if (!GKeyValueExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "KEYVALUE")));
                }

                if (id != GKeyValue.Id)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "KEYVALUE")));
                }
                try
                {
                    db.Entry(GKeyValue).State = EntityState.Modified;
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
            return Ok(GKeyValue);
        }

        // POST: api/GKeyValues
        [ResponseType(typeof(GKeyValue))]
        public async Task<IHttpActionResult> PostGKeyValue(GKeyValue GKeyValue)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "KEYVALUE")));
            }
                try
                {
                    db.GKeyValues.Add(GKeyValue);
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
               
            return CreatedAtRoute("DefaultApi", new { id = GKeyValue.Id }, GKeyValue);
        }

        // DELETE: api/GKeyValues/5
        [ResponseType(typeof(GKeyValue))]
        public async Task<IHttpActionResult> DeleteGKeyValue(int id)
        {
            GKeyValue GKeyValue = await db.GKeyValues.FindAsync(id);
            if (GKeyValue == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "KEYVALUE")));
            }
                try
                {
                    db.GKeyValues.Remove(GKeyValue);
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
                
            return Ok(GKeyValue);
        }

        private bool GKeyValueExists(int id)
        {
            return db.GKeyValues.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
             if (SqEx.Message.IndexOf("UQ_GKeyValues_CompanyCode_Key", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "KEY VALUES"));
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

        //This Function will get the values corresponding to Key from GKeyValues table
        [ResponseType(typeof(GKeyValue))]
        public async Task<IHttpActionResult> GetKeyValue(string Key, string CompanyCode)
        {
            var KeyValue = db.GKeyValues.Where(p => (p.Key == Key && p.CompanyCode == CompanyCode)).Select(aa => new {
                
                aa.Value
                
            }).FirstOrDefault();
            if (KeyValue == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "KEYVALUE")));
            }
            return Ok(KeyValue);

        }

        public IHttpActionResult GetGKeyValuesByName(string KeyName)
        {
            var xx = (from aa in db.GKeyValues.Include(c => c.CompanyCode).Where(p => p.Key == KeyName)
                      select new { aa.Id, aa.Key, aa.Value, aa.CompanyCode }).FirstOrDefault();
           
            return Ok(xx);
        }

        /// Method to get the Key Values from the GKeyValues for L2Admin Page
        [ResponseType(typeof(GKeyValue))]
        public IHttpActionResult GetGKeyValueForConfiguration(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            var SortQuery = "";
            if (!string.IsNullOrEmpty(sortdatafield))
            {
                SortQuery = " order by " + "["  + sortdatafield + "]" + " " + sortorder;
            }
            else
            {
                SortQuery = " ORDER BY [Key] desc";
            }
            string Qry = string.Empty;
            if (FilterQuery == null)
            {
                Qry = "SELECT * FROM(SELECT *,ROW_NUMBER()  OVER (" + SortQuery + ") as  datacount FROM (SELECT g.Id, g.CompanyCode,[Key],[Value],[Description] FROM GKeyValues g " +
                      "INNER JOIN GCompanies gc on gc.CompanyCode = g.CompanyCode)A)B WHERE B.datacount > @P1 AND B.datacount <= @P2 ";
            }
            else
            {
                FilterQuery = "WHERE 1=1 " + FilterQuery;
               
                Qry = "SELECT * FROM(SELECT *,ROW_NUMBER()  OVER (" + SortQuery + ")  as datacount FROM (SELECT g.Id, g.CompanyCode,[Key],[Value],[Description] FROM GKeyValues g " +
                     "INNER JOIN GCompanies gc on gc.CompanyCode = g.CompanyCode " + FilterQuery + " )A)B WHERE B.datacount > @P1 AND B.datacount <= @P2 ";
            }
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<GKeyValue>(Qry, parameters).ToList();
            return Ok(xx);
        }


        /// Method to get the counts for Key Values from the GKeyValues for L2Admin Page       
        /// <returns></returns>
        public IHttpActionResult GetGKeyValueCountForConfiguration()
        {
            string Qry = "SELECT g.Id, g.CompanyCode,[Key],[Value],[Description] FROM GKeyValues g " +
                         "INNER JOIN GCompanies gc on gc.CompanyCode = g.CompanyCode ";
            var xx = db.Database.SqlQuery<GKeyValue>(Qry).Count();
            return Ok(xx);
        }

    }
}

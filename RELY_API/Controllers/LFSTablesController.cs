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
using System.Data;
using System.Data.Entity.Core.Objects;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LFSTablesController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/LFSTables
        public IHttpActionResult GetLFSTablesByCompanyCode(string CompanyCode,string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSTables
                      select new { aa.Id, aa.CompanyCode, aa.NoOfCols, aa.NoOfRows, aa.TableCode,aa.TableTitle }).Where(a=>a.CompanyCode == CompanyCode).OrderBy(p => p.TableCode).ToList();
            return Ok(xx);
        }

        [ResponseType(typeof(LFSTable))]
        public IHttpActionResult GetLFSTablesByTableCode(string TableCode, string CompanyCode, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSTables
                      select new { aa.Id, aa.CompanyCode, aa.NoOfCols, aa.NoOfRows, aa.TableCode, aa.TableTitle }).
                      Where(a => a.CompanyCode == CompanyCode).Where(a => a.TableCode == TableCode).OrderBy(p => p.TableCode).FirstOrDefault();
            return Ok(xx);
        }
        public IHttpActionResult GetSurveyTableLeftGrid(string CompanyCode, string TableCode, int EntityId, string EntityType, string UserName, string WorkFlow)
        {
            string strQuery = "exec dbo.[SpGetSurveyTableLeftGrid] @CompanyCode,@TableCode,@EntityId,@EntityType";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@TableCode", TableCode);
            cmd.Parameters.AddWithValue("@EntityId", EntityId);
            cmd.Parameters.AddWithValue("@EntityType", EntityType);
            var dt = new DataTable();
            dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }




        // PUT: api/LFSTables/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLFSTable(int id, LFSTable LFSTable, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TABLE")));
            }
            if (!LFSTableExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "TABLE")));
            }

            if (id != LFSTable.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TABLE")));
            }
            try
            {
                db.Entry(LFSTable).State = EntityState.Modified;
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
            return Ok(LFSTable);
        }

        // POST: api/LFSTables
        [ResponseType(typeof(LFSTable))]
        public async Task<IHttpActionResult> PostLFSTable(LFSTable LFSTable, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "TABLE")));
            }
            try
            {
                db.LFSTables.Add(LFSTable);
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

            return CreatedAtRoute("DefaultApi", new { id = LFSTable.Id }, LFSTable);
        }

        private bool LFSTableExists(int id)
        {
            return db.LFSTables.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("UQ_LFSTables_CompanyCode_TableCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "TABLES"));
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

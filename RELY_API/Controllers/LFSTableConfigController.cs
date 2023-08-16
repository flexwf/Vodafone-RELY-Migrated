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

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LFSTableConfigController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/LFSTableConfig
        public IHttpActionResult GetLFSTableConfigByCompanyCode(string CompanyCode, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSTableConfigs
                      select new { aa.Id, aa.CompanyCode, aa.Col,aa.Row,  aa.TableCode,aa.ItemTypeId}).Where(a => a.CompanyCode == CompanyCode).OrderBy(p => p.TableCode);
            return Ok(xx);
        }

        public IHttpActionResult GetLFSTableConfigByTableCode(string TableCode, string CompanyCode, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSTableConfigs
                      join qb in db.LFSQuestionBanks on aa.QuestionCode equals qb.QuestionCode
                      join it in db.LFSItemTypes on qb.ItemTypeId equals it.Id
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.Col,
                          aa.Row,
                          aa.ItemText,
                          aa.TableCode,
                          aa.ItemTypeId,
                          qb.QuestionName,
                          qb.QuestionCode,
                          qb.QuestionText,
                          ItemName = it.Name
                      }).
                      Where(a => a.CompanyCode == CompanyCode).Where(a => a.TableCode == TableCode).OrderBy(p => p.TableCode).ToList();
            return Ok(xx);
        }

        public IHttpActionResult GetLFSTableConfigByColRow(int Col,int Row,string TableCode, string CompanyCode, string UserName, string WorkFlow)
        {
            string qry = "";
            qry = "Select aa.Id,aa.CompanyCode,aa.Col,aa.Row,IsNull(aa.ItemText, QuestionText) As QuestionText, aa.QuestionCode,aa.TableCode,aa.ItemTypeId,"
                 + " qb.QuestionName,ItemTypeName = it.Name,aa.ShowResponseFromQuestionCode From LFSTableConfig aa left outer join LFSItemTypes it on it.Id = aa.ItemTypeId left outer join LFSQuestionBank qb on aa.QuestionCode = qb.QuestionCode "
                 + " Where aa.CompanyCode = {0} and aa.TableCode = {1} and aa.Col = {2} and aa.Row = {3}";
            var  xx = db.Database.SqlQuery<LFSTableConfigViewModel>(qry,CompanyCode,TableCode,Col,Row).FirstOrDefault();

            //var xx = (from aa in db.LFSTableConfigs
            //          join qb in db.LFSQuestionBanks on aa.QuestionCode equals qb.QuestionCode
            //          join it in db.LFSItemTypes on aa.ItemTypeId equals it.Id
            //          select new {
            //              aa.Id,
            //              aa.CompanyCode,
            //              aa.Col,
            //              aa.Row,
            //              QuestionText = string.IsNullOrEmpty(aa.ItemText)? qb.QuestionText: aa.ItemText,
            //              aa.QuestionCode,
            //              aa.IsTotal,
            //              aa.TableCode,
            //              aa.ItemTypeId,
            //              aa.ShowResponseFromCol,
            //              aa.ShowResponseFromRow,
            //              qb.QuestionName,
            //              //qb.QuestionText,
            //              ItemTypeName = it.Name
            //          }).
            //          Where(a => a.CompanyCode == CompanyCode).Where(a => a.TableCode == TableCode).Where(a => a.Col == Col).Where(a =>a.Row ==Row)
            //          .OrderBy(a => a.TableCode).FirstOrDefault();
            return Ok(xx);
        }

        // PUT: api/LFSTableConfig/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLFSTableConfigs(int id, LFSTableConfig LFSTableConfig, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TABLE CONFIG")));
            }
            if (!LFSTableExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "TABLE CONFIG")));
            }

            if (id != LFSTableConfig.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TABLE CONFIG")));
            }
            try
            {
                db.Entry(LFSTableConfig).State = EntityState.Modified;
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
            return Ok(LFSTableConfig);
        }

        // POST: api/LFSTableConfig
        [ResponseType(typeof(LFSTableConfig))]
        public async Task<IHttpActionResult> PostLFSTableConfig(LFSTableConfig LFSTableConfig, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "TABLE CONFIG")));
            }
            try
            {
                db.LFSTableConfigs.Add(LFSTableConfig);
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

            return CreatedAtRoute("DefaultApi", new { id = LFSTableConfig.Id }, LFSTableConfig);
        }

        private bool LFSTableExists(int id)
        {
            return db.LFSTableConfigs.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("UQ_LFSTableConfig_CompanyCode_TableCode_Axis_Position", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "TABLES"));
            else if (SqEx.Message.IndexOf("FK_LFSTableConfig_LFSTableResponses_TableConfigId_Col", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "TABLE CONFIG","TABLE RESPONSES"));//[FK_LFSTableConfig_LFSTableResponses_TableConfigId_Row]
            else if (SqEx.Message.IndexOf("FK_LFSTableConfig_LFSTableResponses_TableConfigId_Row", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "TABLE CONFIG","TABLE RESPONSES"));
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

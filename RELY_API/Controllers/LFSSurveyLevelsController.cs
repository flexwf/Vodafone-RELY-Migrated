using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LFSSurveyLevelsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/LFSSurveyLevels? CompanyCode = DE
        //Method to Get Data in Grid

        public IHttpActionResult GetLFSSurveyLevelsByCompanyCode(string CompanyCode, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSSurveyLevels.Where(p => p.CompanyCode == CompanyCode)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.Name,
                          aa.Description,
                          aa.CreatedById,
                          aa.UpdatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedDateTime
                      }).OrderBy(p => p.Name);
            return Ok(xx);
        }

        // POST: api/LFSSurveyLevels
        //Method to Post Data from app to db
        [ResponseType(typeof(LFSSurveyLevel))]
        public async Task<IHttpActionResult> PostLFSSurveyLevels(LFSSurveyLevel LFSSurveyLevel, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "SurveyLevel")));
            }

            try
            {
                if(db.LFSSurveyLevels.Where(p=>p.Id==LFSSurveyLevel.Id).Where(p=>p.CompanyCode==LFSSurveyLevel.CompanyCode).Count() > 0)
                {
                    db.Entry(LFSSurveyLevel).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    LFSSurveyLevel.Id = 0;
                    db.LFSSurveyLevels.Add(LFSSurveyLevel);
                    await db.SaveChangesAsync();
                }
            }
            catch(DbEntityValidationException dbex)
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

            return CreatedAtRoute("DefaultApi", new { id = LFSSurveyLevel.Id }, LFSSurveyLevel);
        }

        // GET: api/LFSSurveyLevels?CompanyCode=DE&Name=System1
        //Method to Get Requested Data by user for Edit
        [ResponseType(typeof(LFSSurveyLevel))]
        public async Task<IHttpActionResult> GetLFSSurveyLevels(string CompanyCode, string Name, string UserName, string WorkFlow)
        {
            var LFSSurveyLevel = db.LFSSurveyLevels.Where(p => p.CompanyCode == CompanyCode && p.Name == Name).Select(x => 
                                new
                                {
                                    x.Id,
                                    x.CompanyCode,
                                    x.Name,
                                    x.Description,
                                    x.CreatedById,
                                    x.UpdatedById,
                                    x.CreatedDateTime,
                                    x.UpdatedDateTime
                                 }).First();

            if(LFSSurveyLevel==null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SurveyLevel")));
            }

            return Ok(LFSSurveyLevel);
        }

        // PUT: api/LFSSurveyLevels?CompanyCode=DE&Name=System(updated name)
        //Method to update Requested Data by User in db
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLFSSurveyLevels(string CompanyCode,string Name,LFSSurveyLevel LFSSurveyLevel)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "SURVEYLEVEL")));
            }

            if(!LFSSurveyLevelExists(CompanyCode) && !LFSSurveyLevelExists(Name))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SURVEYLEVEL")));
            }

            if(CompanyCode != LFSSurveyLevel.CompanyCode && Name!=LFSSurveyLevel.Name)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "SURVEYLEVEL")));
            }

            try
            {
                db.Entry(LFSSurveyLevel).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch(DbEntityValidationException dbex)
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

            return Ok(LFSSurveyLevel);
        }

        // Delete: api/LFSSurveyLevels?id
        [ResponseType(typeof(LFSSurveyLevel))]
        public async Task<IHttpActionResult> DeleteLFSSurveyLevels(int id, string UserName, string WorkFlow)
        {
            LFSSurveyLevel LFSSurveyLevel = await db.LFSSurveyLevels.FindAsync(id);
            if(LFSSurveyLevel==null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Survey Level")));
            }

            try
            {
                db.LFSSurveyLevels.Remove(LFSSurveyLevel);
                await db.SaveChangesAsync();
            }
            catch(Exception ex)
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

            return Ok(LFSSurveyLevel);
        }
        private bool LFSSurveyLevelExists(string CompanyCode)
        {
            return db.LFSSurveyLevels.Count(e => e.CompanyCode == CompanyCode) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_LFSSurveyLevels_LFinancialSurvays_SurveyLevelId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "SURVEY LEVEL", "FINANCIAL SURVAY"));
            else if (SqEx.Message.IndexOf("FK_LFSSurveyLevels_LRequests_SurveyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "SURVEY LEVEL", "REQUESTS"));
            else if (SqEx.Message.IndexOf("FK_LFSSurveyLevels_MLFSQuestionBankLFSSurveyLevels_SurveyLevelId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "SURVEY LEVEL", "QUESTION BANK"));
            else if
                (SqEx.Message.IndexOf("UQ_LFSSurveyLevels_CompanyCode_Name", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "SURVEY LEVEL"));
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

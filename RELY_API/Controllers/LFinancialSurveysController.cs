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
    public class LFinancialSurveysController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        //GET: api/LFinancialSurvey/DE
        //Method to get SurveyName for DropDown 
        public IHttpActionResult GetSurveyforDropDown(string CompanyCode, string UserName, string WorkFlow)
        {
            var Survey = (from aa in db.LFinancialSurveys
                           where (aa.CompanyCode == CompanyCode && aa.IsActive==true)
                           select new
                           {
                               aa.Id,
                               aa.SurveyName
                           }).OrderBy(aa => aa.SurveyName);
            return Ok(Survey);
        }

        public IHttpActionResult GetByCompanyCode(string CompanyCode, string UserName, string WorkFlow)
        {//get all the financial surveys which are active
            var xx = (from aa in db.LFinancialSurveys
                      join bb in db.LFSSurveyLevels on aa.SurveyLevelId equals bb.Id
                      where (aa.CompanyCode == CompanyCode )
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.SurveyLevelId,
                          bb.Name,
                          aa.SurveyName,
                          aa.Description,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime,
                          aa.IsActive
                      }).Where(a=>a.IsActive == true).OrderByDescending(p => p.Id);
            return Ok(xx);
        }

        [HttpGet]
        public IHttpActionResult GetById(int id)
        {
            var xx = (from aa in db.LFinancialSurveys.Where(p => p.Id == id)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.SurveyLevelId,                          
                          aa.SurveyName,
                          aa.Description,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime,
                          aa.IsActive

                      }).FirstOrDefault();
            if (xx == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LFinancialSurveys")));
            }
            return Ok(xx);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(LFinancialSurvey lfinansurvey, int id)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "LFinancialSurveys")));
            }
            if (!LFinancialSurveyExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LFinancialSurveys")));
            }

            if (id != lfinansurvey.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "LFinancialSurveys")));
            }
            try
            {
                db.Entry(lfinansurvey).State = EntityState.Modified;
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
            return Ok(lfinansurvey);
        }

        private bool LFinancialSurveyExists(int id)
        {
            return db.LFinancialSurveys.Count(e => e.Id == id) > 0;
        }

        [HttpPost]
        [ResponseType(typeof(LFinancialSurvey))]
        public async Task<IHttpActionResult> Post(LFinancialSurvey lfinansurvey, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "LFinancialSurvey")));
            }

            try
            {
                if (db.LFinancialSurveys.Where(p => p.Id == lfinansurvey.Id).Where(p => p.CompanyCode == lfinansurvey.CompanyCode).Count() > 0)
                {
                    db.Entry(lfinansurvey).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    lfinansurvey.Id = 0;
                    db.LFinancialSurveys.Add(lfinansurvey);
                    await db.SaveChangesAsync();
                }
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
            return CreatedAtRoute("DefaultApi", new { Id = lfinansurvey.Id }, lfinansurvey);
        }

        [ResponseType(typeof(LFinancialSurvey))]
        public IHttpActionResult GetSurveyIdbyName(string SurveyName)
        {

            var xx = db.LFinancialSurveys.Where(p => p.SurveyName == SurveyName).Select(a => a.Id).FirstOrDefault();
            return Ok(xx);
        }       
        // DELETE: api/LFinancialSurvey/
        [ResponseType(typeof(LFinancialSurvey))]
        public async Task<IHttpActionResult> Delete(int id, string UserName, string WorkFlow)
        {
            LFinancialSurvey financialSurvey = await db.LFinancialSurveys.FindAsync(id);
            if(financialSurvey==null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LFinancialSurvey")));
            }

            try
            {
                db.LFinancialSurveys.Remove(financialSurvey);
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
            return Ok(financialSurvey);
        }
       
        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("FK_LFSAnswerBank_LFSNextSteps_AnswerId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ANSWER BANK", "NEXT STEPS"));
            //else if (SqEx.Message.IndexOf("FK_LFSAnswerBank_MLFSAnswerBookLAccountingScenarios_AnswerId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ANSWER BANK", "ACCOUNTING SCENARIO"));
            //else if (SqEx.Message.IndexOf("FK_LFSAnswerBank_MLFSAnswerBankLFSSurveyLevels_AnswerId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ANSWER BANK", "SURVEY LEVELS"));
            //else if
            if (SqEx.Message.IndexOf("FK_LFinancialSurveys_LFSChapters_SurveyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "FINANCIAL SURVEYS", "CHAPTERS"));
            else if (SqEx.Message.IndexOf("FK_LFinancialSurveys_LFSSectionItems_SurveyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "FINANCIAL SURVEYS", "SECTIONITEMS"));
            else if (SqEx.Message.IndexOf("FK_LFinancialSuveys_LFSSections_SurveyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "FINANCIAL SURVEYS", "SECTIONS"));
            else if (SqEx.Message.IndexOf("FK_LFinancialSurveys_LRequests_SurveyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "FINANCIAL SURVEYS", "REQUESTS"));
            else if (SqEx.Message.IndexOf("FK_LFinancialSurveys_LProducts_SurveyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "FINANCIAL SURVEYS", "PRODUCTS"));
            else if
                (SqEx.Message.IndexOf("UQ_LFinancialSurveys_CompanyCode_SurveyName", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "FINANCIAL SURVEY"));
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


        //------------------Below code is commented by Rakhi Singh on 18/09/18-------------------------------------------------------
      
        //[HttpPost]
        //[ResponseType(typeof(LFinancialSurvey))]
        //public async Task<IHttpActionResult> POSTLFinancialSurvey(LFinancialSurvey LFinancialSurvey, string UserName, string WorkFlow)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "LFinancialSurvey")));
        //    }

        //    try
        //    {
        //        if (db.LFinancialSurveys.Where(p => p.Id == LFinancialSurvey.Id).Where(p => p.CompanyCode == LFinancialSurvey.CompanyCode).Count() > 0)
        //        {
        //            db.Entry(LFinancialSurvey).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            LFinancialSurvey.Id = 0;
        //            db.LFinancialSurveys.Add(LFinancialSurvey);
        //            await db.SaveChangesAsync();
        //        }
        //    }
        //    catch (DbEntityValidationException dbex)
        //    {
        //        var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
        //        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
        //        {
        //            //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
        //            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
        //        }
        //        else
        //        {
        //            throw ex;//This exception will be handled in FilterConfig's CustomHandler
        //        }
        //    }
        //    //using (var transaction = db.Database.BeginTransaction())
        //    //{
        //    //    try
        //    //    {
        //    //        int LoggedInUserId = 0;
        //    //        foreach (var model in LFinancialSurvey)
        //    //        {
        //    //            if (model.Id == 0)//add only when new record is there to insert.
        //    //                db.LFinancialSurveys.Add(model);
        //    //            LoggedInUserId = model.UpdatedById;
        //    //            await db.SaveChangesAsync();
        //    //        }

        //    //    }
        //    //    catch (DbEntityValidationException dbex)
        //    //    {
        //    //        transaction.Rollback();
        //    //        var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
        //    //        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        transaction.Rollback();
        //    //        if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
        //    //        {
        //    //            //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
        //    //            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
        //    //        }
        //    //        else
        //    //        {
        //    //            throw ex;//This exception will be handled in FilterConfig's CustomHandler
        //    //        }
        //    //    }

        //    //    transaction.Commit();
        //    //}

        //    return CreatedAtRoute("DefaultApi", new { Id = LFinancialSurvey.Id }, LFinancialSurvey);
        //}





    }
}

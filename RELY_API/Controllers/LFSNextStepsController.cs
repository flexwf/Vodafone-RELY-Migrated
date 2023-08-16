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
    public class LFSNextStepsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        [HttpGet]
        public IHttpActionResult GetLFSNextSteps()
        {
            var xx = (from aa in db.LFSNextSteps
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.QuestionCode,
                          aa.AnswerOption,
                          aa.NextStepText,
                          aa.InternalNotes,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime

                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }

        // GET: api/LFSNextSteps? CompanyCode = DE
        //Method to Get Data in Grid 

        public IHttpActionResult GetLFSNextStepsByCompanyCode(string CompanyCode, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSNextSteps.Where(p => p.CompanyCode == CompanyCode)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.QuestionCode,
                          aa.AnswerOption,
                          aa.NextStepText,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime
                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }
            // GET: api/LFSNextSteps? Id
            //Method to Get Data in Grid
        public IHttpActionResult GetLFSNextStepsById(int Id, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSNextSteps.Where(p => p.Id == Id)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.QuestionCode,
                          aa.AnswerOption,
                          aa.NextStepText,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime
                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }
        public IHttpActionResult GetNextStepActionsByEntityIdType(int EntityId, string EntityType,string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSNextStepActions
                      join bb in db.LFSResponses on aa.ResponseId equals bb.Id
                      join cc in db.LFSNextSteps on aa.NextStepId equals cc.Id
                      join dd in db.LFSQuestionBanks on bb.QuestionCode equals dd.QuestionCode
                      where bb.EntityId == EntityId && bb.EntityType == EntityType
                      select new
                      {
                          aa.Id,
                          aa.NextStepId,
                          aa.ResponseId,
                          aa.IsDone,
                          aa.ActionTaken,
                          bb.QuestionCode,
                          dd.QuestionName,
                          dd.QuestionText,
                          bb.Response,
                          cc.NextStepText,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime
                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }
        // GET: api/LFSNextSteps? AnswerId
        //Method to Get Data in Grid
        //public IHttpActionResult GetLFSNextStepsByAnswerId(int AnswerId,string UserName,string WorkFlow)
        //{
        //    var xx = (from aa in db.LFSNextSteps.Where(p => p.AnswerId == AnswerId)
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.AnswerId,
        //                  aa.NextStepText
        //              }).First();
        //    return Ok(xx);
        //}

        [HttpPost]
        [ResponseType(typeof(LFSNextStep))]
        public async Task<IHttpActionResult> POSTLFSNextStep(List<LFSNextStep> LFSNextStep, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "LFSNextSteps")));
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int LoggedInUserId = 0;
                    foreach (var model in LFSNextStep)
                    {
                        if (model.Id == 0)//add only when new record is there to insert.
                            db.LFSNextSteps.Add(model);
                        LoggedInUserId = model.UpdatedById;
                        await db.SaveChangesAsync();
                    }

                }
                catch (DbEntityValidationException dbex)
                {
                    transaction.Rollback();
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
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

                transaction.Commit();
            }

            return Ok();
        }

        // POST: api/LFSNextSteps
        //Method to Post Data from app to db
        [ResponseType(typeof(LFSNextStep))]
        public async Task<IHttpActionResult> PostLFSNextSteps(LFSNextStep LFSNextStep, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "NextStep")));
            }

            try
            {
                if(db.LFSNextSteps.Where(p=>p.Id==LFSNextStep.Id).Count()>0)
                {
                    db.Entry(LFSNextStep).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    LFSNextStep.Id = 0;//To override the Id generated by grid
                    db.LFSNextSteps.Add(LFSNextStep);
                    await db.SaveChangesAsync();
                }
            }
            catch(DbEntityValidationException dbex)
            {
                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
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
                    throw ex;//This exception will be handled in FilterConfig's CustomHandler
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = LFSNextStep.Id }, LFSNextStep);
        }

        // PUT: api/LFSNextSteps?Id
        //Method to update Requested Data by User in db
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLFSNextSteps(int Id,LFSNextStep LFSNextStep, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "NEXTSTEP")));
            }

            if(!LFSNextStepExists(Id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "NEXTSTEP")));
            }

            if(Id!=LFSNextStep.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "NEXTSTEP")));
            }

            try
            {
                db.Entry(LFSNextStep).State = EntityState.Modified;
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
            return Ok(LFSNextStep);
        }

        // Delete: api/LFSNextStep?id
        [ResponseType(typeof(LFSAnswerBank))]
        public async Task<IHttpActionResult> DeleteLFSNextStep(int id, string UserName, string WorkFlow)
        {
            LFSNextStep LFSNextStep = await db.LFSNextSteps.FindAsync(id);
            if (LFSNextStep == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Next Step")));
            }

            try
            {
                db.LFSNextSteps.Remove(LFSNextStep);
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
            return Ok(LFSNextStep);
        }

        private bool LFSNextStepExists(int Id)
        {
            return db.LFSNextSteps.Count(e => e.Id == Id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_LFSNextSteps_LNextStepActions_NextStepId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "NEXT STEPS", "NEXT STEP ACTIONS"));
            //else if
            //    (SqEx.Message.IndexOf("UQ_LFSAnswerBank_QuestionId_AnswerOption", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "ANSWER BANK"));
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
            //return null;
        }

    }
}

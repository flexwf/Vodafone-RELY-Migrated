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
    public class LFSSectionsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/LFSSections? Id
        //Method to Get Data in Grid

        public IHttpActionResult GetLFSSectionsById(int Id,string UserName,string WorkFlow)
        {
            var xx = (from aa in db.LFSSections.Where(p => p.Id == Id)
                      select new
                      {
                          aa.Id,
                          aa.SurveyId,
                          aa.ChapterCode,
                          aa.SectionName,
                          aa.SectionCode,
                          aa.Ordinal,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime
                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }

        public IHttpActionResult GetLFSSections()
        {
            var xx = (from aa in db.LFSSections
                      join bb in db.LFinancialSurveys on aa.SurveyId equals bb.Id
                      select new
                      {
                          aa.Id,
                          aa.SurveyId,
                          aa.ChapterCode,
                          aa.SectionName,
                          aa.SectionCode,
                          aa.Ordinal,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime,
                          aa.InternalNotes,
                          bb.SurveyName
                      }).OrderByDescending(p => p.Id);
            return Ok(xx);
        }


        // POST: api/LFSSections
        //Method to Post Data from app to db
        [ResponseType(typeof(LFSSection))]
        public async Task<IHttpActionResult> PostLFSSections(LFSSection LFSSection,string UserName,string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Section")));
            }

            try
            {
                if(db.LFSSections.Where(p=>p.Id==LFSSection.Id).Count()>0)
                {
                    db.Entry(LFSSection).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    LFSSection.Id = 0;
                    db.LFSSections.Add(LFSSection);
                    await db.SaveChangesAsync();
                }
            }
            catch(DbEntityValidationException dbex)
            {
                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));
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
            return CreatedAtRoute("DefaultApi", new { id = LFSSection.Id }, LFSSection);
        }


        [HttpPost]
        [ResponseType(typeof(LFSSection))]
        public async Task<IHttpActionResult> PostLFSSection(List<LFSSection> LFSSection, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "LFSSections")));
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int LoggedInUserId = 0;
                    foreach (var model in LFSSection)
                    {
                        if (model.Id == 0)//add only when new record is there to insert.
                            db.LFSSections.Add(model);
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

        // PUT: api/LFSSection?Id
        //Method to update Requested Data by User in db
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PUTLFSSections(int Id,LFSSection LFSSection,string UserName,string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "SECTION")));
            }
            if(!LFSSectionExists(Id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SECTION")));
            }
            if (Id != LFSSection.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "SECTION")));
            }
            try
            {
                db.Entry(LFSSection).State = EntityState.Modified;
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

            return Ok(LFSSection);
        }

        // Delete: api/LFSSection?id
        [ResponseType(typeof(LFSSection))]
        public async Task<IHttpActionResult> DeleteLFSSection(int id, string UserName, string WorkFlow)
        {
            LFSSection LFSSection = await db.LFSSections.FindAsync(id);
            if (LFSSection == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Section")));
            }

            try
            {
                db.LFSSections.Remove(LFSSection);
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

            return Ok(LFSSection);
        }
        private bool LFSSectionExists(int Id)
        {
            return db.LFSSections.Count(e => e.Id == Id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("FK_LFSSections_LFSSectionItems_SectionId", StringComparison.OrdinalIgnoreCase) >= 0)
                //return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "CHAPTER", "SECTION ITEM"));
            //else
            if
                (SqEx.Message.IndexOf("UQ_LFSSections_SurveyId_ChapterCode_SectionCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "CHAPTER"));
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

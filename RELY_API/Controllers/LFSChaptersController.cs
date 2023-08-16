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
    public class LFSChaptersController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        public IHttpActionResult GetLFSChapters()
        {
            var xx = (from aa in db.LFSChapters
                      join yy in db.LFinancialSurveys on aa.SurveyId equals yy.Id
                      select new
                      {
                          aa.Id,
                          aa.ChapterCode,
                          aa.SurveyId,
                          aa.Name,
                          aa.Ordinal,
                          aa.InternalNotes,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime,
                          yy.SurveyName
                         
                      }).OrderByDescending(p => p.Id);
            return Ok(xx);
        }


        // GET: api/GetLFSChapters? Id
        //Method to Get Data in Grid
        public IHttpActionResult GetLFSChaptersById(int Id, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSChapters.Where(p => p.Id == Id)
                      select new
                      {
                          aa.Id,
                          aa.ChapterCode,
                          aa.SurveyId,
                          aa.Name,
                          aa.Ordinal,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime
                      }).OrderBy(p=>p.Id);
            return Ok(xx);
        }

        [HttpPost]
        [ResponseType(typeof(LFSChapter))]
        public async Task<IHttpActionResult> POSTLFSChapter(List<LFSChapter> LFSChapter, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "LFSChapters")));
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int LoggedInUserId = 0;
                    foreach (var model in LFSChapter)
                    {
                        if (model.Id == 0)//add only when new record is there to insert.
                            db.LFSChapters.Add(model);
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


        // POST: api/LFSChapters
        //Method to Post Data from app to db
        [ResponseType(typeof(LFSChapter))]
        public async Task<IHttpActionResult> PostLFSChapters(LFSChapter LFSChapter, string UserName, string WorkFlow)
        {
            if(!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Chapter")));
            }

            try
            {
                if (db.LFSChapters.Where(p => p.Id == LFSChapter.Id).Count() > 0)
                {
                    db.Entry(LFSChapter).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    LFSChapter.Id = 0;
                    db.LFSChapters.Add(LFSChapter);
                    await db.SaveChangesAsync();
                }
            }
            catch(DbEntityValidationException dbex)
            {
                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));
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

            return CreatedAtRoute("DefaultApi", new { id = LFSChapter.Id }, LFSChapter);

        }

        // PUT: api/LFSChapter?Id
        //Method to update Requested Data by User in db
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PUTLFSChapters(int Id,LFSChapter LFSChapter, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "CHAPTER")));
            }
            if(!LFSChapterExists(Id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "CHAPTER")));
            }
            if(Id!=LFSChapter.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "CHAPTER")));
            }
            try
            {
                db.Entry(LFSChapter).State = EntityState.Modified;
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

            return Ok(LFSChapter);

        }

        // Delete: api/LFSChapter?id
        [ResponseType(typeof(LFSChapter))]
        public async Task<IHttpActionResult> DeleteLFSChapters(int id, string UserName, string WorkFlow)
        {
            LFSChapter LFSChapter = await db.LFSChapters.FindAsync(id);
            if (LFSChapter == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Chapter")));
            }

            try
            {
                db.LFSChapters.Remove(LFSChapter);
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

            return Ok(LFSChapter);
        }
        private bool LFSChapterExists(int Id)
        {
            return db.LFSChapters.Count(e => e.Id == Id) > 0;
        }
        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("FK_LFSChapters_LFSSections_ChapterId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Chapter", "Section"));
            //else if
            //    (SqEx.Message.IndexOf("UQ_LFSChapters_Name_SurveyId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "Chapter"));
            //else
            //{
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
            //}
        }
    }
}

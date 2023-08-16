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

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LFSTableResponsesController : ApiController
    {

        private RELYEntities db = new RELYEntities();

        // GET: api/LFSTableResponses
        public IHttpActionResult GetLFSTableResponsesByTableCode(string TableCode, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSTableResponses
                      select new { aa.Id, aa.EntityId, aa.EntityType, aa.Response, aa.TableCode, aa.Col, aa.Row }).Where(a => a.TableCode == TableCode);
            return Ok(xx);
        }


        public IHttpActionResult GetLFSTableResponsesByEntityIdType(int EntityId, string EntityType, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSTableResponses
                      select new { aa.Id, aa.EntityId, aa.EntityType, aa.Response, aa.TableCode, aa.Col, aa.Row }).
                      Where(a => a.EntityId == EntityId).Where(a => a.EntityType == EntityType).OrderBy(p => p.TableCode);
            return Ok(xx);
        }

        // PUT: api/LFSTableResponses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLFSTable(int id, LFSTableRespons LFSTableRespons, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TABLE RESPONSE")));
            }
            if (!LFSTableResponseExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "TABLE")));
            }

            if (id != LFSTableRespons.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TABLE RESPONSE")));
            }
            try
            {
                db.Entry(LFSTableRespons).State = EntityState.Modified;
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
            return Ok(LFSTableRespons);
        }

        // POST: api/LFSTableResponses
        [ResponseType(typeof(LFSTableRespons))]
        public async Task<IHttpActionResult> PostLFSTableResponse(LFSTableRespons LFSTableRespons, string CompanyCode,int LoggedInUserId ,string UserName, string WorkFlow)
        //public async Task<IHttpActionResult> PostLFSTableResponse( string CompanyCode, int LoggedInUserId, string UserName, string WorkFlow)
        {
            using (var transaction = db.Database.BeginTransaction())
            {

                try
                {

                    var qry = "Select count(*)  From LFSTableResponses Where EntityId={0} And EntityType = 'LProducts' And TableCode={1} and Row = {2} and Col = {3}";
                    var count = db.Database.SqlQuery<int>(qry, LFSTableRespons.EntityId, LFSTableRespons.TableCode,LFSTableRespons.Row,LFSTableRespons.Col).FirstOrDefault();
                    if (count == 0)
                    {
                        db.LFSTableResponses.Add(LFSTableRespons);
                        await db.SaveChangesAsync();
                    }
                    else//responded already
                    {
                        var result = db.LFSTableResponses.Where(p => p.TableCode == LFSTableRespons.TableCode).Where(p => p.EntityId == LFSTableRespons.EntityId).Where(p => p.EntityType == "LProducts").Where(p=>p.Col== LFSTableRespons.Col).Where(p=>p.Row==LFSTableRespons.Row).FirstOrDefault();
                        string existingResponse = result.Response;
                        //Check if the existing Answer is same as provided answer
                        if (existingResponse.Trim() != LFSTableRespons.Response.Trim())
                        {
                            //Remove existing NextSteps and update Responses
                            //Delete from LFSNextStepActions where ReponseId = (select id from LFSReposnses where EntityId, ENtityType, QuestionCode match )
                            var NextStepActionsList = db.Database.SqlQuery<LFSNextStepAction>("Select *  from LFSNextStepActions where ResponseId in(select Id from LFSTableResponses where EntityId={0} and EntityType={1} and TableCode={2} and Row={3} and Col={4}) "
                                , LFSTableRespons.EntityId, "LProducts", LFSTableRespons.TableCode,LFSTableRespons.Row,LFSTableRespons.Col).ToList();
                            // db.LFSNextStepActions.RemoveRange(NextStepActionsList);
                            foreach (var NSAction in NextStepActionsList)
                            {
                                db.Entry(NSAction).State = EntityState.Deleted;
                                await db.SaveChangesAsync();
                            }


                            //Update from LFSTableResponses where EntityId, EntityType, QuestionCode match
                            result.Response = LFSTableRespons.Response;
                            db.Entry(result).State = EntityState.Modified;//update existing one
                            await db.SaveChangesAsync();

                            //find out NextSteps for Question,Answer,CompanyCode
                            var NextSteps = db.LFSNextSteps.Where(a => a.QuestionCode == LFSTableRespons.TableCode).Where(a => a.AnswerOption.Trim() == LFSTableRespons.Response.Trim()).Where(a => a.CompanyCode == CompanyCode).ToList();
                            foreach (var NS in NextSteps)
                            {
                                // Insert Into LFSNextStepsActions(Next Steps form LFSNextSteps where CompanyCode, QuestionCode, AnswerOption match)-- Note there could be multiple next steps
                                LFSNextStepAction NextStepAction = new LFSNextStepAction
                                {
                                    ResponseId = result.Id,
                                    NextStepId = NS.Id,
                                    IsDone = false,
                                    ActionTaken = null,
                                    CreatedById = LoggedInUserId,
                                    CreatedDateTime = DateTime.Now,
                                    UpdatedById = LoggedInUserId,
                                    UpdatedDateTime = DateTime.Now
                                };
                                db.LFSNextStepActions.Add(NextStepAction);
                                await db.SaveChangesAsync();
                            }



                        }
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

            return CreatedAtRoute("DefaultApi", new { id = LFSTableRespons.Id }, LFSTableRespons);
        }

        private bool LFSTableResponseExists(int id)
        {
            return db.LFSTableResponses.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("UQ_LFSTableResponses_EntityId_EntityType_TableCode_TableConfigId_Col_TableConfigId_Row", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "TABLE RESPONSE"));
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
                db.SpLogError("RELY-API", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New");
                return Globals.SomethingElseFailedInDBErrorMessage;
            }
        }
    }
}

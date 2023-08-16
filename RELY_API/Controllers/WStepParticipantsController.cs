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
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class WStepParticipantController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/WStepParticipants
        public IHttpActionResult GetWStepParticipants(string UserName, string WorkFlow)
        {
            var xx = (from aa in db.WStepParticipants
                      select new
                      {
                          aa.Id,
                          aa.WStepId,
                          aa.ParticipantId,
                          aa.Type,
                          aa.IsDefault,
                          aa.Description
                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }


        // GET: api/WStepParticipants/5
        [ResponseType(typeof(WStepParticipant))]
        public async Task<IHttpActionResult> GetWStepParticipantById(int id, string UserName, string WorkFlow)
        {
            var WStepParticipant = db.WStepParticipants.Where(p => p.Id == id).Select(aa => new {
                aa.Id,
                aa.WStepId,
                aa.ParticipantId,
                aa.Type,
                aa.IsDefault,
                aa.Description
            }).FirstOrDefault();
            if (WStepParticipant == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP PARTICIPANT")));
            }
            return Ok(WStepParticipant);
        }
        [ResponseType(typeof(WStepParticipant))]
        public async Task<IHttpActionResult> GetWStepParticipantByStepId(int StepId, string UserName, string WorkFlow)
        {

            var WStepParticipant = (from aa in db.WStepParticipants.Where(p => p.WStepId == StepId)
                                    join bb in db.LRoles on aa.ParticipantId equals bb.Id
                                    select new {
                 aa.Id,
                aa.WStepId,
                aa.ParticipantId,
                aa.Type,
                aa.IsDefault,
                aa.Description,
                ParticipantName = bb.RoleName
            }).ToList();
            if (WStepParticipant == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP PARTICIPANT")));
            }
            return Ok(WStepParticipant);
        }

        // PUT: api/WStepParticipants/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutWStepParticipant(int id, WStepParticipant WStepParticipant, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "STEP")));
            }
            if (!WStepParticipantExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP")));
            }

            if (id != WStepParticipant.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "STEP")));
            }
            try
            {
                db.Entry(WStepParticipant).State = EntityState.Modified;
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
            return Ok(WStepParticipant);
        }

        // POST: api/WStepParticipants
        [ResponseType(typeof(WStepParticipant))]
        public async Task<IHttpActionResult> PostWStepParticipant(WStepParticipant model, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "STEP PARTICIPANT")));
            }
            try
            {
                if (db.WStepParticipants.Where(p => p.Id == model.Id).Where(p => p.WStepId == model.WStepId).Count() > 0)
                {
                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    model.Id = 0;//To override the Id generated by grid
                    db.WStepParticipants.Add(model);
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

            //return CreatedAtRoute("DefaultApi", new { id = model.Id }, model);
            return Ok(model.Id);
        }

        // DELETE: api/WStepParticipants/5
        [ResponseType(typeof(WStepParticipant))]
        public async Task<IHttpActionResult> DeleteById(int id, int StepId, string UserName, string WorkFlow)
        {
            WStepParticipant WStepParticipant = await db.WStepParticipants.FindAsync(id);
            if (WStepParticipant == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP")));
            }
            try
            {
                List<WStepParticipantAction> wStepParticipantAction = db.WStepParticipantActions.Where(a => a.ParticipantId == id).Where(a=>a.ShowInStepId == StepId).ToList();
                db.WStepParticipantActions.RemoveRange(wStepParticipantAction);
                await db.SaveChangesAsync();

                db.WStepParticipants.Remove(WStepParticipant);
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
            return Ok(WStepParticipant);
        }

        private bool WStepParticipantExists(int id)
        {
            return db.WStepParticipants.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message FK_WStepParticipants_MStepRoles_StepId
            if (SqEx.Message.IndexOf("UQ_WStepParticipents_WStepsId_Type_ParticipentId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "STEP PARTICIPANTS"));
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

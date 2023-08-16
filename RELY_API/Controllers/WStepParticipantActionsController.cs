using RELY_API.Models;
using RELY_API.Utilities;
using System;
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
    public class WStepParticipantActionsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        public IHttpActionResult GetWFIdStepIdById(int Id, string UserName, string WorkFlow)
        {
            var qry = "select wpa.Id Id,rw.Id WorkFlowId,rw.Name as WorkflowName, ws.Id StepId,wa.Name ActionName from WStepParticipantActions wpa "
                + " inner join WSteps ws on wpa.ShowInStepId = ws.Id inner join RWorkFlows rw on rw.id = Ws.WorkFlowId inner join WActions wa on wa.Id = wpa.WActionId "
                + " where wpa.Id = {0}";
            var xx = db.Database.SqlQuery<StepParticipantActionForWorkflowViewModel>(qry,Id).FirstOrDefault();
            return Ok(xx);
        }
        

        public IHttpActionResult GetCount(string ActionName, int LoggedInRoleId,int LoggedInUserId, int WorkFlowId, int StepId)
        {
            var qry = "SELECT count(*) from WStepParticipantActions where WActionId in (select Id from WActions where name={0}) and "
                + " ((ParticipantType='ROLE' and ParticipantId={1}) OR (ParticipantType='USER' and ParticipantId={2}) ) "
                + " and ShowInStepId ={3}";
            var xx = db.Database.SqlQuery<int>(qry, ActionName, LoggedInRoleId,LoggedInUserId, StepId).FirstOrDefault();
            return Ok(xx);
        }

        // GET: api/WStepParticipantActions
        public IHttpActionResult GetWStepParticipantActions(string UserName, string WorkFlow)
        {
            var xx = (from x in db.WStepParticipantActions
                      select new { x.Id, x.WActionId, x.Label, x.ParticipantId,x.ParticipantType,x.ShowInStepId,
                          x.VisibilityFunction,x.IsLinkOverWFGrid,x.Glymph,x.Description,x.ButtonOnForm,x.ButtonOnWfGrid }).OrderBy(p => p.Id);
            return Ok(xx);

        }
        public IHttpActionResult GetAllActions(string UserName, string WorkFlow)
        {
            var xx = (from x in db.WActions
                      select new
                      {
                          x.Id,
                          x.Name,
                          x.Label,
                          x.Description,
                          x.Glymph,
                          x.Ordinal,
                          x.ActionURL
                      }).ToList().OrderBy(p => p.Id);
            return Ok(xx);

        }


        public IHttpActionResult GetByParticipantIdStepId(int ParticipantId,int StepId,string UserName, string WorkFlow)
        {
            //var xx = (from x in db.WStepParticipantActions where x.ParticipantId == ParticipantId && x.ShowInStepId == StepId
            //          join p in db.LRoles on x.ParticipantId equals p.Id
            //          join a in db.WActions on x.WActionId equals a.Id
            //          select new
            //          {
            //              x.Id,
            //              x.WActionId,
            //              x.Label,
            //              x.ParticipantId,
            //              x.ParticipantType,
            //              x.ShowInStepId,
            //              x.VisibilityFunction,
            //              x.IsLinkOverWFGrid,
            //              x.Glymph,
            //              x.Description,
            //              x.ButtonOnForm,
            //              x.ButtonOnWfGrid,
            //              ParticipantName = p.RoleName,
            //              ActionName = a.Name,

            //          }).ToList().OrderBy(p => p.Id);
            string sql = "Select x.ActionUrl,x.Id,x.WActionId,x.Label,x.ParticipantId,x.ParticipantType,x.ShowInStepId,x.VisibilityFunction,x.IsLinkOverWFGrid,x.Glymph,x.Description,x.ButtonOnForm, "
                            + " x.ButtonOnWfGrid,ParticipantName = p.RoleName,ActionName = a.Name,x.SendToStepId,SendToStepName = s.Name from WStepParticipantActions x "
                            + " join LRoles p on x.ParticipantId = p.Id join WActions a on x.WActionId = a.Id left outer join WSteps s on x.SendToStepId = s.Id "
                            + " where x.ParticipantId = {0} and x.ShowInStepId = {1} ";
            var xx = db.Database.SqlQuery<StepParticipantActionViewModel>(sql, ParticipantId, StepId).ToList();
            return Ok(xx);

        }
        // GET: api/WStepParticipantActions/5
        [ResponseType(typeof(WStepParticipantAction))]
        public async Task<IHttpActionResult> GetById(int id, string UserName, string WorkFlow)
        {
            var WStepParticipantAction = db.WStepParticipantActions.Where(p => p.Id == id).Select(x => new {
                x.Id,
                x.WActionId,
                x.Label,
                x.ParticipantId,
                x.ParticipantType,
                x.ShowInStepId,
                x.VisibilityFunction,
                x.IsLinkOverWFGrid,
                x.Glymph,
                x.Description,
                x.ButtonOnForm,
                x.ButtonOnWfGrid,
                x.SendToStepId,
                x.ActionUrl
            }).FirstOrDefault();
            if (WStepParticipantAction == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PARTICIPANT ACTION")));
            }
            return Ok(WStepParticipantAction);
        }
        
        // PUT: api/WStepParticipantActions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutWStepParticipantAction(int id, WStepParticipantAction WStepParticipantAction, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PARTICIPANT ACTION")));
            }
            if (!WStepParticipantActionExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PARTICIPANT ACTION")));
            }

            if (id != WStepParticipantAction.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PARTICIPANT ACTION")));
            }
            try
            {
                db.Entry(WStepParticipantAction).State = EntityState.Modified;
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
            return Ok(WStepParticipantAction);
        }

        // POST: api/WStepParticipantActions
        [ResponseType(typeof(WStepParticipantAction))]
        public async Task<IHttpActionResult> PostWStepParticipantAction( WStepParticipantAction model)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "PARTICIPANT ACTION")));
            }
            try
            {
                if (db.WStepParticipantActions.Where(p => p.Id == model.Id).Where(p => p.ShowInStepId == model.ShowInStepId).Where(p=>p.ParticipantId == model.ParticipantId).Count() > 0)
                {
                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    model.Id = 0;//To override the Id generated by grid
                    db.WStepParticipantActions.Add(model);
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

            // return CreatedAtRoute("DefaultApi", new { id = WStepParticipantAction.Id }, WStepParticipantAction);
            return Ok();
        }

        // DELETE: api/WStepParticipantActions/5
        [ResponseType(typeof(WStepParticipantAction))]
        public async Task<IHttpActionResult> DeleteById(int id, string UserName, string WorkFlow)
        {
            WStepParticipantAction WStepParticipantAction = await db.WStepParticipantActions.FindAsync(id);
            if (WStepParticipantAction == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
            }
            try
            {
                db.WStepParticipantActions.Remove(WStepParticipantAction);
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
                    throw ex;
                }
            }

            return Ok(WStepParticipantAction);
        }

        private bool WStepParticipantActionExists(int id)
        {
            return db.WStepParticipantActions.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message 
            if (SqEx.Message.IndexOf("FK_WSteps_WStepParticipantActions_ShowInStepId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "PRODUCT OBLIGATIONS", "STEPS"));
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

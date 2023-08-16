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
    public class LFSManualAccScenariosController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/LFSManualAccScenarios? Id
        //Method to Get Data in Grid 
        public IHttpActionResult GetLFSManualAccScenariosById(int Id, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSManualAccScenarios.Where(p => p.Id == Id)
                      select new
                      {
                          aa.Id,
                          aa.EntityId,
                          aa.EntityType,
                          aa.Reference,
                          aa.Situation,
                          aa.ObjectType,
                          aa.Description,
                          aa.Comments
                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }

        // GET: api/LFSManualAccScenarios? EntityId
        //Method to Get Data in Grid 
        public IHttpActionResult GetLFSManualAccScenariosByEntityId(int EntityId, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSManualAccScenarios.Where(p => p.EntityId == EntityId)
                      select new
                      {
                          aa.Id,
                          aa.EntityId,
                          aa.EntityType,
                          aa.Reference,
                          aa.Situation,
                          aa.ObjectType,
                          aa.Description,
                          aa.Comments
                      }).First();
            return Ok(xx);
        }

        // POST: api/LFSManualAccScenarios
        //Method to Post Data from app to db
        [ResponseType(typeof(LFSManualAccScenario))]
        public async Task<IHttpActionResult> PostLFSManualAccScenarios(LFSManualAccScenario LFSManualAccScenario, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Manual Acc Scenarios")));
            }

            try
            {
                if (db.LFSManualAccScenarios.Where(p => p.Id == LFSManualAccScenario.Id).Count() > 0)
                {
                    db.Entry(LFSManualAccScenario).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    LFSManualAccScenario.Id = 0;
                    db.LFSManualAccScenarios.Add(LFSManualAccScenario);
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
            return CreatedAtRoute("DefaultApi", new { id = LFSManualAccScenario.Id }, LFSManualAccScenario);
        }

        // PUT: api/LFSManualAccScenarios?Id
        //Method to update Requested Data by User in db
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLFSManualAccScenarios(int Id, LFSManualAccScenario LFSManualAccScenario, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "MANUAL ACC SCENARIOS")));
            }

            if (!LFSManualAccScenariosExists(Id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MANUAL ACC SCENARIOS")));
            }

            try
            {
                db.Entry(LFSManualAccScenario).State = EntityState.Modified;
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

            return Ok(LFSManualAccScenario);

        }

        // Delete: api/LFSManualAccScenarios?Id
        [ResponseType(typeof(LFSManualAccScenario))]
        public async Task<IHttpActionResult> DeleteLFSManualAccScenario(int Id, string UserName, string WorkFlow)
        {
            LFSManualAccScenario LFSManualAccScenario = await db.LFSManualAccScenarios.FindAsync(Id);
            if (LFSManualAccScenario == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Manual Acc Scenario")));
            }

            try
            {
                db.LFSManualAccScenarios.Remove(LFSManualAccScenario);
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
            return Ok(LFSManualAccScenario);
        }

        private bool LFSManualAccScenariosExists(int Id)
        {
            return db.LFSManualAccScenarios.Count(e => e.Id == Id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("FK_LFSResponses_LNextStepActions_ResponseId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "RESPONSE", "NEXT STEP ACTIONS"));
            if
                (SqEx.Message.IndexOf("UQ_LFSManualAccScenarios_EntityId_EntityType_Referance", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "MANUAL ACC SCENARIOS"));
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

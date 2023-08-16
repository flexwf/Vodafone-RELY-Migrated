using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Data;
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
    public class LNotificationController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        //GetLNotification/ByCompanyCode
        public IHttpActionResult GetByCompanyCode(string CompanyCode, string UserName, string WorkFlow)
        {
            
            string strQuery = "Exec SPGetLNotification @CompanyCode";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            var dt = new DataTable();
            dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }

        //POST: api/LNotification/
        [HttpPost]
        [ResponseType(typeof(LNotification))]
        public async Task<IHttpActionResult> Post(LNotification LNotification/*, string UserName, string WorkFlow*/)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "LNotification")));
            }

            try
            {
                if (db.LNotifications.Where(p => p.Id == LNotification.Id).Where(p => p.CompanyCode == LNotification.CompanyCode).Count() > 0)
                {
                    db.Entry(LNotification).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    LNotification.Id = 0;
                    db.LNotifications.Add(LNotification);
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
            return CreatedAtRoute("DefaultApi", new { Id = LNotification.Id }, LNotification);
        }
      
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(LNotification lnotifi,int id)
        {           
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "LNotification")));
            }
            if (!LNotificationExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LNotification")));
            }

            if (id != lnotifi.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "LNotification")));
            }
            try
            {
                db.Entry(lnotifi).State = EntityState.Modified;
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
            return Ok(lnotifi);
        }

        private bool LNotificationExists(int id)
        {
            return db.LNotifications.Count(e => e.Id == id) > 0;
        }
        // DELETE: api/LNotification/
        [ResponseType(typeof(LNotification))]
        public async Task<IHttpActionResult> Delete(int id/*, string UserName, string WorkFlow*/)
        {
            LNotification lNotification = await db.LNotifications.FindAsync(id);
            if(lNotification==null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LNotification")));
            }

            try
            {
                db.LNotifications.Remove(lNotification);
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

            return Ok(lNotification);
        }

        [HttpGet]
        public IHttpActionResult GetById(int id)
        {
            var xx = (from aa in db.LNotifications.Where(p => p.Id == id)
                      join WS in db.WSteps on aa.LandingStepId equals WS.Id
                      join RWF in db.RWorkFlows on WS.WorkFlowId equals RWF.Id
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.Type,
                          aa.Description,
                          aa.RemindAfterDays,
                          aa.TemplateId,
                          aa.LandingStepId,
                          aa.OriginatingStepId,
                          aa.RecipientRoleId,
                          WorkFlowId = RWF.Id,
                          Workflow = RWF.Name
                      }).FirstOrDefault();
            if (xx == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LNotification")));
            }
            return Ok(xx);
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
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


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
    public class RRequestSystemsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        [HttpGet]
        public IHttpActionResult GetByCompanyCode(string CompanyCode)
        {
            var sLRequest = db.RRequestSystems.Where(r => r.CompanyCode == CompanyCode).Select(aa => new {
                aa.Id,
                aa.CompanyCode,
                aa.Name,
                aa.Description            

            }).OrderBy(aa => aa.Name);
            if (sLRequest == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LREQUEST")));
            }
            return Ok(sLRequest);
        }

        [ResponseType(typeof(RRequestSystem))]
        public async Task<IHttpActionResult> GetByName(string Name)
        {
            var LRequest = db.RRequestSystems.Where(r => r.Name == Name).Select(aa => new {
                aa.Id,
                aa.CompanyCode,
                aa.Name,
                aa.Description

            }).FirstOrDefault();
            if (LRequest == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Request")));
            }
            return Ok(LRequest);
        }

        [HttpPost]
        [ResponseType(typeof(RRequestSystem))]
        public async Task<IHttpActionResult> Post(RRequestSystem rreqsys)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "RRequest System")));
            }
            //need to remove transactions, as its not required in this scenario
            try
            {
                db.RRequestSystems.Add(rreqsys);
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

            return CreatedAtRoute("DefaultApi", new { id = rreqsys.Id }, rreqsys);
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_RRequestSystems_LRequests_SystemId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "REQUEST SYSTEMS", "REQUEST"));
            else if (SqEx.Message.IndexOf("UQ_RRequestSystems_CompanyCode_Name", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "REQUEST SYSTEMS"));
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

       
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(RRequestSystem rreqsys, int id)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RRequest System")));
            }
            if (!RReqSystemExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RRequest System")));
            }

            if (id != rreqsys.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RRequest System")));
            }
            try
            {
                db.Entry(rreqsys).State = EntityState.Modified;
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
            return Ok(rreqsys);
        }
        [ResponseType(typeof(RRequestSystem))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            RRequestSystem RRequestSystem = await db.RRequestSystems.FindAsync(id);
            if (RRequestSystem == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Request Systems")));
            }

            try
            {
                db.RRequestSystems.Remove(RRequestSystem);
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
            return Ok(RRequestSystem);
        }

        private bool RReqSystemExists(int id)
        {           
            return db.RRequestSystems.Count(e => e.Id == id) > 0;
        }

        [HttpGet]
        [ResponseType(typeof(RRequestSystem))]
        public async Task<IHttpActionResult> GetByRSystemId(int Id)
        {
            var LRequest = db.RRequestSystems.Where(r => r.Id == Id).Select(aa => new
            {
                aa.Id,
                aa.CompanyCode,
                aa.Name,
                aa.Description

            }).FirstOrDefault();
            if (LRequest == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Request")));
            }
            return Ok(LRequest);
        }

        /*Below code is commented by Rakhi Singh on 3rd Aug 2018 as per requirement.*/         
        //[HttpPost]
        //[ResponseType(typeof(RRequestSystem))]
        //public async Task<IHttpActionResult> PostRRequestSystem(RRequestSystem RRequestSystem)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Request System")));
        //    }            
        //    try
        //    {
        //        if (db.RRequestSystems.Where(p => p.Id == RRequestSystem.Id).Where(p => p.CompanyCode == RRequestSystem.CompanyCode).Count() > 0)
        //        {
        //            db.Entry(RRequestSystem).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            RRequestSystem.Id = 0;//To override the Id generated by grid
        //            db.RRequestSystems.Add(RRequestSystem);
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
        //    return CreatedAtRoute("DefaultApi", new { id = RRequestSystem.Id }, RRequestSystem);
        //}*/
    }
}

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
    public class LDropDownValuesController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        public IHttpActionResult GetLDropDownValuesByDropDownId(int DropDownId)
        {
            var xx = (from aa in db.LDropDownValues.Where(p => p.DropDownId == DropDownId).Include(c => c.LDropDown)
                      select new { aa.Id, aa.LDropDown.Name, aa.Description, aa.DropDownId, aa.Value }).OrderBy(p => p.Value);
            return Ok(xx);
        }       

        public IHttpActionResult GetLDropDownValueCountsByDropDownId(int DropDownId)
        {
            var xx = (from aa in db.LDropDownValues.Where(p => p.DropDownId == DropDownId).Include(c => c.LDropDown)
                      select aa).Count();
            return Ok(xx);
        }
       
        [ResponseType(typeof(LDropDownValue))]
        public IHttpActionResult GetLDropDownValue(int id)
        {
            var LDropDownValue = db.LDropDownValues.Where(p => p.Id == id).Select(x => new { x.Id, x.DropDownId, x.LDropDown.Name, x.Value, x.Description }).FirstOrDefault();
            if (LDropDownValue == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "DropDown Value")));
            }
            return Ok(LDropDownValue);
        }

        [ResponseType(typeof(LDropDownValue))]
        public async Task<IHttpActionResult> Post(LDropDownValue ldrpdwn)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "DropDown Value")));
            }
            //need to remove transactions, as its not required in this scenario
            try
            {
                db.LDropDownValues.Add(ldrpdwn);
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

            return CreatedAtRoute("DefaultApi", new { id = ldrpdwn.Id }, ldrpdwn);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(LDropDownValue ldrpdwn, int id)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "DropDown Value")));
            }
            if (!LDropDownValueExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "DropDown Value")));
            }

            if (id != ldrpdwn.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "DropDown Value")));
            }
            try
            {
                db.Entry(ldrpdwn).State = EntityState.Modified;
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
            return Ok(ldrpdwn);
        }      
      

        [ResponseType(typeof(LDropDownValue))]
        public async Task<IHttpActionResult> Delete(int id, int DropdownId)
        {
            // RProductCategory RProductcat = await db.RProductCategories.FindAsync(id);
            LDropDownValue LDropDownValue = db.LDropDownValues.Where(p => p.Id == id).Where(p => p.DropDownId == DropdownId).FirstOrDefault();
            if (LDropDownValue == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Dropdown Value")));
            }

            try
            {
                db.LDropDownValues.Remove(LDropDownValue);
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
            return Ok(LDropDownValue);
        }

        private bool LDropDownValueExists(int id)
        {
            return db.LDropDownValues.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //call db.splogerror SP here and log SQL SqEx.Message
            //Add complete Url in description
            if (SqEx.Message.IndexOf("FK_LDropdownValues_LASLifeCycleEvents_EventId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "DROPDOWN VALUES", "LIFE CYCLE EVENTS"));
            else if (SqEx.Message.IndexOf("FK_LDropdownValues_LASLifeCycleEvents_NatureId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "DROPDOWN VALUES", "LIFE CYCLE EVENTS NATURE"));
            else if (SqEx.Message.IndexOf("FK_LDropDownValues_LAccountingScenarios_BusinessAreaId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "DROPDOWN VALUES", "TRANSCATIONS"));
            else if (SqEx.Message.IndexOf("FK_LDropDownValues_LScenarioDemand_BusinessAreaId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "DROPDOWN VALUES", "TRANSCATIONS"));
            else if (SqEx.Message.IndexOf("UQ_LDropDownValues_DropDownId_Value", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "DROPDOWN VALUES"));            
            else
            { 
            var UserName = "";//System.Web.HttpContext.Current.Session["UserName"] as string;
            string UrlString = Convert.ToString(Request.RequestUri.AbsolutePath);
            var ErrorDesc = "";
            var Desc = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
            if (Desc.Count() > 0)
                ErrorDesc = string.Join(",", Desc);
            string[] s = Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at 2nd and action name at 3 rd index position
            //db.SpLogError("Vodafone-SOS_WebApi", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New");
            ////Globals.LogError(SqEx.Message, ErrorDesc);
            //return Globals.SomethingElseFailedInDBErrorMessage;

             ObjectParameter Result = new ObjectParameter("Result", typeof(int)); //return parameter is declared
                db.SpLogError("RELY-API", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New", Result).FirstOrDefault();
                int errorid = (int)Result.Value; //getting value of output parameter
                //return Globals.SomethingElseFailedInDBErrorMessage;
                return (string.Format(Globals.SomethingElseFailedInDBErrorMessage, errorid));
            }
        }

        /*-----------------------Below code is commented by Rakhi Singh on 6th August 2018 as per requirement-------------------------------------*/
        //[ResponseType(typeof(LDropDownValue))]
        //public async Task<IHttpActionResult> PostLDropDownValue(LDropDownValue LDropDownValue, string UserName, string Workflow)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        //return BadRequest(ModelState);
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "DropDown Value")));
        //    }

        //    try
        //    {
        //        if (db.LDropDownValues.Where(p => p.Id == LDropDownValue.Id).Where(p => p.DropDownId == LDropDownValue.DropDownId).Count() > 0)
        //        {
        //            db.Entry(LDropDownValue).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            LDropDownValue.Id = 0;//Reset the dropdown value id coming from model
        //            db.LDropDownValues.Add(LDropDownValue);
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

        //    return Ok();
        //    //return CreatedAtRoute("DefaultApi", new { id = LDropDownValue.Id }, LDropDownValue);
        //}

    }
}

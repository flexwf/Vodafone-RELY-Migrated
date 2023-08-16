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
    public class LDropDownsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        [HttpGet]
        public IHttpActionResult GetLDropDownByNameCompanyCode(string Name,string CompanyCode)
        {
            var xx = db.LDropDowns.Where(p => p.CompanyCode == CompanyCode).Where(p => p.Name == Name).FirstOrDefault();
            return Ok(xx);
        }

        [HttpGet]
        public IHttpActionResult GetById(int Id)
        {
            var xx = db.LDropDowns.Where(p => p.Id == Id).FirstOrDefault();
            return Ok(xx);
        }
      
        [ResponseType(typeof(LDropDown))]
        public IHttpActionResult GetByCompanyCode(string CompanyCode)
        {
            var LDropDown = db.LDropDowns.Where(p => p.CompanyCode == CompanyCode).Select(aa => new
            {
                aa.Id,
                aa.CompanyCode,
                aa.Name,
                aa.Description
            }).OrderByDescending(aa => aa.Id);
            if (LDropDown == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LDropdowns")));
            }
            return Ok(LDropDown);
        }
       
        [ResponseType(typeof(LDropDown))]
        public async Task<IHttpActionResult> Post(LDropDown ldropdown)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "LDropdowns")));
            }
            //need to remove transactions, as its not required in this scenario
            try
            {
                db.LDropDowns.Add(ldropdown);
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

            return CreatedAtRoute("DefaultApi", new { id = ldropdown.Id }, ldropdown);
        }
       
        [ResponseType(typeof(LDropDown))]
        //Following DeleteLDropDowns Added By Vijay
        // DELETE: api/LDropDowns/5
        public async Task<IHttpActionResult> Delete(int id)
        {
            LDropDown LDropDowns = await db.LDropDowns.FindAsync(id);
            if (LDropDowns == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LDropdowns")));
            }

            try
            {
                db.LDropDowns.Remove(LDropDowns);
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
            return Ok(LDropDowns);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(LDropDown ldropdown, int id)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RLocalPOB Type")));
            }
            if (!LDropDownsExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RLocalPOB Type")));
            }

            if (id != ldropdown.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RLocalPOB Type")));
            }
            try
            {
                db.Entry(ldropdown).State = EntityState.Modified;
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
            return Ok(ldropdown);
        }

        private bool LDropDownsExists(int id)
        {
            return db.LDropDowns.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_LDropDowns_LDropDownValues_DropDownId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Dropdown", "Values"));
            else if (SqEx.Message.IndexOf("FK_LDropDowns_LCompanySpecificColumns_DropDownid", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Dropdown ", "Company Specific Columns"));
            else if (SqEx.Message.IndexOf("UQ_LDropDowns_CompanyCode_Name", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "Drop Down"));
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
                string[] s = Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at 2nd and action name at 3rd index position
                //db.SpLogError("RELY-API", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New");
                //return Globals.SomethingElseFailedInDBErrorMessage;
                 ObjectParameter Result = new ObjectParameter("Result", typeof(int)); //return parameter is declared
                db.SpLogError("RELY-API", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New", Result).FirstOrDefault();
                int errorid = (int)Result.Value; //getting value of output parameter
                //return Globals.SomethingElseFailedInDBErrorMessage;
                return (string.Format(Globals.SomethingElseFailedInDBErrorMessage, errorid));

            }
        }

        /*---------------------------------Below method s commented by Rakhi Singh on 6th august 2018 as per requirement------------------------------*/
        //[HttpGet]
        //public IHttpActionResult GetLDropDownByCompanyCode(string CompanyCode)
        //{
        //    var xx = db.LDropDowns.Where(p => p.CompanyCode == CompanyCode).ToList();
        //    return Ok(xx);
        //}
       
        //Following PostLDropDowns Added By Vijay
        // [HttpPost]
        //[HttpGet]
        // POST: api/LDropDowns
        //[ResponseType(typeof(LDropDown))]
        //public async Task<IHttpActionResult> PostLDropDowns(LDropDown LDropDown)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Dropdowns")));
        //    }

        //    //================================
        //    try
        //    {
        //        if (db.LDropDowns.Where(p => p.Id == LDropDown.Id).Where(p => p.CompanyCode == LDropDown.CompanyCode).Count() > 0)
        //        {
        //            db.Entry(LDropDown).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            LDropDown.Id = 0;//To override the Id generated by grid
        //            db.LDropDowns.Add(LDropDown);
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

        //    return CreatedAtRoute("DefaultApi", new { Id = LDropDown.Id }, LDropDown);
        //}     
    }
}

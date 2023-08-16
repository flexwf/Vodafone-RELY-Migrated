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
    public class RProductSystemsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        /// <summary>
        /// Created by: Rakhi Singh on 31st july 
        /// Description: Method to get data on the basis of id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult GetRProductSystemById(int id)
        {
            var xx = (from aa in db.RProductSystems.Where(p => p.Id == id)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.Name,
                          aa.Description

                      }).FirstOrDefault();
            if (xx == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ProductSystem")));
            }           
            return Ok(xx);
        }

        // GET: api/RProductSystems?CompanyCode=DE
        //Method to Get Data in Grid
        public IHttpActionResult GetRProductSystemsByCompanyCode(string CompanyCode)
        {
            var xx = (from aa in db.RProductSystems.Where(p => p.CompanyCode == CompanyCode)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.Name,
                          aa.Description

                      }).OrderByDescending(p => p.Id);
            return Ok(xx);
        }       
        
        [ResponseType(typeof(RProductSystem))]
        public async Task<IHttpActionResult> Post(RProductSystem rprodsys)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ProductSystem")));
            }
            //need to remove transactions, as its not required in this scenario
            try
            {
                db.RProductSystems.Add(rprodsys);
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

            return CreatedAtRoute("DefaultApi", new { id = rprodsys.Id }, rprodsys);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(RProductSystem rprodsys, int id)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RProductSystem")));
            }
            if (!RProdSysExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RProductSystem")));
            }

            if (id != rprodsys.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RProductSystem")));
            }
            try
            {
                db.Entry(rprodsys).State = EntityState.Modified;
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
            return Ok(rprodsys);
        }

        /// <summary>
        /// Created By Rakhi Singh on 31st July 2018
        /// Description: Method to delete RProductsystem
        /// </summary>
        /// <param name="id"></param>
        /// <param name="UserName"></param>
        /// <param name="WorkFlow"></param>
        /// <returns></returns>
        [ResponseType(typeof(RProductSystem))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            RProductSystem RProductSystem = await db.RProductSystems.FindAsync(id);
            if (RProductSystem == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RLocalPob Type")));
            }

            try
            {
                db.RProductSystems.Remove(RProductSystem);
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
            return Ok(RProductSystem);
        }

        private bool RProdSysExists(int id)
        {
            return db.RProductSystems.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_RProductSystems_RSysCat_SystemId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "PRODUCT SYSTEMS", "SYSCAT"));
            else if
                (SqEx.Message.IndexOf("UQ_RProductSystems_CompanyCode_Name", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "PRODUCT SYSTEMS"));
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


        /*--------------------Below code is commented by Rakhi Singh on 31st july as per requirement-------------------------------------------*/
        // PUT: api/RProductSystems?CompanyCode=DE&Name=System(updated name)
        //Method to update Requested Data by User in db
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutRProductSystems(string CompanyCode,string Name, RProductSystem RProductSystem)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PRODUCTSYSTEM")));
        //    }

        //    if (!RProductSystemExists(CompanyCode)&&!RProductSystemExists(Name))
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PRODUCTSYSTEM")));
        //    }

        //    if (CompanyCode != RProductSystem.CompanyCode&&Name != RProductSystem.Name)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PRODUCTSYSTEM")));
        //    }
        //    try
        //        {
        //            db.Entry(RProductSystem).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //        }
        //        catch (DbEntityValidationException dbex)
        //        {

        //            var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
        //            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
        //        }
        //        catch (Exception ex)
        //        {

        //            if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
        //            {
        //                //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
        //                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
        //            }
        //            else
        //            {
        //                throw ex;//This exception will be handled in FilterConfig's CustomHandler
        //            }
        //        }

        //    // return StatusCode(HttpStatusCode.NoContent);
        //    return Ok(RProductSystem);
        //}
        //[ResponseType(typeof(RSystem))]
        //public async Task<IHttpActionResult> DeleteRSystems(string CompanyCode)
        //{
        //    RSystem RSystem = await db.RSystems.FindAsync(CompanyCode);
        //    if (RSystem == null)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SYSTEM")));
        //    }

        //    try
        //    {
        //        db.RSystems.Remove(RSystem);
        //        await db.SaveChangesAsync();

        //    }
        //    catch (Exception ex)
        //    {

        //        if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
        //        {
        //            Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
        //            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
        //        }
        //        else
        //        {
        //            throw ex;
        //        }
        //    }


        //    return Ok(RSystem);
        //}

        //[ResponseType(typeof(RProductSystem))]
        //public async Task<IHttpActionResult> DeleteRProductSystem(int id, string UserName, string WorkFlow)
        //{
        //    RProductSystem RProductSystem = await db.RProductSystems.FindAsync(id);
        //    if (RProductSystem == null)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Product Systems")));
        //    }

        //    try
        //    {
        //        db.RProductSystems.Remove(RProductSystem);
        //        await db.SaveChangesAsync();

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
        //            throw ex;
        //        }
        //    }
        //    return Ok(RProductSystem);
        //}
        //private bool RProductSystemExists(string CompanyCode)
        //{
        //    return db.RProductSystems.Count(e => e.CompanyCode == CompanyCode) > 0;
        //}     

        // GET: api/RProductSystems?CompanyCode=DE&Name=System1
        //Method to Get Requested Data by user for Edit
        //[ResponseType(typeof(RProductSystem))]
        //public async Task<IHttpActionResult> GetRProductSystems(string CompanyCode,string Name)
        //{
        //    var RProductSystem = db.RProductSystems.Where(p => p.CompanyCode == CompanyCode &&  p.Name==Name).Select(x => new {  x.Id, x.CompanyCode, x.Name, x.Description}).First();
        //    if (RProductSystem == null)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ProductSystem")));
        //    }
        //    return Ok(RProductSystem);
        //}

        // POST: api/RProductSystems
        //Method to Post Data from app to db
        //[ResponseType(typeof(RProductSystem))]
        //public async Task<IHttpActionResult> PostRProductSystems(RProductSystem RProductSystem)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ProductSystem")));
        //    }
        //    try
        //    {
        //        if (db.RProductSystems.Where(p => p.Id == RProductSystem.Id).Where(p => p.CompanyCode == RProductSystem.CompanyCode).Count() > 0)
        //        {
        //            db.Entry(RProductSystem).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            RProductSystem.Id = 0;//To override the Id generated by grid
        //            db.RProductSystems.Add(RProductSystem);
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
        //    return CreatedAtRoute("DefaultApi", new { id = RProductSystem.Id }, RProductSystem);
        //}
    }
}
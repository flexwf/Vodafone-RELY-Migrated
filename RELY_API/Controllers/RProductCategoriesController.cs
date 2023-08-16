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
    public class RProductCategoriesController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
       
        //Method to Get Data in Grid
        public IHttpActionResult GetRProductCategoriesByCompanyCode(string CompanyCode)
        {
            var xx = (from aa in db.RProductCategories.Where(p => p.CompanyCode == CompanyCode)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.Name,
                          aa.Description
                      }).OrderByDescending(p => p.Id);
            return Ok(xx);
        }

        //Method to Post Data from app to db
        [HttpPost]
        [ResponseType(typeof(RProductCategory))]
        public async Task<IHttpActionResult> Post(RProductCategory rprodcat)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "RProduct Category")));
            }
            //need to remove transactions, as its not required in this scenario
            try
            {
                db.RProductCategories.Add(rprodcat);
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

            return CreatedAtRoute("DefaultApi", new { id = rprodcat.Id }, rprodcat);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(RProductCategory rprodcat, int id)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RProduct Category")));
            }
            if (!RProductCategoryExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RProduct Category")));
            }

            if (id != rprodcat.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RProduct Category")));
            }
            try
            {
                db.Entry(rprodcat).State = EntityState.Modified;
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
            return Ok(rprodcat);
        }

        private bool RProductCategoryExists(int id)
        {
            return db.RProductCategories.Count(e => e.Id == id) > 0;
        }
       

        /// <summary>
        /// Created By Rakhi Singh on 31st July 2018
        /// Description: Method to delete RProductcategory
        /// </summary>
        /// <param name="id"></param>
        /// <param name="UserName"></param>
        /// <param name="WorkFlow"></param>
        /// <returns></returns>
        [ResponseType(typeof(RProductCategory))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            RProductCategory RProductcat = await db.RProductCategories.FindAsync(id);
            if (RProductcat == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RLocalPob Type")));
            }

            try
            {
                db.RProductCategories.Remove(RProductcat);
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
            return Ok(RProductcat);
        }

        /// <summary>
        /// Created by: Rakhi Singh on 31st july 
        /// Description: Method to get data on the basis of id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult GetRProductCateById(int id)
        {
            var xx = (from aa in db.RProductCategories.Where(p => p.Id == id)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.Name,
                          aa.Description

                      }).FirstOrDefault();
            if (xx == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ProductCategories")));
            }
            return Ok(xx);
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_RProductCategories_RSysCat_CategoryId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "PRODUCT CATEGORIES", "SYSCAT"));
            else if (SqEx.Message.IndexOf("UQ_RProductCategories_CompanyCode_Name", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "PRODUCT CATEGORIES"));
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
        /*below code is commented by Rakhi Singh on 30th july 2018 as per requirement. */

        //[ResponseType(typeof(RProductCategory))]
        //public async Task<IHttpActionResult> PostRProductCategories(RProductCategory RProductCategory)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ProductCategories")));
        //    }
        //    try
        //    {
        //        if (db.RProductCategories.Where(p => p.Id == RProductCategory.Id).Where(p => p.CompanyCode == RProductCategory.CompanyCode).Count() > 0)
        //        {
        //            db.Entry(RProductCategory).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            RProductCategory.Id = 0;//To override the Id generated by grid
        //            db.RProductCategories.Add(RProductCategory);
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
        //}

        // PUT: api/RProductCategories?CompanyCode=DE&Name=System(updated name)
        //Method to update Requested Data by User in db
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutRProductCategories( RProductCategory RProductCategory)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PRODUCTCATEGORY")));
        //    }

        //    if (!RProductCategoryExists(RProductCategory.CompanyCode) && !RProductCategoryExists(RProductCategory.Name))
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PRODUCTCATEGORY")));
        //    }

        //    if (RProductCategory.CompanyCode != RProductCategory.CompanyCode && RProductCategory.Name != RProductCategory.Name)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PRODUCTCATEGORY")));
        //    }
        //    try
        //    {
        //        db.Entry(RProductCategory).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
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

        //    return StatusCode(HttpStatusCode.NoContent);
        //   // return Ok(RProductCategory);
        //}
        //[ResponseType(typeof(RProductCategory))]
        //public async Task<IHttpActionResult> DeleteRProductCategories(string CompanyCode,string Name)
        //{
        //    ////RProductCategory RProductCategory1 = await db.RProductCategories.FindAsync(CompanyCode);
        //    RProductCategory RProductCategory = await db.RProductCategories.Where(p=>p.CompanyCode ==CompanyCode).Where( p=>p.Name == Name).FirstOrDefaultAsync();

        //    if (RProductCategory == null)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PRODUCT CATEGORY")));
        //    }

        //    try
        //    {
        //        db.RProductCategories.Remove(RProductCategory);


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
        //    return Ok(RProductCategory);
        //}        
    }
}
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
    public class RSysCatController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/RSysCat/5
        [ResponseType(typeof(LProduct))]
        public async Task<IHttpActionResult> GetRSysCat(int id)
        {
            var syscat = (from aa in db.RSysCats.Where(p => p.Id == id)
                            select new
                            {
                                aa.Id,
                                aa.CompanyCode,
                                aa.SystemId,
                                aa.CategoryId,
                                aa.SysCat,
                                aa.SysCatCode
                                
                            }).FirstOrDefault();
            if (syscat == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SYSCAT")));
            }
            return Ok(syscat);
        }

        //GET: api/RSysCat/DE
        //Method to get SysCatDropDown Values 
        [ResponseType(typeof(LProduct))]
        public IHttpActionResult GetSysCatforDropDown(string CompanyCode)
        {
            var RSysCat = (from aa in db.RSysCats
                           where (aa.CompanyCode == CompanyCode)
                           select new
                           {
                               aa.Id,
                               aa.SysCat
                           }).OrderBy(aa => aa.SysCat);
            return Ok(RSysCat);
        }

        // GET: api/RSysCat/5
        [ResponseType(typeof(LProduct))]
        public IHttpActionResult GetByCompanyCode(string CompanyCode)
        {
            var RSysCat = (from aa in db.RSysCats
                           join Cat in db.RProductCategories on aa.CategoryId equals Cat.Id
                           join Sys in db.RProductSystems on aa.SystemId equals Sys.Id where(aa.CompanyCode == CompanyCode)
                           select new
                           { 
                               aa.Id,
                               aa.CompanyCode,
                               aa.SystemId,
                               aa.CategoryId,
                               aa.SysCat,
                               CatName=Cat.Name,
                               SysName=Sys.Name,
                               aa.SysCatCode
                           }).OrderByDescending(aa => aa.Id);
            //var RSysCat = (from aa in db.RSysCats.Where(p => p.CompanyCode == CompanyCode)
            //               select new
            //               {
            //                   aa.Id,
            //                   aa.CompanyCode,
            //                   aa.SystemId,
            //                   aa.CategoryId,
            //                   aa.SysCat

            //               }).OrderByDescending(aa => aa.SysCat);
            if (RSysCat == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SYSCAT")));
            }
            return Ok(RSysCat);
        }        

        [HttpPost]
        [ResponseType(typeof(RSysCat))]
        public async Task<IHttpActionResult> Post(RSysCat RSysCat/*, string UserName, string WorkFlow*/)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "LNotification")));
            }

            try
            {
                if (db.RSysCats.Where(p => p.Id == RSysCat.Id).Where(p => p.CompanyCode == RSysCat.CompanyCode).Count() > 0)
                {
                    db.Entry(RSysCat).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    RSysCat.Id = 0;
                    db.RSysCats.Add(RSysCat);
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
            return CreatedAtRoute("DefaultApi", new { Id = RSysCat.Id }, RSysCat);
        }
        
        // DELETE: api/RSysCat/5
        [ResponseType(typeof(RSysCat))]
        public async Task<IHttpActionResult> Delete(int id/*, string UserName, string WorkFlow*/)
        {
            RSysCat RSysCat = await db.RSysCats.FindAsync(id);
            if (RSysCat == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RSysCat")));
            }

            try
            {
                db.RSysCats.Remove(RSysCat);
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
            return Ok(RSysCat);
        }
        private string GetCustomizedErrorMessage(Exception ex)
        {//UQ_RSysCat_CompanyCode_SysCatCode
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_RSysCat_LProducts_SysCatId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Sys Cat", "Products"));
            else if (SqEx.Message.IndexOf("UQ_RSysCat_CompanyCode_SysCatCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "Sys Cat"));
            else if (SqEx.Message.IndexOf("UQ_RSysCat_CompanyCode_SystemId_CategoryId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "Sys Cat"));
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

        /*---------------------------------commented by Rakhi Singh on 12/09/2018 as per requirement---------------------------------*/
        //private bool LReferenceTypeExists(int id)
        //{
        //    return db.LReferenceTypes.Count(e => e.Id == id) > 0;
        //}
        //[HttpPost]
        ////[HttpGet]
        //// POST: api/RSysCat
        //[ResponseType(typeof(RSysCat))]
        //public async Task<IHttpActionResult> Post(RSysCat RSysCat)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "RSysCat")));
        //    }

        //    //================================
        //    try
        //    {
        //        if (db.RSysCats.Where(p => p.Id == RSysCat.Id).Where(p => p.CompanyCode == RSysCat.CompanyCode).Count() > 0)
        //        {
        //            db.Entry(RSysCat).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            RSysCat.Id = 0;//To override the Id generated by grid
        //            db.RSysCats.Add(RSysCat);
        //            await db.SaveChangesAsync();
        //        }
        //    }
        //    //================================


        //    //    try
        //    //{
        //    //    db.LReferenceTypes.Add(LReferenceType);
        //    //    await db.SaveChangesAsync();
        //    //}
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

        //    return CreatedAtRoute("DefaultApi", new { Id = RSysCat.Id }, RSysCat);
        //}
    }
}

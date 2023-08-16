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
    public class LReferenceTypesController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        
        [ResponseType(typeof(LReferenceType))]
        public async Task<IHttpActionResult> GetRefTypeByName(string Name)
        {
            var RefType = (from aa in db.LReferenceTypes.Where(p => p.Name == Name)
                           select new
                           {
                               aa.Id,
                               aa.CompanyCode,
                               aa.Name,
                               aa.IsEffectiveDated,
                               aa.CreatedById,
                               aa.CreatedDateTime,
                               aa.UpdatedById,
                               aa.UpdatedDateTime

                           }).FirstOrDefault();
            if (RefType == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SYSCAT")));
            }
            return Ok(RefType);
        }

        [ResponseType(typeof(LReferenceType))]
        public async Task<IHttpActionResult> GetRefTypeById(int id)
        {
            var RefType = (from aa in db.LReferenceTypes.Where(p => p.Id == id)
                           select new
                           {
                               aa.Id,
                               aa.CompanyCode,
                               aa.Name,
                               aa.IsEffectiveDated,
                               aa.CreatedById,
                               aa.CreatedDateTime,
                               aa.UpdatedById,
                               aa.UpdatedDateTime

                           }).FirstOrDefault();
            if (RefType == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SYSCAT")));
            }
            return Ok(RefType);
        }
        
        [ResponseType(typeof(LReferenceType))]
        public IHttpActionResult GetByCompanyCode(string CompanyCode, string UserName, string WorkFlow)
        {
            var LReferenceType = db.LReferenceTypes.Where(p => p.CompanyCode == CompanyCode).Select(aa => new
            {
                aa.Id,
                aa.CompanyCode,
                aa.Name,
                aa.IsEffectiveDated,
                aa.CreatedById,
                aa.UpdatedById,
                aa.CreatedDateTime,
                aa.UpdatedDateTime
            }).OrderByDescending(aa => aa.Id);
            if (LReferenceType == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Reference Type")));
            }
            return Ok(LReferenceType);
        }               

        [ResponseType(typeof(LReferenceType))]
        public IHttpActionResult GetForEdit(string CompanyCode,int? ExistingTypeId, string UserName, string WorkFlow)
        {
            //RefernceType for which reference data is not available and if available should not be in Cancelled status.
            if (ExistingTypeId != 0)
            {
                //also include the existing Referencetype
                var xx = db.Database.SqlQuery<LReferenceType>("select * from LReferenceTypes rt  where rt.CompanyCode = {0} and rt.id={1} OR rt.id not in (select distinct ReferenceTypeId from LReferences where WFStatus != 'Cancelled') order by Name", CompanyCode, ExistingTypeId).ToList();
                return Ok(xx);
            }
            else
            {
                var xx = db.Database.SqlQuery<LReferenceType>("select * from LReferenceTypes rt  where rt.CompanyCode = {0} and rt.id not in (select distinct ReferenceTypeId from LReferences where WFStatus != 'Cancelled') order by Name", CompanyCode).ToList();
                return Ok(xx);
            }
        }        
       
        [HttpPost]
        [ResponseType(typeof(LReferenceType))]
        public async Task<IHttpActionResult> Post(LReferenceType lreftype)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Reference Type")));
            }
            //need to remove transactions, as its not required in this scenario
            try
            {               
                db.LReferenceTypes.Add(lreftype);
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

            return CreatedAtRoute("DefaultApi", new { id = lreftype.Id }, lreftype);
        }

        [ResponseType(typeof(LReferenceType))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            LReferenceType LReferenceType = await db.LReferenceTypes.FindAsync(id);
            if (LReferenceType == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Reference Type")));
            }

            try
            {
                db.LReferenceTypes.Remove(LReferenceType);
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
            return Ok(LReferenceType);
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_LReferenceTypes_LCompanySpecificColumns_ReferenceTypeId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Reference Type", "Company Specific Columns"));
            else if (SqEx.Message.IndexOf("FK_LReferenceTypes_LReferences_ReferencyTypeId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Reference Type", "References"));
            else if (SqEx.Message.IndexOf("UQ_LReferenceTypes_CompanyCode_Name", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "Reference Type"));
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

       
        private bool LReferenceTypeExists(int id)
        {
            return db.LReferenceTypes.Count(e => e.Id == id) > 0;
        }

        // PUT: api/LReferenceTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(int id, LReferenceType LReferencetype)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Reference Type")));
            }

            if (!LReferenceTypeExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Reference Type")));
            }

            if (id != LReferencetype.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Reference Type")));
            }
            try
            {
                db.Entry(LReferencetype).State = EntityState.Modified;
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
            return Ok(LReferencetype);
        }


        /*Below code is commented by Rakhi Singh on 3rd Aug as per requirement*/
        //[ResponseType(typeof(LReferenceType))]
        //public IHttpActionResult GetByRefDataUnavailable(string CompanyCode, string UserName, string WorkFlow)
        //{
        //    //RefernceType for which reference data is not available and if available should not be in Cancelled status.
        //    var xx = db.Database.SqlQuery<LReferenceType>("select * from LReferenceTypes rt  where rt.CompanyCode = {0} and rt.id not in (select distinct ReferenceTypeId from LReferences where WFStatus != 'Cancelled')", CompanyCode).ToList();
        //    return Ok(xx);
        //}

        //[HttpPost]       
        //[ResponseType(typeof(LReferenceType))]
        //public async Task<IHttpActionResult> PostLReferenceType(LReferenceType LReferenceType, string UserName, string WorkFlow)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Reference Type")));
        //    }

        //    //================================
        //    try
        //    {
        //        if (db.LReferenceTypes.Where(p => p.Id == LReferenceType.Id).Where(p => p.CompanyCode == LReferenceType.CompanyCode).Count() > 0)
        //        {
        //            db.Entry(LReferenceType).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            LReferenceType.Id = 0;//To override the Id generated by grid
        //            db.LReferenceTypes.Add(LReferenceType);
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

        //    return CreatedAtRoute("DefaultApi", new { Id = LReferenceType.Id }, LReferenceType);
        //}

    }
}

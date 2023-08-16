
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
        public class RLocalPobTypesController : ApiController
        {
            private RELYDevDbEntities db = new RELYDevDbEntities();
            
            [ResponseType(typeof(RLocalPobType))]
            public async Task<IHttpActionResult> GetRLocalPobType(int id)
            {
                var rlocalpob = (from aa in db.RLocalPobTypes.Where(p => p.Id == id)
                                select new
                                {
                                    aa.Id,
                                    aa.CompanyCode,
                                    aa.Name,
                                    aa.Description
                                }).FirstOrDefault();
                if (rlocalpob == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LocalPOBType")));
                }
                return Ok(rlocalpob);
            }
            
            [ResponseType(typeof(RLocalPobType))]
            public async Task<IHttpActionResult>  GetRLocalPobTypeByName(string Name)
            {
                var rlocalpob =  (from aa in  db.RLocalPobTypes.Where(p => p.Name == Name)
                              select new
                              {
                                  aa.Id,
                                  aa.CompanyCode,
                                  aa.Name,
                                  aa.Description
                              }).FirstOrDefault();
                if (rlocalpob == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LocalPOBType")));
                }
                return Ok(rlocalpob);
            }
            
            [ResponseType(typeof(RLocalPobType))]
            public IHttpActionResult GetByCompanyCode(string CompanyCode)
            {
                var rlocalpob = (from aa in db.RLocalPobTypes.Where(p => p.CompanyCode == CompanyCode)
                                select new
                                {
                                    aa.Id,
                                    aa.CompanyCode,
                                    aa.Name,
                                    aa.Description

                                }).OrderByDescending(aa => aa.Name);
                if (rlocalpob == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LocalPOBType")));
                }
                return Ok(rlocalpob);
            }
            
            [HttpPost]
            [ResponseType(typeof(RLocalPobType))]
            public async Task<IHttpActionResult> Post(RLocalPobType rlocalpob)
            {
                if (!ModelState.IsValid)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "RLocalPOB Type")));
                }
                //need to remove transactions, as its not required in this scenario
                try
                {
                    db.RLocalPobTypes.Add(rlocalpob);
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

                return CreatedAtRoute("DefaultApi", new { id = rlocalpob.Id }, rlocalpob);
            }
       
            [ResponseType(typeof(void))]
            public async Task<IHttpActionResult> Put(RLocalPobType rlocalpob,int id)
            {
                if (!ModelState.IsValid)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RLocalPOB Type")));
                }
                if (!RLopobTypeExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RLocalPOB Type")));
                }

                if (id != rlocalpob.Id)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RLocalPOB Type")));
                }
                try
                {
                    db.Entry(rlocalpob).State = EntityState.Modified;
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
                return Ok(rlocalpob);
            }        
        
            [ResponseType(typeof(RLocalPobType))]
            public async Task<IHttpActionResult> Delete(int id)
            {
                RLocalPobType RLocalPobType = await db.RLocalPobTypes.FindAsync(id);
                if (RLocalPobType == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RLocalPob Type")));
                }

                try
                {
                    db.RLocalPobTypes.Remove(RLocalPobType);
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
                return Ok(RLocalPobType);
            }
        
            private bool RLopobTypeExists(int id)
            {
                return db.RLocalPobTypes.Count(e => e.Id == id) > 0;
            }

            private string GetCustomizedErrorMessage(Exception ex)
            {
                //Convert the exception to SqlException to get the error message returned by database.
                var SqEx = ex.GetBaseException() as SqlException;
                //Depending upon the constraint failed return appropriate error message
                //if (SqEx.Message.IndexOf("FK_LReferenceTypes_LCompanySpecificColumns_ReferenceTypeId", StringComparison.OrdinalIgnoreCase) >= 0)
                //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Reference Type", "Company Specific Columns"));
                if (SqEx.Message.IndexOf("FK_RLocalPobTypes_LLocalPobs_LocalPobTypeId", StringComparison.OrdinalIgnoreCase) >= 0)
                    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Reference Type", "Local Pobs"));
                else if (SqEx.Message.IndexOf("UQ_RLocalPobTypes_CompanyCode_Name", StringComparison.OrdinalIgnoreCase) >= 0)
                    return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "Local Pobs Types"));
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

            /*Below code is commented by Rakhi Singh on 3rd Aug during review with Mr.Vikas*/
            //[HttpPost]
            ////[HttpGet]
            //// POST: api/RLocalPOBType
            //[ResponseType(typeof(RLocalPobType))]
            //public async Task<IHttpActionResult> PostRLocalPobType(RLocalPobType RLocalPobType, string UserName, string WorkFlow)
            //{
            //    if (!ModelState.IsValid)
            //    {
            //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "RLocal Pob Type")));
            //    }

            //    //================================
            //    try
            //    {
            //        if (db.RLocalPobTypes.Where(p => p.Id == RLocalPobType.Id).Where(p => p.CompanyCode == RLocalPobType.CompanyCode).Count() > 0)
            //        {
            //            db.Entry(RLocalPobType).State = EntityState.Modified;
            //            await db.SaveChangesAsync();
            //        }
            //        else
            //        {
            //            RLocalPobType.Id = 0;//To override the Id generated by grid
            //            db.RLocalPobTypes.Add(RLocalPobType);
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

            //    return CreatedAtRoute("DefaultApi", new { Id = RLocalPobType.Id }, RLocalPobType);
            //}
        
        }
}

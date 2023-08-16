using RELY_API.Models;
using RELY_API.Utilities;
using System;
//using System.Collections.Generic;
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
    public class LReconFileFormatsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        //GetLReconFileFormat/ByCompanyCode
        public IHttpActionResult GetByCompanyCode(string CompanyCode/*, string UserName, string WorkFlow*/)
        {
            var xx = (from aa in db.LReconFileFormats
                      join bb in db.RSysCats on aa.SysCatId equals bb.Id
                      where (aa.CompanyCode == CompanyCode)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.SysCatId,
                          bb.SysCat,
                          aa.FormatName
                      }).OrderByDescending(p => p.Id); //Newest item will apear top of the grid because of this ordering.
            return Ok(xx);
        }

        //POST: api/LReconFileFormats/
        [HttpPost]
        [ResponseType(typeof(LReconFileFormat))]
        public async Task<IHttpActionResult> POST(LReconFileFormat LReconFileFormat/*,string UserName, string WorkFlow*/)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "LReconFileFormat")));
            }

            try
            {
                if(db.LReconFileFormats.Where(p=>p.Id==LReconFileFormat.Id).Where(p=>p.CompanyCode==LReconFileFormat.CompanyCode).Count() > 0)
                {
                    db.Entry(LReconFileFormat).State= EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    LReconFileFormat.Id = 0;
                    db.LReconFileFormats.Add(LReconFileFormat);
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
            return CreatedAtRoute("DefaultApi", new { Id = LReconFileFormat.Id }, LReconFileFormat);
        }

       
      [HttpDelete]
        //[ResponseType(typeof(LReconFileFormat))]
        public async Task<IHttpActionResult> Delete(int id,string CompanyCode)
        {
           // string errorMessage = "";
            LReconFileFormat lReconFileFormat = await db.LReconFileFormats.FindAsync(id);
            if(lReconFileFormat==null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LReconFileFormat")));
            }

            using (var transaction = db.Database.BeginTransaction())
            {

                try
                {
                    
                    //Before deleting FileFormatId, we need to make sure that, Recon batches should not associated with it, if they exist then check whether there are records corresponding to these batches if yes then throw exception and prevent deletion of FormatId, otherwise delete batch numbers first and then continue the process
                    var ExistingData = db.LReconBatches.Where(p => p.FileFormatId == id).Where(p => p.CompanyCode == CompanyCode).ToList();
                    if (ExistingData.Count != 0)
                    {
                        for (int i = 0; i < ExistingData.Count; i++)
                        {

                            //Get batch no in a variable because direct passingb value in LReconBucket Query was throwing exception
                            int batchNo = ExistingData[i].BatchNumber;
                            var ReconBucketData= db.LReconBuckets.Where(p => p.BatchNumber == batchNo).Where(p => p.CompanyCode == CompanyCode).ToList();
                            if (ReconBucketData.Count != 0)
                            {
                                //errorMessage = "Can Not delete";
                                //throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ReconFileFormat", "ReconBaches")));//type 2 error
                                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.CanNotUpdateDeleteErrorMessage,"ReconFileFormat","ReconBatch")));
                            }
                            else
                            {
                                //This block will execute if batch no exist but no records in LReconBucket associated with this batch. so delete the batch and proceed
                                LReconBatch lReconBatch = await db.LReconBatches.FindAsync(ExistingData[i].Id);
                                db.LReconBatches.Remove(lReconBatch);
                                await db.SaveChangesAsync();
                            }

                        }
                        
                    }
                 
                        //If No batch is exist then delete ReconColumnMapping before deleting FileFormatId
                        var MappingData = db.LReconColumnMappings.Where(p => p.FileFormatId == id).Where(p => p.CompanyCode == CompanyCode).ToList();
                        db.LReconColumnMappings.RemoveRange(MappingData);
                        await db.SaveChangesAsync();

                        //Now delete the FileFormatId
                        db.LReconFileFormats.Remove(lReconFileFormat);
                        await db.SaveChangesAsync();
                    
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
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
                transaction.Commit();
            }
           

            return Ok();
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(LReconFileFormat lrff, int id)
        {         

            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "LReconFileFormat")));
            }
            if (!lreconfileformatExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LReconFileFormat")));
            }

            if (id != lrff.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "LReconFileFormat")));
            }
            try
            {
                db.Entry(lrff).State = EntityState.Modified;
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
            return Ok(lrff);
        }

        private bool lreconfileformatExists(int id)
        {
            return db.LReconFileFormats.Count(e => e.Id == id) > 0;
        }

        [HttpGet]
        public IHttpActionResult GetById(int id)
        {
            var xx = (from aa in db.LReconFileFormats.Where(p => p.Id == id)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.FormatName,
                          aa.LReconBatches,
                          aa.LReconColumnMappings,
                          aa.RSysCat,
                          aa.SysCatId                          
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
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_LReconFileFormats_LReconBatches_FileFormatId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "RECON FILE FORMAT", "RECON BATCHES"));
            else if (SqEx.Message.IndexOf("FK_LReconFileFormats_LReconColumnMapping_FileFormatId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "RECON FILE FORMAT", "RECON COLUMN MAPPING"));
            else if
                (SqEx.Message.IndexOf("UQ_LReconFileFormats_CompanyCode_SysCatId_FormatName", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "RECON FILE FORMAT"));
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

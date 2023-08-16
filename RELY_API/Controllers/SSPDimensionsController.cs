using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class SSPDimensionsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        [HttpPut]
        public async Task<IHttpActionResult> DetachSSP(int EntityId, string EntityType)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Entity")));
            }
            try
            {
                if ("Product".Equals(EntityType))
                {
                    var LProduct = db.LProducts.Find(EntityId);
                    if (LProduct != null) { 
                        LProduct.SspId = null;
                        db.Entry(LProduct).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PRODUCT")));
                    }
                    
                }
                else if("LLocalPob".Equals(EntityType))
                {
                    var Lpob = db.LLocalPobs.Find(EntityId);
                    if (Lpob != null)
                    {
                        Lpob.SspId = null;
                        db.Entry(Lpob).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PRODUCT")));
                    }
                    
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
            return Ok();
        }
        [HttpPut]
        public async Task<IHttpActionResult> AttachSSP(int EntityId, int SspId,string EntityType)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Entity")));
            }
            try
            {
                if ("Product".Equals(EntityType))
                {
                    var LProduct = db.LProducts.Find(EntityId);
                    if (LProduct != null)
                    {
                        LProduct.SspId = SspId;
                        db.Entry(LProduct).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PRODUCT")));
                    }

                }
                else if ("LLocalPob".Equals(EntityType))
                {
                    var Lpob = db.LLocalPobs.Find(EntityId);
                    if (Lpob != null)
                    {
                        Lpob.SspId = SspId;
                        db.Entry(Lpob).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        //Find out whether localpob related to ant product or not
                        var LProductPob = db.LProductPobs.Where(p => p.PobCatalogueId == Lpob.PobCatalogueId).FirstOrDefault();
                        if(LProductPob != null)
                        {
                            var product = db.LProducts.Find(LProductPob.ProductId);
                            if(product.SspId == null)
                            {
                                //we need to default it with LocalPob SSPID
                                product.SspId = SspId;
                                db.Entry(product).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LocalPob")));
                    }

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
            return Ok();
        }

        public async Task<IHttpActionResult> GetById(int Id)
        {
            var xx = db.SSPDimensions.Where(a => a.Id == Id).FirstOrDefault();
            return Ok(xx);
        }
        public async Task<IHttpActionResult> GetBySspId(int SspId,string CompanyCode)
        {
            var xx = db.SSPDimensions.Where(a => a.SspId == SspId).Where(a=> a.CompanyCode == CompanyCode).ToList().OrderByDescending(a=>a.Id);
            return Ok(xx);
        }

        public async Task<IHttpActionResult> GetSSPIdForNew(string CompanyCode)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
           
            SqlCommand command = new SqlCommand("select ISNULL(max(SspId),0)+1 from SSPDimensions where CompanyCode=@CompanyCode", conn);
            command.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            int SspId = (int)(command.ExecuteScalar());
            return Ok(SspId);
        }
        public IHttpActionResult GetExistingSspsCount(string CompanyCode)
        {
            var sqlQuery = "select  count(*) from  SSPDimensions where CompanyCode = {0} ";
            var counts = db.Database.SqlQuery<int>(sqlQuery, CompanyCode).FirstOrDefault();
            return Ok(counts);

        }
        public async Task<IHttpActionResult> GetExistingSsps(string CompanyCode, string sortdatafield, string sortorder, string FilterQuery, int PageNumber, int PageSize)
        {

            var Query = "Exec [SpGetExistingSSPs] @CompanyCode,@pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@pagesize", PageSize);
            cmd.Parameters.AddWithValue("@pagenum", PageNumber);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            var dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);

        }
        [HttpPost]
        public async Task<IHttpActionResult> Post(SSPDimension sSPDimension, string Source,string EntityId)
        {
            //if (!ModelState.IsValid)
            //{
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "LDropdowns")));
            //}
            //need to remove transactions, as its not required in this scenario
            using (var transaction = db.Database.BeginTransaction())
            {

                try
                {
                    //if (!string.IsNullOrEmpty(Source) && "Product".Equals(Source))
                    //{
                    //    int intProductId = Convert.ToInt32(EntityId);
                    //    var Product = db.LProducts.Where(a => a.Id == intProductId).FirstOrDefault();
                    //    Product.SspId = sSPDimension.SspId;
                    //    db.Entry(sSPDimension).State = EntityState.Modified;
                    //    await db.SaveChangesAsync();
                    //}
                    //if (!string.IsNullOrEmpty(Source) && "LocalPob".Equals(Source))
                    //{
                    //    int intEntityId = Convert.ToInt32(EntityId);
                    //    var pob = db.LLocalPobs.Where(a => a.Id == intEntityId).FirstOrDefault();
                    //    pob.SspId = sSPDimension.SspId;
                    //    db.Entry(sSPDimension).State = EntityState.Modified;
                    //    await db.SaveChangesAsync();
                    //}
                    int SourceId  = 0;
                    if (!string.IsNullOrEmpty(Source)){
                        SourceId = Convert.ToInt32(EntityId);
                    }
                    db.SpManageSSPDimension(sSPDimension.SspId, sSPDimension.SspAmount, sSPDimension.EffectiveStartDate, sSPDimension.EffectiveEndDate, sSPDimension.CompanyCode, sSPDimension.CreatedById, "Add", Source, SourceId);

                   
                    //previous logic
                    ////EnDt of previous will be -1 StDt of new version.
                    //var PreviousVersionEndDate = sSPDimension.EffectiveStartDate.AddDays(-1);
                    //string qry = "update SSPDimensions set EffectiveEndDate= {1} " +
                    //    "from SSPDimensions t1 inner join (select SspId, max(EffectiveStartDate) as MaxDate from SSPDimensions group by SspId ) t2 " +
                    //    "on t1.SspId = t2.SspId and t1.EffectiveStartDate = t2.MaxDate where t1.SspId = {0}";
                    //db.Database.ExecuteSqlCommand(qry, sSPDimension.SspId, PreviousVersionEndDate);
                    //db.SSPDimensions.Add(sSPDimension);
                    //await db.SaveChangesAsync();
                    //if (!string.IsNullOrEmpty(Source) && "Product".Equals(Source))
                    //{
                    //    int intProductId = Convert.ToInt32(EntityId);
                    //    var Product = db.LProducts.Where(a => a.Id == intProductId).FirstOrDefault();
                    //    Product.SspId = sSPDimension.SspId;
                    //    db.Entry(sSPDimension).State = EntityState.Modified;
                    //    await db.SaveChangesAsync();
                    //}
                    //if (!string.IsNullOrEmpty(Source) && "LocalPob".Equals(Source))
                    //{
                    //    int intEntityId = Convert.ToInt32(EntityId);
                    //    var pob = db.LLocalPobs.Where(a => a.Id == intEntityId).FirstOrDefault();
                    //    pob.SspId = sSPDimension.SspId;
                    //    db.Entry(sSPDimension).State = EntityState.Modified;
                    //    await db.SaveChangesAsync();
                    //}
                }
                catch (DbEntityValidationException dbex)
                {
                    transaction.Rollback();
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((System.Net.HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
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
                        throw ex;//This exception will be handled in FilterConfig's CustomHandler
                    }
                }
                transaction.Commit();
            }
            return CreatedAtRoute("DefaultApi", new { id = sSPDimension.Id }, sSPDimension);
        }
        public async Task<IHttpActionResult> Delete(int id)
        {
            SSPDimension sSPDimension = await db.SSPDimensions.FindAsync(id);
            if (sSPDimension == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "TEMPLATE")));
            }
            try
            {
                    db.SSPDimensions.Remove(sSPDimension);
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
            return Ok();
        }


        private bool SspExists(int id)
        {
            return db.SSPDimensions.Count(e => e.Id == id) > 0;
        }
        public async Task<IHttpActionResult> Put(SSPDimension sSPDimension, int id)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "SSP")));
            }
            if (!SspExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SSP")));
            }

            if (id != sSPDimension.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "SSP")));
            }
            try
            {
                db.Entry(sSPDimension).State = EntityState.Modified;
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

            return Ok(sSPDimension);
        }

        public async Task<IHttpActionResult> GetDataForGrid(string EntityType, int EntityId,string CompanyCode)
        {
            var Query = "Exec [SpGetSSPDetails] @EntityType,@EntityId,@CompanyCode";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@EntityType", EntityType);
            cmd.Parameters.AddWithValue("@EntityId", EntityId);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            var dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }
        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as System.Data.SqlClient.SqlException;
            if (SqEx.Message.IndexOf("UQ_SSPDimensions_SSPId_EffectiveStartDate", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "SSP"));
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
    } 
 }
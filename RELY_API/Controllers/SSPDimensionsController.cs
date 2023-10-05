using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
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


        // Author: Bharat
        // To Download grid Data in excel
        [HttpGet]
        public IHttpActionResult DownloadGenericDataGrid(string Workflow, string UserName, int LoggedInUserId, int LoggedInRoleId, string CompanyCode)
        {
            var Query = "select SspId,SspAmount,FORMAT(EffectiveStartDate,'dd/MM/yyyy') as 'Start Date',FORMAT(EffectiveEndDate,'dd/MM/yyyy') as 'End Date',NULL as 'Operation' from SSPDimensions ORDER BY SspId DESC";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@Workflow", Workflow);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
            cmd.Parameters.AddWithValue("@LoggedInRoleId", LoggedInRoleId);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            var dt = Globals.GetDataTableUsingADO(cmd);

            string path = ConfigurationManager.AppSettings["RelyTempPath"] + "/";
            string Filename = Workflow + "_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            if (dt.Columns.Count > 0)
            {
                Globals.ExportToExcel(dt, path, Filename);
            }

            if (!string.IsNullOrEmpty(Filename))
            {
                string fullpath = path + "\\" + Filename;//
                string localpath = fullpath;
                string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + Filename;
                Globals.UploadFileToS3(localpath, S3TargetPath);
            }
            GenericNameAndIdViewModel model = new GenericNameAndIdViewModel { Name = Filename };
            return Ok(model);
        }
        /// <summary>
        /// To Display the Grid data of upload excel
        /// </summary>
        /// <param name="CompanyCode"></param>
        /// <param name="AspnetUserid"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetByUserForLRequestUploadGrid(string CompanyCode, string AspnetUserid)
        {
            string Qry = "select Case lb.XStatus WHEN 'ValidationFailed' Then 0 else 1 END as IsImport,lb.Id,lb.XStatus,lb.XBatchNumber,isnull(lb.XRecordCount,0) as XRecordCount,lb.XUploadStartDateTime,lbf.LbfFileName from XBatches lb join XBatchFiles lbf on  lb.id = lbf.LbfBatchId where lb.XBatchType='SSPDimensions' and lb.XCompanyCode = {0} and lb.XUpdatedBy={1} and lb.XStatus <> 'Deleted' order by lb.Id desc ";
            var xx = db.Database.SqlQuery<LBatchViewModelGrid>(Qry, CompanyCode, AspnetUserid).ToList();
            return Ok(xx);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetUploadSSPs(string FileName, string UserName, string LoggedInRoleId, string iCompanyCode, string WorkflowName, string UpdatedBy)
        {

            var BatchModel = new XBatch();
            var RawQuery = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo.SQ_BatchNumber");
            var Task = RawQuery.SingleAsync();
            var BatchNumber = Task.Result;
            Task.Dispose();

            using (var transaction = db.Database.BeginTransaction())
            {
                BatchModel = new XBatch
                {
                    XCompanyCode = iCompanyCode,
                    XUpdatedBy = UpdatedBy,
                    XStatus = "Saved",
                    XBatchNumber = BatchNumber,
                    XBatchType = "SSPDimensions",
                    XRawDataType = null,
                    XUploadFinishDateTime = DateTime.UtcNow,
                    XAlteryxBatchNumber = null,
                    XComments = null,
                    BatchName = null,
                    XUploadStartDateTime = DateTime.UtcNow
                };
                db.XBatches.Add(BatchModel);
                await db.SaveChangesAsync();
                db.SaveChanges();
                var LBatchFiles = new XBatchFile { LbfBatchId = BatchModel.Id, LbfFileName = FileName, LbfFileTimeStamp = DateTime.UtcNow };
                db.XBatchFiles.Add(LBatchFiles);
                await db.SaveChangesAsync();

                try
                {
                    var CompanyDetails = db.GCompanies.Where(p => p.CompanyCode == iCompanyCode).FirstOrDefault();
                    DataTable dtdataSheet = null;

                    try
                    {
                        //Read Excel File data
                        dtdataSheet = ReadExcelData(FileName, iCompanyCode);

                        // Check the start & end date
                        dtdataSheet = CheckTableDate(dtdataSheet);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var models = new GErrorLog { UserName = "Rely", Controller = "LRequests", Method = "UploadPPM", ErrorDateTime = DateTime.UtcNow, StackTrace = ex.ToString(), SourceProject = "[Vodafone-Rely WebApi]" };
                        db.GErrorLogs.Add(models);
                        db.SaveChanges();
                    }
                    //Adding new columns into table
                    dtdataSheet = AddNewColumns(dtdataSheet, iCompanyCode, UpdatedBy, BatchNumber);

                    SqlBulkInsert(dtdataSheet);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    var models = new GErrorLog { UserName = "RELY", Controller = "LClaims", Method = "UploadClaims", ErrorDateTime = DateTime.UtcNow, StackTrace = ex.ToString(), SourceProject = "[Vodafone-Rely WebApi]" };
                    db.GErrorLogs.Add(models);
                    db.SaveChanges();
                    DataTable dtE = new DataTable();
                    dtE.Columns.Add("ExceptionMessage");
                    return Ok(dtE);
                }
            }

            #region commented by bharat
            var Query = "Exec dbo.ValidateXSSPDimensions @UserID,@CompanyCode,@BatchNo";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@UserID", UpdatedBy);
            cmd.Parameters.AddWithValue("@CompanyCode", iCompanyCode.ToString());
            cmd.Parameters.AddWithValue("@BatchNo", BatchNumber);
            DataTable dtErrors = new DataTable();
            dtErrors = Globals.GetDataTableUsingADO(cmd);
            return Ok();
            #endregion
        }
        //---------------------------------------------
        /// <summary>
        /// Here we are reading the excel sheet data
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="iCompanyCode"></param>
        /// <returns></returns>
        private DataTable ReadExcelData(string FileName, string iCompanyCode)
        {
            DataTable dtdataSheet = new DataTable();

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet TempdsSheet = new DataSet();
            try
            {
                string S3BucketPPMDataFilesFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                string S3TargetPath = "/" + iCompanyCode.ToLower() + "/" + S3BucketPPMDataFilesFolder + "/" + FileName;
                byte[] bytedata = Globals.DownloadFromS3(S3TargetPath);

                string fileExtension = System.IO.Path.GetExtension(FileName);
                string name = System.IO.Path.GetFileNameWithoutExtension(FileName);
                string FileName_New = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + "_UPLOAD" + fileExtension;

                string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
                string fullpath = path + "\\" + FileName_New;
                System.IO.File.WriteAllBytes(fullpath, bytedata);

                string excelConnectionString = ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"].Replace("{0}", fullpath);
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                excelConnection.Open();
                OleDbDataAdapter cmd2 = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [Sheet1$]", excelConnection);

                cmd2.Fill(TempdsSheet);
                dtdataSheet = TempdsSheet.Tables[0].Clone();
                foreach (DataRow filterrow in TempdsSheet.Tables[0].Rows)
                {
                    string Operation_Case = Convert.ToString(filterrow["Operation"]);
                    if (!string.IsNullOrEmpty(Operation_Case) && (string.Equals(Operation_Case.ToUpper(), "I") || string.Equals(Operation_Case.ToUpper(), "U")))
                    {
                        dtdataSheet.ImportRow(filterrow);
                    }
                }

                excelConnection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtdataSheet;
        }
        /// <summary>
        /// Here we are checking the sheet data date column format and converting it into the correct format
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private DataTable CheckTableDate(DataTable table)
        {

            try
            {
                foreach (DataRow row in table.Rows)
                {
                    DateTime dtOut = new DateTime();

                    if (!DateTime.TryParseExact(row["Start Date"].ToString(), "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtOut)) //DateTime.TryParse(row["Start Date"].ToString(), out dtOut)
                    {
                        row["Start Date"] = null;
                    }
                    else
                    {
                        row["Start Date"] = dtOut;
                    }

                    dtOut = new DateTime();
                    if (!DateTime.TryParseExact(row["End Date"].ToString(), "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtOut)) //DateTime.TryParse(row["End Date"].ToString(), out dtOut)
                    {
                        row["End Date"] = null;
                    }
                    else
                    {
                        row["End Date"] = dtOut;
                    }

                    row["Operation"] = row["Operation"].ToString().ToUpper();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        /// <summary>
        /// So Here we are adding few more columns into the Datatable
        /// </summary>
        /// <param name="table"></param>
        /// <param name="iCompanyCode"></param>
        /// <param name="updatedBy"></param>
        /// <param name="batchNumber"></param>
        /// <returns></returns>
        private DataTable AddNewColumns(DataTable table, string iCompanyCode, string updatedBy, int batchNumber)
        {
            try
            {
                System.Data.DataColumn newColumn1 = new System.Data.DataColumn("CompanyCode", typeof(System.String));
                newColumn1.DefaultValue = iCompanyCode;
                table.Columns.Add(newColumn1);

                System.Data.DataColumn newColumn2 = new System.Data.DataColumn("CreatedDateTime", typeof(System.DateTime));
                newColumn2.DefaultValue = DateTime.UtcNow;
                table.Columns.Add(newColumn2);

                System.Data.DataColumn newColumn3 = new System.Data.DataColumn("UpdatedDateTime", typeof(System.DateTime));
                newColumn3.DefaultValue = DateTime.UtcNow;
                table.Columns.Add(newColumn3);

                System.Data.DataColumn newColumn4 = new System.Data.DataColumn("CreatedById", typeof(System.String));
                newColumn4.DefaultValue = updatedBy;
                table.Columns.Add(newColumn4);

                System.Data.DataColumn newColumn5 = new System.Data.DataColumn("UpdatedById", typeof(System.String));
                newColumn5.DefaultValue = updatedBy;
                table.Columns.Add(newColumn5);

                System.Data.DataColumn newColumn6 = new System.Data.DataColumn("BatchNumber", typeof(System.Int32));
                newColumn6.DefaultValue = batchNumber;
                table.Columns.Add(newColumn6);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        /// <summary>
        /// So in this Method we are inserting the data into the DestinationTableName we Defined
        /// </summary>
        /// <param name="finalData"></param>
        private void SqlBulkInsert(DataTable finalData)
        {
            System.Data.SqlClient.SqlBulkCopy sqlBulk = new SqlBulkCopy(db.Database.Connection.ConnectionString);

            sqlBulk.ColumnMappings.Add("SspId", "SspId");
            sqlBulk.ColumnMappings.Add("SspAmount", "SspAmount");
            sqlBulk.ColumnMappings.Add("Start Date", "EffectiveStartDate");
            sqlBulk.ColumnMappings.Add("End Date", "EffectiveEndDate");
            sqlBulk.ColumnMappings.Add("Operation", "Operation");
            sqlBulk.ColumnMappings.Add("CompanyCode", "CompanyCode");
            sqlBulk.ColumnMappings.Add("CreatedDateTime", "CreatedDateTime");
            sqlBulk.ColumnMappings.Add("UpdatedDateTime", "UpdatedDateTime");
            sqlBulk.ColumnMappings.Add("CreatedById", "CreatedById");
            sqlBulk.ColumnMappings.Add("UpdatedById", "UpdatedById");
            sqlBulk.ColumnMappings.Add("BatchNumber", "BatchNumber");

            sqlBulk.DestinationTableName = "XSSPDimensions";

            try
            {
                sqlBulk.WriteToServer(finalData);
            }
            catch (Exception ex)
            {
                var models = new GErrorLog { UserName = "RELY", Controller = "SSPDimensions", Method = "UploadPPM", ErrorDateTime = DateTime.UtcNow, StackTrace = ex.ToString(), SourceProject = "[Vodafone-RELY WebApi]" };
                db.GErrorLogs.Add(models);
                db.SaveChanges();
            }
        }
        [HttpGet]
        public IHttpActionResult GetById(string CompanyCode, int Id)
        {
            string Qry = "select lb.Id,lb.XStatus,lb.XBatchNumber,lb.XRecordCount,lb.XUploadStartDateTime,lbf.LbfFileName from XBatches lb " +
                " join XBatchFiles lbf on  lb.id = lbf.LbfBatchId where lb.XBatchType='SSPDimensions' and lb.XCompanyCode = {0} and lb.Id={1}";
            var xx = db.Database.SqlQuery<LBatchViewModelForRequestGrid>(Qry, CompanyCode, Id).FirstOrDefault();
            return Ok(xx);
        }

        [HttpGet]
        public IHttpActionResult DownloadRequestUploadErrors(string CompanyCode, int BatchNumber)
        {
            string Filename = null;
            string Query = "select SspId as [Ssp Id],ValidationMessage as [Validation Message] from XSSPDimensions where CompanyCode = '" + CompanyCode + "' and BatchNumber = " + BatchNumber + " and ValidationMessage is not null";
            DataSet ds = new DataSet();
            DataTable dtPayee = Globals.GetDdatainDataTable(Query);
            ds.Tables.Add(dtPayee);
            ds.Tables[0].TableName = "XSSPDimensions";
            Filename = BatchNumber + "_SSPDimensionsUploadErrors" + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".xlsx";
            var TempPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/";
            var OutPutMessage = Globals.ExportDataSetToExcel(ds, TempPath, Filename, "all text", "dd.mm.yyyy");

            if (!string.IsNullOrEmpty(Filename))
            {
                string fullpath = TempPath + "\\" + Filename;
                string localpath = fullpath;
                string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + Filename;
                Globals.UploadFileToS3(localpath, S3TargetPath);
            }
            return Ok(Filename);
        }
        [HttpGet]
        public IHttpActionResult UploadValidatedRequestBatch(string CompanyCode, int BatchNumber, string AspNetUserId, int LoggedinRoleId, string Workflow)
        {
            string Query = "exec SpUploadValidatedSSPDimensions {0},{1},{2},{3}";
            db.Database.SqlQuery<List<Object>>(Query, CompanyCode, BatchNumber, AspNetUserId, LoggedinRoleId).FirstOrDefault();
            return Ok();
        }
    }
}
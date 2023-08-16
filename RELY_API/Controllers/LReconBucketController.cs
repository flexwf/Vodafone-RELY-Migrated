using CsvHelper;
using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RELY_API.Controllers
{
    public class LReconBucketController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
     
        //API function created by Rakhi Singh
        //API method to get list of columns for Recon Grid
        [HttpGet]
        public async Task<IHttpActionResult> GetDataForReconColumns(string ProductCode, Int32 SysCatId, string CompanyCode)
        {
            var xx = await db.Database.SqlQuery<string>("Select Label from LReconColumnMapping where FileFormatId = (select  FileFormatId from LReconBatches where BatchNumber = (select  max(BatchNumber) from LReconBucket where ProductCode = {0} and SysCatId = {1} AND CompanyCode  = {2} )) and Label is not null", ProductCode,SysCatId,CompanyCode).ToListAsync();
            
            return Ok(xx);
           
        }
        
        
        /// <summary>
        /// Created by Rakhi Singh
        /// API function to get data for columns of Recon grid
        /// </summary>
        /// <param name="ProductCode"></param>
        /// <param name="SysCatId"></param>
        /// <param name="CompanyCode"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetDataForRecon(string ProductCode, int SysCatId, string CompanyCode)
        {
            string strQuery = "Exec SpGetReconDataforProduct @ProductCode,@SysCatId,@CompanyCode";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@ProductCode", ProductCode);
            cmd.Parameters.AddWithValue("@SysCatId", SysCatId);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            DataTable dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
           
        }
        public IHttpActionResult GetMissingProducts(string CompanyCode)
        {
            string strQuery = "Exec SpGenerateMissingProductsReport @CompanyCode";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            DataTable dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }


        [HttpPost]
        public IHttpActionResult PostLReconBucket(string CompanyCode, int SysCatId, int FileFormatId, int LoggedInUserId, string filename)
        {
            try
            {
                db.Database.ExecuteSqlCommand("exec SpSaveReconData {0},{1}", SysCatId, CompanyCode);
                

            }
            catch(Exception ex)
            {
                throw ex;
            }
            //First save the file in Rely Temp by getting it from bucket
            //string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketReconBucketFolder"];
            //string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + filename;
            //var bytedata = Globals.DownloadFromS3(S3TargetPath);
            //string path = ConfigurationManager.AppSettings["RelyTempPath"];
            //string fullpath = path + "\\" + filename;
            //if (System.IO.File.Exists(fullpath))
            //{
            //    System.IO.File.Delete(fullpath);
            //}
            //DataTable dt1 = new DataTable();
            //DataTable dt2 = new DataTable();
            //string[] columns;
            //try
            //{
            //    System.IO.File.WriteAllBytes(fullpath, bytedata); // Save File
            //    OleDbConnection con = null;
            //    OleDbCommand cmd1 = null;
            //    FileInfo file = new FileInfo(filename);
            //    //read large files 
            //    try
            //    {
            //        string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + "\\;Extended Properties = 'text;HDR=Yes;FMT=Delimited(^)'; ";
            //        con = new System.Data.OleDb.OleDbConnection(connectionString);
            //        con.Open();
            //        cmd1 = new OleDbCommand(string.Format("SELECT * FROM [{0}]", file.Name), con);
            //        OleDbDataReader reader = cmd1.ExecuteReader();
            //        //Reading Header from file
            //        var columnNames = (string.Join("^", Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList()));
            //        reader.Close();
            //        //string strcolumnNames = columnNames[0].ToString();
            //         columns = columnNames.Split('^');
            //        OleDbDataAdapter adp = new OleDbDataAdapter(cmd1);
            //        adp.Fill(dt1);
            //        con.Close();

            //        for (int j = 0; j < columns.Count();j++)
            //        {

            //            dt2.Columns.Add(columns[j]);
            //        }

            //        for (int i = 0; i < dt1.Rows.Count; i++)
            //        {

            //            DataRow OriginalRow = dt1.Rows[i];
            //            string OriginalRowdata = OriginalRow.ItemArray[0].ToString();
            //            string[] OriginalRowdataArray = OriginalRowdata.Split('^');
            //            DataRow dr = dt2.NewRow();

            //            for (int c = 0; c < dt2.Columns.Count; c++)
            //            {
            //                dr[c] = OriginalRowdataArray[c];
            //            }

            //            dt2.Rows.Add(dr);
            //        }

            //    }

            //    catch (Exception ex)
            //    {
            //        //DebugEntry("exception ocuured in inner try-catch block :" + ex.StackTrace.ToString());
            //        throw ex;
            //    }

            //}
            //catch (Exception ex)
            //{
            //   // DebugEntry("exception ocuured in outer try-catch block:" + ex.StackTrace.ToString());
            //    throw ex;
            //}

            ////insertand getting updated batch from following method
            //int batchNo = GetBachNo(CompanyCode, FileFormatId, LoggedInUserId, filename);

            ////var Sequence_qry = db.Database.SqlQuery<Int64>("SELECT NEXT VALUE FOR dbo.SQ_ReconBatches");
            ////var Task = Sequence_qry.Single();
            ////int sequenceValue = 1;//sequence value needs to be updated with Task.result

            //using (var transaction = db.Database.BeginTransaction())
            //{
            //    try
            //    { 

            //    //After filling the data read from excel sheet in a data set this for loop reads the data row by row 
            //    for (int i = 1; i < dt2.Rows.Count; i++)
            //    {
            //        var model = new LReconBucket { CompanyCode = CompanyCode, SysCatId = SysCatId, BatchNumber = batchNo };
            //        for (int k = 0; k < columns.Count(); k++)
            //        {
            //            string Label = columns[k].ToString();
            //            var mapping = GetReconColumnMapping(Label, FileFormatId, CompanyCode);
            //            string AttributeName = null; bool isProductCodeColumn = false;
            //            if (mapping != null)
            //            {
            //                AttributeName = mapping.ColumnName;
            //                isProductCodeColumn = mapping.IsProductCodeColumn;
            //            }
            //            string AttributeValue = dt2.Rows[i][Label] as string;
            //            //Setting Value of ProductCode 
            //            if (isProductCodeColumn)
            //            {
            //                    //if this ProductCode already exist then remove first
            //                    var ExistingData = db.LReconBuckets.Where(p => p.ProductCode == AttributeValue);
            //                    db.LReconBuckets.RemoveRange(ExistingData);
            //                    model.ProductCode = AttributeValue;

            //            }

            //            if (!string.IsNullOrEmpty(AttributeName))
            //            {

            //                PropertyInfo propertyInfo = model.GetType().GetProperty(AttributeName);
            //                if (propertyInfo != null)
            //                {
            //                    var datatype = propertyInfo.PropertyType.Name;
            //                    propertyInfo.SetValue(model, Convert.ChangeType(AttributeValue, propertyInfo.PropertyType), null);
            //                }
            //            }
            //        }
            //        db.LReconBuckets.Add(model);
            //        db.SaveChanges();
            //    }
            //}
            //    catch (DbEntityValidationException dbex)
            //    {
            //        transaction.Rollback();
            //        var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
            //        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            //    }
            //    catch (Exception ex)
            //    {
            //        transaction.Rollback();
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
            //    transaction.Commit();
            //}
            return Ok();
        }


        
        private void DebugEntry(string value)
        {
            SqlConnection SqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
             string sql = "insert into debug(DebugValue,DebugDateTime) values(@value,@DT)";
             SqlCommand cmd1 = new SqlCommand(sql, SqlConnection);
             cmd1.Parameters.AddWithValue("@value", value);
             cmd1.Parameters.AddWithValue("@DT", DateTime.UtcNow);
             SqlConnection.Open();
             cmd1.ExecuteNonQuery();
             SqlConnection.Close();

            //24/09/2018
            //string SqlQuery = "insert into debug(DebugValue,DebugDateTime) values({0},{1})";
            //db.Database.ExecuteSqlCommand(SqlQuery, value, DateTime.UtcNow);           
            //db.SaveChanges();
        }
        private LReconColumnMapping GetReconColumnMapping(string Label, int FileFormatId, string CompanyCode)
        {
            var xx = db.LReconColumnMappings.Where(p => p.FileFormatId == FileFormatId).Where(p => p.CompanyCode == CompanyCode).Where(p => p.Label == Label).FirstOrDefault();
            return (xx);
        }


        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            
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


        public int GetBachNo(string CompanyCode, int FileFormatId, int LoggedInUserId, string filename)
        {


            var BatchNo = db.LReconBatches.Max(aa => aa.BatchNumber);
            BatchNo = BatchNo + 1;

            var model = new LReconBatch {
                CompanyCode = CompanyCode,
                FileFormatId = FileFormatId,
                BatchNumber = BatchNo,
                SourceFileName = filename,
                UploadMode = "Manual",
                CreatedById = LoggedInUserId,
                CreatedDateTime = DateTime.Now,
                UpdatedById = LoggedInUserId,
                UpdatedDateTime=DateTime.Now

            };
            db.LReconBatches.Add(model);
            db.SaveChanges();
            return (BatchNo);

            
        }


        [HttpGet]
        public IHttpActionResult ReadAndValidateCSVData(string UserName, string WorkFlow, int LoggedInUserId, string filename, int FileFormatId, string CompanyCode, int SysCatId)
        {
            ObjectParameter Resultforsp = new ObjectParameter("Result", typeof(int)); //return parameter is declared
            var Qry = "delete from TReconDataValidation where SysCatId ={0}";
            db.Database.ExecuteSqlCommand(Qry, SysCatId);
            db.SaveChanges();
            //save bulk data to temporary table.
            try
            {
                SaveBulkDataToDB(SysCatId, filename, CompanyCode, "TReconDataValidation", LoggedInUserId, FileFormatId);
            }
            catch (Exception ex)
            {
                //make entry in db.SpLogError

                //db.SpLogError("RELY-API", "LReconBucket", "ReadAndValidateCSVData", ex.StackTrace, "RELY", "Type1", ex.StackTrace, "resolution", "L2Admin", "field", 0, "New");

                db.SpLogError("RELY-API", "LReconBucket", "ReadAndValidateCSVData", ex.StackTrace, "RELY", "Type1", ex.StackTrace, "resolution", "L2Admin", "field", 0, "New", Resultforsp);
               
                db.SaveChanges();
                //send back error
                String ExceptionMsg = "Preliminary validation is failed.There could be several reasons for this. Please contact L2Admin.";
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, ExceptionMsg));
            }


            //Reaching here means Data is inserted into temp table and needs validation further. SpValidateTReconData does the same and returns the error row counts
            //var Result = "";
            //var Query = "Exec SpValidateTReconData @SysCatId,@CompanyCode,@Result output";
            //using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            //{
            //    conn.Open();
            //    SqlCommand cmd = new SqlCommand(Query);
            //    cmd.Connection = conn;
            //    cmd.Parameters.AddWithValue("@SysCatId", SysCatId);
            //    cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //    cmd.Parameters.Add("@Result", SqlDbType.Int);
            //    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            //    cmd.ExecuteNonQuery();
            //    Result = cmd.Parameters["@Result"].Value.ToString();
            //    conn.Close();
            //}
            //int ErrorRowsCount = 0;
            //if (!String.IsNullOrEmpty(Result))
            //{
            //    ErrorRowsCount = Convert.ToInt32(Result);
            //}

            ObjectParameter Result = new ObjectParameter("ErrorRowCount", typeof(int)); //return parameter is declared
            db.SpValidateTReconData(SysCatId, CompanyCode, Result).FirstOrDefault();
            int ErrorRowsCount = (int)Result.Value; //getting value of output parameter
            return Ok(ErrorRowsCount);
        }


        private void SaveBulkDataToDB(int SysCatId, string filename, string CompanyCode, string TableName,int LoggedInUserId,int FileFormatId)
        {
            //First save the file in Rely Temp by getting it from bucket
            string S3BucketReconBucketFolder = ConfigurationManager.AppSettings["S3BucketReconBucketFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReconBucketFolder + "/" + filename;
            var bytedata = Globals.DownloadFromS3(S3TargetPath);
            string path = ConfigurationManager.AppSettings["RelyTempPath"];
            string fullpath = path + "\\" + filename;


           if (System.IO.File.Exists(fullpath))
            {
                System.IO.File.Delete(fullpath);
            }
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable dt = null;
            
            System.IO.File.WriteAllBytes(fullpath, bytedata); // Save File
        

            dt = ReadCSVAndGenerateDataTable(path, filename);
           

            //DataTable TempDt = dt.Clone();
            DataTable TempDt = new DataTable();

            //Adding fixed columns to DataTable
            DataColumn BatchNumber = TempDt.Columns.Add("BatchNumber", typeof(int));
            BatchNumber.DefaultValue = GetBachNo(CompanyCode, FileFormatId, LoggedInUserId, filename);

            //DataColumn ESDColumn = TempDt.Columns.Add("EffectiveStartDate" );
            //DataColumn EEDColumn = TempDt.Columns.Add("EffectiveEndDate" );

            DataColumn strCompanyCode = TempDt.Columns.Add("CompanyCode", typeof(string));
            strCompanyCode.DefaultValue = CompanyCode;

            DataColumn iOrdinal = TempDt.Columns.Add("Ordinal", typeof(int));
            iOrdinal.DefaultValue = null;

             TempDt.Columns.Add("ProductCode", typeof(string));
            //Find ColumnName for ProductCode
            //var lblProductCode = db.Database.SqlQuery<string>("select Label from LReconColumnMapping where FileFormatId={0} and IsProductCodeColumn=1", FileFormatId).FirstOrDefault();

            DataColumn iSysCatId = TempDt.Columns.Add("SysCatId", typeof(int));
            iSysCatId.DefaultValue = SysCatId;

            DataColumn SrNo = TempDt.Columns.Add("SrNo");
            SrNo.DataType = System.Type.GetType("System.Int32");
            SrNo.AutoIncrement = true;
            SrNo.AutoIncrementSeed = 1;
            SrNo.AutoIncrementStep = 1;


            //set Flexible columns in TempDt
            var ColumnList = dt.Columns;
            for (int i = 0; i < ColumnList.Count; i++)
            {
                var mapping = GetReconColumnMapping(ColumnList[i].ToString(), FileFormatId, CompanyCode);
                TempDt.Columns.Add(mapping.ColumnName, typeof(string));

            }

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                TempDt.Rows.Add();
                for (int k=0;k<dt.Columns.Count;k++)
                {
                    
                    var Recondata = GetReconColumnMapping(dt.Columns[k].ToString(), FileFormatId, CompanyCode);
                    TempDt.Rows[j][Recondata.ColumnName] = dt.Rows[j][dt.Columns[k]];
                    if (Recondata.IsProductCodeColumn)
                    {
                        TempDt.Rows[j]["ProductCode"] = dt.Rows[j][dt.Columns[k]];
                    }
                 

                }



            }
            

            var SQLConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(SQLConnString))
            {
                // make sure to enable triggers
                // more on triggers in next post
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy
                    (
                    connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                    );

                // set the destination table name
                bulkCopy.DestinationTableName = TableName;
                foreach (DataColumn col in TempDt.Columns)
                {
                    // Set up the column mappings by name.
                    SqlBulkCopyColumnMapping col1 =
                        new SqlBulkCopyColumnMapping(col.ColumnName, col.ColumnName);
                    bulkCopy.ColumnMappings.Add(col1);
                }
                connection.Open();
                try
                {
                    // write the data in the "dataTable"
                    bulkCopy.WriteToServer(TempDt);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                    dt.Clear();
                    TempDt.Clear();
                }
            }
           

        }


        public static DataTable ReadCSVAndGenerateDataTable(String path, String filename)
        {
            //Here I am getting Encoding of the received file
            FileStream textfileStream = File.OpenRead(path + "\\" + filename);
            textfileStream.Position = 0;
            byte[] bomBytes = new byte[textfileStream.Length > 4 ? 4 : textfileStream.Length];
            textfileStream.Read(bomBytes, 0, bomBytes.Length);
            Encoding encodingFound = null;
            string EnCodeValue = "";
            encodingFound = DetectBOMBytes(bomBytes);

            //In some cases, It might possible that value of encodingFound variable is null that getting value from this variable is throwing error, so this condition is applied
            if (encodingFound == null)
            {
                EnCodeValue = "";
            }
            else
            {
                EnCodeValue = encodingFound.BodyName;
            }
            textfileStream.Close();

            //We used StreamReader instead of OleDbConnection because OleDb was not supporting Encoding and Error was coming in received file as encoded in UTF8.Therefore to get DataTable we use streamReader and commented OleDbConnection code
            DataTable dt = new DataTable();
            bool headerFlag = true;
            Encoding FileCodeValue = null;

            //StreamReader supports the following UniCode, In future more cases can be added to this condition
            switch (EnCodeValue)
            {
                case "utf-8":
                    FileCodeValue = Encoding.UTF8;
                    break;
                case "utf-7":
                    FileCodeValue = Encoding.UTF7;
                    break;
                case "utf-32":
                    FileCodeValue = Encoding.UTF32;
                    break;
                default:
                    FileCodeValue = Encoding.Default;
                    break;
            }

            using (StreamReader sr = new StreamReader(path + "\\" + filename, FileCodeValue, true))
            {
                String line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (headerFlag)
                    {
                        dt.Columns.Add(line);
                        headerFlag = false;
                    }
                    else
                    {
                        dt.Rows.Add(line);
                    }
                    
                }
            }
           

            DataTable dt2 = new DataTable();
            //Commented because it was not supporting Encoding or CHARSET parameter
            //FileInfo file = new FileInfo(filename);
            //using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + "\\;Extended Properties = 'text;HDR=Yes;FMT=Delimited(^)'; "))
            //{
            //    using (OleDbCommand cmd = new OleDbCommand(string.Format
            //                              ("SELECT * FROM [{0}]", file.Name), con))
            //    {
            //        con.Open();
            //        using (OleDbDataAdapter adp = new OleDbDataAdapter(cmd))
            //        {
            //            adp.Fill(dt);
            //        }

            //    }
            //}

            //Following code will return datatable in table form because csv was coming in singla row
            var strcolumnNames = dt.Columns;
            var columns = (strcolumnNames[0].ToString()).Split('^');

            for (int j = 0; j < columns.Count(); j++)
            {
                dt2.Columns.Add(columns[j]);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                DataRow OriginalRow = dt.Rows[i];
                string OriginalRowdata = OriginalRow.ItemArray[0].ToString();
                string[] OriginalRowdataArray = OriginalRowdata.Split('^');
                DataRow dr = dt2.NewRow();

                for (int c = 0; c < dt2.Columns.Count; c++)
                {
                    // int length = OriginalRowdataArray[c].Length;


                    if (OriginalRowdataArray.Length < c + 1)
                    {
                        dr[c] = null;

                    }
                    else
                    {
                        if (OriginalRowdataArray[c].Length > 255)
                        {
                            // int CharToRemove = length - 255;

                            OriginalRowdataArray[c] = OriginalRowdataArray[c].Substring(0, 254);
                        }
                        dr[c] = OriginalRowdataArray[c];
                    }


                }

                dt2.Rows.Add(dr);
            }

            return dt2;
        }

        //This function will return the received file , Encoding
        public static Encoding DetectBOMBytes(byte[] BOMBytes)
        {
            if (BOMBytes == null)
                throw new ArgumentNullException("Must provide a valid BOM byte array!", "BOMBytes");

            if (BOMBytes.Length < 2)
                return null;

            if (BOMBytes[0] == 0xff
                && BOMBytes[1] == 0xfe
                && (BOMBytes.Length < 4
                    || BOMBytes[2] != 0
                    || BOMBytes[3] != 0
                    )
                )
                return Encoding.Unicode;

            if (BOMBytes[0] == 0xfe
                && BOMBytes[1] == 0xff
                )
                return Encoding.BigEndianUnicode;

            if (BOMBytes.Length < 3)
                return null;

            if (BOMBytes[0] == 0xef && BOMBytes[1] == 0xbb && BOMBytes[2] == 0xbf)
                return Encoding.UTF8;

            if (BOMBytes[0] == 0x2b && BOMBytes[1] == 0x2f && BOMBytes[2] == 0x76)
                return Encoding.UTF7;

            if (BOMBytes.Length < 4)
                return null;

            if (BOMBytes[0] == 0xff && BOMBytes[1] == 0xfe && BOMBytes[2] == 0 && BOMBytes[3] == 0)
                return Encoding.UTF32;

            if (BOMBytes[0] == 0 && BOMBytes[1] == 0 && BOMBytes[2] == 0xfe && BOMBytes[3] == 0xff)
                return Encoding.GetEncoding(12001);

            return null;
        }



        //this method calls Sp for getting Invalid records from TReconDataValidation table with server side paging
        public IHttpActionResult GetInValidDataRecords(int SysCatId, int FileFormatId,string CompanyCode, string UserName, string WorkFlow, string sortdatafield, string sortorder, string FilterQuery, int PageNumber, int PageSize)
        {
            var Query = "Exec [SpGetInvalidRecordsTReconData] @SysCatId,@FileFormatId,@CompanyCode,@pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@SysCatId", SysCatId);
            cmd.Parameters.AddWithValue("@FileFormatId", FileFormatId);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@pagesize", PageSize);
            cmd.Parameters.AddWithValue("@pagenum", PageNumber);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            var dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }


    }
}

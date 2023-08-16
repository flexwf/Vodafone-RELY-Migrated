using RELY_API.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using System.Web;
using System.Data.OleDb;
using RELY_API.Utilities;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class S15ExtractsController : ApiController
    {
        //private RELYDevDbEntities1 db = new RELYDevDbEntities1();

        [HttpPost]//We used HttpPost here because HttpGet method doesn't accept Model
        
        public IHttpActionResult GetS15Extracts(List<S15ExtractsViewModel>  model,DateTime StartDate, DateTime EndDate, string CompanyCode, string ExportType, string FileFormat, bool IsAutomatic)
        {
            //StartDate and EndDate parameters are no longer in use. Can be abondened
            try
            { 
                string CSVfileName, ChecksumFileName;
                string OutPutMessage = "";
                string TempFileFolder = ConfigurationManager.AppSettings["LocalTempFileFolder"];
                List<string> ListOfFiles = new List<string>();
                string S3BucketS15ExtractFolder = ConfigurationManager.AppSettings["S3BucketS15ExtractFolder"];//This path is used when "download" option is selected from front End(CSV Download/Excel Download)
                string S3BucketS15ExtractOutFolder = ConfigurationManager.AppSettings["S3BucketS15ExtractOutFolder"];//This path is used when "Generate"(either Manual from front end or automatic from task Scheduler) option is selected 
                string S3TargetPath = "";
                string ChecksumString = "";
                string ExcelFileName = "";
                string ZipFileName = "";
                DataSet DSExcel = new DataSet();
               
                //As discussed with Jas Sir , DateTime Convention for all files(OC files, zip files in Excel or CSV) should be same
                string DateTimeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HHmm");
                if (ExportType.ToLower() == "generate")
                {
                    S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketS15ExtractOutFolder + "/";
                }
                else//That means ExportType="Download"
                {
                    S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketS15ExtractFolder + "/";
                }
                if (ExportType.ToLower() == "generate")//when Export is called for auto matic generation , do not add timestamp
                {
                    if (FileFormat == "Excel")
                    {
                        //ExcelFileName = "RELY-OC"   + ".xlsx";
                        ExcelFileName = "S_15_PO_Mapping_technisch_nur_Werte.xlsx";
                        ChecksumFileName = "RELY-CTRL"  + ".xlsx";
                        //ZipFileName = "RELY-OC" + ".zip";
                        ZipFileName = "S_15_PO_Mapping_technisch_nur_Werte" + ".zip";
                    }
                    else
                    {
                        ChecksumFileName = "CONTROL"   + ".csv";
                        ZipFileName = "S15_Extracts"  + ".zip";
                    }
                }
                else//manual export 
                {
                    if (FileFormat == "Excel")
                    {
                        //ExcelFileName = "S_15_PO_Mapping_technisch_nur_Werte-" + DateTimeStamp + ".xlsx";
                        ExcelFileName = "RELY-OC" + "-" + DateTimeStamp + ".xlsx";
                        ChecksumFileName = "RELY-CTRL-" + DateTimeStamp + ".xlsx";
                        ZipFileName = "RELY-OC" + "-" + DateTimeStamp + ".zip";
                       // ZipFileName = "S_15_PO_Mapping_technisch_nur_Werte-"  + DateTimeStamp + ".zip";
                    }
                    else
                    {
                        ChecksumFileName = "CONTROL-" + DateTimeStamp + ".csv";
                        ZipFileName = "S15_Extracts_" + DateTimeStamp + ".zip";
                    }
                }
                foreach (var Extract in model)
                {
                    DataTable dt = new DataTable(Extract.FileName);
                    if (Extract.ExtractFileType == "RefData")
                    {
                        string strQuery = "Exec SpGetReferencedata @SelecterType,@CompanyCode";
                        SqlCommand cmd = new SqlCommand(strQuery);
                        cmd.Parameters.AddWithValue("@SelecterType", Extract.Extracts);
                        cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                        dt = Globals.GetDataTableUsingADO(cmd);

                    }
                    else
                    {
                        string strQuery = "Exec SpS15Extracts @ExtractName,@CompanyCode";
                        SqlCommand cmd = new SqlCommand(strQuery);
                        cmd.Parameters.AddWithValue("@ExtractName", Extract.Extracts);
                        cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                        dt = Globals.GetDataTableUsingADO(cmd);
                    }
                    if (dt != null)
                    {
                        //dt returned from GetDataTableUsingADO method doesn't have TableName. So we passed TableName here
                        dt.TableName = Extract.FileName;
                        if (FileFormat.ToLower() == "excel")
                        {
                            //ListSheetNames.Add(newfileName);
                            DSExcel.Tables.Add(dt);//In case of Excel, we are filling datatable in dataset. Later we will use this dataset to call function GenericMethodForExportToExcel
                        }
                        else if (FileFormat.ToLower() == "csv")
                        {
                            CSVfileName = Extract.FileName + "-" + DateTimeStamp + ".csv";
                            CSVfileName = CSVfileName.Replace('/', '-');
                            CSVfileName = CSVfileName.Replace(" ", "");
                            CSVfileName = CSVfileName.Replace(":", "-");
                            //S3TargetPath += CSVfileName;
                            ListOfFiles.Add(CSVfileName);//Only in the case of csv, every file need to be added in ListOfFiles because In Excel only one file will be created with multiple sheets
                            OutPutMessage = Globals.ExportToCSV(dt, TempFileFolder, CSVfileName);
                            if (OutPutMessage == "Success")
                            {
                                StreamReader reader = new StreamReader(TempFileFolder + "/" + CSVfileName);
                                Globals.UploadStreamToS3(reader.BaseStream, S3TargetPath + CSVfileName);
                                ChecksumString = ChecksumString + CSVfileName + "," + GetChecksum(TempFileFolder, CSVfileName) + Environment.NewLine;//In case of csv, checksum result need to be generated of every file but in Excel it will be recorded only once after loop
                            }
                            else
                            {
                                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Some issue found while Creating CSV file")));
                            }
                        }
                    }
                }//End of Extract Foreach loop
                
                //Execution of Loop is over. In case  where FileFormat="Excel", now we got a DataSet which needs to be converted in an Excel file(containing as many sheets as many Extracts were requested)
                if (FileFormat.ToLower() == "excel")
                {

                    OutPutMessage = Globals.GenericMethodForExportToExcel(DSExcel, TempFileFolder, ExcelFileName, "all text", "dd.MM.yyyy");//mm changed to MM
                    if (OutPutMessage == "Success")
                    {
                        Globals.UploadFileToS3(TempFileFolder + "/" + ExcelFileName, S3TargetPath + ExcelFileName);
                        ListOfFiles.Add(ExcelFileName);
                        //Making a string which later on will be written in Control file(Checksum file)
                        ChecksumString = ExcelFileName + "," + GetChecksum(TempFileFolder, ExcelFileName) + Environment.NewLine;

                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Some issue found while Creating excel file")));
                    }
                }
                //Generate checksum file to accompany the Extracts
                ListOfFiles.Add(ChecksumFileName);
                if (ExportType == "Download")
                {
                    OutPutMessage = Globals.GetChecksumFile(ChecksumString, TempFileFolder, ChecksumFileName, FileFormat);
                    if (OutPutMessage == "Success")
                    {
                        Globals.UploadFileToS3(TempFileFolder + "/" + ChecksumFileName, S3TargetPath + ChecksumFileName);
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Some issue found while Creating excel checksum file")));
                    }
                }
                //Everything done and now compress the files in a single zip for downloading
                
                if (ExportType == "Download")
                {
                    OutPutMessage = Globals.GetZipFolder(ListOfFiles, TempFileFolder, ZipFileName, S3TargetPath + ZipFileName);
                    if (OutPutMessage == "Success")
                    {
                        return Ok(ZipFileName);
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Some issue found while Creating Zip file")));
                    }
                }
                //need to revisit this line during code review because of time constraint
                return Ok(ListOfFiles);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public void ExportToExcel(DataTable table, string tempfileName)
        {
            HttpContext context = HttpContext.Current;
            context.Response.Clear();
            
            //if (tempfileName != @"C:\RELYtemp\ExportTemplate.xlsx")
            //{
            //    tempfileName = @"C:\RELYtemp\ExportTemplate.xlsx";
            //}
            //if(!File.Exists)
            //{

            //}
            //if(tempfileName==null)
            //{
            //    tempfileName = @"C:\RELYtemp\ExportTemplate.xlsx";
            //}
            string query;
            OleDbCommand cmd;
            //OleDbConnection cnn;
            
            try
            {
                string cnStr = GetConnectionString(tempfileName, Types.Excel_2007_XML_xlsx, true, true);

                using (OleDbConnection cnn = new OleDbConnection(cnStr))
                { 
                    cnn.Open();

                //Drop the existing sheet(first Sheet)
                query = "DROP TABLE [Sheet1$]";
                cmd = new OleDbCommand(query, cnn);
                cmd.ExecuteNonQuery();

                //Create new sheet with our requirements
                query = "CREATE TABLE [Sheet1$] (";
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    query += table.Columns[i].ColumnName;
                    if (i + 1 == table.Columns.Count)
                        if (table.Columns[i].DataType == System.Type.GetType("System.Int32"))
                            query += " INT)";
                        else
                            query += " VARCHAR(255))";
                    else
                        if (table.Columns[i].DataType == System.Type.GetType("System.Int32"))
                        query += " INT,";
                    else
                        query += " VARCHAR(255),";
                }
                cmd = new OleDbCommand(query, cnn);
                cmd.ExecuteNonQuery();

                    //Insert Data
                    foreach (DataRow row in table.Rows)
                    {
                        string values = "(";
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            if (i + 1 == table.Columns.Count)
                            {
                                if (table.Columns[i].DataType == System.Type.GetType("System.Int32"))
                                    values += String.IsNullOrEmpty(row[i].ToString()) ? "0)" : row[i] + ")";
                                else
                                    values += "'" + System.Security.SecurityElement.Escape(row[i].ToString()) + "')";
                            }
                            else
                            {
                                if (table.Columns[i].DataType == System.Type.GetType("System.Int32"))
                                    values += String.IsNullOrEmpty(row[i].ToString()) ? "0," : row[i] + ",";
                                else
                                    values += "'" + System.Security.SecurityElement.Escape(row[i].ToString()) + "',";
                            }
                        }
                        query = String.Format("Insert into [Sheet1$] VALUES {0}", values);
                        cmd = new OleDbCommand(query, cnn);
                       // cnn.Open();
                        cmd.ExecuteNonQuery();
                        //cnn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
                return;
            }
            finally
            {
                //cmd = null;
                //if (cnn != null)
                //cnn.Close();
            }

            context.Response.ContentType = "application/ms-excel";
            context.Response.AppendHeader("Content-Disposition", "attachment;filename="+tempfileName);
            context.Response.WriteFile(tempfileName);
            //MemoryStream ms = new MemoryStream();
            //ms.Flush();
            //context.Response.BinaryWrite(ms.ToArray());

            //ms.WriteTo(context.Response.OutputStream);
        }

        private static string GetConnectionString(string fileName, string Type, bool isHeaderExists, bool TreatIntermixedAsText)
        {
            string cnnStr;
            string provider;

            if (Type == "Excel 5.0" || Type == "Excel 8.0")
                provider = "Microsoft.Jet.OLEDB.4.0";
            else
                provider = "Microsoft.ACE.OLEDB.12.0";

            cnnStr = "Provider=" + provider +
                        ";Data Source=" + fileName +
                        ";Extended Properties=\"" + Type +
                                               ";HDR=" + (isHeaderExists ? "Yes;\"" : "No;\"");

            return cnnStr;
        }

        struct Types
        {
            /// <summary>
            /// Excel 2007 XML (*.xlsx)
            /// </summary>
            public const string Excel_2007_XML_xlsx = "Excel 12.0 Xml";

            /// <summary>
            /// Excel 2007 Binary (*.xlsb)
            /// </summary>
            public const string Excel_2007_Binary_xlsb = "Excel 12.0";

            /// <summary>
            /// Excel 2007 Macro-enabled (*.xlsm)
            /// </summary>
            public const string Excel_2007_MacroEnabled_xlsm = "Excel 12.0 Macro";

            /// <summary>
            /// Excel 97/2000/2003 (*.xls)
            /// </summary>
            public const string Excel_97_2000_2003_xls = "Excel 8.0";

            /// <summary>
            /// Excel 5.0/95 (*.xls)
            /// </summary>
            public const string Excel_95_xls = "Excel 5.0";
        }


        //[HttpPost]
        //public HttpResponseMessage GetExcelFile(DataTable table,string fileName)
        //{

        //    var response = new HttpResponseMessage();
        //    fileName = Guid.NewGuid().ToString() + ".xlsx";
        //    string filePath = HttpContext.Current.Server.MapPath(String.Format("~/FileDownload/{0}", fileName));
        //    StringBuilder fileContent = new StringBuilder();
        //    //Get Data here
        //    DataTable dt = table;
        //    if (dt != null)
        //    {
        //        string str = string.Empty;
        //        foreach (DataColumn dtcol in dt.Columns)
        //        {
        //            fileContent.Append(str + dtcol.ColumnName);
        //            str = "\t";
        //        }
        //        fileContent.Append("\n");
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            str = "";
        //            for (int j = 0; j < dt.Columns.Count; j++)
        //            {
        //                fileContent.Append(str + Convert.ToString(dr[j]));
        //                str = "\t";
        //            }
        //            fileContent.Append("\n");
        //        }
        //    }
        //    // write the data into Excel file
        //    using (StreamWriter sw = new StreamWriter(fileName.ToString(), false))
        //    {
        //        sw.Write(fileContent.ToString());
        //    }
        //    IFileProvider FileProvider = new FileProvider();
        //    //Get the File Stream
        //    FileStream fileStream = FileProvider.Open(filePath);
        //    //Set response
        //    response.Content = new StreamContent(fileStream);
        //    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //    response.Content.Headers.ContentDisposition.FileName = fileName;
        //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/ms-excel");
        //    response.Content.Headers.ContentLength = fileStream.Length;
        //    return response; 

        //}
        //public void ExportToExcel(DataTable table, string fileName)
        //{
        //    HttpContext context = HttpContext.Current;
        //    context.Response.Clear();

        //    //Begin Table
        //    context.Response.Write("<table><tr>");

        //    //Write Header
        //    foreach (DataColumn column in table.Columns)
        //    {
        //        context.Response.Write("<th>" + column.ColumnName + "</th>");
        //    }
        //    context.Response.Write("</tr>");

        //    //Write Data
        //    foreach (DataRow row in table.Rows)
        //    {
        //        context.Response.Write("<tr>");
        //        for (int i = 0; i < table.Columns.Count; i++)
        //        {
        //            context.Response.Write("<td>" + row[i].ToString().Replace(",", string.Empty) + "</td>");
        //        }
        //        context.Response.Write("</tr>");
        //    }

        //    //End Table
        //    context.Response.Write("</table>");

        //    context.Response.ContentType = "application/ms-excel";
        //    context.Response.AppendHeader("Content-Disposition", "attachment;filename="+fileName);
        //    context.Response.End();
        //}


        //public static class ZipHelper
        //{
        //    public static void ZipFiles(string path, IEnumerable<string> files,
        //           CompressionOption compressionLevel = CompressionOption.Normal)
        //    {
        //        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        //        {
        //            ZipHelper.ZipFilesToStream(fileStream, files, compressionLevel);
        //        }
        //    }

        //    public static byte[] ZipFilesToByteArray(IEnumerable<string> files,
        //           CompressionOption compressionLevel = CompressionOption.Normal)
        //    {
        //        byte[] zipBytes = default(byte[]);
        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            ZipHelper.ZipFilesToStream(memoryStream, files, compressionLevel);
        //            memoryStream.Flush();
        //            zipBytes = memoryStream.ToArray();
        //        }

        //        return zipBytes;
        //    }

        //    private static void ZipFilesToStream(Stream destination,
        //      IEnumerable<string> files, CompressionOption compressionLevel)
        //    {
        //        using (Package package = Package.Open(destination, FileMode.Create))
        //        {
        //            foreach (string path in files)
        //            {
        //                // fix for white spaces in file names (by ErrCode)
        //                Uri fileUri = PackUriHelper.CreatePartUri(new Uri(@"/" +
        //                              Path.GetFileName(path), UriKind.Relative));
        //                string contentType = @"data/" + ZipHelper.GetFileExtentionName(path);

        //                using (Stream zipStream =
        //                        package.CreatePart(fileUri, contentType, compressionLevel).GetStream())
        //                {
        //                    using (FileStream fileStream = new FileStream(path, FileMode.Open))
        //                    {
        //                        fileStream.CopyTo(zipStream);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    private static string GetFileExtentionName(string path)
        //    {
        //        string extention = Path.GetExtension(path);
        //        if (!string.IsNullOrWhiteSpace(extention) && extention.StartsWith("."))
        //        {
        //            extention = extention.Substring(1);
        //        }

        //        return extention;
        //    }
        //}

        //public static string ExportToExcel(DataTable dt, string TempPath, string Filename)
        //{
        //    /*Set up work book, work sheets, and excel application*/
        //    Microsoft.Office.Interop.Excel.Application oexcel = new Microsoft.Office.Interop.Excel.Application();
        //    try
        //    {
        //        string path = AppDomain.CurrentDomain.BaseDirectory;
        //        object misValue = System.Reflection.Missing.Value;
        //        Microsoft.Office.Interop.Excel.Workbook obook = oexcel.Workbooks.Add(misValue);
        //        Microsoft.Office.Interop.Excel.Worksheet osheet = new Microsoft.Office.Interop.Excel.Worksheet();
        //        osheet = (Microsoft.Office.Interop.Excel.Worksheet)obook.Sheets["Sheet1"];
        //        int colIndex = 0;
        //        int rowIndex = 1;
        //        foreach (DataColumn dc in dt.Columns)
        //        {
        //            colIndex++;
        //            osheet.Cells[1, colIndex] = dc.ColumnName;
        //        }
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            rowIndex++;
        //            colIndex = 0;

        //            foreach (DataColumn dc in dt.Columns)
        //            {
        //                colIndex++;
        //                osheet.Cells[rowIndex, colIndex] = dr[dc.ColumnName];
        //            }
        //        }

        //        osheet.Columns.AutoFit();
        //        string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
        //        obook.SaveAs(FilePath);

        //        //string[] csvData = System.IO.File.ReadAllLines(TempPath);
        //        //string result = new StreamReader(file.InputStream).ReadToEnd();
        //        //File saved at temporary path, now we need to move it to S3 bucket using AWS SDK 
        //        // string FileUploadSucceeded = UploadToS3(System.IO.StreamReader(TempPath.InputStream).ReadToEnd(), Filename);

        //        obook.Close();
        //        oexcel.Quit();
        //        // releaseObject(osheet);
        //        // releaseObject(obook);
        //        //releaseObject(oexcel);
        //        GC.Collect();
        //        using (StreamReader sr = new StreamReader(FilePath))
        //        {

        //            UploadStreamToS3(sr.BaseStream, Filename);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        oexcel.Quit();
        //        throw (ex); //this will be caught in another enclosing try/catch or in FilterConfig exception handler.
        //    }

        //    return ("Success");
        //}

        public static bool UploadStreamToS3(Stream stream, string S3TargetPath)
        {
            //string _awsAccessKey = ConfigurationManager.AppSettings["AWSS3AccessKey"];
            //string _awsSecretKey = ConfigurationManager.AppSettings["AWSS3SecretKey"];

            string _awsAccessKey = Globals.GetValue("rely_accesskey");
            string _awsSecretKey = Globals.GetValue("rely_secretkey");

            string _bucketName = ConfigurationManager.AppSettings["S3Bucketname"];
            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    CannedACL = S3CannedACL.Private,//PERMISSION TO FILE PUBLIC ACCESIBLE
                    Key = string.Format("{0}{1}", ConfigurationManager.AppSettings["S3BucketRootFolder"], S3TargetPath),
                    InputStream = stream//SEND THE FILE STREAM
                };

                client.PutObject(request);
            }
            return true;

        }


        //private DataTable GetData(SqlCommand cmd)
        //{
        //    DataTable dt = new DataTable();

        //    String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].

        //    ConnectionString;

        //    SqlConnection con = new SqlConnection(strConnString);

        //    SqlDataAdapter sda = new SqlDataAdapter();

        //    cmd.CommandType = CommandType.Text;

        //    cmd.Connection = con;

        //    try

        //    {

        //        con.Open();

        //        sda.SelectCommand = cmd;

        //        sda.Fill(dt);

        //        return dt;

        //    }

        //    catch (Exception ex)

        //    {

        //        return null;

        //    }

        //    finally

        //    {

        //        con.Close();

        //        sda.Dispose();

        //        con.Dispose();

        //    }
        //}

        //public IHttpActionResult GetGGlobalPOB()
        //{
        //    var xx = (from aa in db.GGlobalPobs
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.Type,
        //                  aa.Name,
        //                  aa.Description,
        //                  aa.Category,
        //                  aa.IFRS15Account
        //              }).OrderBy(p=>p.Id);
        //    return Ok(xx);

        //}

        //public IHttpActionResult GetGCopaDimensions()
        //{

        //    var xx = (from aa in db.GCopaDimensions
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.Dimension,
        //                  aa.Class,
        //                  aa.CopaValue,
        //                  aa.Description
        //              }).OrderBy(c=>c.Id);

        //    return Ok(xx);
        //}


        private static string GetChecksum(string FilePath,string FileName)
        {
            string FileFullPath = FilePath + "\\" + FileName;

            using (var stream = new FileStream((string)FileFullPath, FileMode.Open, FileAccess.Read))
            {
                //SHA256 procedure takes time to execute,May be in future it will be replaced with MD5
                SHA256Managed sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }



        [HttpGet]
        public async Task<IHttpActionResult> GetS15GridData(string CompanyCode)
        {
            string strQuery = "Exec SpGetS15GridData @CompanyCode";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            DataTable dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }



    


    }
}

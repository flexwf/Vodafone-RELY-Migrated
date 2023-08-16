using Amazon.S3;
using Amazon.S3.Model;
using CsvHelper;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RELY_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web.Http;
using ExportWord = NPOI.XWPF.UserModel;
using NPOI.OpenXml4Net.OPC.Internal;
using System.IO.Packaging;
using System.Linq;
using NPOI.XWPF.Model;
using System.Text;
using System.Collections;

using System.Data.Entity;


//using NPOI.XWPF.UserModel;

namespace RELY_API.Utilities
{
    public class Globals
    {
        public static string NotFoundErrorMessage { get { return "The {0} you are looking for could not be found. It may already have been deleted."; } }
        public static string BadRequestErrorMessage { get { return "The data passed for {0} the {1} is not valid. Please check the values and try again."; } }
        public static string CanNotUpdateDeleteErrorMessage { get { return "Can not Update/Delete {0} because there are  {1} associated with it."; } }
        public static string CannotInsertDuplicateErrorMessage { get { return "Can not Insert Duplicate {0}."; } }
        //public static string SomethingElseFailedInDBErrorMessage { get { return "Error message to be defined in Globals.cs after discussion with JS"; } }
        public static string SomethingElseFailedInDBErrorMessage { get { return "Oops! Something went wrong. The issue has been reported and will be resolved soon. You can reach L2 Admin with error reference #{0}"; } }

        public static string _awsAccessKey = Globals.GetValue("rely_accesskey");
        public static string _awsSecretKey = Globals.GetValue("rely_secretkey");
        public static string SMSAccessKey = Globals.GetValue("sns_accesskey"); 
        public static string SMSSecretKey = Globals.GetValue("sns_secretkey");

        static RELYDevDbEntities db = new RELYDevDbEntities();
        //new HTTP Status types has been defined.
        public enum ExceptionType
        {
            Type1 = 551,//Internal Server Excpetion - redirect to Standard Error Page
            Type2 = 552,//SQL Exception - 
            Type3 = 553,//Redirect to other page + Error Message
            Type4 = 554//Popup  and leaves on same page
        };

        public static DataTable GetDdatainDataTable(string Query)
        {
            DataTable tb = new DataTable();

            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            //The Ado.Net code ends here
            return tb;
        }

        public static string ExportDataSetToExcel(DataSet ds, string Path, string Filename, string ContentType, string dateformat)
        {
            try
            {
                string CompletePath = Path + "/" + Filename;
                ContentType = (ContentType.Replace(" ", "")).ToLower();
                string OutputMsg = "";

                if (string.IsNullOrEmpty(Path))
                {
                    OutputMsg = "Path not defined";
                    return OutputMsg;
                }
                else
                {
                    //check the existence of path, if do not exist then create it
                    if (!Directory.Exists(Path))
                    {
                        Directory.CreateDirectory(Path);
                    }
                }
                if (string.IsNullOrEmpty(Filename))
                {
                    OutputMsg = "Filename is Mandatory";
                    return OutputMsg;
                }
                else
                {

                    IWorkbook workbook = new XSSFWorkbook();

                    ICellStyle _TextCellStyle = workbook.CreateCellStyle();
                    _TextCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("@");

                    for (int k = 0; k < ds.Tables.Count; k++)
                    {
                        ISheet sheet1 = null;
                        sheet1 = workbook.CreateSheet(ds.Tables[k].TableName);
                        //Creating first row for Columns
                        IRow row1 = sheet1.CreateRow(0);
                        DataTable dt = ds.Tables[k];
                        //Setting Column names in first row of excel
                        for (int j = 0; j < ds.Tables[k].Columns.Count; j++)
                        {
                            ICell cell = row1.CreateCell(j);
                            //cell.CellStyle = _TextCellStyle;
                            string columnName = dt.Columns[j].ToString();
                            cell.SetCellValue(columnName);

                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var value = "";
                            IRow row = sheet1.CreateRow(i + 1);
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                ICell cell = row.CreateCell(j);
                                value = dt.Rows[i][j].ToString();
                                //getting datatype for datacolumn. If it is datatime, convert its value to contain only date in dd.mm.yyyy format.
                                var type = dt.Columns[j].DataType.Name.ToString();
                                if (type == "DateTime")
                                {
                                    if (!String.IsNullOrEmpty(value))
                                        value = DateTime.Parse(value).ToString(dateformat);//parameter should passed
                                }
                                cell.SetCellValue(value);
                            }
                        }
                    }
                    FileStream xfile = new FileStream(CompletePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);

                    workbook.Write(xfile);
                    xfile.Close();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public static void AddSupportingDocuments(SupportingDocumentViewModel documentModel,string CompanyCode, AuditViewModel AuditModel)
        {
            var OriginalFileArray = documentModel.OriginalFileNameList.Split(',').ToList();
            var FileArray = documentModel.FileList.Split(',').ToList();
            List<string> DescriptionArray = null;

            if (!string.IsNullOrEmpty(documentModel.Description))
            {
                DescriptionArray = documentModel.Description.Split(',').ToList();
            }
            for (var i = 0; i < FileArray.Count(); i++)
            {
                //Move File Over S3
                var Source = documentModel.FilePath + "/" + FileArray[i];
                var Destination = "/" + CompanyCode.ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                var DestinationCompleteFilePath = Destination + "/" + FileArray[i];
                var sucess = Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                if (sucess)
                    documentModel.FilePath = Destination;

                var SupportingDocument = new LSupportingDocument
                {
                    StepId = documentModel.StepId,
                    FileName = FileArray[i],
                    OriginalFileName = OriginalFileArray[i],
                    FilePath = documentModel.FilePath,
                    EntityId = documentModel.EntityId,
                    EntityType = documentModel.EntityType,
                    CreatedById = documentModel.CreatedById,
                    UpdatedById = documentModel.UpdatedById,
                    CreatedByRoleId = documentModel.UpdatedById,
                    CreatedDateTime = DateTime.UtcNow,
                    UpdatedDateTime = DateTime.UtcNow,
                    Description = (!string.IsNullOrEmpty(documentModel.Description) ? DescriptionArray[i] : null)
                };
                db.LSupportingDocuments.Add(SupportingDocument);
                db.SaveChanges();
                /*(b) An entry is to be made in audit table when a file is attached. (RelyProcessName, VFProcessName = WFName, ControlCode = 'Audiit',
                 * Action = 'AddAttachment',ActionType = Create/Edit depending upon the mode (create/edit) in which the form is open, OldStatus,
                 * NewStatus should be same as current status of the entry, comments = 
                 * "Uploaded <FileName> :" + "User Description: " + <Description entered by user in FileUploadUtility>, SupportindDocumentId
                = LSupportingDocument.Id, rest of the columns are obvious.*/
                db.SpLogAudit(AuditModel.RelyProcessName, AuditModel.VFProcessName, "Audit", string.Empty, AuditModel.Action,AuditModel.ActionType, AuditModel.ActionedById,
                    AuditModel.ActionedByRoleId, DateTime.UtcNow, AuditModel.OldStatus, AuditModel.NewStatus,AuditModel.EntityTyppe, AuditModel.EntityId, AuditModel.EntityName, 
                    AuditModel.WorkFlowId, CompanyCode, SupportingDocument.Description, AuditModel.StepId,AuditModel.ActionLabel, SupportingDocument.Id);
                db.SaveChanges();
            }
        }


        //upload file to S3
        public static bool UploadStreamToS3(Stream InputStream, string S3TargetPath)
        {
            //string _awsAccessKey = ConfigurationManager.AppSettings["AWSS3AccessKey"];
            //string _awsSecretKey = ConfigurationManager.AppSettings["AWSS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["S3Bucketname"];
            //using (StreamReader sr = new StreamReader(LocalFilePath))
            //{

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    CannedACL = S3CannedACL.Private,//PERMISSION TO FILE PUBLIC ACCESIBLE
                    Key = string.Format("{0}{1}", ConfigurationManager.AppSettings["S3BucketRootFolder"], S3TargetPath),
                     InputStream = InputStream//SEND THE FILE STREAM
                    //FilePath = LocalFilePath,
                    // ContentType = "text/plain"
                };

                PutObjectResponse response2 = client.PutObject(request);
            }

            // }
            return true;

        }


        //UploadFileToS3
        public static bool UploadFileToS3(string LocalFilePath, string S3TargetPath)
        {
            //string _awsAccessKey = ConfigurationManager.AppSettings["AWSS3AccessKey"];
            //string _awsSecretKey = ConfigurationManager.AppSettings["AWSS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["S3Bucketname"];
            //using (StreamReader sr = new StreamReader(LocalFilePath))
            //{

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    CannedACL = S3CannedACL.Private,//PERMISSION TO FILE PUBLIC ACCESIBLE
                    Key = string.Format("{0}{1}", ConfigurationManager.AppSettings["S3BucketRootFolder"], S3TargetPath),
                    // InputStream = sr.BaseStream//SEND THE FILE STREAM
                    FilePath = LocalFilePath,
                    // ContentType = "text/plain"
                };

                PutObjectResponse response2 = client.PutObject(request);
            }

            // }
            return true;

        }
        public static string ExportToExcel(DataTable dt, string TempPath, string Filename)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet1");
            IRow row1 = sheet1.CreateRow(0);
            // ICellStyle _TextCellStyle = workbook.CreateCellStyle();
            // _TextCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("text");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
               // GC.Collect();
            }
            //loops through data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var value = "";
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    value = dt.Rows[i][columnName].ToString();
                    //getting datatype for dataclumn. If it is datatime, convert its value to contain only date in MM/dd/yyyy format.

                    var type = dt.Columns[j].DataType.Name.ToString();
                    if (type == "DateTime")
                    {
                        if (!String.IsNullOrEmpty(value))
                            value = DateTime.Parse(value).ToString("dd/MM/yyyy");                           
                    }
                    cell.SetCellValue(value);
                   // GC.Collect();
                }
                
            }
            string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
                                                   //obook.SaveAs(FilePath);

            FileStream xfile = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            return ("Success");

        }

        public static string ExportToExcelForRefGrid(DataTable dt, string TempPath, string Filename)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet1");
            IRow row1 = sheet1.CreateRow(0);
            // ICellStyle _TextCellStyle = workbook.CreateCellStyle();
            // _TextCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("text");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
                // GC.Collect();
            }

            //loops through data  
            for (int i = 0; i < dt.Rows.Count; i++) 
            {
                var value = "";
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    value = dt.Rows[i][columnName].ToString();
                    //getting datatype for dataclumn. If it is datatime, convert its value to contain only date in MM/dd/yyyy format.

                    var type = dt.Columns[j].DataType.Name.ToString();
                    if (type == "DateTime")
                    {
                        //Update cell style to Date columns
                        ICellStyle cellStyle = workbook.CreateCellStyle();
                        ICreationHelper createHelper = workbook.GetCreationHelper();
                        short dateFormat = createHelper.CreateDataFormat().GetFormat("dd-MM-yyyy");
                        cellStyle.DataFormat = dateFormat;

                        //cellStyle.
                        cell.CellStyle = cellStyle;
                        if (!String.IsNullOrEmpty(value))
                            value = DateTime.Parse(value).ToString("dd-MM-yyyy");
                    }
                    cell.SetCellValue(value);
                    // GC.Collect();
                }

            }


            string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
                                                   //obook.SaveAs(FilePath);

            FileStream xfile = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            xfile.Close();
            return ("Success");

        }


        public static string ExportToExcelForRefGridNew(DataTable dt, string TempPath, string Filename)
        {

            dt.Columns.Remove("ID");
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet1");
            IRow row1 = sheet1.CreateRow(0);
            // ICellStyle _TextCellStyle = workbook.CreateCellStyle();
            // _TextCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("text");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
                // GC.Collect();
            }

            for (int i = 0; i < 1; i++)
            {
                var value = "";
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    value = dt.Rows[i][columnName].ToString();
                    cell.SetCellValue(value);
                }
            }

            //loops through data  
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                var value = "";
                IRow row = sheet1.CreateRow(i + 1); ;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    value = dt.Rows[i][columnName].ToString();
                    //getting datatype for dataclumn. If it is datatime, convert its value to contain only date in MM/dd/yyyy format.
                   
                    var type = dt.Columns[j].DataType.Name.ToString();
                    //if (type == "DateTime")
                    ////if (IsDateTime(value) == true)
                    //{
                    //    //Update cell style to Date columns
                    //    ICellStyle cellStyle = workbook.CreateCellStyle();
                    //    ICreationHelper createHelper = workbook.GetCreationHelper();
                    //    short dateFormat = createHelper.CreateDataFormat().GetFormat("dd.MM.yyyy");
                    //    cellStyle.DataFormat = dateFormat;

                    //    //cellStyle.
                    //    cell.CellStyle = cellStyle;
                    //    if (!String.IsNullOrEmpty(value))
                    //        value = DateTime.Parse(value).ToString("dd.MM.yyyy");
                    //}
                    if (columnName.Contains("AttributeD"))
                    {
                        if (IsDateTime(value) == true)
                        {
                            if (!String.IsNullOrEmpty(value))
                                value = DateTime.Parse(value).ToString("dd.MM.yyyy");//parameter should passed
                        }
                    }



                    cell.SetCellValue(value);
                    // GC.Collect();
                }

            }


            string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
                                                   //obook.SaveAs(FilePath);

            FileStream xfile = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            xfile.Close();
            return ("Success");

        }

        public static bool IsDateTime(string text)
        {
            if(text == "4.2017")
            {

                string s = "sdfsdf";
            }
            DateTime dateTime;
            bool isDateTime = false;

            // Check for empty string.
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            isDateTime = DateTime.TryParse(text, out dateTime);

            return isDateTime;
        }

        public static string GenericMethodForExportToExcel(DataSet ds, string Path, string Filename, string ContentType, string dateformat)
        {
            try
            {
                string CompletePath = Path + "/" + Filename;
                ContentType = (ContentType.Replace(" ", "")).ToLower();
                string OutputMsg = "";
                
                if (string.IsNullOrEmpty(Path))
                {
                    OutputMsg = "Path not defined";
                    return OutputMsg;
                }
                else {
                    //check the existence of path, if do not exist then create it
                    if (!Directory.Exists(Path))
                    {
                        Directory.CreateDirectory(Path);
                    }
                }
                if (string.IsNullOrEmpty(Filename))
                {
                    OutputMsg = "Filename is Mandatory";
                    return OutputMsg;
                }
                else
                {

                    IWorkbook workbook = new XSSFWorkbook();

                    ICellStyle _TextCellStyle = workbook.CreateCellStyle();
                    _TextCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("@");

                    for (int k = 0; k < ds.Tables.Count; k++)
                    {
                        ISheet sheet1 = null;
                        sheet1 = workbook.CreateSheet(ds.Tables[k].TableName);
                        //Creating first row for Columns
                        IRow row1 = sheet1.CreateRow(0);
                        DataTable dt = ds.Tables[k];
                        //Setting Column names in first row of excel
                        for (int j = 0; j < ds.Tables[k].Columns.Count; j++)
                        {
                            ICell cell = row1.CreateCell(j);
                            //cell.CellStyle = _TextCellStyle;
                            string columnName = dt.Columns[j].ToString();
                            cell.SetCellValue(columnName);

                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var value = "";
                            IRow row = sheet1.CreateRow(i + 1);
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                ICell cell = row.CreateCell(j);
                                //cell.CellStyle = _TextCellStyle;
                                //if (ContentType == "alltext")
                                //{
                                //    cell.SetCellType(CellType.String);

                                //}
                                value = dt.Rows[i][j].ToString();
                                //getting datatype for datacolumn. If it is datatime, convert its value to contain only date in dd.mm.yyyy format.
                                var type = dt.Columns[j].DataType.Name.ToString();
                                if (type == "DateTime")
                                {
                                    if (!String.IsNullOrEmpty(value))
                                        value = DateTime.Parse(value).ToString(dateformat);//parameter should passed
                                }
                                cell.SetCellValue(value);
                            }
                        }
                    }
                    FileStream xfile = new FileStream(CompletePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                    
                    workbook.Write(xfile);
                    xfile.Close();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }

        //public static string GetChecksumFileCSV(string content,string FilePath, string FileName)
        //{
        //    try
        //    {
        //        string OutputMsg = "";
        //        if (string.IsNullOrEmpty(FilePath))
        //        {
        //            OutputMsg = "Path not defined";
        //            return OutputMsg;
        //        }
        //        else
        //        {
        //            //check the existence of path, if do not exist then create it
        //            if (!Directory.Exists(FilePath))
        //            {
        //                Directory.CreateDirectory(FilePath);
        //            }


        //        }

        //        if (string.IsNullOrEmpty(FileName))
        //        {
        //            OutputMsg = "Filename is Mandatory";
        //            return OutputMsg;
        //        }
        //        else
        //        {

        //            using (var CTextWriterCheckSum = new StreamWriter(FilePath + "/" + FileName))
        //            using (var csvCheckSum = new CsvWriter(CTextWriterCheckSum))
        //            {
        //                csvCheckSum.Configuration.QuoteNoFields = true;

        //                csvCheckSum.WriteField(content);


        //            }

                    
        //            return "Success";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }

            
        //}


        public static string GetChecksumFile(string content, string FilePath, string FileName,string FileFormat)
        {
            try
            {
                string OutputMsg = "";
                if (string.IsNullOrEmpty(FilePath))
                {
                    OutputMsg = "Path not defined";
                    return OutputMsg;
                }
                else
                {
                    //check the existence of path, if do not exist then create it
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }
                }
                if (string.IsNullOrEmpty(FileName))
                {
                    OutputMsg = "Filename is Mandatory";
                    return OutputMsg;
                }
                else
                {
                    if (FileFormat.ToLower() == "excel")
                    {
                        IWorkbook workbook = new XSSFWorkbook();
                        ISheet sheet1 = workbook.CreateSheet("Sheet1");
                        IRow row1 = sheet1.CreateRow(0);
                        ICell cell = row1.CreateCell(0);
                        cell.SetCellValue(content);
                        string CompleteFilePath = FilePath + '/' + FileName;
                        FileStream xfile = new FileStream(CompleteFilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                        workbook.Write(xfile);
                        return "Success";
                    }
                    else //if(FileFormat.ToLower() == "csv")
                    {
                        using (var CTextWriterCheckSum = new StreamWriter(FilePath + "/" + FileName))
                        using (var csvCheckSum = new CsvWriter(CTextWriterCheckSum))
                        {
                            csvCheckSum.Configuration.QuoteNoFields = true;
                            csvCheckSum.WriteField(content);
                        }
                        return "Success";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }

        public static string GetZipFolder(List<string> ListOfFiles, string FilePath, string FileName, string TargetPath)
        {
            try
            {
                string OutputMsg = "";
                if (string.IsNullOrEmpty(FilePath))
                {
                    OutputMsg = "Path not defined";
                    return OutputMsg;
                }
                else
                {
                    //check the existence of path, if do not exist then create it
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }


                }

                if (string.IsNullOrEmpty(FileName))
                {
                    OutputMsg = "Filename is Mandatory";
                    return OutputMsg;
                }
                else
                {

                    var FilesToBezipped = ListOfFiles.Select(p => new { CompleteFileName = (FilePath + "/" + p) }).Select(p => p.CompleteFileName).ToList();
                    var ZippedData = ZipHelper.ZipFilesToByteArray(FilesToBezipped, System.IO.Packaging.CompressionOption.Normal);
                    Stream stream = new MemoryStream(ZippedData);
                    Globals.UploadStreamToS3(stream, TargetPath);
                    foreach (var file in FilesToBezipped)
                    {
                        System.IO.File.Delete(file);
                    }
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }




        public static string ExportToCSV(DataTable dt, string path, string Filename)
        {
            try
            {
                string OutputMsg = "";
                if (string.IsNullOrEmpty(path))
                {
                    OutputMsg = "Path not defined";
                    return OutputMsg;
                }
                else
                {
                    //check the existence of path, if do not exist then create it
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
               }

                if (string.IsNullOrEmpty(Filename))
                {
                    OutputMsg = "Filename is Mandatory";
                    return OutputMsg;
                }
                else
                {
                   using (var CTextWriter = new StreamWriter(path + "/" + Filename))
                    using (var csv = new CsvWriter(CTextWriter))
                    {
                        //seperater
                        csv.Configuration.Delimiter = "^";
                        csv.Configuration.QuoteAllFields = false;
                        if (dt != null)
                        {

                            foreach (DataColumn column in dt.Columns)
                            {
                                csv.WriteField(column.ColumnName, false);
                            }
                            csv.NextRecord();
                            foreach (DataRow row in dt.Rows)
                            {
                                for (var i = 0; i < dt.Columns.Count; i++)
                                {
                                    if (dt.Columns[i].DataType == typeof(DateTime))
                                    {

                                        var rowdata = row[i].ToString();
                                        if (!string.IsNullOrEmpty(rowdata))
                                        {
                                            DateTime DT = Convert.ToDateTime(rowdata);

                                            var DTStr = DT.ToString("dd-MM-yyyy HH:mm");
                                            csv.WriteField(DTStr, false);
                                        }
                                        else
                                        {
                                            csv.WriteField("",false);
                                        }
                                    }
                                    //to remove unwanted space from the data and to consider it one
                                    else if  (dt.Columns[i].DataType == typeof(string))
                                    {
                                        var rowdata = row[i].ToString();
                                        if (!string.IsNullOrEmpty(rowdata))
                                        {
                                            row[i] = Convert.ToString(row[i]).Replace(Environment.NewLine, "!!");   //\r\n 
                                            row[i] = Convert.ToString(row[i]).Replace(("\n"), "!!");
                                            //row[i] = Convert.ToString(row[i]).Replace(("\n\r"), "!!");
                                            row[i] = Convert.ToString(row[i]).Replace(("\r"), "!!");

                                            csv.WriteField(Convert.ToString(row[i]), false);
                                        }
                                        //-- added on 20 Feb 2019 If there will be no data then it will enter into the else condition and write "" in file.
                                        else
                                        {
                                            csv.WriteField("", false);
                                        }
                                    }

                                    //-- added on 20 Feb 2019 If there will be data of datatype other than string and datetime then it will enter into the below else condition.
                                    else
                                    {
                                        var rowdata = row[i].ToString();
                                        if (!string.IsNullOrEmpty(rowdata))
                                        {
                                            csv.WriteField(Convert.ToString(row[i]), false);
                                        }
                                        //-- added on 20 Feb 2019 If there will be no data then it will enter into the else condition and write "" in file.
                                        else
                                        {
                                            csv.WriteField("", false);
                                        }
                                    }
                                }
                                csv.NextRecord();
                            }
                        }
                    }
                }

                
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
           
        }


            //Method for Creating two excel sheets 
            //Code written by RG
            public static string ExportToExcelforAccountingscenariolist(DataTable dtAccountingScenario, DataTable dtLifeCycleEvent, string TempPath, string Filename,string Sheet1, string Sheet2)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet(Sheet1);
            IRow row1 = sheet1.CreateRow(0);
           
            for (int j = 0; j < dtAccountingScenario.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = dtAccountingScenario.Columns[j].ToString();
                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
               // GC.Collect();
            }

            //loops through data  
            for (int i = 0; i < dtAccountingScenario.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dtAccountingScenario.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = dtAccountingScenario.Columns[j].ToString();
                    cell.SetCellValue(dtAccountingScenario.Rows[i][columnName].ToString());
                    //GC.Collect();
                }
            }


            ISheet sheet2 = workbook.CreateSheet(Sheet2);
            IRow row2 = sheet2.CreateRow(0);
           
            for (int j = 0; j < dtLifeCycleEvent.Columns.Count; j++)
            {
                ICell cell = row2.CreateCell(j);
                string columnName = dtLifeCycleEvent.Columns[j].ToString();

                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
                //GC.Collect();
            }

            //loops through data  
            for (int i = 0; i < dtLifeCycleEvent.Rows.Count; i++)
            {
                IRow row = sheet2.CreateRow(i + 1);
                for (int j = 0; j < dtLifeCycleEvent.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = dtLifeCycleEvent.Columns[j].ToString();
                    cell.SetCellValue(dtLifeCycleEvent.Rows[i][columnName].ToString());
                   // GC.Collect();
                }
            }
            string FilePath = TempPath + Filename; 

            FileStream xfile = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            return ("Success");
        }


        //public static string ExportDatatoWord(DataTable dt)
        //{
        //    //Create Word Document
        //    ExportWord.XWPFDocument doc = new ExportWord.XWPFDocument();


        //    doc.Document.body.sectPr = new CT_SectPr();
        //    CT_SectPr secPr = doc.Document.body.sectPr;

        //    //Create header and set its text
        //    CT_Hdr header = new CT_Hdr();
        //    header.AddNewP().AddNewR().AddNewT().Value = "This is Header";


        //    //Create footer and set its text
        //    CT_Ftr footer = new CT_Ftr();
        //    footer.AddNewP().AddNewR().AddNewT().Value = "This is Footer";

        //    CT_P footerParagraph = footer.AddNewP();
        //    footer.SetPArray(1, footerParagraph);
        //    CT_PPr ppr = footerParagraph.AddNewPPr();
        //    CT_Jc align = ppr.AddNewJc();
        //    align.val = ST_Jc.right;

        //    //Create the relation of header
        //    ExportWord.XWPFRelation relation1 = ExportWord.XWPFRelation.HEADER;
        //    ExportWord.XWPFHeader myHeader = (ExportWord.XWPFHeader)doc.CreateRelationship(relation1, ExportWord.XWPFFactory.GetInstance(), doc.HeaderList.Count + 1);

        //    //Create the relation of footer
        //    ExportWord.XWPFRelation relation2 = ExportWord.XWPFRelation.FOOTER;
        //    ExportWord.XWPFFooter myFooter = (ExportWord.XWPFFooter)doc.CreateRelationship(relation2, ExportWord.XWPFFactory.GetInstance(), doc.FooterList.Count + 1);

        //    //Set the header
        //    myHeader.SetHeaderFooter(header);
        //    CT_HdrFtrRef myHeaderRef = secPr.AddNewHeaderReference();
        //    myHeaderRef.type = ST_HdrFtr.@default;
        //    myHeaderRef.id = myHeader.GetPackageRelationship().Id;

        //    //Set the footer
        //    myFooter.SetHeaderFooter(footer);
        //    CT_HdrFtrRef myFooterRef = secPr.AddNewFooterReference();
        //    myFooterRef.type = ST_HdrFtr.@default;
        //    myFooterRef.id = myFooter.GetPackageRelationship().Id;

        //    ExportWord.XWPFParagraph paraleft = doc.CreateParagraph(); // Create paharagraph
        //    ExportWord.XWPFRun rleft = paraleft.CreateRun();


        //    //left alignment
        //    rleft.SetText("his is a left justified paragraph"); // Write test in word document 
        //    rleft.IsBold = true; //Set Bold text
        //    rleft.IsItalic = true; //Set Italic Text
        //    rleft.SetUnderline(ExportWord.UnderlinePatterns.Single); //Set different type of underline on text
        //    paraleft.Alignment = ExportWord.ParagraphAlignment.LEFT; //Alignment set Left,Right,Center and both
        //    rleft.FontSize = 20; // Set Font size
        //    paraleft.VerticalAlignment = ExportWord.TextAlignment.TOP; // Set vertical allignment
        //    rleft.FontFamily = "Arial"; // Set font family or font name
        //    rleft.SetColor("FF0000"); // Set color

        //    //SuperScript and Strike on text
        //    ExportWord.XWPFParagraph paraSuperscript = doc.CreateParagraph();
        //    ExportWord.XWPFRun rSuperScript = paraSuperscript.CreateRun();
        //    rSuperScript.SetText("and went away");
        //    rSuperScript.IsStrike = true;
        //    rSuperScript.FontSize = 20;
        //    rSuperScript.Subscript = ExportWord.VerticalAlign.SUPERSCRIPT;
        //    rSuperScript.SetColor("Green");

        //    //right alignment
        //    ExportWord.XWPFParagraph pararight = doc.CreateParagraph();
        //    ExportWord.XWPFRun rright = pararight.CreateRun();
        //    //ExportWord.XWPFStyles xWPFStyles=doc.CreateStyles();

        //    rright.SetColor("FF0000");
        //    rright.SetText("his is a"); // Write test in word document

        //    rright = pararight.CreateRun();
        //    rright.SetColor("Black");
        //    rright.SetText(" right");
        //    rright.IsBold = true; //Set Bold text
        //    rright.IsItalic = true; //Set Italic Text
        //    rright.FontSize = 20;
        //    rright = pararight.CreateRun();
        //    rright.SetColor("FF0000");
        //    rright.SetText(" justified paragraph");
        //    pararight.Alignment = ExportWord.ParagraphAlignment.RIGHT; //Alignment set Left,Right,Center and both            
        //    rright.FontFamily = "Arial"; // Set font family or font name
        //                                 // Set color


        //    //Center alignment
        //    ExportWord.XWPFParagraph paracenter = doc.CreateParagraph();
        //    ExportWord.XWPFRun rcenter = paracenter.CreateRun();
        //    //ExportWord.XWPFStyles xWPFStyles=doc.CreateStyles();

        //    rcenter.SetText("his is a center justified paragraph"); // Write test in word document
        //    paracenter.Alignment = ExportWord.ParagraphAlignment.CENTER; //Alignment set Left,Right,Center and both            
        //    rcenter.FontFamily = "Times New Roman"; // Set font family or font name
        //    rcenter.SetColor("FF0000"); // Set color


        //    //BORDERS
        //    paracenter.BorderBottom = ExportWord.Borders.Double;
        //    paracenter.BorderTop = ExportWord.Borders.Double;
        //    paracenter.BorderRight = ExportWord.Borders.Double;
        //    paracenter.BorderLeft = ExportWord.Borders.Double;
        //    paracenter.BorderBetween = ExportWord.Borders.Double;

        //    //Both alignment
        //    ExportWord.XWPFParagraph paraboth = doc.CreateParagraph();
        //    ExportWord.XWPFRun rboth = paraboth.CreateRun();
        //    //ExportWord.XWPFStyles xWPFStyles=doc.CreateStyles();

        //    rboth.SetText("his is a both justified paragraph"); // Write test in word document
        //    paraboth.Alignment = ExportWord.ParagraphAlignment.BOTH; //Alignment set Left,Right,Center and both            
        //    rboth.FontFamily = "Arial"; // Set font family or font name
        //    rboth.SetColor("Blue"); // Set color

        //    //Distribute alignment
        //    ExportWord.XWPFParagraph paraDistribute = doc.CreateParagraph();
        //    ExportWord.XWPFRun rDistribute = paraDistribute.CreateRun();
        //    //ExportWord.XWPFStyles xWPFStyles=doc.CreateStyles();

        //    rDistribute.SetText("his is a Distribute justified paragraph"); // Write test in word document
        //    paraDistribute.Alignment = ExportWord.ParagraphAlignment.DISTRIBUTE; //Alignment set Left,Right,Center,Distribute and both            
        //    rDistribute.FontFamily = "Arial"; // Set font family or font name
        //    rDistribute.SetColor("Blue"); // Set color

        //    //Simple Bulleted list
        //    ExportWord.XWPFNumbering numbering = doc.CreateNumbering(); //Create numbering
        //    string abstractNumId = numbering.AddAbstractNum();
        //    string numId = numbering.AddNum(abstractNumId);

        //    //Create simple buttet text
        //    ExportWord.XWPFParagraph p0 = doc.CreateParagraph();
        //    ExportWord.XWPFRun r0 = p0.CreateRun();
        //    r0.SetText("simple bullet");
        //    r0.IsBold = true;
        //    r0.FontFamily = "Courier";
        //    r0.FontSize = 12;

        //    //First line add in bulleted list.
        //    ExportWord.XWPFParagraph p1 = doc.CreateParagraph();
        //    ExportWord.XWPFRun r1 = p1.CreateRun();
        //    r1.SetText("first, create paragraph and run, set text");
        //    p1.SetNumID(numId);

        //    //Second line add in bulleted list.
        //    ExportWord.XWPFParagraph p2 = doc.CreateParagraph();
        //    ExportWord.XWPFRun r2 = p2.CreateRun();
        //    r2.SetText("second, call XWPFDocument.CreateNumbering() to create numbering");
        //    p2.SetNumID(numId);

        //    //Third line add in bulleted list.
        //    ExportWord.XWPFParagraph p3 = doc.CreateParagraph();
        //    ExportWord.XWPFRun r3 = p3.CreateRun();
        //    r3.SetText("third, add AbstractNum[numbering.AddAbstractNum()] and Num(numbering.AddNum(abstractNumId))");
        //    p3.SetNumID(numId);

        //    //Fourth line add in bulleted list.
        //    ExportWord.XWPFParagraph p4 = doc.CreateParagraph();
        //    ExportWord.XWPFRun r4 = p4.CreateRun();
        //    r4.SetText("next, call XWPFParagraph.SetNumID(numId) to set paragraph property, CT_P.pPr.numPr");
        //    p4.SetNumID(numId);

        //    //multi level
        //    abstractNumId = numbering.AddAbstractNum();
        //    numId = numbering.AddNum(abstractNumId);
        //    doc.CreateParagraph();
        //    doc.CreateParagraph();

        //    p1 = doc.CreateParagraph();
        //    r1 = p1.CreateRun();
        //    r1.SetText("multi level bullet");
        //    r1.IsBold = true;
        //    r1.FontFamily = "Courier";
        //    r1.FontSize = 12;

        //    p1 = doc.CreateParagraph();
        //    r1 = p1.CreateRun();
        //    r1.SetText("first");
        //    p1.SetNumID(numId, "0");

        //    p1 = doc.CreateParagraph();
        //    r1 = p1.CreateRun();
        //    r1.SetText("first-first");
        //    p1.SetNumID(numId, "1");
        //    p1 = doc.CreateParagraph();
        //    r1 = p1.CreateRun();
        //    r1.SetText("first-second");
        //    p1.SetNumID(numId, "1");
        //    p1 = doc.CreateParagraph();
        //    r1 = p1.CreateRun();
        //    r1.SetText("first-third");
        //    p1.SetNumID(numId, "1");

        //    p1 = doc.CreateParagraph();
        //    r1 = p1.CreateRun();
        //    r1.SetText("second");
        //    p1.SetNumID(numId, "0");

        //    p1 = doc.CreateParagraph();
        //    r1 = p1.CreateRun();
        //    r1.SetText("second-first");
        //    p1.SetNumID(numId, "1");
        //    p1 = doc.CreateParagraph();
        //    r1 = p1.CreateRun();
        //    r1.SetText("second-second");
        //    p1.SetNumID(numId, "1");
        //    p1 = doc.CreateParagraph();
        //    r1 = p1.CreateRun();
        //    r1.SetText("second-third");
        //    p1.SetNumID(numId, "1");

        //    p1 = doc.CreateParagraph();
        //    r1 = p1.CreateRun();
        //    r1.SetText("second-third-first");
        //    p1.SetNumID(numId, "2");
        //    p1 = doc.CreateParagraph();
        //    r1 = p1.CreateRun();
        //    r1.SetText("second-third-second");
        //    p1.SetNumID(numId, "2");

        //    p1 = doc.CreateParagraph();
        //    r1 = p1.CreateRun();
        //    r1.SetText("third");
        //    p1.SetNumID(numId, "0");


        //    // Creating Table
        //    ExportWord.XWPFTable tab = doc.CreateTable();
        //    ExportWord.XWPFTableRow row = tab.GetRow(0); // First row  
        //                                                 // Columns  
        //    row.GetCell(0).SetText("Sl. No.");
        //    row.AddNewTableCell().SetText("Name");
        //    row.AddNewTableCell().SetText("Email");
        //    row = tab.CreateRow(); // Second Row  
        //    row.GetCell(0).SetText("1.");
        //    row.GetCell(1).SetText("Ankit");
        //    row.GetCell(2).SetText("ankit@gmail.com");
        //    row = tab.CreateRow(); // Third Row  
        //    row.GetCell(0).SetText("2.");
        //    row.GetCell(1).SetText("Mohan");
        //    row.GetCell(2).SetText("mohan@gmail.com");

        //   // var TempFileFolder = ConfigurationManager.AppSettings["LocalTempFileFolder"];
        //    string S3BucketPath= ConfigurationManager.AppSettings["S3BucketConsolidatedMemoFolder"];
        //    string S3TargetPath = "";
        //    string FileName = "AM_RequestName_" + DateTime.UtcNow.ToString("yyyy-MM-dd  HH:mm") + ".docx";
        //    // string fullPath = "R:\\dev\\de\\download\\" + "_AM_RequestName_" + DateTime.UtcNow.ToString("yyyy-MM-dd  HH:mm") + ".docx";


        //    S3TargetPath = "/" + S3BucketPath + "/manual/generate/" + FileName;

        //    //For testing point of view
        //    string fullPath = "R:\\Test\\WordSample.docx";

        //    if (File.Exists(fullPath))
        //    {
        //        File.Delete(fullPath);
        //    }




        //   // FileStream out1 = new FileStream(S3TargetPath, FileMode.Create);
        //    FileStream out1 = new FileStream(fullPath, FileMode.Create);

        //    doc.Write(out1);
        //    out1.Close();
        //    return ("Success");
        //}

            //If possible remove ADO Connection and use Entity Framework instead
        public static void DebugEntry(string value)
        {
            SqlConnection SqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            string sql = "insert into debug(DebugValue,DebugDateTime) values(@value,@DT)";
            SqlCommand cmd1 = new SqlCommand(sql, SqlConnection);
            cmd1.Parameters.AddWithValue("@value", value);
            cmd1.Parameters.AddWithValue("@DT", DateTime.UtcNow);
            SqlConnection.Open();
            cmd1.ExecuteNonQuery();
            SqlConnection.Close();

        }

        public static string ExportWordSample(DataTable dt, string TempPath, string Filename,string WDTemplatePath)
        {
 
            Stream stream = new FileStream(WDTemplatePath, FileMode.Open);
            ExportWord.XWPFDocument doc = new ExportWord.XWPFDocument(stream);
            ExportWord.XWPFTableCell Wordcell;
            ExportWord.XWPFParagraph Wordpdt;
            ExportWord.XWPFRun Wordrdt;
            var BGColor = "";
            var TextColor = "";
            var Font = "";
            var FontSize = 0;
            var tableColumnCount = 0;
            int count = 0;
            int IsTable = 1;
            DataTable dtTable = new DataTable();
            dtTable = dt.Clone();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //Function will be creating later on to avoid rewriting the code
                var OutputFormat = dt.Rows[i]["OutputFormat"].ToString();
                string[] strOutPutFormatval = OutputFormat.Split(',');
                for (int k = 0; k < strOutPutFormatval.Count(); k++)
                {
                    string[] Keyvalue = strOutPutFormatval[k].Split('=');
                    switch (Keyvalue[0])
                    {
                        case "BgColor":
                            BGColor = Keyvalue[1];
                            break;
                        case "TextColor":
                            TextColor = Keyvalue[1];
                            break;
                        case "Font":
                            Font = Keyvalue[1];
                            break;
                        case "FontSize":
                            FontSize = Convert.ToInt16(Keyvalue[1]);
                            break;
                    }

                }
                var ItemType = dt.Rows[i]["ItemType"].ToString();

                //Getting Row count for Table Item Types for creating table in Document
                if (dt.Rows[i]["ItemType"].ToString() == "TABLE")
                {
                    count = count + 1;
                    IsTable = i;
                    var Rowdata = dt.Rows[i];
                    dtTable.ImportRow(Rowdata);
                    if (i == dt.Rows.Count - 1)//This condition will implement if last itemType of last iteration will be "Table"
                    {
                        goto next;
                    }
                    else
                    {
                        goto nextIteration;
                    }
                }
                else
                {
                    goto next;
                }
              next:
                if (dt.Rows[IsTable]["ItemType"].ToString() == "TABLE")
                    {

                        //this is table column counting to avoid nullable columns in the table.
                        for (int a = 0; a < dtTable.Rows.Count; a++)
                        {
                            for (int b = 0; b < 16; b++)
                            {
                                var values = dtTable.Rows[a][b].ToString();
                                if (values != "" && values != null)
                                {
                                    if (tableColumnCount <= b)
                                    {
                                        tableColumnCount = b + 1;
                                    }
                                }
                            }
                        }

                        var tableBGColor = "";
                        var tableTextColor = "";
                        var tableFont = "";
                        var tableFontSize = 0;
                        OutputFormat = dt.Rows[IsTable]["OutputFormat"].ToString();
                        strOutPutFormatval = OutputFormat.Split(',');
                        for (int k = 0; k < strOutPutFormatval.Count(); k++)
                        {
                            string[] Keyvalue = strOutPutFormatval[k].Split('=');
                            switch (Keyvalue[0])
                            {
                                //case "WordStyle":

                                //    break;
                                case "BgColor":
                                    tableBGColor = Keyvalue[1];
                                    break;
                                case "TextColor":
                                    tableTextColor = Keyvalue[1];
                                    break;
                                case "Font":
                                    tableFont = Keyvalue[1];
                                    break;
                                case "FontSize":
                                    tableFontSize = Convert.ToInt16(Keyvalue[1]);
                                    break;
                            }

                        }

                        // For Table Item type,creating table in document and assigned the values of dtTable in doc table
                        ExportWord.XWPFTable table = doc.CreateTable(count, tableColumnCount - 1);
                        for (int RowIndex = 0; RowIndex < count; RowIndex++)
                        {
                            for (int columnIndex = 0; columnIndex < tableColumnCount - 1; columnIndex++)
                            {
                                Wordcell = table.GetRow(RowIndex).GetCell(columnIndex);
                                Wordpdt = Wordcell.AddParagraph();
                                Wordrdt = Wordpdt.CreateRun();
                                Wordrdt.SetText(dtTable.Rows[RowIndex][columnIndex + 1].ToString());
                                Wordrdt.SetColor(tableTextColor);
                                Wordcell.SetColor(tableBGColor);
                                Wordrdt.FontFamily = tableFont;
                                Wordrdt.FontSize = tableFontSize;
                                Wordpdt.Alignment = ExportWord.ParagraphAlignment.BOTH;
                            }

                        }

                        count = 0;
                        dtTable.Clear();
                        tableColumnCount = 0;
                        IsTable = 1;
                    }
            
                //These are those ItemTypes which don't exists in LFSItemTypes, These are only extra information which we need to display in Document
                if (ItemType == "RequestInfo" || ItemType == "ProductInfo" || ItemType == "SectionInfo" || ItemType == "ApplicableTo")
                {
                    ExportWord.XWPFParagraph pdt = null;
                    ExportWord.XWPFRun rdt = null;
                    //giving bullets in the before the products Name
                    ExportWord.XWPFNumbering numbering = doc.CreateNumbering(); //Create numbering
                    string abstractNumId = numbering.AddAbstractNum();
                    string numId = numbering.AddNum(abstractNumId);
                    if (ItemType == "ProductInfo")
                    {
                        //Giving a paragraph space between RequestInfo and ProductInfo
                        pdt = doc.CreateParagraph();
                        rdt = pdt.CreateRun();
                        rdt.SetText("");
                        pdt = null;
                        rdt = null;
                        var value = (dt.Rows[i]["MemoCol1"].ToString()).Split(':').ToList();
                        pdt = doc.CreateParagraph();
                        rdt = pdt.CreateRun();
                        rdt.SetText(value[0].ToString());
                        rdt.SetColor(TextColor);
                        pdt.FillBackgroundColor = BGColor;
                        pdt.FillPattern = ST_Shd.clear;
                        rdt.FontFamily = Font;
                        rdt.FontSize = FontSize;
                        rdt.IsBold = true;
                        pdt.Alignment = ExportWord.ParagraphAlignment.BOTH;
                        var ProductValues = (value[1].ToString()).Split('^').ToList();
                        foreach (var product in ProductValues)
                        {

                            pdt = doc.CreateParagraph();
                            rdt = pdt.CreateRun();
                            rdt.SetText(product.ToString());
                            rdt.SetColor(TextColor);
                            pdt.FillBackgroundColor = BGColor;
                            pdt.FillPattern = ST_Shd.clear;
                            rdt.FontFamily = Font;
                            rdt.FontSize = FontSize;
                            pdt.Alignment = ExportWord.ParagraphAlignment.BOTH;
                            pdt.SetNumID(numId);

                        }
                        pdt = doc.CreateParagraph();
                        rdt = pdt.CreateRun();
                        pdt.IsPageBreak = true;
                        rdt.SetText("");
                    }
                    else if (ItemType == "ApplicableTo")
                    {
                        //Giving a paragraph space between RequestInfo and ProductInfo

                        pdt = null;
                        rdt = null;

                        var value = (dt.Rows[i]["MemoCol1"].ToString()).Split('=').ToList();
                        pdt = doc.CreateParagraph();
                        rdt = pdt.CreateRun();
                        rdt.SetText(value[0].ToString());
                        rdt.SetColor(TextColor);
                        pdt.FillBackgroundColor = BGColor;
                        pdt.FillPattern = ST_Shd.clear;
                        rdt.FontFamily = Font;
                        rdt.FontSize = FontSize;
                        rdt.IsBold = true;
                        pdt.Alignment = ExportWord.ParagraphAlignment.BOTH;
                        var ProductValues = (value[1].ToString()).Split('^').ToList();
                        foreach (var product in ProductValues)
                        {

                            pdt = doc.CreateParagraph();
                            rdt = pdt.CreateRun();
                            rdt.SetText(product.ToString());
                            rdt.SetColor(TextColor);
                            pdt.FillBackgroundColor = BGColor;
                            pdt.FillPattern = ST_Shd.clear;
                            rdt.FontFamily = Font;
                            rdt.FontSize = FontSize;
                            pdt.Alignment = ExportWord.ParagraphAlignment.BOTH;
                            pdt.SetNumID(numId);

                        }
                        // pdt = doc.CreateParagraph();
                        //rdt = pdt.CreateRun();
                        //rdt.SetText("");
                    }
                    else
                    {
                        pdt = doc.CreateParagraph();
                        rdt = pdt.CreateRun();
                        rdt.SetText(dt.Rows[i]["MemoCol1"].ToString());
                        rdt.IsBold = true;
                        rdt.SetColor(TextColor);
                        pdt.FillBackgroundColor = BGColor;
                        pdt.FillPattern = ST_Shd.clear;
                        rdt.FontFamily = Font;
                        rdt.FontSize = FontSize;
                        if (ItemType == "RequestInfo")
                        {
                            pdt.Alignment = ExportWord.ParagraphAlignment.BOTH;
                        }
                        else
                        {
                            pdt.Alignment = ExportWord.ParagraphAlignment.BOTH;
                        }

                    }

                    goto nextIteration;
                }

                //For all ItemType except table.
                for (int l = 0; l < 3; l++)//we run this loop 3 times because ItemType except Table will have data in three columns
                {

                    if (ItemType.Equals("CHOOSE_QUESTION") || ItemType.Equals("ENTER_QUESTION") || ItemType.Equals("YN_QUESTION"))
                    {
                        Wordpdt = doc.CreateParagraph();
                        Wordrdt = Wordpdt.CreateRun();
                        Wordrdt.SetText(dt.Rows[i][l].ToString() + " - " + dt.Rows[i][l + 1].ToString() + " : " + dt.Rows[i][l + 2].ToString());
                        Wordrdt.SetColor(TextColor);
                        Wordpdt.FillBackgroundColor = BGColor;
                        Wordpdt.FillPattern = ST_Shd.clear;
                        Wordrdt.FontFamily = Font;
                        Wordrdt.FontSize = FontSize + 1;
                        //Wordrdt.IsBold = true;
                        Wordpdt.Alignment = ExportWord.ParagraphAlignment.BOTH;
                        l = l + 2;

                    }
                    else if (ItemType.Equals("TABLE"))//Control will come here when last iteration will have ItemType="TABLE"
                    {
                        goto nextIteration;
                    }
                    else
                    {
                        if (l == 0)
                        {
                            bool FirstTimeFlag = true;
                            string[] subStrings = dt.Rows[i][l + 1].ToString().Split('\r');
                            foreach (string val in subStrings)
                            {
                                Wordpdt = doc.CreateParagraph();
                                Wordrdt = Wordpdt.CreateRun();
                                //This flag is applied because we want to display Question Name only one time inside this foreach loop
                                if (FirstTimeFlag)
                                {
                                    Wordrdt.SetText(dt.Rows[i][l].ToString() + "  " + val);
                                }
                                else
                                {
                                    Wordrdt.SetText(val);
                                }
                                Wordrdt.SetColor(TextColor);
                                Wordpdt.FillBackgroundColor = BGColor;
                                Wordpdt.FillPattern = ST_Shd.clear;
                                Wordrdt.FontFamily = Font;
                                Wordrdt.FontSize = FontSize;
                                Wordpdt.Alignment = ExportWord.ParagraphAlignment.BOTH;
                                FirstTimeFlag = false;

                            }
                            l = l + 1;
                        }
                        else
                        {
                            if (dt.Rows[i][l].ToString() != "")//This condition is applied to avoid blank spaces between two items which was coming when MemoCol2 was blank because blank row was inserting in hat case.
                            {
                                Wordpdt = doc.CreateParagraph();
                                Wordrdt = Wordpdt.CreateRun();
                                Wordrdt.SetText(dt.Rows[i][l].ToString());
                                Wordrdt.SetColor(TextColor);
                                Wordpdt.FillBackgroundColor = BGColor;
                                Wordpdt.FillPattern = ST_Shd.clear;
                                Wordrdt.FontFamily = Font;
                                Wordrdt.FontSize = FontSize;
                                Wordpdt.Alignment = ExportWord.ParagraphAlignment.BOTH;
                            }
                        }
                    }
                }
                nextIteration:
                bool flag;
            }
            string FilePath = TempPath + "/" + Filename; 
            FileStream out1 = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            doc.Write(out1);
            out1.Close();
            doc.Close();
            stream.Close();
            return ("Success");
        }


        //section to download file from S3 drive drectly 
        public static byte[] DownloadFromS3(string FilePath)
        {
            //string _awsAccessKey = ConfigurationManager.AppSettings["AWSS3AccessKey"];
            //string _awsSecretKey = ConfigurationManager.AppSettings["AWSS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["S3Bucketname"];
            byte[] FileData;
            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = string.Format("{0}{1}", ConfigurationManager.AppSettings["S3BucketRootFolder"], FilePath),
                };

                using (GetObjectResponse response = client.GetObject(request))
                {
                    FileData = ReadFully(response.ResponseStream);
                }
            }
            return FileData;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        //S3 Section starts here
        //Move local file across location in S3
        public static bool MoveFileinS3(string SourcePath, string DestinationPath)
        {
            //string _awsAccessKey = ConfigurationManager.AppSettings["AWSS3AccessKey"];
            //string _awsSecretKey = ConfigurationManager.AppSettings["AWSS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["S3Bucketname"];
            try
            {
                using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
                {
                    CopyObjectRequest request = new CopyObjectRequest
                    {
                        SourceBucket = _bucketName,
                        SourceKey = ConfigurationManager.AppSettings["S3BucketRootFolder"] + SourcePath,
                        DestinationBucket = _bucketName,
                        DestinationKey = ConfigurationManager.AppSettings["S3BucketRootFolder"] + DestinationPath
                    };
                    CopyObjectResponse response = client.CopyObject(request);
                }
                return true;
            }
            catch (Exception ex)
            {
                //Just added so that code does not break any execution
                return false;
            }
        }
        //S3 section ends here


        public static DataTable GetDataTableUsingADO(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(strConnString);
            SqlDataAdapter sda = new SqlDataAdapter();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            try
            {
                con.Open();
                //Following line is added by NS, because timeout exception was coming in S15Extract
                //cmd.CommandTimeout = 600;
                cmd.CommandTimeout = 1200;//18 Oct 2021, added by NS because SpDownloadGenericGridData proc was taking too long(1.41 minutes) because it was calling two functions per RequestId(3150 Requests). After rebuilding indexes it came to 56 secondes but still increase timeout to 2 min for future growth in RequestNo.
                sda.SelectCommand = cmd;
                sda.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                con.Close();
                sda.Dispose();
                con.Dispose();
            }
        }


        //This method returns the WFComments in the desired format
        public static string GenerateWFComments(string UserName, string NewComments, string PreviousComments, string Action)
        {
            string WFComments = null;
            if (string.IsNullOrEmpty(Action)) { Action = ""; }
            if (!string.IsNullOrEmpty(NewComments))
            {
                if (string.IsNullOrEmpty(PreviousComments) )//when previous commments do not exist
                {
                    WFComments = "[" + DateTime.UtcNow + "] [" + UserName + "] " + NewComments + " " + Action;
                }
                else//when previous commments exist
                {
                    WFComments = "[" + DateTime.UtcNow + "] [" + UserName + "] " + NewComments + " " + Action + Environment.NewLine + PreviousComments;
                }

            }
            return WFComments;
        }

        //This method will return the ErrorMessage string when called for DbEntityValidationException
        public static string GetEntityValidationErrorMessage(DbEntityValidationException e)
        {
            string ErrorMessage = "";
            foreach (var eve in e.EntityValidationErrors)
            {
                ErrorMessage += string.Format("Entity of type {0} in state {1} has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    ErrorMessage += string.Format("- Property: {0}, Error: {1}", ve.PropertyName, ve.ErrorMessage);
                }
            }
            return ErrorMessage;
        }
        //This method is using SMTP client to send emails directly, without db interaction(In some cases, when DB might fail)
        public static bool SendExceptionEmail(string EmailBody)
        {
            try
            {
                 String FROM = ConfigurationManager.AppSettings["SenderEmailId"];   //This address must be verified.

                // Supply your SMTP credentials below. Note that your SMTP credentials are different from your AWS credentials.
                String SMTP_USERNAME = ConfigurationManager.AppSettings["SmtpLoginId"];  //SMTP username. 
                 String SMTP_PASSWORD = ConfigurationManager.AppSettings["SmtpPassword"];  //SMTP password.

                // Amazon SES SMTP host name. This example uses the US West (Oregon) region.
                String HOST = ConfigurationManager.AppSettings["SmtpHost"]; 

                // The port you will connect to on the Amazon SES SMTP endpoint. We are choosing port 587 because we will use
                // STARTTLS to encrypt the connection.
                 int PORT = Convert.ToInt32(ConfigurationManager.AppSettings["PortNumber"]);

                // Create an SMTP client with the specified host name and port.
                using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(HOST, PORT))
                {
                    // Create a network credential with your SMTP user name and password.
                    client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                    // Use SSL when accessing Amazon SES. The SMTP session will begin on an unencrypted connection, and then 
                    // the client will issue a STARTTLS command to upgrade to an encrypted connection using SSL.
                    client.EnableSsl = true;
                    string ToAddress = ConfigurationManager.AppSettings["ExceptionEmailTo"];
                    string EmailSubject = ConfigurationManager.AppSettings["ExceptionEmailSubject"];
                    MailMessage message = new MailMessage(FROM, ToAddress, EmailSubject, EmailBody);
                    message.IsBodyHtml = true;
                    //client.Send(FROM, ToAddress, EmailSubject, EmailBody);
                    client.Send(message);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        //This method returns the Principal Context using Admin user and password.
        private static PrincipalContext getPrincipalContext()
        {
            string AdminUserName = ConfigurationManager.AppSettings["ADUserName"];
            string AdminUserPassword = ConfigurationManager.AppSettings["ADUserPassword"];
            string stringDomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string OUstring = ConfigurationManager.AppSettings["ActiveDirectoryOU"];
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, stringDomainName, OUstring, AdminUserName, AdminUserPassword);
            return ctx;
        }

        //Validate the user credentials for SignIn
        public static AuthenticationResult SignIn(ADModel model)
        {
            // var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
            string ProjectEnviournment = db.GKeyValues.Where(a => a.Key == "ProjectEnviournment").Select(a => a.Value).FirstOrDefault();
            string EUG = "RELY " + ProjectEnviournment + " Users";
            PrincipalContext principalContext = getPrincipalContext();
            bool isAuthenticated = false;
            UserPrincipal userPrincipal = null;
            bool isMember = false;
            try
            {
                isAuthenticated = principalContext.ValidateCredentials(model.Email, model.Password, ContextOptions.Negotiate);

                userPrincipal = UserPrincipal.FindByIdentity(principalContext, model.Email);
                if (userPrincipal.LastPasswordSet == null && userPrincipal.PasswordNeverExpires == false) //Must reset Password
                {
                    return new AuthenticationResult("Must reset Password");
                }
                //Check user is member of EUG
                if (IsUserMemberOfEUG(userPrincipal, EUG))
                {
                    isMember = true;
                }
            }
            catch (Exception ex)
            {
                isAuthenticated = false;
                userPrincipal = null;
            }
            if (!isAuthenticated || userPrincipal == null || !isMember)
            {
                //Username or Password is incorrect.
                return new AuthenticationResult("Username or Password is incorrect");
            }
            return new AuthenticationResult();
        }


        //Method for adding PwdHistory ChangePwdAtNextLogin and SendEmail
        public static void SetPwdHistoryAndEmail( LUser UserDetails,string Password, int LoggedInRoleId, int LoggedInUserId)
        {
            
        }
        //CreateUser method is a utility to create AD user
        public static AuthenticationResult CreateUser(ADModel model)
        {
            PrincipalContext pc = getPrincipalContext();
            UserPrincipal userPrincipal = new UserPrincipal(pc);
            string user = model.Email;
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, user);
            //var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
            string ProjectEnviournment = db.GKeyValues.Where(a => a.Key == "ProjectEnviournment").Select(a => a.Value).FirstOrDefault();
            string EUG = "RELY " + ProjectEnviournment + " Users";
            //If User already exists, and member of EUG, raise error, User exists else, add to EUG
            //If User does not exists, Create new User and add it to EUG(Env User Group)
            if (up == null)
            {
                try
                {
                    if (user.Length > 20)
                    {
                        string substr = user.Substring(0, 20);
                        char[] charsToTrim = { '@', '.' };
                        substr = substr.TrimEnd(charsToTrim);
                        userPrincipal.SamAccountName = substr;
                    }
                    else
                    {
                        userPrincipal.SamAccountName = user;
                    }
                    userPrincipal.UserPrincipalName = user;
                    userPrincipal.SetPassword(model.Password);
                    userPrincipal.PasswordNeverExpires = true;//SS changed this method24-72017.ExpirePasswordNow();//using the property as user need to reset password on first logon
                    userPrincipal.Enabled = true;
                    userPrincipal.Save();
                    //Add User to Group
                    var result = AddUserToGroup(userPrincipal, EUG);
                    if (!result.IsSuccess)
                    {
                        //User could  not be added to EUG
                        return new AuthenticationResult(result.ErrorMessage);
                    }
                    return new AuthenticationResult();
                }
                catch (Exception ex)
                {
                    return new AuthenticationResult(ex.Message);
                }
            }
            else
            {
                //chcek user is Member of EUG
                if (IsUserMemberOfEUG(up, EUG))
                {
                    return new AuthenticationResult("User Already exists.");
                }
                else
                {//Add User to Group 
                    var result = AddUserToGroup(up, EUG);
                    if (!result.IsSuccess)
                    {
                        //User could  not be added to EUG
                        return new AuthenticationResult(result.ErrorMessage);
                    }
                    //and reset pwd if PROD
                    if (ProjectEnviournment.Equals("PROD", StringComparison.OrdinalIgnoreCase))
                    {
                        model.NewPassword = model.Password; ;
                        SetUserPassword(model);
                    }
                    return new AuthenticationResult("added to EUG");
                }
            }

        }
        public static AuthenticationResult ChangeMyPassword(ADModel model)
        {
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                userPrincipal.ChangePassword(model.Password, model.NewPassword);
                userPrincipal.PasswordNeverExpires = true;
                userPrincipal.Save();
                //password changed successfully
                return new AuthenticationResult();
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }

        //Set password for user
        public static AuthenticationResult SetUserPassword(ADModel model)
        {
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                if (userPrincipal != null)
                {
                    userPrincipal.SetPassword(model.NewPassword);
                    userPrincipal.PasswordNeverExpires = true;
                    userPrincipal.Save();
                    //password set successfully
                    return new AuthenticationResult();
                }
                else
                {
                    return new AuthenticationResult("User not found");
                }
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }
        /*SG - 8th Feb 2019 - Commenting as not be used as of now
        //Update userDetails
        public static AuthenticationResult UpdateUserDetails(ADModel model)
        {
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                userPrincipal.SamAccountName = model.Email.Substring(0, 20);//SamAccounNamtName accepts only 20 chars
                userPrincipal.EmailAddress = model.Email;
                userPrincipal.Save();
                return new AuthenticationResult();
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }

        public static AuthenticationResult ActivateDeactivateUser(ADModel model)
        {
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                if (userPrincipal != null)
                {
                    userPrincipal.Enabled = model.Status;
                    userPrincipal.Save();
                }
                else
                {
                    return new AuthenticationResult("user not found");
                }
                return new AuthenticationResult();
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }
        */
        //Delete an existing user 
        public static AuthenticationResult DeleteUser(ADModel model)
        {
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                //var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
                string ProjectEnviournment = db.GKeyValues.Where(a => a.Key == "ProjectEnviournment").Select(a => a.Value).FirstOrDefault();
                string EUG = "RELY " + ProjectEnviournment + " Users";
                string ProdEUG = "RELY PROD Users";

                if (userPrincipal != null)
                {
                    //Delete User only when request Env is Prod. 
                    if (ProjectEnviournment.Equals("Prod", StringComparison.OrdinalIgnoreCase))
                    {
                        userPrincipal.Delete();//deleted successfully
                    }
                    else if (!IsUserMemberOfEUG(userPrincipal, ProdEUG))//not Prod user
                    {
                        userPrincipal.Delete();//deleted successfully
                    }
                    else //remove user from EUG
                    {
                        RemoveUserFromGroup(userPrincipal, EUG);
                    }
                }
                else
                {
                    return new AuthenticationResult("user not found");
                }
                return new AuthenticationResult();

            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }

        }

        //**************added on 05 jan 2019**********************************************************************
        private static PrincipalContext getPrincipalContextForUserGroup()
        {
            string AdminUserName = ConfigurationManager.AppSettings["ADUserName"];
            string AdminUserPassword = ConfigurationManager.AppSettings["ADUserPassword"];
            string stringDomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string OUstring = ConfigurationManager.AppSettings["ActiveDirectoryOUForUserGroup"];
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, stringDomainName, OUstring, AdminUserName, AdminUserPassword);
            return ctx;
        }

        public static GroupPrincipal GetGroup(string sGroupName)
        {
            PrincipalContext oPrincipalContext = getPrincipalContextForUserGroup();

            GroupPrincipal oGroupPrincipal =
            GroupPrincipal.FindByIdentity(oPrincipalContext, IdentityType.Name, sGroupName);
            return oGroupPrincipal;
        }
        public static ArrayList GetUserGroups(string sUserName)
        {
            ArrayList myItems = new ArrayList();
            UserPrincipal oUserPrincipal = GetUser(sUserName);

            PrincipalSearchResult<Principal> oPrincipalSearchResult = oUserPrincipal.GetGroups();

            foreach (Principal oResult in oPrincipalSearchResult)
            {
                myItems.Add(oResult.Name);
            }
            return myItems;
        }

        public static bool IsUserMemberOfEUG(UserPrincipal oUserPrincipal, string sGroupName)
        {
            GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);
            if (oUserPrincipal != null && oGroupPrincipal != null)
            {
                if (oGroupPrincipal.Members.Contains(oUserPrincipal))
                    return true;
                else
                    return false;
            }
            return false;
        }

        public static bool CheckGroupExistence(string GroupName)
        {
            try
            {
                GroupPrincipal oGroupPrincipal = GetGroup(GroupName);

                if (oGroupPrincipal != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static AuthenticationResult AddUserToGroup(UserPrincipal oUserPrincipal, string sGroupName)
        {
            try
            {
                GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);
                if (oUserPrincipal != null && oGroupPrincipal != null)
                {
                    oGroupPrincipal.Members.Add(oUserPrincipal);
                    oGroupPrincipal.Save();
                }
                return new AuthenticationResult();
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }
        public static AuthenticationResult RemoveUserFromGroup(UserPrincipal oUserPrincipal, string sGroupName)
        {
            try
            {
                GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);
                if (oUserPrincipal != null && oGroupPrincipal != null)
                {
                    oGroupPrincipal.Members.Remove(oUserPrincipal);
                    oGroupPrincipal.Save();
                }
                return new AuthenticationResult();
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }

        public static UserPrincipal GetUser(string sUserName)
        {
            PrincipalContext oPrincipalContext = getPrincipalContext();

            UserPrincipal oUserPrincipal =
               UserPrincipal.FindByIdentity(oPrincipalContext, sUserName);
            return oUserPrincipal;
        }
//*****************************************************************************************************

        public partial class AuthenticationResult
        {
            public AuthenticationResult(string errorMessage = null)
            {
                ErrorMessage = errorMessage;
            }

            public String ErrorMessage { get; private set; }
            public Boolean IsSuccess => String.IsNullOrEmpty(ErrorMessage);
        }

        public static string GetValue(string Key)
        {
             RELYDevDbEntities db = new RELYDevDbEntities();
        string KeyValue = (from aa in db.GKeyValues.Where(aa => aa.Key == Key)
                               select aa.Value).FirstOrDefault();
            return KeyValue;
        }

    }
    public class RandomPassword
    {
        // Define default min and max password lengths.
        private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static string PASSWORD_CHARS_NUMERIC = "23456789";
        //private static string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";
        public string Generate()
        {
            return Generate(DEFAULT_MIN_PASSWORD_LENGTH,
                            DEFAULT_MAX_PASSWORD_LENGTH);
        }
        public string Generate(int length)
        {
            return Generate(length, length);
        }
        public string Generate(int minLength,
                                      int maxLength)
        {
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            char[][] charGroups = new char[][]
            {
            PASSWORD_CHARS_LCASE.ToCharArray(),
            PASSWORD_CHARS_UCASE.ToCharArray(),
            PASSWORD_CHARS_NUMERIC.ToCharArray()

            //According to requirement, Password should have only numbers and characters
            //PASSWORD_CHARS_SPECIAL.ToCharArray()
            };

            // Use this array to track the number of unused characters in each
            // character group.
            int[] charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            int[] leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = BitConverter.ToInt32(randomBytes, 0);

            // Now, this is real randomization.
            Random random = new Random(seed);

            // This array will hold password characters.
            char[] password = null;

            // Allocate appropriate memory for the password.
            if (minLength < maxLength)
                password = new char[random.Next(minLength, maxLength + 1)];
            else
                password = new char[minLength];

            // Index of the next character to be added to password.
            int nextCharIdx;

            // Index of the next character group to be processed.
            int nextGroupIdx;

            // Index which will be used to track not processed character groups.
            int nextLeftGroupsOrderIdx;

            // Index of the last non-processed character in a group.
            int lastCharIdx;

            // Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            for (int i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                         lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                              charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(password);
        }


      
    }

    public static class ZipHelper
    {
        public static void ZipFiles(string path, IEnumerable<string> files,
               CompressionOption compressionLevel = CompressionOption.Normal)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                ZipHelper.ZipFilesToStream(fileStream, files, compressionLevel);
            }
        }

        public static byte[] ZipFilesToByteArray(IEnumerable<string> files,
               CompressionOption compressionLevel = CompressionOption.Normal)
        {
            byte[] zipBytes = default(byte[]);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ZipHelper.ZipFilesToStream(memoryStream, files, compressionLevel);
                memoryStream.Flush();
                zipBytes = memoryStream.ToArray();
            }

            return zipBytes;
        }

        private static void ZipFilesToStream(Stream destination,
          IEnumerable<string> files, CompressionOption compressionLevel)
        {
            using (Package package = Package.Open(destination, FileMode.Create))
            {
                foreach (string path in files)
                {
                    // fix for white spaces in file names (by ErrCode)
                    Uri fileUri = PackUriHelper.CreatePartUri(new Uri(@"/" +
                                  Path.GetFileName(path), UriKind.Relative));
                    string contentType = @"data/" + ZipHelper.GetFileExtentionName(path);

                    using (Stream zipStream =
                            package.CreatePart(fileUri, contentType, compressionLevel).GetStream())
                    {
                        using (FileStream fileStream = new FileStream(path, FileMode.Open))
                        {
                            fileStream.CopyTo(zipStream);
                        }
                    }
                }
            }
        }

        private static string GetFileExtentionName(string path)
        {
            string extention = Path.GetExtension(path);
            if (!string.IsNullOrWhiteSpace(extention) && extention.StartsWith("."))
            {
                extention = extention.Substring(1);
            }

            return extention;
        }

        
    }

}
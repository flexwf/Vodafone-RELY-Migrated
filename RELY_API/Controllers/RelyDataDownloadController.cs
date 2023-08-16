using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;

namespace RELY_API.Controllers
{
    public class RelyDataDownloadController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();             

        [HttpGet]
        public IHttpActionResult GetCompanyCodeById(int id)
        {
            var xx = (from aa in db.GCompanies.Where(p => p.Id == id)
                      select new
                      {
                          aa.CompanyCode

                      }).FirstOrDefault();            
            return Ok(xx);
        }


        //Author : Rakhi Singh
        //Description : method to generate zip file of all CSVs which contain the data of required tables as dump.
        [HttpGet]
        public IHttpActionResult GenerateRelyDataDump(string CompanyCode)
        {
            //Obtain the list of tables whose data is to be dumped in CSV format
            string KeyValue = (from aa in db.GKeyValues.Where(aa => aa.Key == "RelyDataDumpTableList" && aa.CompanyCode == CompanyCode)
                               select aa.Value).FirstOrDefault();
            
            //Loop through the list of tables and generate CSV files one by one at a temporary location
            var TableNameArray = KeyValue.Split(',').ToList();
            var ListOfCSVFiles = new List<string>(); //variable which will contain all the CSVs files.
            string TempFileFolder = "";
            string checksumResult = "";
            var DateTimeStampForFile = DateTime.UtcNow.ToString("dd-MM-yyyy-hhmmss");
            for (int j = 0; j < TableNameArray.Count; j++)
            {
                try
                {
                    string TableName = TableNameArray[j];
                    string strQuery = "Exec SpDownloadRelyData @CompanyCode, @TableName";
                    SqlCommand cmd = new SqlCommand(strQuery);
                    cmd.Parameters.AddWithValue("@TableName", TableName);
                    cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                    DataTable dt = GetData(cmd);                    
                    string CSVfileName = TableName + "_" + DateTimeStampForFile + ".csv";
                    TempFileFolder = ConfigurationManager.AppSettings["LocalTempFileFolder"];                    
                    ListOfCSVFiles.Add(CSVfileName);
                    String ExportResut = Globals.ExportToCSV(dt, TempFileFolder, CSVfileName);                   
                    if (ExportResut == "Success")
                    {
                        checksumResult = checksumResult + CSVfileName + "," + GetChecksum(TempFileFolder, CSVfileName) + Environment.NewLine;//In case of csv, checksum result need to be generated of every file but in Excel it will be recorded only once after loop
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Some issue found while Creating CSV file")));
                    }
                }
                catch (Exception ex)
                {
                    //Log entry in Error Log here
                    //Throw the exception to the calling environment
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Some issue found while Creating CSV file")));
                    
                }
            }

            //code forchecksum file (This file contains the information of all the CSVs (length,security), it is in coded form)
            string S3TargetPath = "";
            string IntermediatePath = "RelyData";
            string ChecksumFileName = "CONTROL-" + DateTimeStampForFile + ".csv";
            ListOfCSVFiles.Add(ChecksumFileName);
            var CheckSumMessage = Globals.GetChecksumFile(checksumResult, TempFileFolder, ChecksumFileName,"csv");
           
            //All CSVs are generated at temporary location, now compress them in a zip file           
            string ZipFileName = "Rely-Data-Dump" + "_" + DateTimeStampForFile + ".zip";

            //Copy that zip file to S3 folder
            S3TargetPath = "/" + CompanyCode.ToLower() + "/" + IntermediatePath + "/" + ZipFileName;            
            var OutPutMessage = Globals.GetZipFolder(ListOfCSVFiles, TempFileFolder, ZipFileName, S3TargetPath);

            //Copy successful, remove CSVs and zip fie from temporary location to clean up the workspace
            //------it itself gets deleted from the temp location

            //Return the zip file name to the calling environment for further use
            GenericNameAndIdViewModel model = new GenericNameAndIdViewModel { Name = ZipFileName }; //using this model as function is returning string value which needs to be sent back to APP
            return Ok(model);
           
        }       

        private DataTable GetData(SqlCommand cmd)
        {
            DataTable dt = new DataTable();

            String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].

            ConnectionString;

            SqlConnection con = new SqlConnection(strConnString);

            SqlDataAdapter sda = new SqlDataAdapter();

            cmd.CommandType = CommandType.Text;

            cmd.Connection = con;

            try

            {

                con.Open();

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

        private static string GetChecksum(string FilePath, string FileName)
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

    }
}

using RELY_APP.Utilities;
using RestSharp;
using RELY_APP.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using RELY_APP.ViewModel;
using Newtonsoft.Json;
using System.Configuration;
using System.IO.Packaging;
using System.IO.Compression;
using System.Security.Cryptography;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class S15ExtractsController : Controller
    {
        //public string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        IS15ExtractsRestClient RestClient = new S15ExtractsRestClient();
        // GET: S15Exports
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "S15 Extracts";
            ViewBag.Title = "S15 Extracts";
            //string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            //var ApiData = RestClient.GetRefData(CompanyCode);
            //ViewBag.RefData = ApiData;
            return View();
        }

        string Filename, newfileName;
        [HttpPost]
        [ControllerActionFilter]
        public JsonResult ExportToExcelS15ExtractsModel(string StartDate,string EndDate,string ExtractNameList, string ExtractFileNameList)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);

            DateTime StartDT = DateTime.ParseExact(StartDate, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat);
            DateTime EndDT = DateTime.ParseExact(EndDate, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat);
            var FileFormat = Globals.GetKeyvalue("S15ExtractFileFormat");

            var ExtractNameArray = ExtractNameList.Split(',').ToList();
            var ExtractFileNameArray = ExtractFileNameList.Split(',').ToList();
            List<S15ExtractsViewModel> modelList = new List<S15ExtractsViewModel>();
            for (int j = 0; j < ExtractNameArray.Count; j++)
            {
                var KeyValueArray = ExtractNameArray[j].Split(':').ToList();
                var model = new S15ExtractsViewModel

                {
                    Extracts = KeyValueArray[0].ToString(),
                    FileName = ExtractFileNameArray[j].ToString(),
                    ExtractFileType = KeyValueArray[1].ToString()
                };
                // Count = Count + 1;
                modelList.Add(model);

            }


            // RestClient.GetS15Extracts(Convert.ToDateTime(StartDT), Convert.ToDateTime(EndDT), ExtractNameList, CompanyCode, "Generate", ExtractFileNameList,(FileFormat.Value).ToString());
            RestClient.GetS15Extracts(modelList, Convert.ToDateTime(StartDT), Convert.ToDateTime(EndDT), CompanyCode, "Generate", (FileFormat.Value).ToString());

            //string Filename = ConfigurationManager.AppSettings["S15ReportsBucketPath"] + "S15_" + System.DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";
            //List<string> fileList = new List<string>();
            //List<string> ExtractNames = ExtractNameList.Split(',').ToList<string>();
            //foreach (var ExtractName in ExtractNames)
            //{
            //    string ApiData = RestClient.GetS15Extracts(Convert.ToDateTime(StartDT), Convert.ToDateTime(EndDT), ExtractName, CompanyCode);
            //    DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));
            //    newfileName = "S15_" + ExtractName + "_" + StartDate + "_" + EndDate + ".xlsx";
            //    newfileName = newfileName.Replace('/', '-');
            //    newfileName = newfileName.Replace(" ", "");
            //    newfileName = newfileName.Replace(":", "-");
            //    Filename = ConfigurationManager.AppSettings["S15ReportsBucketPath"] + newfileName;

            //    Globals.ExportToExcel(dt, ConfigurationManager.AppSettings["LocalTempUploadFolder"], Filename);

            //    //move file temp to S3TargetPath, 
            //    //Saving in S3 from ExportToExcel is not saving into required folder, Instead it is save in root of the bucket hence below mechanism is used to move file from Temp to S3
            //    string localpath = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + Filename;
            //    string S3BucketS15ExtractFolder = ConfigurationManager.AppSettings["S3BucketS15ExtractFolder"];
            //    string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketS15ExtractFolder + "/" + newfileName;
            //    Globals.UploadFileToS3(localpath, S3TargetPath);

            //    if ((System.IO.File.Exists(localpath)))
            //    {
            //        System.IO.File.Delete(localpath);
            //    }
            //    //fileList.Add(Filename);
            //    //fileArray = fileList.ToArray();
            //}

            //ZipArchive zip = ZipFile.Open("S15ExtractsList", ZipArchiveMode.Create);
            //foreach (string file in fileList)
            //{
            //    zip.CreateEntryFromFile(Filename, Path.GetFileName(Filename), CompressionLevel.Optimal);
            //}
            //zip.Dispose();
            //var FileData = Globals.DownloadFromS3(Filename);
            //return File(FileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);


            return Json("Success", JsonRequestBehavior.AllowGet); ;
        }

        //Method to download extracts in zipped format
        [ControllerActionFilter]
        public ActionResult DownloadS15ExtractsModel(string StartDate, string EndDate, string ExtractNameList,string ExtractFileNameList,string FileFormat)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var ExtractNameArray = ExtractNameList.Split(',').ToList();
            var ExtractFileNameArray = ExtractFileNameList.Split(',').ToList();
            List<S15ExtractsViewModel> modelList = new List<S15ExtractsViewModel>();
            for (int j = 0; j < ExtractNameArray.Count; j++)
            {
                var KeyValueArray = ExtractNameArray[j].Split(':').ToList();
                var model = new S15ExtractsViewModel

                {
                    Extracts = KeyValueArray[0].ToString(),
                    FileName= ExtractFileNameArray[j].ToString(),
                    ExtractFileType= KeyValueArray[1].ToString()
                };
                // Count = Count + 1;
                modelList.Add(model);

            }

            DateTime StartDT = DateTime.ParseExact(StartDate, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat);
            DateTime EndDT = DateTime.ParseExact(EndDate, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat);
            //var result=RestClient.GetS15Extracts(Convert.ToDateTime(StartDT), Convert.ToDateTime(EndDT), ExtractNameList, CompanyCode,"Download",ExtractFileNameList, FileFormat);
            
            var result = RestClient.GetS15Extracts(modelList,Convert.ToDateTime(StartDT), Convert.ToDateTime(EndDT),CompanyCode, "Download",FileFormat);
            if (result.Count() > 0)
            {
                string S3BucketS15ExtractFolder = ConfigurationManager.AppSettings["S3BucketS15ExtractFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketS15ExtractFolder + "/" + result.ElementAt(0);
                //var ZippedData = Globals.DownloadFromS3(result.ElementAt(0));
                var ZippedData = Globals.DownloadFromS3(S3TargetPath);
                return File(ZippedData, "application/zip", result.ElementAt(0));
            }
            else
            {
                return File("", "application/zip", result.ElementAt(0));
            }
            
            
            //newfileName = "S15_Extracts-" + DateTime.UtcNow.ToString("dd-MM-yyyy") + ".zip";
            //newfileName = newfileName.Replace('/', '-');
            //newfileName = newfileName.Replace(" ", "");
            //newfileName = newfileName.Replace(":", "-");
            //return File(ZippedData, "application/zip",newfileName);
           
        }

        

        [HttpPost]
        [ControllerActionFilter]
        public ActionResult ExportToExcelGlobalPOB()
        {
            //string.Format("{0}/{1}", ConfigurationManager.AppSettings["S15ReportsBucketPath"], FileName),
            string Filename = ConfigurationManager.AppSettings["S15ReportsBucketPath"] + "GlobalPOB_" + System.DateTime.Now.ToString("ddMMyyyyhhmmss")+".xlsx";
            string ApiData = RestClient.GetGGlobalPOB();
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable))); 
            Globals.ExportToExcel(dt, ConfigurationManager.AppSettings["TempUploadFolderName"], Filename);
          
            //Globals.ExportToExcel(dt, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Arvind" + System.DateTime.Now.ToString("ddMMyyyyhhmmss"));
            // return Json(ApiData,JsonRequestBehavior.AllowGet);
          
            var FileData =  Globals.DownloadFromS3(Filename);
            return File(FileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);

        }
        [HttpPost]
        [ControllerActionFilter]
        public ActionResult ExportToExcelGCopaDimension()
        {
            string Filename = "GCopaDimensions_" + System.DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";
            string ApiData = RestClient.GetGCopaDimensions();
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));
            Globals.ExportToExcel(dt, ConfigurationManager.AppSettings["TempUploadFolderName"], Filename);
            var FileData = Globals.DownloadFromS3(Filename);
            return File(FileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);

        }

        public JsonResult GetS15GridData()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var ApiData = RestClient.GetS15GridData(CompanyCode);
            //ViewBag.RefData = ApiData;
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

    }

}
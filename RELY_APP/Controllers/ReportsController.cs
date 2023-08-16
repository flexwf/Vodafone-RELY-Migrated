using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RELY_APP.Helper;
using System.Data;
using Newtonsoft.Json;
using RELY_APP.Utilities;
using System.Configuration;
using RELY_APP.ViewModel;
using System.IO;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class ReportsController : Controller
    {
        // GET: Reports
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        IReportRestClient RestClient = new ReportsRestClient();
        IRLFSAccountingMemoGeneratorRestClient AccountingMemoGeneratorRestClient = new LFSAccountingMemoGeneratorRestClient();
        IRLFSNextStepRestClient NextStepRestClient = new LFSNextStepRestClient();
        ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());

        [ControllerActionFilter]
        public ActionResult RequestPerStatus()
        {
            System.Web.HttpContext.Current.Session["Title"] = "RequestPerStatus";
            ViewBag.Title = "Request Per Status";
            return View();
        }

        [ControllerActionFilter]
        public JsonResult GetRequestPerStatus(string WFType)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetRequestPerStatus(CompanyCode, WFType);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created By : Rakhi Singh
        /// Date: 17th Aug 2018
        /// Desc: method to download Request per status
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadRequestPerStatus(string wftype)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.DownloadRequestPerStatus(CompanyCode, wftype);
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));

            if (dt.Columns.Count > 0)
            {
                /*renaming of the columns in downloaded file as to match with the front end.*/
                dt.Columns["Name"].ColumnName = "Workflow Step";
            }

            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "RequestPerStatus_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
        }
        [ControllerActionFilter]
        public ActionResult AuditReports()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var FileName = RestClient.DownloadAuditReports(CompanyCode);
            string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + FileName;
            var fileData = Globals.DownloadFromS3(S3TargetPath);
            string NewFileName = "AuditReport_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", NewFileName);

        }

        [ControllerActionFilter]
        public JsonResult GetWfTypes(string BaseTableName)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

            var ApiData = RestClient.GetWftypeByBaseTableName(BaseTableName, CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult RequestsPerDateInterval()
        {
            System.Web.HttpContext.Current.Session["Title"] = "RequestsPerDateInterval";
            ViewBag.Title = "Requests Per Date Interval";
            return View();
        }

        [ControllerActionFilter]
        public JsonResult GetRequestsPerDateInterval()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetRequestsPerDateInterval(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created By : Rakhi Singh
        /// Date: 17th Aug 2018
        /// Desc: method to download Request per date interval
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadReqPerDateInt()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.DownloadReqPerDateInt(CompanyCode);
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));

            if (dt.Columns.Count > 0)
            {
                /*renaming of the columns in downloaded file as to match with the front end.*/
                dt.Columns["Request"].ColumnName = "# of Requests";              
            }
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "RequestPerDateInterval_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
        }

        [ControllerActionFilter]
        public ActionResult DownloadAccountingMemo(string EntityType,int EntityId,int SurveyId)
        {
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());

            var ApiData = AccountingMemoGeneratorRestClient.DownloadAccountingMemo(EntityType, EntityId, SurveyId, LoggedInUserId);
            var ApiNextStepsData = AccountingMemoGeneratorRestClient.DownloadNextSteps(EntityType,EntityId);
            var ApiAccountingScenarioMatrix = AccountingMemoGeneratorRestClient.DownloadAccountingScenarioMatrix(EntityId, EntityType);
            string path, FileName;
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));
            DataTable dtNextSteps = (DataTable)JsonConvert.DeserializeObject(ApiNextStepsData, (typeof(DataTable)));
            DataTable dtAccountingScenarioMatrix = (DataTable)JsonConvert.DeserializeObject(ApiAccountingScenarioMatrix, (typeof(DataTable)));
            string[] NextStepColumns = { "QuestionName", "QuestionText", "Response", "IsDone", "ActionTaken", "NextStepText" };
            string[] ASMColumns = { "QuestionCode", "Situation", "ObjectType", "Product", "Scenario", "ScenarioDescription", "Comments" };
            if(dtNextSteps.Rows.Count > 0 && dtAccountingScenarioMatrix.Rows.Count == 0)
            { 
                DataTable newdtNextSteps = dtNextSteps.DefaultView.ToTable(false, NextStepColumns);
                path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
                FileName = "AccountingMemo__" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";

                Globals.ExportAccountingMemoToExcel(dt, newdtNextSteps, dtAccountingScenarioMatrix, path, FileName);
                return File(path + "/" + FileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
            }
            if (dtAccountingScenarioMatrix.Rows.Count > 0 && dtNextSteps.Rows.Count == 0)
            {
                DataTable newdtAccountingScenarioMatrix = dtAccountingScenarioMatrix.DefaultView.ToTable(false, ASMColumns);
                path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
                FileName = "AccountingMemo__" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
                Globals.ExportAccountingMemoToExcel(dt, dtNextSteps, newdtAccountingScenarioMatrix, path, FileName);
                return File(path + "/" + FileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
            }
            if(dtNextSteps.Rows.Count > 0 && dtAccountingScenarioMatrix.Rows.Count > 0)
            {
                DataTable newdtNextSteps = dtNextSteps.DefaultView.ToTable(false, NextStepColumns);
                DataTable newdtAccountingScenarioMatrix = dtAccountingScenarioMatrix.DefaultView.ToTable(false, ASMColumns);
                path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
                FileName = "AccountingMemo__" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
                Globals.ExportAccountingMemoToExcel(dt, newdtNextSteps, newdtAccountingScenarioMatrix, path, FileName);
                return File(path + "/" + FileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
            }
            else
            {
                path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
                FileName = "AccountingMemo__" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
                Globals.ExportAccountingMemoToExcel(dt, dtNextSteps, dtAccountingScenarioMatrix, path, FileName);
                return File(path + "/" + FileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
            }
            //string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            //string Filename = "AccountingMemo__" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            //DataTable newdt = dt.DefaultView.ToTable(false, "MemoCol1", "MemoCol2");
            //Globals.ExportToExcel(newdt, path, Filename);

            //Globals.ExportAccountingMemoToExcel(dt, newdtNextSteps, newdtAccountingScenarioMatrix, path, Filename);
            
            //string Filename = "\\AccountingMemo_"+DateTime.Now.Date.ToLongDateString()+".xlsx" ;
            //Globals.ExportToExcel(dt, ConfigurationManager.AppSettings["LocalTempUploadFolder"], Filename);
            //return Json("Success", JsonRequestBehavior.AllowGet);
        }

       //Commented By Namita--03 October 2018
        //[ControllerActionFilter]
        //public ActionResult DownloadWordSample(string EntityType, int EntityId, int SurveyId)
        //{
        //    int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //    var ApiData = AccountingMemoGeneratorRestClient.DownloadAccountingMemo(EntityType, EntityId, SurveyId, LoggedInUserId);
        //    DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));

           
        //    string path, FileName;
        //    path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
        //    FileName = "WordSample__" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".docx";

        //    Globals.ExportWordSample(dt,path, FileName);
        //    System.Web.HttpContext.Current.Session["Title"] = "Export Successfully";
        //    ViewBag.Title = "Export Successfully";
        //    return File(path + "/" + FileName, "application/doc",FileName) ;
        //}
        [ControllerActionFilter]
        public ActionResult RequestDetails()
        {
            System.Web.HttpContext.Current.Session["Title"] = "RequestDetails";
            ViewBag.Title = "Request Details";
            return View();
        }

        [ControllerActionFilter]
        public JsonResult GetRequestName()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetRequestName(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult DownloadRequestDetails(int RequestId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetRequestDetails(CompanyCode,RequestId);
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "RequestDetails_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
            
        }

        [ControllerActionFilter]
        public ActionResult RequestsInProgress()
        {
            System.Web.HttpContext.Current.Session["Title"] = "RequestsInProgress";
            ViewBag.Title = "Requests In Progress";
            return View();
        }

        [ControllerActionFilter]
        public JsonResult GetRequestsInProgress(int Interval, int NumberofBuckets)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetRequestsInProgress(CompanyCode,Interval,NumberofBuckets);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        
        /// <summary>
        /// Created By : Rakhi Singh
        /// Date: 21st August 2018
        /// Description: Method to download Request In Progress
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadRequestInProg(int Interval, int NumberofBuckets)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.DownloadRequestInProg(CompanyCode, Interval, NumberofBuckets);
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));
            if (dt.Columns.Count > 0)
            {
                /*renaming of the columns in downloaded file as to match with the front end.*/
                dt.Columns["Id"].ColumnName = "Request Id";
            }
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "RequestsInProgress_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
        }


        [ControllerActionFilter]
        public JsonResult GetStatus(string BaseTableName)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetStatusByBaseTableName(BaseTableName,CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportOfProducts()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Report Of Products";
            ViewBag.Title = "Report Of Products";
            //To do dynamic SysCat and LocalPobType
            var SysCatCode = "KIAS/TARIFF_OPTION";
            var LocalPobType = "Finance";
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ColumnsDataForProductReport = LCSCRC.GetColumnsForProductReport(CompanyCode, SysCatCode, LocalPobType);
            ViewBag.ColumnsDataForProductReport = ColumnsDataForProductReport;
            return View();
        }

        public JsonResult GetFReportOfProducts(string FilterType,string FilterValue)
        {
             string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetReportofproduct(CompanyCode, FilterType,FilterValue);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

     
        /// <summary>
        /// Created By : Rakhi Singh
        /// Date: 17th August 2018
        /// Description: Method to download Reports of Products
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadReportofProducts(string FilterType, string FilterValue)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            string FileName = "";
            DownloadFileNameViewModel model = RestClient.DownloadReportofProducts(CompanyCode, FilterType, FilterValue);
           // FileName = FileName.Replace(" ", "");
            FileName = FileName.Replace('"', ' ').Trim();
            //DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));

            if (FileName != null)
            {
                FileName = model.FileName;
            }
            string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + FileName;
            var fileData = Globals.DownloadFromS3(S3TargetPath);
            var newfileName = "ReportOfProducts_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", newfileName);
        }


        public ActionResult AccountingScenarios()
        {
            System.Web.HttpContext.Current.Session["Title"] = "AccountingScenarios";
            ViewBag.Title = "Accounting Scenarios";
            return View();
        }

        [ControllerActionFilter]
        public JsonResult GetAccountingScenarios(string StartDate,string EndDate)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetAccountingScenarios(CompanyCode, StartDate, EndDate);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created By : Rakhi Singh
        /// Date: 21st August 2018
        /// Description: Method to download Accounting Senerio
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadAccountingSenario(string StartDate, string EndDate)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.DownloadAccountingSenario(CompanyCode, StartDate, EndDate);           
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));
            if (dt.Columns.Count > 0)
            {
                /*renaming of the columns in downloaded file as to match with the front end.*/
                dt.Columns["Id"].ColumnName = "Request ID";
                dt.Columns["Name"].ColumnName = "Request";
                dt.Columns["ScenarioCategory"].ColumnName = "Category";
                dt.Columns["ScenarioTrigger"].ColumnName = "Trigger";
            }
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "AccountingScenarios_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
        }


        [ControllerActionFilter]
        public ActionResult NewScenarioDemands()
        {
            System.Web.HttpContext.Current.Session["Title"] = "NewScenarioDemands";
            ViewBag.Title = "New Scenario Demands";
            return View();
        }

        public JsonResult GetNewScenarioDemand(int Interval, int NumberofBuckets)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetNewScenarioDemand(CompanyCode, Interval, NumberofBuckets);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created By : Rakhi Singh
        /// Date: 21st August 2018
        /// Description: Method to download Senerio Demand
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadScenarioDemands(int Interval, int NumberofBuckets)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.DownloadScenarioDemands(CompanyCode, Interval, NumberofBuckets);
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));
            if (dt.Columns.Count > 0)
            {
                /*renaming of the columns in downloaded file as to match with the front end.*/
                dt.Columns["Id"].ColumnName = "Request Id";
            }
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "NewScenarioDemand_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
        }
        public ActionResult AccountingScenarioList()
        {
            System.Web.HttpContext.Current.Session["Title"] = "AccountingScenarioList";
            ViewBag.Title = "Accounting Scenario List";
            return View();
        }

        public JsonResult GetAccountingScenarioList()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetAccountingScenarioList(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        
        // Created By: Ritu Gangwar Date: 03/08/2018
        // Method to download Accounting Scenerio List with associtaed LifeCycleEvents
        public ActionResult DownloadAccountingScenarioList()
        {
            
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            string FileName = "";
            var model = RestClient.DownloadAccountingScenariosList(CompanyCode);
            if (model != null)
            {
                FileName = model.Name;
            }
            string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + FileName;
            var fileData = Globals.DownloadFromS3(S3TargetPath);
            string NewFileName = "AccountingScenarioList__" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", NewFileName);

            
        }
        
    }
}
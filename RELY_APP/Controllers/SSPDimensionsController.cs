using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static RELY_APP.Utilities.Globals;
using System.IO;

namespace RELY_APP.Controllers
{
    public class SSPDimensionsController : Controller
    {
        SSPDimensionsRestClient SDRC = new SSPDimensionsRestClient();

        FilesUploadHelper filesUploadHelper;

        String LocalStoragePath = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "/";
        
        private string UrlBase = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "/";
        String DeleteURL = "/FileUpload/DeleteFile/?file=";
        String DeleteType = "GET";
        // GET: SSPDimensions
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage SSP Dimensions";
            // Author: Bharat
            // To Download the grid data into the excel
            System.Web.HttpContext.Current.Session["ExcelName"] = "SSPDimensions_File";
            return View();
        }

        public ActionResult Create(string Source,string EntityId)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            System.Web.HttpContext.Current.Session["Title"] = "Configure SSP Dimensions";
            ViewBag.FormType = "Create";
            ViewBag.NewSSPId = SDRC.GetSSPIdForNew(CompanyCode);
            ViewBag.Source = Source;
            ViewBag.EntityId = EntityId;
            var model = new SSPDimensionViewModel();
            model.EffectiveEndDate = new DateTime(2099,12,31,13,00,00);
            return View(model);
        }
        public ActionResult Edit(int Id)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            System.Web.HttpContext.Current.Session["Title"] = "Configure SSP Dimensions";
            ViewBag.FormType = "Edit";
            var model = SDRC.GetById(Id);
            var dt = model.EffectiveEndDate;
            ViewBag.str = dt.Day.ToString() + "/" + dt.Month.ToString() + "/" + dt.Year.ToString();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(SSPDimensionViewModel model,string EffectiveStartDate,string FormType,string Source,string EntityId)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
            try
            {//Date manipulations for Effective Start and End Date
                DateTime SSPStartDate = new DateTime();
                if (!string.IsNullOrEmpty(EffectiveStartDate))
                {
                    EffectiveStartDate = EffectiveStartDate + " 13:00:00"; //This is just a workaround, not fix. due to some time/offset difference, db was saving dates with 2hrs difference. so, taking 3hrs instead of 00hrs
                    SSPStartDate = DateTime.ParseExact(EffectiveStartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                model.EffectiveStartDate = SSPStartDate;
                model.EffectiveEndDate = new DateTime(2099,12,31,13,00,00);
                model.UpdatedById = LoggedInUserId;
                model.UpdatedDateTime = DateTime.UtcNow;
                if (FormType.Equals("Create"))//add new
                {
                    model.CompanyCode = CompanyCode;
                    model.CreatedById = LoggedInUserId;
                    model.CreatedDateTime = DateTime.UtcNow;
                    SDRC.Add(model, Source, EntityId, null);
                }
                else//update
                {
                    SDRC.Update(model, null);
                }
                var OutputJson = new { Source =Source,ErrorMessage = "", PopupMessage = "SSP saved successfully", RedirectToUrl = "/SSPDimensions/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Source = Source;
                //If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    TempData["Error"] = ex.Data["ErrorMessage"] as string;
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page and because of this error message is left blank.
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/SSPDimensions/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/SSPDimensions/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Request System could not be created";
                    throw ex;
                }
            }
        }
        [HttpGet]
        public JsonResult Delete(int id)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            try
            {
                SDRC.Delete(id, null);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "SSP deleted successfully", RedirectToUrl = "" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                    }
                    var OutputJson = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "SSP could not be deleted";
                    throw ex;
                }
            }
        }


        [HttpGet]
        public JsonResult GetDataForGrid(string EntityType, int EntityId)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var data = SDRC.GetDataForGrid(EntityType, EntityId, CompanyCode);
            return Json(data,JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetExistingSspsCount()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            int count = SDRC.GetExistingSspsCount(CompanyCode);
            return Json(count, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetExistingSsps(string sortdatafield, string sortorder, int pagesize, int pagenum)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var data = SDRC.GetExistingSsps(CompanyCode, sortdatafield, sortorder, pagesize, pagenum, FilterQuery);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetBySSPId(int SelectedSspIdValue)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var data = SDRC.GetBySSPId(SelectedSspIdValue, CompanyCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DetachSSP(int EntityId, string EntityType)
        {
            SDRC.DetachSSP(EntityId, EntityType,null);
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AttachSSP(int EntityId, string EntityType, int SspId)
        {
            SDRC.AttachSSP(EntityId, EntityType, SspId, null);
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        // Author: Bharat
        // To Download SSP Dimension grid data
        public ActionResult DownloadGridData()
        {
            int LoggedInUserId = Convert.ToInt32(Globals.GetSessionData("UserId"));
            int LoggedInRoleId = Convert.ToInt32(Globals.GetSessionData("CurrentRoleId"));
            var Workflow = System.Web.HttpContext.Current.Session["ExcelName"] as string;
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            string FileName = "";
            var model = SDRC.DownloadGetGridDataByWorkflowId(LoggedInUserId, Workflow, LoggedInRoleId, CompanyCode);
            if (model != null)
            {
                FileName = model.Name;
            }
            string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + FileName;
            var fileData = Globals.DownloadFromS3(S3TargetPath);
            var newfileName = Workflow + "_" + DateTime.UtcNow.ToString("dd-MM-yyyy") + ".xlsx";
            return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", newfileName);
        }

        // Author: Bharat
        // To Upload bulk Data

        public JsonResult GetByUserForRequestUploadGrid()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            string UserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
            var Data = SDRC.GetByUserForRequestUploadGrid(CompanyCode, UserId);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Uploads()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Upload";
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            XSSPDimensionViewModel model = new XSSPDimensionViewModel();
            ViewBag.FormType = "Upload";
            System.Web.HttpContext.Current.Session["Title"] = "Upload SSP";
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;
            //Below line will decide how to display UploadDocument PartialView on the page for Upload Utility
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true, ExistingFilesList = null };
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "SSPDimensions";
            return View(model);

        }
        //This method is called when user licks on upload button after adding payee file
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Uploads(HttpPostedFileBase File1, string AttachedComments, bool SaveFileInBucket, bool IsDataUpload)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            String S3tempPath = "/" + Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]).ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
            filesUploadHelper = new FilesUploadHelper(DeleteURL, DeleteType, StorageRoot, UrlBase, S3tempPath, CompanyCode, LocalStoragePath);
            var context = HttpContext.Request;
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            ILCompanySpecificColumnsRestClient PayeeColsClient = new LCompanySpecificColumnsRestClient();
            string fileLocation = "";
            try
            {
                if (Request.Files["File1"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["File1"].FileName);
                    string name = System.IO.Path.GetFileNameWithoutExtension(Request.Files["File1"].FileName);
                    string FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        fileLocation = string.Format("{0}/{1}", ConfigurationManager.AppSettings["LocalTempUploadFolder"], FileNames);

                        Request.Files["File1"].SaveAs(fileLocation);

                        string filename = FileNames;
                        string fullpath = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\" + filename;//
                        string localpath = fullpath;
                        string S3BucketCopaDimensionFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                        string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketCopaDimensionFolder + "/" + filename;
                        Globals.UploadFileToS3(localpath, S3TargetPath);

                        #region Loading sheets for excel in dataset

                        string excelConnectionString = string.Empty;
                        excelConnectionString = string.Format(ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"], fileLocation);
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable PayeeSheetColumns = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, "Sheet1$", null }); //

                        if (PayeeSheetColumns == null || PayeeSheetColumns.Rows.Count == 0)
                        {
                            TempData["Message"] = "Sheet name should be Sheet1";
                            excelConnection.Dispose();
                            return View();
                        }

                        var columnListPayee = new List<string>();

                        if (PayeeSheetColumns != null)
                        {
                            columnListPayee.AddRange(from DataRow column in PayeeSheetColumns.Rows select column["Column_name"].ToString());
                        }

                        #region Validating Payee Template First
                        if (!ValidatePayeeSheetHeader(columnListPayee))
                        {
                            TempData["Message"] = "Uploaded file does not seem to match the Rely template. Kindly download fresh template and try again.";
                            excelConnection.Dispose();
                            if (System.IO.File.Exists(fileLocation))
                                System.IO.File.Delete(fileLocation);
                            return View();
                        }

                        #endregion

                        OleDbCommand command_reader = new OleDbCommand();
                        try
                        {
                            string dataReader = "SELECT count(*) from [Sheet1$]";
                            command_reader = new OleDbCommand(dataReader, excelConnection);
                            int sheetRowCount = (int)command_reader.ExecuteScalar();
                            command_reader.Dispose();
                            excelConnection.Dispose();
                        }
                        catch (Exception)
                        {
                            TempData["Message"] = "Uploaded file does not seem to match the template. Kindly download fresh template and try again.";
                            excelConnection.Dispose();
                            if (System.IO.File.Exists(fileLocation))
                                System.IO.File.Delete(fileLocation);
                            command_reader.Dispose();
                            return View();
                        }
                        #region Validating Template First                       
                        var resultList = new List<FilesUploadViewModel>();
                        var data = new FilesUploadViewModel();
                        data.FileName = File1.FileName;
                        resultList.Add(data);
                        var currentContext = HttpContext;
                        filesUploadHelper.UploadAndShowResults(HttpContext, resultList, false);
                        #endregion
                        //call RestClient for bulk upload
                        var Result = SDRC.UploadSSPs(FileNames, UserRoleId.ToString(), CompanyCode, LoggedInUserId.ToString(), null);
                        ViewBag.ReturnMessage = "Validation completed please check status of your batch in the grid.";
                        #endregion
                        return View();
                    }

                }
                return RedirectToAction("Index", "SSPDimensions");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Record could not be validated";
                File1 = null;
                return View();
            }
        }
        private Boolean ValidatePayeeSheetHeader(List<string> columnList)
        {	//Checking Header column	
            if (!columnList.Contains("SspId")) return false;
            if (!columnList.Contains("SspAmount")) return false;
            if (!columnList.Contains("Start Date")) return false;
            if (!columnList.Contains("End Date")) return false;
            if (!columnList.Contains("Operation")) return false;
            return true;
        }
        private LBatchViewModelForRequestGrid GetBatchDetailsById(int Id)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);

            LBatchViewModelForRequestGrid model = SDRC.GetDetailsById(CompanyCode, Id);
            return model;
        }

        [HttpGet]
        public ActionResult DownloadErrorFile(int Id)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);

            var model = GetBatchDetailsById(Id);
            int BatchNumber = model.XBatchNumber;
            //call to API for getting filename.
            string FileName = SDRC.DownloadRequestUploadErrors(CompanyCode, BatchNumber);
            Thread.Sleep(10000);
            string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + FileName;
            var FileData = Globals.DownloadFromS3(S3TargetPath);
            return File(FileData, "application/unknown", FileName);
        }


        [HttpGet]
        public ActionResult DownloadDocument(int Id)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var model = GetBatchDetailsById(Id);
            string FileName = model.LbfFileName;
            string OriginalFileName = FileName.Split('.')[0];
            string extn = FileName.Split('.')[1];
            string S3BucketPPMDataFilesFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketPPMDataFilesFolder + "/" + FileName;
            var FileData = Globals.DownloadFromS3(S3TargetPath);
            return File(FileData, "application/unknown", FileName);
        }
        [HttpGet]
        public JsonResult UploadValidatedRequestBatch(int Id)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            string UserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            var model = GetBatchDetailsById(Id);
            int BatchNumber = model.XBatchNumber;
            SDRC.UploadValidatedRequestBatch(CompanyCode, BatchNumber, UserId, Convert.ToInt32(UserRoleId));
            return Json(String.Empty, JsonRequestBehavior.AllowGet);
        }

        private string StorageRoot
        {
            get { return Path.Combine((LocalStoragePath)); }
        }
    }
}
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{
    public class LReconFileUploadController : Controller
    {

        IRSysCatRestClient SysCatRestClient = new RSysCatRestClient();
        //LReconFileFormatsRestClient FileFormatRestClient = new LReconFileFormatsRestClient();
        ILReconColumnMappingRestClient RCMRC = new LReconColumnMappingRestClient();
        ILReconBucketRestclient RestClient = new LReconBucketRestclient();
        // GET: LReconFileUpload
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Recon Uploads";
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;
            ReadAndValidateReferenceDataViewModel ValidationResult = new ReadAndValidateReferenceDataViewModel();
            ViewBag.HideButton = ValidationResult.HideButton;
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false, CanUploadMultipleFiles = false, SaveFileInBucket = false, ExistingFilesList = null };

            return View();
        }

        public JsonResult BindSysCatDropDown()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var SysCatName = SysCatRestClient.GetSysCatforDropDown(CompanyCode);
            return Json(SysCatName, JsonRequestBehavior.AllowGet);
        }

        //GetFileFormat
        public JsonResult BindFileFormatDropDown(int SysCatId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var FileFormat = RCMRC.GetFileFormatBySysCat(SysCatId, CompanyCode);
            return Json(FileFormat, JsonRequestBehavior.AllowGet);
        }



        public JsonResult UploadFile(int SysCatId, int FileFormatId)
        {

            var File = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>; //Session["UploadedFilesList"] as List<FileUploadViewModel>;
           // File = File.Where(p => p.ActivityType == "Upload").ToList();
            if (File != null && File.Count() > 0)
            {
                Session["fname"] = File[0].OriginalFileName;
                Session["FileName"] = File[0].FileName;
                Session["FilePath"] = File[0].FilePath;
            }
            BulkDataValidationViewModel BulkDataModel = new BulkDataValidationViewModel();


            var Ext = File[0].FileName.Split('.');
            if (Ext[1] == "csv")
            { 
                string ValidationResult = Globals.ReadAndValidateCSV(FileFormatId, SysCatId);

                

                //If result contains word "sorry", it means Validation of header fails. We will validate data only if header validation succeeds. 
                //Else, if ValidationResult value > 0 means there are invalid records
                if (ValidationResult.IndexOf("Sorry", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    BulkDataModel.PopUpErrorMessage = ValidationResult;
                    BulkDataModel.HideSaveButton = false;
                    BulkDataModel.PopUpSuccessMessage = null;
                }
                else // Reaching here means header is valid and data is inserted and validated into temporary table (TRefDataValidation) in database.
                {
                    if (Convert.ToInt32(ValidationResult) > 0) //if ValidationResult value > 0 means there are invalid records
                    {
                        //show invalid grid
                        ViewBag.HideButton = 0;//Disable Valid Records button
                        BulkDataModel.PopUpErrorMessage = null;
                        BulkDataModel.HideSaveButton = false;
                        BulkDataModel.PopUpSuccessMessage = null;
                    }
                    else
                    {
                        //Data is valid, validation success message needs to be displayed
                        BulkDataModel.PopUpErrorMessage = null;
                        BulkDataModel.HideSaveButton = true;
                        BulkDataModel.PopUpSuccessMessage = "Data has been successfully validated. Click on save button to proceed.";
                    }
                    BulkDataModel.ErrorRowsCount = Convert.ToInt32(ValidationResult);
                }
                return Json(BulkDataModel, JsonRequestBehavior.AllowGet);
            }
            //return Json(string.Empty, JsonRequestBehavior.AllowGet);
        
            else
            {
                BulkDataModel.PopUpErrorMessage = "Only csv files are allowed";
                return Json(BulkDataModel, JsonRequestBehavior.AllowGet);
            }
        }




        [ControllerActionFilter]
        public string DataTableToJSONWithJavaScriptSerializer(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }

        public JsonResult GetAttributesForCSVData(int FileFormatId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //Effective Dating
            //bool IsEffectiveDated = TypesRestClient.GetByReferenceTypeByName(LRefType).IsEffectiveDated;

            var LRCM = new LReconColumnMappingRestClient();
            //List<LReconColumnMappingViewModel> ReconData = new List<LReconColumnMappingViewModel>();
            var ReconColumnData = LRCM.GetLReconColumnsByFormatId(CompanyCode, FileFormatId);
            //if (IsEffectiveDated)
            //{
            //    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "EffectiveStartDate", Label = "StartDate", DataType = "datetime", IsMandatory = true });
            //    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "EffectiveEndDate", Label = "EndDate", DataType = "datetime", IsMandatory = true });
            //}
            //CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Id", Label = "Id", DataType = "int" });
            ViewBag.LReferenceDataColumnList = ReconColumnData;
            return Json(ReconColumnData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
       // [ControllerActionFilter]//will be uncommented after db mappings
        //To Save Data
        public ActionResult SaveData(int SysCatId, int FileFormatId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
            try
            {
                //Upload Utility Section to move files to specified location and  save FileList  in db starts
                string UploadedDataFile = "";
                if (System.Web.HttpContext.Current.Session["UploadedFilesList"] != null)
                {
                    //Get Uploaded File List
                    var AttachedSupportingFiles = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
                    AttachedSupportingFiles = AttachedSupportingFiles.Where(p => p.ActivityType == "Attachment").ToList();
                    if (AttachedSupportingFiles.Count > 0)
                        UploadedDataFile = AttachedSupportingFiles.ElementAt(0).FileName;
                    System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//Clear Session now
                    //Move file to S3
                    string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
                    if (!string.IsNullOrEmpty(UploadedDataFile))
                    {
                        string fullpath = path + "\\" + UploadedDataFile;//
                        string localpath = fullpath;
                        string S3BucketReconBucketFolder = ConfigurationManager.AppSettings["S3BucketReconBucketFolder"];
                        string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReconBucketFolder + "/" + UploadedDataFile;
                        Globals.UploadFileToS3(localpath, S3TargetPath);
                    }
                }

                RestClient.Add(CompanyCode, SysCatId,FileFormatId,LoggedInUserId, UploadedDataFile,null);
                return Json(new { success = true, ErrorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string redirectUrl = ex.Data["RedirectToUrl"] as string;
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = ex.Data["ErrorMessage"].ToString(), PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
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
                    var OutputJson = new { ErrorMessage = "Record could not be updated", PopupMessage = "", RedirectToUrl = "" };
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Record could not be updated";
                    //throw ex;
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
            }

        }



        public JsonResult GetInvalidRecords(int SysCatId,int FileFormatId, string sortdatafield, string sortorder, int pagesize, int pagenum)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var ErrorData = RestClient.GetInvalidRecords(SysCatId, FileFormatId, CompanyCode, sortdatafield, sortorder, pagesize, pagenum, FilterQuery);
            return Json(ErrorData, JsonRequestBehavior.AllowGet);
        }

    }
}
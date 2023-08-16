using Newtonsoft.Json;
using NPOI.SS.UserModel;
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class GCopaDimensionsController : Controller
    {
        //int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();

       // OleDbConnection con = new OleDbConnection(@"Data Source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com; Initial Catalog = RELYDevDb; User ID=euitdsadbmuser; Password=d-4nATAk&gezac; Provider=SQLOLEDB");
        DataTable dt = new DataTable();
        IGCopaDimensionRestClient RestClient = new GCopaDimensionRestClient();
        // GET: GCopaDimensions
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Global Copa Dimension";
            ViewBag.Title = "Manage Global Copa Dimension";
            ViewBag.HideButton = 0;
            //var File = Session["UploadedFilesList"] as List<string>;
            //if (File != null)
            //{
            //    Session["FileName"] = File[0];
            //    ReadAndValidateExcel();
            //    Session["UploadedFilesList"] = null;
            //}
            ReadAndValidateViewModel model = null;
            var File = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>; //Session["UploadedFilesList"] as List<FileUploadViewModel>;
            if (File != null)
            {
                Session["FileName"] = File[0].FileName;
                Session["FilePath"] = File[0].FilePath;
                model = ReadAndValidateExcel();
                ViewBag.PopUpErrorMessage = model.PopUpErrorMessage;
                Session["UploadedFilesList"] = null;
            }
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false, CanUploadMultipleFiles = false,SaveFileInBucket=false };
            // ViewBag.GetGCopaDimensions = GetGCopaDimensions();
            return View();
        }

        [CustomAuthorize]
        //as discussed with JS, CustomAuthorize filter is sufficientfor this method. This method is not being called directly but from with in LocalPobs
        //[ControllerActionFilter]
        public JsonResult GetGCopaDimensions(int Class,int PobCatalogueId)
        {

            var ApiData = RestClient.GetGCopaDimensionsByClass(Class, PobCatalogueId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGCopaDimensionsDropDown(int Class)
        {

            var ApiData = RestClient.GetGCopaDimensionsforDropDown(Class);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetAllGCopaDimensions()
        {

            var ApiData = RestClient.GetGCopaDimensions();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        //DownloadGCopaDimensions

        //ExportCSV GCopaDimension
        public ActionResult DownloadGCopaDimensions()
        {

            var ApiData = RestClient.DownloadGCopaDimensions();
          //  return Json(ApiData, JsonRequestBehavior.AllowGet);

           // var ApiData = LERC.DownloadLifeEventDetails(ASId);
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));
            if (dt.Columns.Count > 0)
            {
                dt.Columns.Remove("Id");
                dt.Columns.Remove("CreatedById");
                dt.Columns.Remove("UpdatedById");
                dt.Columns.Remove("UpdatedDateTime");
                dt.Columns.Remove("CreatedDateTime");
                dt.Columns.Remove("CompanyCode");
                //dt.Columns["CreatedDateTime"].ColumnName = "Uploaded On";
                dt.Columns["Class"].SetOrdinal(0);
                dt.Columns["CopaValue"].SetOrdinal(1);
                dt.Columns["Description"].SetOrdinal(2);
                dt.Columns["Dimension"].SetOrdinal(3);
                dt.Columns["DimentionClassDescription"].SetOrdinal(4);
                //dt.Columns["Uploaded On"].SetOrdinal(5);
            }
               
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "CopaDimensions_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);

        }

        //To Download Template
        [ControllerActionFilter]
        public ActionResult DownloadTemplate()
        {
            //code migrated to Globals
            //TBD for Vijay
            //All columns need to retirive from Database not from Grid
            //List<GCopaDimensionViewModel> list = new List<GCopaDimensionViewModel>();
            //var gv = new GridView();
            //var CopaDimensionList = RestClient.GetGCopaDimensions().ToList();
            //DataTable dt = new DataTable();

            //var s = CopaDimensionList[0];
            //dt.Columns.Add("Class");
            //dt.Columns.Add("CopaValue");
            //dt.Columns.Add("Description");
            //dt.Columns.Add("Dimension");
            //dt.Columns.Add("DimentionClassDescription");

            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string filename = "CopaDimension.xlsx";
            // string Result = Globals.ExportToExcel(dt, path, filename);
            string result = Globals.GenerateTemplate("GCopaDimensions", null);

            return File(path + "/" + filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
           
        }

        [ControllerActionFilter]
        public ReadAndValidateViewModel ReadAndValidateExcel()
        {
            ReadAndValidateViewModel model = Globals.ReadAndValidateExcel("GCopaDimensions", "");//no selecter Type here
            if (model != null)
            {
                ViewBag.ValidData = model.ValidData;
                ViewBag.ErrorData = model.ErrorData;
                ViewBag.HideButton = model.HideButton? 1 : 0 ;
                if (!string.IsNullOrEmpty(model.PopUpErrorMessage))
                {
                    ViewBag.PopUpErrorMessage = model.PopUpErrorMessage;
                }

            }
            return model;
            //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
            ////string path=Session["FilePath"].ToString();
            ////string filename = "GGLobalPOB.xlsx";
            ////string path = "C:\\RELYTemp\\";
            //string filename = Session["FileName"].ToString();
            //string fullpath = path + "\\" + filename;

            //string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
            //OleDbConnection con = new System.Data.OleDb.OleDbConnection(connectionString);
            //con.Open();
            //OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter("select * from [SHEET1$]", con);
            //DataSet ds = new DataSet();
            //cmd.Fill(ds);
            //DataTable dt = ds.Tables[0];
            //con.Close();
            //List<string> list = new List<string>();
            ////validating templates
            //bool IsRowOk = true;
            //string RowLevelValidationMessage = null;
            //DataTable ValidationOK = dt.Clone();
            //DataTable ValidationError = dt.Clone();
            //var InvalidRowIndex = 0;
            //var ValidRowIndex = 0;

            //DataColumn Col = ValidationError.Columns.Add("ValidationMessage", System.Type.GetType("System.String"));
            //Col.SetOrdinal(0);// to put the column in position 0;

            //DataColumn RowNo = ValidationError.Columns.Add("SrNo", typeof(int));
            //Col.SetOrdinal(1);// to put the column in position 0;
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{

            //    foreach (DataColumn col in dt.Columns)
            //    {
            //        // string ValidationResult = ValidateExcelData(col.ColumnName, dt.Rows[i].Field<object>(col));
            //        string ValidationResult = Globals.ValidateCellData("GCopaDimensions", col.ColumnName, dt.Rows[i].Field<object>(col), col.ColumnName, "", CompanyCode);
            //        if (ValidationResult != "Success")
            //        {
            //            //Reaching here means Field Data is Not valid,APPEND Validation Error Message for the row
            //            //As we are reporting only first issue for the record, now there is no need to append messages
            //            //RowLevelValidationMessage = RowLevelValidationMessage + "| " + ValidationResult;
            //            RowLevelValidationMessage = ValidationResult;

            //            IsRowOk = false;
            //            break;
            //        }
            //        else
            //        {
            //            IsRowOk = true;
            //        }
            //    }
            //    if (IsRowOk)
            //    {
            //        //Add Row to ValidationOK Table
            //        //ValidationOK.Rows.Add(dt.Rows[i]);
            //        ValidationOK.ImportRow(dt.Rows[i]);
            //        ValidRowIndex = i - 1;

            //    }
            //    else
            //    {
            //        //Add Row to ValidationError Table
            //        //DataRow ErrorRow =ValidationError.NewRow();
            //        //Reset the Variables

            //        ValidationError.ImportRow(dt.Rows[i]);
            //        InvalidRowIndex = ValidationError.Rows.Count - 1;
            //        ValidationError.Rows[InvalidRowIndex]["ValidationMessage"] = RowLevelValidationMessage;//setting cell value for the invalid record
            //        ValidationError.Rows[InvalidRowIndex]["SrNo"] = i;
            //        IsRowOk = false;
            //    }
            //}

            //ViewBag.ValidData = ValidationOK;
            //ViewBag.ErrorData = ValidationError;
            //if (ValidationError.Rows.Count == 0)
            //{
            //    ViewBag.HideButton = 1;
            //}
            //else
            //{
            //    ViewBag.HideButton = 1;
            //}
            ////  return View();

            #region VG Sir
            //foreach (DataRow row in dt.Rows)
            //{
            //    foreach (DataColumn col in dt.Columns)
            //    {
            //        string ValidationResult = ValidateExcelData(col.ColumnName, row.Field<object>(col));
            //        if (ValidationResult != "Success")
            //        {
            //            //Reaching here means Field Data is Not valid,APPEND Validation Error Message for the row

            //            RowLevelValidationMessage = RowLevelValidationMessage + "| " + ValidationResult;
            //            IsRowOk = false;

            //        }
            //    }
            //    if (IsRowOk)
            //    {
            //        //Add Row to ValidationOK Table
            //    }
            //    else
            //    {
            //        //Add Row to ValidationError Table

            //        //Reset the Variables
            //        IsRowOk = true;
            //        RowLevelValidationMessage = null;
            //    }

            //}
            #endregion

        }
        //not being used anymore

        //private string ValidateExcelData(string DbColumnName, object ExcelCellValue)
        //{

        //    var data = Globals.ValidateData("GCopaDimensions", DbColumnName, ExcelCellValue);
        //    //var ValidData = new List<LReferenceDataViewModel>();
        //    //var ModelList = new List<LReferenceDataViewModel>();
        //    //var ErroredList = new List<LReferenceDataViewModel>();
        //    //var model = new LReferenceDataViewModel();
        //    // var LReferenceDataColumnList = LCSRDC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LReferenceData", "Germany").ToList();

        //    //if (DbColumnName == "AttributeC01" && typeof(string) == ExcelCellValue.GetType())
        //    //{
        //    //    return "";
        //    //}
        //    //else
        //    //{
        //    //    return "Incorect Value for C01";
        //    //}
        //    //if (DbColumnName == "AttributeC02" && typeof(string) == ExcelCellValue.GetType())
        //    //{
        //    //    model.AttributeC02 = Convert.ToString(ExcelCellValue);
        //    //}
        //    //else
        //    //{
        //    //    return "Incorect Value for C02";
        //    //}
        //    //if (DbColumnName == "AttributeI01" && typeof(int) == ExcelCellValue.GetType())
        //    //{
        //    //    model.AttributeI01 = Convert.ToInt32(ExcelCellValue);
        //    //}
        //    //else
        //    //{
        //    //    return "Incorect Value for I01";
        //    //}
        //    //if (DbColumnName == "AttributeB01" && typeof(bool) == ExcelCellValue.GetType())
        //    //{
        //    //    model.AttributeB01 = Convert.ToBoolean(ExcelCellValue);
        //    //}
        //    //else
        //    //{
        //    //    return "Incorect Value for B01";
        //    //}
        //    //if (DbColumnName == "AttributeM01" && typeof(int) == ExcelCellValue.GetType())
        //    //{
        //    //    model.AttributeM01 = Convert.ToString(ExcelCellValue);
        //    //}
        //    //else
        //    //{
        //    //    return "Incorect Value for M01";
        //    //}
        //    return "Success";
        //}

        [ControllerActionFilter]
        public JsonResult SaveValidRecords(object[] GridData)
        {
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //Need to Implement Full Load(Delete Data from Table Then Save it)
            try
            {
                if (GridData != null)
                {
                    foreach (string[] Data in GridData)
                    {
                        var data = Data;
                        var GridArray = data;
                        var model = new GCopaDimensionViewModel
                        {
                            //Id = Convert.ToInt32(GridArray[0]),
                            Class = Convert.ToInt32(GridArray[0]),
                            CopaValue = String.IsNullOrEmpty(GridArray[1]) ? null : GridArray[1],
                            Description = String.IsNullOrEmpty(GridArray[2]) ? null : GridArray[2],
                            Dimension = String.IsNullOrEmpty(GridArray[3]) ? null : GridArray[3],
                            DimentionClassDescription = String.IsNullOrEmpty(GridArray[4]) ? null : GridArray[4],
                            CreatedById = LoggedInUserId,
                            UpdatedById = LoggedInUserId,
                            CreatedDateTime = System.DateTime.Now,
                            UpdatedDateTime = System.DateTime.Now,
                            CompanyCode = CompanyCode
                        };
                        RestClient.Add(model, null);
                    }
                
                //if (model.Id == 0)//entry does not exist in database
                //{
                //    RestClient.Add(model, null);
                //}
                //else
                //{
                //    RestClient.Update(model, null);
                //}

                //Move file to S3
                string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
                string filename = Session["FileName"].ToString();
                //string filename = "TestFile.txt";
                string fullpath = path + "\\" + filename;//
                string localpath = fullpath;
                string S3BucketCopaDimensionFolder = ConfigurationManager.AppSettings["S3BucketCopaDimensionFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketCopaDimensionFolder + "/" + filename;
                Globals.UploadFileToS3(localpath, S3TargetPath);
                // ===============  Vijay Added Following code for Audit Table
                //insert entry to LAudit table                
                LAuditViewModel audit = new LAuditViewModel();
                audit.RelyProcessName = "CopaDimensions";
                audit.VFProcessName = "CopaDimensions";
                audit.ControlCode = "Audit";
                audit.ControlDescription = null;
                audit.Action = "Upload";
                audit.ActionType = "Create";//any one of these 3 Create/Update/Delete
                audit.ActionedById = LoggedInUserId;
                audit.ActionedByRoleId = LoggedInRoleId;
                audit.ActionDateTime = DateTime.Now;
                audit.OldStatus = null;
                audit.NewStatus = "Create"; //when there is no WF, use Status Column
                audit.EntityType = "GCopaDimensions";//BasetableName, hardcoded when no WF
                audit.EntityId = 0;
                audit.EntityName = "Copa Dimensions";
                audit.WorkFlowId = null;
                audit.CompanyCode = CompanyCode;
                audit.Comments = "Uploaded file " + Session["FileName"];
                ILAuditRestClient ARC = new LAuditRestClient();
                ARC.Add(audit, null);
            }
                //================================End Audit Table
                var OutputJson = new { ErrorMessage = "Create Types", PopupMessage = "", RedirectToUrl = "/GGlobalPOB/Index" };
                return Json(new { success = true, ErrorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string redirectUrl = ex.Data["RedirectToUrl"] as string;
                if (ex.Data["ErrorCode"] != null)
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)Globals.ExceptionType.Type1:
                            return Json(new { success = false, ErrorMessage = ex.Data["ErrorMessage"] }, JsonRequestBehavior.AllowGet);

                        case (int)Globals.ExceptionType.Type2:

                            ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                            return Json(new { success = false, ErrorMessage = ex.Data["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                        case (int)Globals.ExceptionType.Type3:
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                            return Json(new { success = false, ErrorMessage = ex.Data["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                        case (int)Globals.ExceptionType.Type4:
                            ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                            return Json(new { success = false, ErrorMessage = ex.Data["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                        default:
                            return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
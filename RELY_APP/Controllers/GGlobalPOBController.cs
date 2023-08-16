using Newtonsoft.Json;
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class GGlobalPOBController : Controller
    {
        DataTable dt = new DataTable();
        //int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        
        IGGlobalPOBRestClient RestClient = new GGlobalPOBRestClient();
        // GET: GGlobalPOB
        [ControllerActionFilter]
        public ActionResult Index()

        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Global POB";
            ViewBag.Title = "Manage Global POB";
            ViewBag.HideButton = 0;
            var File = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>; //Session["UploadedFilesList"] as List<FileUploadViewModel>;
            ReadAndValidateViewModel model = null;
            if (File != null)
            {
                Session["FileName"] = File[0].FileName;
                Session["FilePath"] = File[0].FilePath;
                model = ReadAndValidateExcel();
                ViewBag.PopUpErrorMessage = model.PopUpErrorMessage;
                Session["UploadedFilesList"] = null;
            }
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false, CanUploadMultipleFiles = false, SaveFileInBucket = false };

            return View();
        }
        [CustomAuthorize]
        public JsonResult GetAllGGlobalPOB()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var ApiData = RestClient.GetAllGGlobalPOB(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize]
        //[ControllerActionFilter]
        public JsonResult GetGGlobalPOB(int PobCatalogueId,string GpobType)
        {
            var ApiData = RestClient.GetGGlobalPOB(PobCatalogueId, GpobType);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //Method to Get GlobalPob for Localpob Dropdown 
        public JsonResult GetGGlobalPOBforDropDown()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var ApiData = RestClient.GetGGlobalPOBforDropDown(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        


        /// <summary>
        /// Date:29 june 2018
        /// Created By: Rakhi Singh
        /// Description: This method is used to download GlobalPOB data & export to excel 
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadGGlobalPOB()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var ApiData = RestClient.DownloadGGlobalPOB(CompanyCode);           
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));

            if (dt.Columns.Count > 0)
            {
                //following lines are used to remove the columns from dt which do not want to show in grid of front view
                dt.Columns.Remove("Id");
                dt.Columns.Remove("CreatedById");
                dt.Columns.Remove("UpdatedById");
                dt.Columns.Remove("UpdatedDateTime");
                dt.Columns.Remove("Label");
                dt.Columns.Remove("CreatedDateTime");
                dt.Columns.Remove("CompanyCode");
                //lines used to update the names of columns of grid
                //dt.Columns["CreatedDateTime"].ColumnName = "Uploaded On";
                dt.Columns["Type"].SetOrdinal(0);
                dt.Columns["Name"].SetOrdinal(1);
                dt.Columns["Description"].SetOrdinal(2);
                dt.Columns["Category"].SetOrdinal(3);
                dt.Columns["IFRS15Account"].SetOrdinal(4);
                //dt.Columns["Uploaded On"].SetOrdinal(5);
            }
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "GlobalPOB_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);

        }

        //To Download Template
        [ControllerActionFilter]
        public ActionResult DownloadTemplate()
        {
            //code migrated to Globals

            //List<GGlobalPOBViewModel> list = new List<GGlobalPOBViewModel>();
            //var gv = new GridView();
            //var GGlobalPOBList = RestClient.GetGGlobalPOB().ToList();
            //DataTable dt = new DataTable();
            //var s = GGlobalPOBList[0];
            //dt.Columns.Add("Type");
            //dt.Columns.Add("Name");
            //dt.Columns.Add("Description");
            //dt.Columns.Add("Category");
            //dt.Columns.Add("IFRS15Account");
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string filename = "GGLobalPOB.xlsx";
           // string FullFilePath = path + filename;
            //if (Directory.Exists(Path.GetDirectoryName(file)))
            //{
            //    System.IO.File.Delete(file);
            //}

            //if (System.IO.File.Exists(FullFilePath))
            //{
            //    System.IO.File.Delete(FullFilePath);
            //}
            //string Result = Globals.ExportToExcel(dt, path, filename);
            Globals.GenerateTemplate("GGlobalPobs", null);
            return File(path + "/" + filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);

        }

        [ControllerActionFilter]
        public ReadAndValidateViewModel ReadAndValidateExcel()
        {
            ReadAndValidateViewModel model = Globals.ReadAndValidateExcel("GGlobalPobs", "");//no selecter Type here
            if (model != null)
            {
                ViewBag.ValidData = model.ValidData;
                ViewBag.ErrorData = model.ErrorData;
                ViewBag.HideButton = model.HideButton ? 1 : 0;
                if(!string.IsNullOrEmpty(model.PopUpErrorMessage))
                {
                    ViewBag.PopUpErrorMessage = model.PopUpErrorMessage;
                }
            }
            return model;
            /*string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //Read And Validate Comment
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
            //string path=Session["FilePath"].ToString();
            //string filename = "GGLobalPOB.xlsx";
            //string path = "C:\\RELYTemp\\";
            string filename = Session["FileName"].ToString();
            string fullpath = path + "\\" + filename;
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
            //string connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES""", fullpath);
            OleDbConnection con = new System.Data.OleDb.OleDbConnection(connectionString);
            con.Open();
            OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter("select * from [SHEET1$]", con);
            DataSet ds = new DataSet();
            cmd.Fill(ds);
            DataTable dt = ds.Tables[0];
            con.Close();
            List<string> list = new List<string>();



            bool IsRowOk = true;
            string RowLevelValidationMessage = null;
            DataTable ValidationOK = dt.Clone();
            DataTable ValidationError = dt.Clone();

            var InvalidRowIndex = 0;
            var ValidRowIndex = 0;

            DataColumn Col = ValidationError.Columns.Add("ValidationMessage", System.Type.GetType("System.String"));
            Col.SetOrdinal(0);// to put the column in position 0;

            DataColumn RowNo = ValidationError.Columns.Add("SrNo", typeof(int));
            Col.SetOrdinal(1);// to put the column in position 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                foreach (DataColumn col in dt.Columns)
                {
                    // string ValidationResult = ValidateExcelData(col.ColumnName, dt.Rows[i].Field<object>(col));
                    string ValidationResult = Globals.ValidateCellData("GGlobalPobs", col.ColumnName, dt.Rows[i].Field<object>(col), col.ColumnName, "", CompanyCode);
                    if (ValidationResult != "Success")
                    {//Reaching here means Field Data is Not valid,APPEND Validation Error Message for the row
                        //As we are reporting only first issue for the record, now there is no need to append messages
                        //RowLevelValidationMessage = RowLevelValidationMessage + "| " + ValidationResult;
                        RowLevelValidationMessage = ValidationResult;

                        IsRowOk = false;
                        break;
                    }
                    else
                    {
                        IsRowOk = true;
                    }
                }
                if (IsRowOk)
                {

                    //Add Row to ValidationOK Table
                    //ValidationOK.Rows.Add(dt.Rows[i]);
                    ValidationOK.ImportRow(dt.Rows[i]);
                    ValidRowIndex = i - 1;

                }
                else
                {
                    //Add Row to ValidationError Table
                    //DataRow ErrorRow =ValidationError.NewRow();
                    //Reset the Variables

                    ValidationError.ImportRow(dt.Rows[i]);
                    InvalidRowIndex = ValidationError.Rows.Count - 1;
                    ValidationError.Rows[InvalidRowIndex]["ValidationMessage"] = RowLevelValidationMessage;//setting cell value for the invalid record
                    ValidationError.Rows[InvalidRowIndex]["SrNo"] = i;
                    IsRowOk = false;
                }
            }

            ViewBag.ValidData = ValidationOK;
            ViewBag.ErrorData = ValidationError;
            if (ValidationError.Rows.Count == 0)
            {
                ViewBag.HideButton = 1;
            }
            else
            {
                ViewBag.HideButton = 1;
            } //  return View();*/

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

        //    var data = Globals.ValidateData("GGLobalPOB", DbColumnName, ExcelCellValue);
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
        public JsonResult SaveData(string GridArray,int collength)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());

            try
            {
                string GrdAry = GridArray.TrimEnd(',');
                string[] keyValuePair = GrdAry.Split(',');
                List<GGlobalPOBViewModel> modelList = new List<GGlobalPOBViewModel>();
                for (var i = 0; i < keyValuePair.Length; i = i + collength)
                {
                    GGlobalPOBViewModel model = new GGlobalPOBViewModel();
                    for (var j = 0; j < collength; j++)
                    {
                        string[] dataList = keyValuePair[i + j].Split(':');
                        var AttributeName = dataList[0]; var AttributeValue = dataList[1];
                        PropertyInfo propertyInfo = model.GetType().GetProperty(AttributeName);
                        if (propertyInfo != null)
                        {
                            var datatype = propertyInfo.PropertyType.Name;
                            if (datatype.Contains("Nullable"))
                            {
                                if (AttributeValue.Equals("undefined") || AttributeValue.Equals("null") || String.IsNullOrEmpty(AttributeValue))
                                {
                                    //no need to set the value when null or undefined
                                }
                                else
                                {
                                    propertyInfo.SetValue(model, Convert.ChangeType(AttributeValue, Nullable.GetUnderlyingType(propertyInfo.PropertyType)), null);
                                }
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(AttributeValue) || AttributeValue.Equals("undefined") || AttributeValue.Equals("null"))
                                {
                                    //no need to set the value when null or undefined
                                }
                                else
                                {
                                    propertyInfo.SetValue(model, Convert.ChangeType(AttributeValue, propertyInfo.PropertyType), null);
                                }
                            }
                        }
                    }
                    model.CreatedById = LoggedInUserId;
                    model.UpdatedById = LoggedInUserId;
                    model.UpdatedDateTime = DateTime.Now;
                    model.CreatedDateTime = DateTime.Now;
                    model.CompanyCode = CompanyCode;
                    modelList.Add(model);
                   
                }

                if (modelList.Count > 0)
                {
                    RestClient.Add(modelList, null);
                }
                //Move file to S3
                string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
                string filename = Session["FileName"].ToString();
                //string filename = "TestFile.txt";
                string fullpath = path + "\\" + filename;//
                string localpath = fullpath;
                // string S3TempFolderPath =CompanyCode+"/"+ ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                string S3BucketGlobalPobFolder = ConfigurationManager.AppSettings["S3BucketGlobalPobFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketGlobalPobFolder + "/" + filename;
                Globals.UploadFileToS3(localpath, S3TargetPath);

                // ===============  Vijay Added Following code for Audit Table
                //insert entry to LAudit table                
                LAuditViewModel audit = new LAuditViewModel();
                audit.RelyProcessName = "GlobalPOB";
                audit.VFProcessName = "GlobalPOB";
                audit.ControlCode = "Audit";
                audit.ControlDescription = null;
                audit.Action = "Upload";
                audit.ActionType = "Create";//any one of these 3 Create/Update/Delete
                audit.ActionedById = LoggedInUserId;
                audit.ActionedByRoleId = LoggedInRoleId;
                audit.ActionDateTime = DateTime.Now;
                audit.OldStatus = null;
                audit.NewStatus = "Create"; //when there is no WF, use Status Column
                audit.EntityType = "GGlobalPOBs";//BasetableName, hardcoded when no WF
                audit.EntityId = 0;
                audit.EntityName = "Global POB";
                audit.WorkFlowId = null;
                audit.CompanyCode = CompanyCode;
                audit.Comments = "Uploaded file " + Session["FileName"];
                ILAuditRestClient ARC = new LAuditRestClient();
                ARC.Add(audit, null);


                //================================End Audit Table

                return Json(new { success = true,  ErrorMessage = string.Empty }, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetAllUseCaseIndicators()
        {
            UseCaseIndicatorRestClient UCRC = new UseCaseIndicatorRestClient();
            var ApiData = UCRC.GetAll();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        //SG commented this code as this function had lots of flaws and not being used now.
        /*
                public JsonResult SaveValidRecords(object[] GridData)
                {
                    //Need to Implement Full Load(Delete Data from Table Then Save it)
                    try
                    {
                        foreach (string[] Data in GridData)
                        {
                            var data = Data;
                            var GridArray = data;
                            var model = new GGlobalPOBViewModel
                            {
                                // Id = Convert.ToInt32(GridArray[0]),
                                Name = GridArray[0],
                                Type = GridArray[1],
                                Description = GridArray[2],
                                Category = GridArray[3],
                                IFRS15Account = GridArray[4],
                                // UploadedOn=GridArray[6],
                                CreatedById = LoggedInUserId,
                                UpdatedById = LoggedInUserId,
                                CreatedDateTime = System.DateTime.Now,
                                UpdatedDateTime = System.DateTime.Now
                            };
                            RestClient.Add(model, null);
                        }
                        //Move file to S3
                        string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
                        string filename = Session["FileName"].ToString();
                        //string filename = "TestFile.txt";
                        string fullpath = path + "\\" + filename;//
                        string localpath = fullpath;
                        // string S3TempFolderPath =CompanyCode+"/"+ ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                        string S3BucketGlobalPobFolder = ConfigurationManager.AppSettings["S3BucketGlobalPobFolder"];
                        string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketGlobalPobFolder + "/" + filename;
                        Globals.UploadFileToS3(localpath, S3TargetPath);

                        // ===============  Vijay Added Following code for Audit Table
                        //insert entry to LAudit table                
                        LAuditViewModel audit = new LAuditViewModel();
                        audit.RelyProcessName = "GlobalPOB";
                        audit.VFProcessName = "GlobalPOB";
                        audit.ControlCode = "Audit";
                        audit.ControlDescription = null;
                        audit.Action = "Upload";
                        audit.ActionType = "Create";//any one of these 3 Create/Update/Delete
                        audit.ActionedById = LoggedInUserId;
                        audit.ActionedByRoleId = LoggedInRoleId;
                        audit.ActionDateTime = DateTime.Now;
                        audit.OldStatus = null;
                        audit.NewStatus = "Create"; //when there is no WF, use Status Column
                        audit.EntityType = "GGlobalPOBs";//BasetableName, hardcoded when no WF
                        audit.EntityId = 0;
                        audit.EntityName = "Global POB";
                        audit.WorkFlowId = null;
                        audit.CompanyCode = CompanyCode;
                        audit.Comments = "Uploaded file " + Session["FileName"];
                        ILAuditRestClient ARC = new LAuditRestClient();
                        ARC.Add(audit, null);


                        //================================End Audit Table

                        var OutputJson = new { ErrorMessage = "Create Types", PopupMessage = "", RedirectToUrl = "/GGlobalPOB/Index" };
                        return Json(OutputJson, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        throw ex;

                    }
                }

                */
    }


}
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LFSQuestionBankController : Controller
    {
        DataTable dt = new DataTable();
        //int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();

        IRLFSQuestionBankRestClient RestClient = new LFSQuestionBankRestClient();
        ILFSItemTypeRestClient LITRC = new LFSItemTypeRestClient();
        // GET: LFSQuestionBank
        [CustomAuthorize]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Question Bank ";
            ViewBag.Title = "Manage Question Bank";
            ViewBag.HideButton = 0;
            var File = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>; 
            if (File != null)
            {
                Session["FileName"] = File[0].FileName;
                Session["FilePath"] = File[0].FilePath;
                ReadAndValidateExcel();
                Session["UploadedFilesList"] = null;
            }
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false, CanUploadMultipleFiles = false, SaveFileInBucket = false };

            return View();
        }

        [CustomAuthorize]
        public JsonResult GetQuestionBank()
        {

            var ApiData = RestClient.GetQuestionBank();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
                
        /// <summary>
        /// Created By : Rakhi Singh
        /// Date: 17th August 2018
        /// Description: Method to download LFSQuestion Bank
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadLFSQuestionBank()
        {
            var ApiData = RestClient.DownloadLFSQuestionBank();
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));
            if (dt.Columns.Count > 0)
            {
                dt.Columns.Remove("Id");
                dt.Columns.Remove("CreatedById");
                dt.Columns.Remove("UpdatedById");
                dt.Columns.Remove("UpdatedDateTime");
                dt.Columns.Remove("ItemtypeId");
                dt.Columns["CreatedDateTime"].ColumnName = "Uploaded On";
                dt.Columns["Name"].ColumnName = "ItemTypeName";               
                dt.Columns["CompanyCode"].SetOrdinal(0);
                dt.Columns["ItemTypeName"].SetOrdinal(1);              
            }
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "QuestionBank_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
        }



        [CustomAuthorize]
        public JsonResult GetQuestionDetailsByQuestionCode(string QuestionCode)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetQuestionDetailsForDemand(QuestionCode, CompanyCode);
            return Json(ApiData);
        }
        //To Download Template
        [CustomAuthorize]
        public ActionResult DownloadTemplate()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            List<GGlobalPOBViewModel> list = new List<GGlobalPOBViewModel>();
            var gv = new GridView();
            var QuestionBankList = RestClient.GetQuestionBank().ToList();
            DataTable dt = new DataTable();

            var s = QuestionBankList[0];


            dt.Columns.Add("CompanyCode");
           // dt.Columns.Add("ItemTypeId");
            dt.Columns.Add("ItemTypeName");
            dt.Columns.Add("QuestionCode");
            dt.Columns.Add("QuestionName");
            dt.Columns.Add("Comments");
            dt.Columns.Add("ReadableName");
            dt.Columns.Add("QuestionText");
            dt.Columns.Add("ToolTip");
            dt.Columns.Add("VGAPReference");
            dt.Columns.Add("IFRSReference");
            dt.Columns.Add("IsActive");
            dt.Columns.Add("InternalNotes");

            //ItemTypeName Display
            DataTable dtItemTypeName = new DataTable();
            dtItemTypeName.Columns.Add("Id");
            dtItemTypeName.Columns.Add("Name");

            var ApiItemTypeName = LITRC.GetByCompanyCode(CompanyCode);
            for (var i = 0; i < ApiItemTypeName.Count(); i++)
            {
                DataRow drItemTypeName = dtItemTypeName.NewRow();
                drItemTypeName[0] = ApiItemTypeName.ElementAt(i).Id;
                drItemTypeName[1] = ApiItemTypeName.ElementAt(i).Name;
                dtItemTypeName.Rows.Add(drItemTypeName);
                dtItemTypeName.AcceptChanges();
            }

            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string filename = "LFSQuestionBank.xlsx";

            string FullFilePath = path + filename;
            //if (Directory.Exists(Path.GetDirectoryName(file)))
            //{
            //    System.IO.File.Delete(file);
            //}

            //if (System.IO.File.Exists(FullFilePath))
            //{
            //    System.IO.File.Delete(FullFilePath);
            //}

            string Result = Globals.ExportTemplateToExcelForLFSQuestionBank(dt, dtItemTypeName, path, filename);
            return File(path + "/" + filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);

        }

        [CustomAuthorize]
        public void ReadAndValidateExcel()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

            //Read And Validate Comment
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
            string filename = Session["FileName"].ToString();
            string fullpath = path + "\\" + filename;
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
            //string connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES""", fullpath);
            OleDbConnection con = new System.Data.OleDb.OleDbConnection(connectionString);
            con.Open();         
            //get the sheets/tables in uploaded excel 
            var tables = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            var sheet1 = tables.Rows[0]["TABLE_NAME"].ToString(); // fetching name of first sheet as we upload data from first sheet only.
            OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [" + sheet1 + "]", con);
            // OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter("select * from [SHEET1$]", con);
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
            string ValidationResult;

            DataColumn Col = ValidationError.Columns.Add("ValidationMessage", System.Type.GetType("System.String"));
            Col.SetOrdinal(0);// to put the column in position 0;

            DataColumn RowNo = ValidationError.Columns.Add("SrNo", typeof(int));
            Col.SetOrdinal(1);// to put the column in position 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName == "ItemTypeName")
                    {
                        string value = dt.Rows[i].Field<string>(col);
                        int ItemTypeId = LITRC.GetItemTypeIdbyName(value);
                        col.ColumnName = "ItemTypeId";
                        ValidationResult = Globals.ValidateCellData("LFSQuestionBank", col.ColumnName, ItemTypeId, col.ColumnName, "", CompanyCode);
                        col.ColumnName = "ItemTypeName";
                    }
                    else
                    {
                        ValidationResult = Globals.ValidateCellData("LFSQuestionBank", col.ColumnName, dt.Rows[i].Field<object>(col), col.ColumnName, "", CompanyCode);

                    }
                    // string ValidationResult = ValidateExcelData(col.ColumnName, dt.Rows[i].Field<object>(col));
                    //string ValidationResult = Globals.ValidateCellData("LFSQuestionBank", col.ColumnName, dt.Rows[i].Field<object>(col), col.ColumnName, "", CompanyCode);
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
            }
            //  return View();

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

        //    var data = Globals.ValidateData("LFSQuestionBank", DbColumnName, ExcelCellValue);
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

        [CustomAuthorize]
        public JsonResult SaveData(string GridArray, int collength)
        {
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

            try
            {
                string GrdAry = GridArray.TrimEnd(',');
                string[] keyValuePair = GrdAry.Split(',');
                List<LFSQuestionBankViewModel> modelList = new List<LFSQuestionBankViewModel>();
                for (var i = 0; i < keyValuePair.Length; i = i + collength)
                {
                    LFSQuestionBankViewModel model = new LFSQuestionBankViewModel();
                    for (var j = 0; j < collength; j++)
                    {
                        string[] dataList = keyValuePair[i + j].Split(':');
                        var AttributeName = dataList[0]; var AttributeValue = dataList[1];

                        //Saving SurveyId instead of Survey Name in LFSChapter table
                        if (AttributeName == "ItemTypeName")
                        {
                            AttributeName = "ItemTypeId";
                            int ItemTypeId = LITRC.GetItemTypeIdbyName(AttributeValue);
                            AttributeValue = (ItemTypeId).ToString();

                        }

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

                    modelList.Add(model);

                }

                if (modelList.Count > 0)
                {
                    RestClient.AddData(modelList, null);
                }
                //Move file to S3
                string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
                string filename = Session["FileName"].ToString();
                //string filename = "TestFile.txt";
                string fullpath = path + "\\" + filename;//
                string localpath = fullpath;
                // string S3TempFolderPath =CompanyCode+"/"+ ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                string S3BucketGlobalPobFolder = ConfigurationManager.AppSettings["S3BucketFSQuestionBankFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketGlobalPobFolder + "/" + filename;
                Globals.UploadFileToS3(localpath, S3TargetPath);

                // ===============  Vijay Added Following code for Audit Table
                //insert entry to LAudit table                
                LAuditViewModel audit = new LAuditViewModel();
                audit.RelyProcessName = "QuestionBank";
                audit.VFProcessName = "QuestionBank";
                audit.ControlCode = "Audit";
                audit.ControlDescription = null;
                audit.Action = "Upload";
                audit.ActionType = "Create";//any one of these 3 Create/Update/Delete
                audit.ActionedById = LoggedInUserId;
                audit.ActionedByRoleId = LoggedInRoleId;
                audit.ActionDateTime = DateTime.Now;
                audit.OldStatus = null;
                audit.NewStatus = "Create"; //when there is no WF, use Status Column
                audit.EntityType = "LFSQuestionBank";//BasetableName, hardcoded when no WF
                audit.EntityId = 0;
                audit.EntityName = "QuestionBank";
                audit.WorkFlowId = null;
                audit.CompanyCode = CompanyCode;
                audit.Comments = "Uploaded file " + Session["FileName"];
                ILAuditRestClient ARC = new LAuditRestClient();
                ARC.Add(audit, null);


                //================================End Audit Table

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
        //RG Commented this code
        //public JsonResult SaveValidRecords(object[] GridData)
        //{
        //    //Need to Implement Full Load(Delete Data from Table Then Save it)
        //    try
        //    {
        //        foreach (string[] Data in GridData)
        //        {
        //            var data = Data;
        //            var GridArray = data;
        //            var model = new LFSQuestionBankViewModel
        //            {
        //                // Id = Convert.ToInt32(GridArray[0]),
        //                CompanyCode = GridArray[0],
        //                ItemTypeId = Convert.ToInt32(GridArray[1]),
        //                QuestionCode = GridArray[2],
        //                QuestionName = GridArray[3],
        //                Comments = GridArray[4],
        //                ReadableName = GridArray[5],
        //                QuestionText = GridArray[6],
        //                ToolTip = GridArray[7],
        //                VGAPReference = GridArray[8],
        //                IFRSReference = GridArray[9],
        //                IsActive = Convert.ToBoolean(GridArray[10]),
        //                InternalNotes = GridArray[11],
        //                // UploadedOn=GridArray[6],
        //                CreatedById = LoggedInUserId,
        //                UpdatedById = LoggedInUserId,
        //                CreatedDateTime = System.DateTime.Now,
        //                UpdatedDateTime = System.DateTime.Now
        //            };
        //            RestClient.AddData(model, null);
        //        }
        //        //Move file to S3
        //        string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
        //        string filename = Session["FileName"].ToString();
        //        //string filename = "TestFile.txt";
        //        string fullpath = path + "\\" + filename;//
        //        string localpath = fullpath;
        //        // string S3TempFolderPath =CompanyCode+"/"+ ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
        //        string S3BucketGlobalPobFolder = ConfigurationManager.AppSettings["S3BucketGlobalPobFolder"];
        //        string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketGlobalPobFolder + "/" + filename;
        //        Globals.UploadFileToS3(localpath, S3TargetPath);

        //        // ===============  Vijay Added Following code for Audit Table
        //        //insert entry to LAudit table                
        //        LAuditViewModel audit = new LAuditViewModel();
        //        audit.RelyProcessName = "QuestionBank";
        //        audit.VFProcessName = "QuestionBank";
        //        audit.ControlCode = "Audit";
        //        audit.ControlDescription = null;
        //        audit.Action = "Upload";
        //        audit.ActionType = "Create";//any one of these 3 Create/Update/Delete
        //        audit.ActionedById = LoggedInUserId;
        //        audit.ActionedByRoleId = LoggedInRoleId;
        //        audit.ActionDateTime = DateTime.Now;
        //        audit.OldStatus = null;
        //        audit.NewStatus = "Create"; //when there is no WF, use Status Column
        //        audit.EntityType = "LFSQuestionBank";//BasetableName, hardcoded when no WF
        //        audit.EntityId = 0;
        //        audit.EntityName = "Question Bank";
        //        audit.WorkFlowId = null;
        //        audit.CompanyCode = CompanyCode;
        //        audit.Comments = "Uploaded file " + Session["FileName"];
        //        ILAuditRestClient ARC = new LAuditRestClient();
        //        ARC.Add(audit, null);


        //        //================================End Audit Table

        //        var OutputJson = new { ErrorMessage = "Create Types", PopupMessage = "", RedirectToUrl = "/LFSQuestionBank/Index" };
        //        return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;

        //    }
        //}


    }
}
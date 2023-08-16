using Newtonsoft.Json;
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

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LFSAnswerBankController : Controller
    {
        DataTable dt = new DataTable();
        //int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();

        IRLFSAnswerBankRestClient RestClient = new LFSAnswerBankRestClient();

        // GET: LFSAnswerBank
        [CustomAuthorize]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Answer Bank ";
            ViewBag.Title = "Manage Answer Bank";
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
        public JsonResult GetAnswerBank()
        {

            var ApiData = RestClient.GetAnswerBank();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created By : Rakhi Singh
        /// Date: 17th August 2018
        /// Description: Method to download Answer Bank
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadAnswerBank()
        {
            var ApiData = RestClient.DownloadAnswerBank();
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));

            if (dt.Columns.Count > 0)
            {
                dt.Columns.Remove("id");
                dt.Columns.Remove("createdbyid");
                dt.Columns.Remove("updatedbyid");
                dt.Columns.Remove("updateddatetime");
                dt.Columns["createddatetime"].ColumnName = "uploaded on";               
            }
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "AnswerBank_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
        }

        /*not required anymore*/
        //[CustomAuthorize]
        //public JsonResult GetConclusionText(int EntityId,string EntityType, string QuestionCode)
        //{
        //    string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //    string ConclusionText = null;
        //    var ApiData = RestClient.GetConclusionText(CompanyCode,EntityId,EntityType,QuestionCode);
        //    if (ApiData != null)
        //    {
        //        if (string.IsNullOrEmpty(ApiData.Name))
        //        {
        //            ConclusionText = "No conclusion text found";
        //        }
        //        else
        //        {
        //            ConclusionText = ApiData.Name;
        //        }
        //    }
        //    else
        //    {
        //        ConclusionText = "No conclusion text found";
        //    }
        //    return Json(new { Conclusion = ConclusionText }, JsonRequestBehavior.AllowGet);
        //}

        [CustomAuthorize]
        public JsonResult GetByQuestionCode(string QuestionCode, string ItemTypeName)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var AnswerData = RestClient.GetByQuestionCode(QuestionCode, ItemTypeName,CompanyCode);
            var htmlTobeRendered ="<select class='form-control' id='" + QuestionCode.Trim() + "' name='" + QuestionCode + "' style='text-align: left; '><option>-- Select Here --</option>AnswerOptionsPlaceholder</select></div>";
            //retrieve all the answer options for the Question
            if (AnswerData != null )
            {
                foreach (var aa in AnswerData)
                {
                    string value = "";
                    if (aa.AnswerOption.IndexOf(',') > 0)
                    {
                        //value = aa.AnswerOption.Substring(0, aa.AnswerOption.IndexOf(','));
                        value = aa.AnswerOption.Replace(",","\\,");
                    }
                    else
                    {
                        value = aa.AnswerOption;
                    }

                    var AnsOption = "<option value='" + value + "'>" + aa.AnswerOption + "</option>";
                    htmlTobeRendered = htmlTobeRendered.Replace("AnswerOptionsPlaceholder", AnsOption + "AnswerOptionsPlaceholder");
                }
            }
            htmlTobeRendered = htmlTobeRendered.Replace("AnswerOptionsPlaceholder", "");
            return Json(new {answerData= htmlTobeRendered }, JsonRequestBehavior.AllowGet);
        }

        //To Download Template
        [CustomAuthorize]
        public ActionResult DownloadTemplate()
        {
            List<LFSAnswerBankViewModel> list = new List<LFSAnswerBankViewModel>();
            var gv = new GridView();
            var AnswerList = RestClient.GetAnswerBank().ToList();
            DataTable dt = new DataTable();

            var s = AnswerList[0];


            dt.Columns.Add("CompanyCode");
            dt.Columns.Add("QuestionCode");
            dt.Columns.Add("AnswerOption");
            dt.Columns.Add("MemoText");
            dt.Columns.Add("Comments");
            dt.Columns.Add("ScenarioCategory");
            dt.Columns.Add("ScenarioTrigger");
            dt.Columns.Add("InternalNotes");
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string filename = "LFSAnswerBank.xlsx";

            string FullFilePath = path + filename;
            string Result = Globals.ExportToExcel(dt, path, filename);
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

            DataColumn Col = ValidationError.Columns.Add("ValidationMessage", System.Type.GetType("System.String"));
            Col.SetOrdinal(0);// to put the column in position 0;

            DataColumn RowNo = ValidationError.Columns.Add("SrNo", typeof(int));
            Col.SetOrdinal(1);// to put the column in position 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                foreach (DataColumn col in dt.Columns)
                {
                    // string ValidationResult = ValidateExcelData(col.ColumnName, dt.Rows[i].Field<object>(col));
                    string ValidationResult = Globals.ValidateCellData("LFSAnswerBank", col.ColumnName, dt.Rows[i].Field<object>(col), col.ColumnName, "", CompanyCode);
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



        }

        //not being ued
        //private string ValidateExcelData(string DbColumnName, object ExcelCellValue)
        //{

        //    var data = Globals.ValidateData("LFSAnswerBank", DbColumnName, ExcelCellValue);
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
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
            try
            {
                string GrdAry = GridArray.TrimEnd(',');
                string[] keyValuePair = GrdAry.Split(',');
                List<LFSAnswerBankViewModel> modelList = new List<LFSAnswerBankViewModel>();
                for (var i = 0; i < keyValuePair.Length; i = i + collength)
                {
                    LFSAnswerBankViewModel model = new LFSAnswerBankViewModel();
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
                string S3BucketGlobalPobFolder = ConfigurationManager.AppSettings["S3BucketFSAnswerBankFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketGlobalPobFolder + "/" + filename;
                Globals.UploadFileToS3(localpath, S3TargetPath);

                // ===============  Vijay Added Following code for Audit Table
                //insert entry to LAudit table                
                LAuditViewModel audit = new LAuditViewModel();
                audit.RelyProcessName = "AnswerBank";
                audit.VFProcessName = "AnswerBank";
                audit.ControlCode = "Audit";
                audit.ControlDescription = null;
                audit.Action = "Upload";
                audit.ActionType = "Create";//any one of these 3 Create/Update/Delete
                audit.ActionedById = LoggedInUserId;
                audit.ActionedByRoleId = LoggedInRoleId;
                audit.ActionDateTime = DateTime.Now;
                audit.OldStatus = null;
                audit.NewStatus = "Create"; //when there is no WF, use Status Column
                audit.EntityType = "LFSAnswerBank";//BasetableName, hardcoded when no WF
                audit.EntityId = 0;
                audit.EntityName = "AnswerBank";
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
        //            var model = new LFSAnswerBankViewModel
        //            {
        //                // Id = Convert.ToInt32(GridArray[0]),
        //                CompanyCode = GridArray[0],
        //                QuestionCode = GridArray[1],
        //                AnswerOption = GridArray[2],
        //                MemoText = GridArray[3],
        //                Comments = GridArray[4],
        //                InternalNotes=GridArray[5],
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
        //        string fullpath = path + "\\" + filename;//
        //        string localpath = fullpath;
        //        // string S3TempFolderPath =CompanyCode+"/"+ ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
        //        string S3BucketGlobalPobFolder = ConfigurationManager.AppSettings["S3BucketGlobalPobFolder"];
        //        string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketGlobalPobFolder + "/" + filename;
        //        Globals.UploadFileToS3(localpath, S3TargetPath);


        //        //insert entry to LAudit table                
        //        LAuditViewModel audit = new LAuditViewModel();
        //        audit.RelyProcessName = "AnswerBank";
        //        audit.VFProcessName = "AnswerBank";
        //        audit.ControlCode = "Audit";
        //        audit.ControlDescription = null;
        //        audit.Action = "Upload";
        //        audit.ActionType = "Create";//any one of these 3 Create/Update/Delete
        //        audit.ActionedById = LoggedInUserId;
        //        audit.ActionedByRoleId = LoggedInRoleId;
        //        audit.ActionDateTime = DateTime.Now;
        //        audit.OldStatus = null;
        //        audit.NewStatus = "Create"; //when there is no WF, use Status Column
        //        audit.EntityType = "LFSAnswerBank";//BasetableName, hardcoded when no WF
        //        audit.EntityId = 0;
        //        audit.EntityName = "Answer Bank";
        //        audit.WorkFlowId = null;
        //        audit.CompanyCode = CompanyCode;
        //        audit.Comments = "Uploaded file " + Session["FileName"];
        //        ILAuditRestClient ARC = new LAuditRestClient();
        //        ARC.Add(audit, null);


        //        //================================End Audit Table

        //        var OutputJson = new { ErrorMessage = "Create Types", PopupMessage = "", RedirectToUrl = "/LFSAnswerBank/Index" };
        //        return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;

        //    }
        //}

    }
}
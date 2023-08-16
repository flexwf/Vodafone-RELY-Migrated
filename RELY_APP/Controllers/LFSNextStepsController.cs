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
    public class LFSNextStepsController : Controller
    {
        DataTable dt = new DataTable();
        //int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();

        LFSNextStepRestClient RestClient = new LFSNextStepRestClient();


        // GET: LFSNextSteps
        [CustomAuthorize]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage NextStep ";
            ViewBag.Title = "Manage NextStep";
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
        public JsonResult GetNextStep()
        {

            var ApiData = RestClient.GetNextSteps();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created By : Rakhi Singh
        /// Date: 17th August 2018
        /// Description: Method to download LFSNextSteps
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadNextSteps()
        {
            var ApiData = RestClient.DownloadNextSteps();
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
            string Filename = "NextSteps_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
        }

        //To Download Template
        [CustomAuthorize]
        public ActionResult DownloadTemplate()
        {
            List<LFSNextStepViewModel> list = new List<LFSNextStepViewModel>();
            //var gv = new GridView();
            var NextStepList = RestClient.GetNextSteps().ToList();
            DataTable dt = new DataTable();

            var s = NextStepList[0];


            dt.Columns.Add("CompanyCode");
            dt.Columns.Add("QuestionCode");
            dt.Columns.Add("AnswerOption");
            dt.Columns.Add("NextStepText");
            dt.Columns.Add("InternalNotes");
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string filename = "LFSNextSteps.xlsx";

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
                    string ValidationResult = Globals.ValidateCellData("LFSNextSteps", col.ColumnName, dt.Rows[i].Field<object>(col), col.ColumnName, "", CompanyCode);
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

        }

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
                List<LFSNextStepViewModel> modelList = new List<LFSNextStepViewModel>();
                for (var i = 0; i < keyValuePair.Length; i = i + collength)
                {
                    LFSNextStepViewModel model = new LFSNextStepViewModel();
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
                string S3BucketGlobalPobFolder = ConfigurationManager.AppSettings["S3BucketFSNextStepsFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketGlobalPobFolder + "/" + filename;
                Globals.UploadFileToS3(localpath, S3TargetPath);

               
                //insert entry to LAudit table                
                LAuditViewModel audit = new LAuditViewModel();
                audit.RelyProcessName = "NextSteps";
                audit.VFProcessName = "NextSteps";
                audit.ControlCode = "Audit";
                audit.ControlDescription = null;
                audit.Action = "Upload";
                audit.ActionType = "Create";//any one of these 3 Create/Update/Delete
                audit.ActionedById = LoggedInUserId;
                audit.ActionedByRoleId = LoggedInRoleId;
                audit.ActionDateTime = DateTime.Now;
                audit.OldStatus = null;
                audit.NewStatus = "Create"; //when there is no WF, use Status Column
                audit.EntityType = "LFSNextSteps";//BasetableName, hardcoded when no WF
                audit.EntityId = 0;
                audit.EntityName = "NextSteps";
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

    }
}
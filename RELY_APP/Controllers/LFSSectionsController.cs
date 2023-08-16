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
    public class LFSSectionsController : Controller
    {
        //int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();

        DataTable dt = new DataTable();
        IRLFSSectionRestClient RestClient = new LFSSectionRestClient();
        LFinancialSurveysRestClient LFSRC = new LFinancialSurveysRestClient();
        // GET: LFSSections
        [CustomAuthorize]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Sections";
            ViewBag.Title = "Manage Sections";
            ViewBag.HideButton = 0;
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
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false, CanUploadMultipleFiles = false, SaveFileInBucket = false };
            return View();
        }

        [CustomAuthorize]
        public JsonResult GetAllSections()
        {

            var ApiData = RestClient.GetSections();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created By : Rakhi Singh
        /// Date: 17th August 2018
        /// Description: Method to download LFSSections
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadLFSSections()
        {
            var ApiData = RestClient.DownloadLFSSections();
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));
            if (dt.Columns.Count > 0)
            {
                dt.Columns.Remove("Id");
                dt.Columns.Remove("CreatedById");
                dt.Columns.Remove("UpdatedById");
                dt.Columns.Remove("UpdatedDateTime");
                dt.Columns.Remove("SurveyId");
                dt.Columns["CreatedDateTime"].ColumnName = "Uploaded On";
                dt.Columns["SurveyName"].ColumnName = "Survey";
                dt.Columns["Survey"].SetOrdinal(0);
                dt.Columns["ChapterCode"].SetOrdinal(1);
                dt.Columns["SectionName"].SetOrdinal(2);
                dt.Columns["SectionCode"].SetOrdinal(3);
                dt.Columns["Ordinal"].SetOrdinal(4);
                dt.Columns["InternalNotes"].SetOrdinal(5);
                dt.Columns["Uploaded On"].SetOrdinal(6);
            }
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "Sections_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
        }


        [CustomAuthorize]
        public ActionResult DownloadTemplate()
        {
            //common code is moved to Globals 
            // string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //// List<LFSSectionItemViewModel> list = new List<LFSSectionItemViewModel>();
            //// var gv = new GridView();
            //// var SectionList = RestClient.GetSections().ToList();
            // DataTable dt = new DataTable();
            // //var s = SectionList[0];
            // dt.Columns.Add("Survey");
            // dt.Columns.Add("ChapterCode");
            // dt.Columns.Add("SectionName");
            // dt.Columns.Add("SectionCode");
            // dt.Columns.Add("Ordinal");
            // dt.Columns.Add("InternalNotes");

            // //Survey Name Display
            // DataTable dtSurvey = new DataTable();
            // dtSurvey.Columns.Add("Id");
            // dtSurvey.Columns.Add("SurveyName");

            // var ApiSurveyName = RestClient.GetSurveyName(CompanyCode);
            // for (var i = 0; i < ApiSurveyName.Count(); i++)
            // {
            //     DataRow dr = dtSurvey.NewRow();
            //     dr[0] = ApiSurveyName.ElementAt(i).Id;
            //     dr[1] = ApiSurveyName.ElementAt(i).SurveyName;
            //     dtSurvey.Rows.Add(dr);
            //     dtSurvey.AcceptChanges();
            // }

             string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
             string filename = "SurveySections.xlsx";
            // string Result = Globals.ExportTemplateToExcel(dt, dtSurvey, path, filename);
            string result = Globals.GenerateTemplateForSurvey("LFSSections",filename);
            return File(path + "/" + filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);

        }

        [CustomAuthorize]
        public ReadAndValidateViewModel ReadAndValidateExcel()
        {

            var model = Globals.ReadAndValidateExcel("LFSSections", "");//Selecter type value is empty here, NULL throws error in REST WS
            if (model != null)
            {
                ViewBag.ValidData = model.ValidData;
                ViewBag.ErrorData = model.ErrorData;
                ViewBag.HideButton = model.HideButton ? 1 : 0;
                if (!string.IsNullOrEmpty(model.PopUpErrorMessage))
                {
                    ViewBag.PopUpErrorMessage = model.PopUpErrorMessage;
                }
            }
            return model;

            //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
            //string filename = Session["FileName"].ToString();
            //string fullpath = path + "\\" + filename;
            //string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
            //OleDbConnection con = new System.Data.OleDb.OleDbConnection(connectionString);
            //con.Open();
            ////get the sheets/tables in uploaded excel 
            //var tables = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            //var sheet1 = tables.Rows[0]["TABLE_NAME"].ToString(); // fetch name of first sheet as we upload data from first sheet only.
            //OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [" + sheet1 + "]", con);

            //DataSet ds = new DataSet();
            //cmd.Fill(ds);
            //DataTable dt = ds.Tables[0];
            //con.Close();
            //List<string> list = new List<string>();

            //bool IsRowOk = true;
            //string RowLevelValidationMessage = null;
            //DataTable ValidationOK = dt.Clone();
            //DataTable ValidationError = dt.Clone();

            //var InvalidRowIndex = 0;
            //var ValidRowIndex = 0;
            //string ValidationResult;

            //DataColumn Col = ValidationError.Columns.Add("ValidationMessage", System.Type.GetType("System.String"));
            //Col.SetOrdinal(0);// to put the column in position 0;

            //DataColumn RowNo = ValidationError.Columns.Add("SrNo", typeof(int));
            //Col.SetOrdinal(1);// to put the column in position 0;
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    foreach (DataColumn col in dt.Columns)
            //    {
            //        if (col.ColumnName == "Survey")
            //        {
            //            string value = dt.Rows[i].Field<string>(col);
            //            int SurveyId = LFSRC.GetSurveyIdbyName(value);
            //            col.ColumnName = "SurveyId";
            //            ValidationResult = Globals.ValidateCellData("LFSSections", col.ColumnName, SurveyId, col.ColumnName, "", CompanyCode);
            //            col.ColumnName = "Survey";
            //        }
            //        else
            //        {
            //            ValidationResult = Globals.ValidateCellData("LFSSections", col.ColumnName, dt.Rows[i].Field<object>(col), col.ColumnName, "", CompanyCode);
            //        }
            //        if (ValidationResult != "Success")
            //        {//Reaching here means Field Data is Not valid,APPEND Validation Error Message for the row
            //            //As we are reporting only first issue for the record, now there is no need to append messages
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
            //        ValidationOK.ImportRow(dt.Rows[i]);
            //        ValidRowIndex = i - 1;
            //    }
            //    else
            //    {
            //        //Add Row to ValidationError Table
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
                List<LFSSectionViewModel> modelList = new List<LFSSectionViewModel>();
                for (var i = 0; i < keyValuePair.Length; i = i + collength)
                {
                    LFSSectionViewModel model = new LFSSectionViewModel();
                    for (var j = 0; j < collength; j++)
                    {
                        string[] dataList = keyValuePair[i + j].Split(':');
                        var AttributeName = dataList[0]; var AttributeValue = dataList[1];

                        //Saving SurveyId instead of Survey Name in LFSSection table
                        if (AttributeName == "Survey")
                        {
                            AttributeName = "SurveyId";
                            int SurveyId = LFSRC.GetSurveyIdbyName(AttributeValue);
                            AttributeValue = (SurveyId).ToString();

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
                string fullpath = path + "\\" + filename;//
                string localpath = fullpath;
                string S3BucketGlobalPobFolder = ConfigurationManager.AppSettings["S3BucketFSSectionsFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketGlobalPobFolder + "/" + filename;
                Globals.UploadFileToS3(localpath, S3TargetPath);
                //insert entry to LAudit table                
                LAuditViewModel audit = new LAuditViewModel();
                audit.RelyProcessName = "Section";
                audit.VFProcessName = "Section";
                audit.ControlCode = "Audit";
                audit.ControlDescription = null;
                audit.Action = "Upload";
                audit.ActionType = "Create";//any one of these 3 Create/Update/Delete
                audit.ActionedById = LoggedInUserId;
                audit.ActionedByRoleId = LoggedInRoleId;
                audit.ActionDateTime = DateTime.Now;
                audit.OldStatus = null;
                audit.NewStatus = "Create"; //when there is no WF, use Status Column
                audit.EntityType = "LFSSections";//BasetableName, hardcoded when no WF
                audit.EntityId = 0;
                audit.EntityName = "Section";
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
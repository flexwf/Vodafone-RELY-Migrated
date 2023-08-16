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
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LFSSectionItemsController : Controller
    {
        DataTable dt = new DataTable();
        //int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();

        ILFSSectionItemRestClient RestClient = new LFSSectionItemRestClient();
        ILFinancialSurveysRestClient FSRC = new LFinancialSurveysRestClient();
        ILFSItemTypeRestClient LFSITRC = new LFSItemTypeRestClient();


        // GET: LFSSectionItems
        [CustomAuthorize]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Section Item ";
            ViewBag.Title = "Manage Section Item";
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
        public JsonResult GetSectionItem()
        {

            var ApiData = RestClient.GetSectionItem();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created By : Rakhi Singh
        /// Date: 17th August 2018
        /// Description: Method to download Survey Config
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadSectionItems()
        {
            var ApiData = RestClient.DownloadSectionItems();
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));

            if (dt.Columns.Count > 0)
            {
                dt.Columns.Remove("Id");
                dt.Columns.Remove("SurveyId");
                dt.Columns.Remove("ItemTypeId");
                dt.Columns.Remove("createdbyid");
                dt.Columns.Remove("updatedbyid");
                dt.Columns.Remove("updateddatetime");
                dt.Columns["createddatetime"].ColumnName = "uploaded on";
                dt.Columns["Survey"].SetOrdinal(0);
            }
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "SectionItems_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
        }

        [CustomAuthorize]
        public JsonResult GetBySectionCode(string SectionCode, string ChapterCode, int EntityId, string EntityType,int SurveyId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //getting details of the section
            var ApiData = RestClient.GetBySectionCode(SectionCode, ChapterCode,EntityId,EntityType, CompanyCode,SurveyId);

            for (int i = 0; i < ApiData.Count(); i++)
            {
                //tooltip manipulation for LF characters
                string tooltip = ApiData.ElementAt(i).ToolTip;
                if (tooltip != null)
                {
                    tooltip = tooltip.Replace("\r\n", "<br>");
                    tooltip = tooltip.Replace("!!", "<br/>");
                }
                ApiData.ElementAt(i).ToolTip = tooltip;

                //response manipulation for LF characters
                string response = ApiData.ElementAt(i).Response;
                if (response != null)
                {
                    response = response.Replace("\n\r", "");
                }
                ApiData.ElementAt(i).Response = response;
            }
            ViewBag.GridSectionItems = ApiData;
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //To Download Template
        [CustomAuthorize]
        public ActionResult DownloadTemplate()
        {
            List<LFSSectionItemViewModel> list = new List<LFSSectionItemViewModel>();
            var gv = new GridView();
            var SectionItemList = RestClient.GetSectionItem().ToList();
            DataTable dt = new DataTable();

            var s = SectionItemList[0];
            //dt.Columns.Add("SurveyId");
            dt.Columns.Add("Survey");
            dt.Columns.Add("ChapterCode");
            dt.Columns.Add("SectionCode");
            dt.Columns.Add("QuestionCode");
            dt.Columns.Add("TableCode");
           // dt.Columns.Add("ItemTypeId");
           dt.Columns.Add("ItemTypeName");
            dt.Columns.Add("ItemText");
            dt.Columns.Add("Ordinal");
            dt.Columns.Add("NestingLevel");
            dt.Columns.Add("IsResponseMandatory");
            dt.Columns.Add("IsReponseAutoGenerated");
            dt.Columns.Add("ShowOnQuestionCode");
            dt.Columns.Add("ShowOnAnswerOption");
            dt.Columns.Add("SumOfQuestions");
            dt.Columns.Add("Operator");
            dt.Columns.Add("SumValue");
            dt.Columns.Add("AutomatedResponseTrueValue");
            dt.Columns.Add("AutomatedResponseFalseValue");
            dt.Columns.Add("RelatedQuestionCode");
            dt.Columns.Add("ShowOnAccountingMemo");
            dt.Columns.Add("InternalNotes");

            //Survey Name Display
            DataTable dtSurvey = new DataTable();
            dtSurvey.Columns.Add("Id");
            dtSurvey.Columns.Add("SurveyName");
     
            var ApiSurveyName = RestClient.GetSurveyName(CompanyCode);
            for(var i =0;i<ApiSurveyName.Count();i++)
            {
                DataRow dr = dtSurvey.NewRow();
                dr[0]=ApiSurveyName.ElementAt(i).Id;
                dr[1] = ApiSurveyName.ElementAt(i).SurveyName;
                //dtSurvey.ImportRow(dr);
                dtSurvey.Rows.Add(dr);
                dtSurvey.AcceptChanges();
            }

            //ItemTypeName Display
            DataTable dtItemTypeName = new DataTable();
            dtItemTypeName.Columns.Add("Id");
            dtItemTypeName.Columns.Add("Name");

            var ApiItemTypeName = LFSITRC.GetByCompanyCode(CompanyCode);
            for(var i=0;i<ApiItemTypeName.Count();i++)
            {
                DataRow drItemTypeName = dtItemTypeName.NewRow();
                drItemTypeName[0] = ApiItemTypeName.ElementAt(i).Id;
                drItemTypeName[1] = ApiItemTypeName.ElementAt(i).Name;
                dtItemTypeName.Rows.Add(drItemTypeName);
                dtItemTypeName.AcceptChanges();
            }


            //DataTable dtSurveyName = (DataTable)JsonConvert.DeserializeObject(ApiSurveyName, (typeof(DataTable)));

            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string filename = "LFSSectionItems.xlsx";

            string FullFilePath = path + filename;
            //if (Directory.Exists(Path.GetDirectoryName(file)))
            //{
            //    System.IO.File.Delete(file);
            //}

            //if (System.IO.File.Exists(FullFilePath))s
            //{
            //    System.IO.File.Delete(FullFilePath);
            //}
            string Result = Globals.ExportTemplateToExcelForLFSSectionItems(dt, dtSurvey, dtItemTypeName, path, filename);
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
            int SurveyId=0;

            DataColumn Col = ValidationError.Columns.Add("ValidationMessage", System.Type.GetType("System.String"));
            Col.SetOrdinal(0);// to put the column in position 0;

            DataColumn RowNo = ValidationError.Columns.Add("SrNo", typeof(int));
            Col.SetOrdinal(1);// to put the column in position 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                foreach (DataColumn col in dt.Columns)
                {
                    object value = dt.Rows[i].Field<object>(col);

                    //Check Validation that Question Code should be unique for each survey
                    if (col.ColumnName == "Survey")
                    {
                        SurveyId = FSRC.GetSurveyIdbyName(Convert.ToString(value));
                        //value = SurveyId;
                        //col.ColumnName = "SurveyId";
                    }
                    string QuestionCode = dt.Rows[i].Field<string>("QuestionCode");

                    int QuestionCount = RestClient.GetQuestionCodeCountForSurvey(QuestionCode, SurveyId);
                    if (QuestionCount > 0)
                    {

                        ValidationResult = "Question Code can not be duplicate against same SurveyId";
                    }
                    else { 



                    if (col.ColumnName == "ItemTypeName")
                    {
                        int ItemTypeId = LFSITRC.GetItemTypeIdbyName(Convert.ToString(value));
                        value = (Object)ItemTypeId;
                        col.ColumnName = "ItemTypeId";
                        ValidationResult = Globals.ValidateCellData("LFSSectionItems", col.ColumnName, value, col.ColumnName, "", CompanyCode);
                        col.ColumnName = "ItemTypeName";

                    }
                    else
                    {
                        // string ValidationResult = ValidateExcelData(col.ColumnName, dt.Rows[i].Field<object>(col));
                        ValidationResult = Globals.ValidateCellData("LFSSectionItems", col.ColumnName, value, col.ColumnName, "", CompanyCode);
                    }
                    if (col.ColumnName != "Survey")
                    {




                        if (col.ColumnName == "IsReponseAutoGenerated" && Convert.ToBoolean(value) == true)
                        {
                            var SumOfQuestions = dt.Rows[i].Field<object>("SumOfQuestions");
                            var Operator = dt.Rows[i].Field<object>("Operator");
                            var SumValue = dt.Rows[i].Field<object>("SumValue");
                            var AutomatedResponseTrueValue = dt.Rows[i].Field<object>("AutomatedResponseTrueValue");
                            var AutomatedResponseFalseValue = dt.Rows[i].Field<object>("AutomatedResponseFalseValue");
                            var ItemTypeName = dt.Rows[i].Field<object>("ItemTypeName");
                            var RelatedQuestionCode = dt.Rows[i].Field<object>("ItemTypeName");

                            if ((string)SumOfQuestions != null && (string)Operator != null && (int)SumValue != 0 && (string)AutomatedResponseTrueValue != null && (string)AutomatedResponseFalseValue != null)
                            {
                                if ((string)ItemTypeName == "AS_QUESTION")
                                {
                                    if ((string)RelatedQuestionCode != null)

                                    {

                                        ValidationResult = "Success";

                                    }

                                    else

                                    {

                                        ValidationResult = "	For question of type AS_QUESTION, ensure RelatedQuestionCode is populated";

                                    }

                                }

                                ValidationResult = "Success";
                            }

                            else

                            {

                                ValidationResult = "If IsResponseAutoGenerated=1 then values should  be present in SumOfQuestions , Operator , SumValue, AutomatedResponseTrueValue, AutomatedResponseFalseValue ";


                            }

                        }

                        if (col.ColumnName == "ItemTypeName" && Convert.ToString(value) == "AS_QUESTION")

                        {
                            var ItemTypeName = dt.Rows[i].Field<object>("ItemTypeName");
                            var RelatedQuestionCode = dt.Rows[i].Field<object>("RelatedQuestionCode");

                            if ((string)RelatedQuestionCode != null)

                            {
                                ValidationResult = "Success";

                            }

                            else

                            {

                                ValidationResult = "	For question of type AS_QUESTION, ensure RelatedQuestionCode is populated";

                            }

                        }
                    }
                    else
                    {
                        ValidationResult = "Success";
                    }

                }//This is the end of else block of Duplicate question Code Validation

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

        //not being used
        //private string ValidateExcelData(string DbColumnName, object ExcelCellValue)
        //{

        //    var data = Globals.ValidateData("LFSSectionItems", DbColumnName, ExcelCellValue);
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
            int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

            try
            {
                string GrdAry = GridArray.TrimEnd(',');
                string[] keyValuePair = GrdAry.Split(',');
                List<LFSSectionItemViewModel> modelList = new List<LFSSectionItemViewModel>();
                for (var i = 0; i < keyValuePair.Length; i = i + collength)
                {
                    LFSSectionItemViewModel model = new LFSSectionItemViewModel();
                    for (var j = 0; j < collength; j++)
                    {
                        string[] dataList = keyValuePair[i + j].Split(':');
                        var AttributeName = dataList[0]; var AttributeValue = dataList[1];

                        if (AttributeName == "Survey")
                        {
                            AttributeName = "SurveyId";
                            int SurveyId = FSRC.GetSurveyIdbyName(AttributeValue);
                            AttributeValue = (SurveyId).ToString();

                        }
                        if (AttributeName == "ItemTypeName")
                        {
                            AttributeName = "ItemTypeId";
                            int ItemTypeId = LFSITRC.GetItemTypeIdbyName(AttributeValue);
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
                    //Implemented by RG
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
                string S3BucketGlobalPobFolder = ConfigurationManager.AppSettings["S3BucketFSSectionItemsFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketGlobalPobFolder + "/" + filename;
                Globals.UploadFileToS3(localpath, S3TargetPath);

                // ===============  Vijay Added Following code for Audit Table
                //insert entry to LAudit table                
                LAuditViewModel audit = new LAuditViewModel();
                audit.RelyProcessName = "SectionItem";
                audit.VFProcessName = "SectionItem";
                audit.ControlCode = "Audit";
                audit.ControlDescription = null;
                audit.Action = "Upload";
                audit.ActionType = "Create";//any one of these 3 Create/Update/Delete
                audit.ActionedById = LoggedInUserId;
                audit.ActionedByRoleId = LoggedInRoleId;
                audit.ActionDateTime = DateTime.Now;
                audit.OldStatus = null;
                audit.NewStatus = "Create"; //when there is no WF, use Status Column
                audit.EntityType = "LFSSectionItems";//BasetableName, hardcoded when no WF
                audit.EntityId = 0;
                audit.EntityName = "SectionItem";
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

        //    //RG Commented this Code

        //public JsonResult SaveValidRecords(object[] GridData)
        //{
        //    //Need to Implement Full Load(Delete Data from Table Then Save it)
        //    try
        //    {
        //        foreach (string[] Data in GridData)
        //        {
        //            var data = Data;
        //            var GridArray = data;
        //            var model = new LFSSectionItemViewModel
        //            {
        //                // Id = Convert.ToInt32(GridArray[0]),
        //                SurveyId = Convert.ToInt32(GridArray[0]),
        //                ChapterCode = GridArray[1],
        //                SectionCode = GridArray[2],
        //                QuestionCode = GridArray[3],
        //                ItemTypeId = Convert.ToInt32(GridArray[4]),
        //                ItemText = GridArray[5],
        //                Ordinal = Convert.ToInt32(GridArray[6]),
        //                NestingLevel = Convert.ToInt32(GridArray[7]),
        //                IsResponseMandatory = Convert.ToBoolean(GridArray[8]),
        //                IsReponseAutoGenerated = Convert.ToBoolean(GridArray[9]),
        //                ShowOnQuestionCode = GridArray[10],
        //                ShowOnAnswerOption = GridArray[11],
        //                SumOfQuestions = GridArray[12],
        //                Operator = GridArray[13],
        //                SumValue = Convert.ToInt32(GridArray[14]),
        //                AutomatedResponseTrueValue = GridArray[15],
        //                AutomatedResponseFalseValue = GridArray[16],
        //                RelatedQuestionCode = GridArray[17],
        //                ShowOnAccountingMemo = Convert.ToBoolean(GridArray[18]),
        //                InternalNotes = GridArray[19],

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
        //        audit.RelyProcessName = "SectionItem";
        //        audit.VFProcessName = "SectionItem";
        //        audit.ControlCode = "Audit";
        //        audit.ControlDescription = null;
        //        audit.Action = "Upload";
        //        audit.ActionType = "Create";//any one of these 3 Create/Update/Delete
        //        audit.ActionedById = LoggedInUserId;
        //        audit.ActionedByRoleId = LoggedInRoleId;
        //        audit.ActionDateTime = DateTime.Now;
        //        audit.OldStatus = null;
        //        audit.NewStatus = "Create"; //when there is no WF, use Status Column
        //        audit.EntityType = "LFSSectionItems";//BasetableName, hardcoded when no WF
        //        audit.EntityId = 0;
        //        audit.EntityName = "Section Item";
        //        audit.WorkFlowId = null;
        //        audit.CompanyCode = CompanyCode;
        //        audit.Comments = "Uploaded file " + Session["FileName"];
        //        ILAuditRestClient ARC = new LAuditRestClient();
        //        ARC.Add(audit, null);


        //        //================================End Audit Table

        //        var OutputJson = new { ErrorMessage = "Create Types", PopupMessage = "", RedirectToUrl = "/LFSSectionItems/Index" };
        //        return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;

        //    }
        //}

    }
}
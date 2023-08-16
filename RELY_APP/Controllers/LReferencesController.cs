using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LReferencesController : Controller
    {
        ILReferencesRestClient RestClient = new LReferencesRestClient();
        ILReferenceTypesRestClient TypesRestClient = new LReferenceTypesRestClient();
        ILReferenceDataRestClient RDRestClient = new LReferenceDataRestClient();
        ILCompanySpecificColumnsRestClient LCSRDC = new LCompanySpecificColumnsRestClient();

        //// GET: LReferences
        //[ControllerActionFilter]
        //public ActionResult Index()
        //{
        //    System.Web.HttpContext.Current.Session["Title"] = "Manage References";
        //    ViewBag.Title = "Manage References";
        //    return View();
        //}

        private SelectList GetRefType(int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = TypesRestClient.GetByRefDataUnavailable(CompanyCode,Selected);
            var x = new SelectList(ApiData, "Id", "Name", Selected);
            return x;
        }

        //[ControllerActionFilter]
        //public JsonResult GetRefData(int id)
        //{
        //    var ApiData = RDRestClient.GetByReferenceId(id);
        //    return Json(ApiData);
        //}

        private void SetViewBagDetails(int id,string FormType,int ReferenceTypeId,int OrdinalValue,string EntityName)
        {
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            ViewBag.Title = FormType + " References ( " + EntityName + " )";
            ViewBag.HideButton = 2; // 2 means Manually Enter Reference Data option will be set as default
            ViewBag.FormType = FormType;
            ViewBag.ReferenceTypeId = GetRefType(ReferenceTypeId);
            ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            int StepId = Globals.GetStepId(OrdinalValue, Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]));
            ViewBag.StepId = StepId;
            //bottom buttons Implemented by RG after bug found on Reference Page
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, FormType);
            ViewBag.StepId = StepId;
            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LReferences", id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false,
                CanUploadMultipleFiles = false, SaveFileInBucket = false, ExistingFilesList = existingFilesList };
            //ViewBag.DataUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false,
            //    CanUploadMultipleFiles = false,SaveFileInBucket = false,ExistingFilesList = existingFilesList};
            // ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false, CanUploadMultipleFiles = false, SaveFileInBucket = true, ExistingFilesList = existingFilesList };

            //Access these values in Partial View
            ViewBag.EntityId = id;
            ViewBag.EntityType = "LReferences";
        }

        [ControllerActionFilter]
        //For Edit Of LReference
        public ActionResult Edit(int id /*,int WorkFlowId, int StepId*/)
        {
            //int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;

            System.Web.HttpContext.Current.Session["Title"] = "Edit References";
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//To Empty the List of Uploaded Files,in case session already has some file in the variable
            //ViewBag.HideButton = 2; //<Vikas> What does this 2 means here ?? 

            //To Get Data from LReferences Table
            LReferencesViewModel model = RestClient.GetLReferencesById(id);
            // ViewBag.WorkFlowId = WorkFlowId;
            // ViewBag.Title = "Edit References ( "+ model.Name + " )";

            //Get total Grid count in advance
            ViewBag.RefDataGridCount = RDRestClient.GetReferenceDataGridCounts(model.Id);
            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            //commented Common code for Change/Edit/review and wrote a private method instead.
            SetViewBagDetails(id, "Edit", model.ReferenceTypeId, OrdinalValue,model.Name);
            ////To get Data from LReferenceData Table
            //ViewBag.DataTable = RDRestClient.GetByReferenceId(id);

            //<Vikas>, Instead of showing dropdown for this, in Edit mode only a non editable text box should be shown. No need to get whole list from server
           /* ViewBag.ReferenceTypeId = GetRefType(model.ReferenceTypeId); 

            //To Get Data from LReferenceTypes(I am using here  Name only)
            var RefTypeObj = TypesRestClient.GetByReferenceTypeId(model.ReferenceTypeId);

            //var CompanySpecificData = LCSRDC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LReferenceData", RefTypeObj.Name).ToList();
            //ViewBag.LReferenceDataColumnList = CompanySpecificData;

            ViewBag.FormType = "Edit";
            
            //bottom buttons Implemented by RG after bug found on Reference Page
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Edit");
            ViewBag.StepId = StepId;

            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LReferences", id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false, CanUploadMultipleFiles = false, SaveFileInBucket = false ,ExistingFilesList=existingFilesList};

            //Access these values in Partial View
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LReferences";*/
            return View(model);
        }

        [ControllerActionFilter]
        public JsonResult GetReferenceDataGridCounts(int ReferenceId)
        {
            var ApiData = RDRestClient.GetReferenceDataGridCounts(ReferenceId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GenerateReferenceDataGrid(int ReferenceId,string sortdatafield, string sortorder, int pagesize, int pagenum)
        {
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RDRestClient.GenerateReferenceDataGrid(ReferenceId, sortdatafield, sortorder, pagesize, pagenum, FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult Review(int id /*,int WorkFlowId, int StepId*/)
        {
            //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            //int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            System.Web.HttpContext.Current.Session["Title"] = "Review References";
            
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//To Empty the List of Uploaded Files
            //This will checkmark the radio button labelled "Manually Enter Reference Data". Variable name "HideButton" is misleading here but since it is now used at many places, it will be a long task to identify and fix the name
            //ViewBag.HideButton = 2; 
            
            //To Get Data from LReferences Table
            LReferencesViewModel model = RestClient.GetLReferencesById(id);
            //Get total Grid count in advance
            ViewBag.RefDataGridCount = RDRestClient.GetReferenceDataGridCounts(model.Id);

            // ViewBag.Title = "Review References ( " + model.Name + " )";
            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            //commented Common code for Change/Edit/review and wrote a private method instead.
            SetViewBagDetails(id,  "Review", model.ReferenceTypeId, OrdinalValue,model.Name);
            //ViewBag.ReferenceTypeId = GetRefType(model.ReferenceTypeId);

            //To Get Data from LReferenceTypes(I am using here  Name only)
            //var RefTypeObj = TypesRestClient.GetByReferenceTypeId(model.ReferenceTypeId);
            //var CompanySpecificData = LCSRDC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LReferenceData", RefTypeObj.Name).ToList();
            //ViewBag.LReferenceDataColumnList = CompanySpecificData;

            //ViewBag.WorkFlowId = WorkFlowId;
            //ViewBag.FormType = "Review";

            //GenericGridRestClient GRC = new GenericGridRestClient();
            //ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Review");
            //ViewBag.StepId = StepId;

            ////supporting documents details that are already attached
            //ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            //var DocumentsList = LSDRC.GetByEntityType("LReferences", id);
            //List<string> existingFilesList = new List<string>();
            //foreach (var document in DocumentsList)
            //{
            //    existingFilesList.Add(document.OriginalFileName);
            //}

            //ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false, CanUploadMultipleFiles = false, SaveFileInBucket = false ,ExistingFilesList=existingFilesList};

            ////Access these values in Partial View
            //ViewBag.EntityId = model.Id;
            //ViewBag.EntityType = "LReferences";
            return View(model);
        }

        /**VG Comment: Edit, Review, and Change methods have almost same code. Suggest here to move common functionality to a new private method and call that private method
         * from all 3 methods **/

        [ControllerActionFilter]
        public ActionResult Change(int id/*,int WorkFlowId, int StepId*/)
        {
            //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            System.Web.HttpContext.Current.Session["Title"] = "Change References";
            
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//To Empty the List of Uploaded Files
           // ViewBag.HideButton = 2;
            //To Get Data from LReferences Table
            LReferencesViewModel model = RestClient.GetLReferencesById(id);
            //Get total Grid count in advance
            ViewBag.RefDataGridCount = RDRestClient.GetReferenceDataGridCounts(model.Id);

            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            //commented Common code for Change/Edit/review and wrote a private method instead.
            SetViewBagDetails(id, "Change", model.ReferenceTypeId, OrdinalValue,model.Name);
            //To get Data from LReferenceData Table
            //var ReferenceDataModel = RDRestClient.GetByReferenceId(id);
            //ViewBag.DataTable = ReferenceDataModel;
            //GetLReferenceData(id);
            //ViewBag.ReferenceTypeId = GetRefType(model.ReferenceTypeId);
            //To Get Data from LReferenceTypes(I am using here  Name only)
            //var RefTypeObj = TypesRestClient.GetByReferenceTypeId(model.ReferenceTypeId);

            //var CompanySpecificData = LCSRDC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LReferenceData", RefTypeObj.Name).ToList();

            //ViewBag.LReferenceDataColumnList = CompanySpecificData;

            ////To get LReference Data
            ////LReferencesViewModel RefModel = RestClient.GetLReferencesById(modelData.Id);

            ////ViewBag.ReferenceTypeId = GetReferenceTypeId(model.ReferenceTypeId);
            ////supporting documents details that are already attached
            //ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            //var DocumentsList = LSDRC.GetByEntityType("LReferences", id);
            //List<string> existingFilesList = new List<string>();
            //foreach (var document in DocumentsList)
            //{
            //    existingFilesList.Add(document.OriginalFileName);
            //}
            //ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false, CanUploadMultipleFiles = false, SaveFileInBucket = false ,ExistingFilesList=existingFilesList};
            //To Get Counts
            // ViewBag.RowCounts = RDRestClient.GetLReferenceDataCounts(CompanyCode, "LReferenceData").ToList();
            //LReferencesViewModel model = new LReferencesViewModel();
            return View("Edit",model);
        }

        [ControllerActionFilter]
        public JsonResult ValidateUploadData(string LRefType)
        {
            var File = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
            File = File.Where(p => p.ActivityType == "Upload").ToList();
            if (File != null && File.Count() > 0)
            {
                //Session["fname"] = File[0].OriginalFileName; //Commented because probably not in use
                Session["FileName"] = File[0].FileName; //this is reuired in  reading uploaded filename
                //Session["FilePath"] = File[0].FilePath;

                string ValidationResult = Globals.ReadAndValidateExcelColumnVise("LReferenceData", LRefType);
                BulkDataValidationViewModel BulkDataModel = new BulkDataValidationViewModel();

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
                    if(Convert.ToInt32(ValidationResult) > 0) //if ValidationResult value > 0 means there are invalid records
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
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetInvalidRecords(string sortdatafield, string sortorder, int pagesize, int pagenum)
        {
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var ErrorData = RestClient.GetInvalidRecords(LoggedInUserId, sortdatafield, sortorder, pagesize, pagenum, FilterQuery);
            return Json(ErrorData, JsonRequestBehavior.AllowGet);
        }

        ////To convert Datatable to Json string
        //[ControllerActionFilter]
        //public string DataTableToJSONWithJavaScriptSerializer(DataTable table)
        //{
        //    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        //    List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
        //    Dictionary<string, object> childRow;
        //    foreach (DataRow row in table.Rows)
        //    {
        //        childRow = new Dictionary<string, object>();
        //        foreach (DataColumn col in table.Columns)
        //        {
        //            childRow.Add(col.ColumnName, row[col]);
        //        }
        //        parentRow.Add(childRow);
        //    }
        //    return jsSerializer.Serialize(parentRow);
        //}

        [ControllerActionFilter]
        public ActionResult Create(/*int WorkFlowId, int StepId*/)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create References";
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//Clear Session now
            ViewBag.Title = "Create References";
            ViewBag.FormType = "Create";
            ViewBag.HideButton = false;
            ViewBag.ReferenceTypeId = GetRefType(null);
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false, CanUploadMultipleFiles = false, SaveFileInBucket = false ,ExistingFilesList=null};
            LReferencesViewModel model = new LReferencesViewModel();
            return View(model);
        }

        [ControllerActionFilter]
        //This method is being used to define the structure of the dynamic column list for the grid in UI. Data in the grid is populated using some other method
        public JsonResult GetAttributesForRefData(string LRefType)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            bool IsEffectiveDated = TypesRestClient.GetByReferenceTypeByName(LRefType).IsEffectiveDated;
            var CompanySpecificData = LCSRDC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LReferenceData", LRefType).ToList();
            if (IsEffectiveDated)
            {
                CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "EffectiveStartDate", Label = "StartDate", DataType = "datetime" , IsMandatory =true});
                CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "EffectiveEndDate", Label = "EndDate", DataType = "datetime", IsMandatory = true });
            }
            CompanySpecificData.Add(new LCompanySpecificColumnViewModel {ColumnName="Id",Label="Id",DataType="int" });
            ViewBag.LReferenceDataColumnList = CompanySpecificData;
            return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ControllerActionFilter]
        //To Save Data in case of CREATE and EDIT as well
        //NOTE rbGrp are the group of radio buttons//
        public ActionResult SaveData(LReferencesViewModel model, int RefTypeId, string Comments,string rbGrp,string FormType,string rbGrpOne,string LRefType)
        {
            /* There are two different ways the data can be sent to the system.
             * 1. By uploading in bulk using excel file
             * 2. By manually entering the data in the UI Grid displayed on the grid. 
             * Treatment for both of the types of data needs different approaches.
             */

            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);

            bool IsDataUploadedByFile = false;
            ViewBag.FormType = FormType;

            try
            {
                var ValidationErrorMessage = "";
                //check existence of duplicate Extract File Name,if Extract FileName is duplicate then stop the user to save the data
                if (!String.IsNullOrEmpty(model.ExtractFileName))
                {
                    var  ExistingFiles = RestClient.GetExtractFileNames(CompanyCode, model.Id);
                    if (ExistingFiles != null)
                    {
                        //check whether the provided FileName exist in Existing list or not
                        if (ExistingFiles.Contains(model.ExtractFileName))
                        {
                            ValidationErrorMessage = "File Name already in use, duplicates are not allowed";
                            return Json(new { ErrorMessage = ValidationErrorMessage }, JsonRequestBehavior.AllowGet);

                        }
                    }
                    //SG Commented the below code as logic for checking duplicateExtractFile is updated.
                   // int CountOfFileName = RestClient.CheckExistenceOfExtractFileName(CompanyCode, model.ExtractFileName);
                   //if (CountOfFileName > 0)
                   // {
                   // ValidationErrorMessage = "File Name already in use, duplicates are not allowed";
                   // return Json(new { ErrorMessage = ValidationErrorMessage }, JsonRequestBehavior.AllowGet);
                   // }
                 }
                //when rbGrpOne is 1, it means data is Uploaded using excel file. In this case we will upload the data to a temporary table (TRefDataValidation) in database, 
                //validate it there, and then if all of the data is valid, we will insert it into main table (LREferenceData) from the temporary table
                if (rbGrpOne == "1")
                {
                    model.GridArray = null;//no need to send GridArray when we are using file upload
                    model.collength = 0;
                    IsDataUploadedByFile = true;
                }
                else if(rbGrpOne == "2") //that means, data is added manuallly
                {
                    IsDataUploadedByFile = false;
                    //In this case we will get the data in comma separated list (GridArray) of keyvalue pairs, we will split it for further use
                    if (!String.IsNullOrEmpty(model.GridArray))
                    {
                        string GrdAry = model.GridArray.TrimEnd(',');
                        string[] keyValuePair = GrdAry.Split(',');
                        

                        //Validate the data being saved
                        for (var i = 0; i < keyValuePair.Length; i = i + model.collength)
                        {
                            for (var j = 0; j < model.collength; j++)
                            {
                                string[] dataList = keyValuePair[i + j].Split(':');
                                var AttributeName = dataList[0]; var AttributeValue = dataList[1];
                                //validating single cell values
                                //FormLabel value is unavailable due to which AttributeName is being used
                                var result = Globals.ValidateCellData("LReferenceData", AttributeName, AttributeValue, AttributeName, LRefType, CompanyCode);
                                if (result != "Success")
                                {
                                    ValidationErrorMessage = ValidationErrorMessage + "|" + result;
                                }
                            }
                        } //End of Validation loop

                        //If there are validation errors, then do not process further and return the invalid data grid to user. 
                        if (!String.IsNullOrEmpty(ValidationErrorMessage))
                        {
                            ValidationErrorMessage = ValidationErrorMessage.TrimEnd(',');
                            ValidationErrorMessage = ValidationErrorMessage.TrimStart('|');
                            return Json(new { ErrorMessage = ValidationErrorMessage }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                #region Save valid ReferenceData 

                //Data is valid and Following code workds for both scenarios (Upload through excel, and, manual entry)
                //Upload Utility Section to move files to specified location and  save FileList  in db starts
                string FileList = "";
                string OriginalFileList = "";
                string FilePath = "";
                string SupportingDocumentsDescription = "";
                string UploadedDataFile = "";

                //Supporting documnet upload starts here
                //if (System.Web.HttpContext.Current.Session["UploadedFilesList"] != null) //this list contains both type of files (1. Supporting documents attached by the user, 2. Reference Data file uploaded by useer)
                //{
                //    var AttachedSupportingFiles = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
                //    AttachedSupportingFiles = AttachedSupportingFiles.Where(p => p.ActivityType == "Attachment").ToList();
                //    FileList = string.Join(",", AttachedSupportingFiles.Select(p => p.FileName));
                //    OriginalFileList = string.Join(",", AttachedSupportingFiles.Select(p => p.OriginalFileName));
                //    SupportingDocumentsDescription = string.Join(",", AttachedSupportingFiles.Select(p => p.Description));
                //    if(AttachedSupportingFiles.Count()>0)
                //    FilePath = AttachedSupportingFiles.ElementAt(0).FilePath;
                //    foreach (var files in AttachedSupportingFiles)//Code to move file 
                //    {
                //        var Source = files.FilePath + "/" + files.FileName;
                //        var Destination = "/" + Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]).ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                //        var DestinationCompleteFilePath = Destination + "/" + files.FileName;
                //        var sucess = Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                //        if (sucess)
                //            FilePath = Destination;
                //    }

                //} //Supporting document upload ends here


                if (System.Web.HttpContext.Current.Session["UploadedFilesList"] != null)
                {
                    var AttachedSupportingFiles = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
                    
                    AttachedSupportingFiles = AttachedSupportingFiles.Where(p => p.ActivityType == "Attachment").ToList();
                    FileList = string.Join(",", AttachedSupportingFiles.Select(p => p.FileName));
                    OriginalFileList = string.Join(",", AttachedSupportingFiles.Select(p => p.OriginalFileName));
                    SupportingDocumentsDescription = string.Join(",", AttachedSupportingFiles.Select(p => p.Description));
                    if(AttachedSupportingFiles.Count()>0)
                    FilePath = AttachedSupportingFiles.ElementAt(0).FilePath;
                    //foreach (var files in AttachedSupportingFiles)//Code to move file 
                    //{
                    //    var Source = files.FilePath + "/" + files.FileName;
                    //    var Destination = "/" + Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]).ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                    //    var DestinationCompleteFilePath = Destination + "/" + files.FileName;
                    //   var sucess = Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                    //    if (sucess)
                    //        FilePath = Destination;
                    //}
                    System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//Clear Session now
                }

               // System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//clear attachment from session 
                //Everythinng looks good now, save the data to database. 
                //Depending upon the parameters IsDataUploadedByFile and model.GridArray API will determine the method to be adopted for inseretion of data
                model.WFStatus = "Saved";
                model.WFType = "LReferences";
                model.WFRequesterId = LoggedInUserId;
                model.WFRequesterRoleId = LoggedInRoleId;
                model.ReferenceTypeId = RefTypeId;
                model.CreatedDateTime = DateTime.UtcNow;
                model.UpdatedDateTime = DateTime.UtcNow;
                model.CreatedById = LoggedInUserId;
                model.UpdatedById = LoggedInUserId;
                model.Version = 1;
                model.CompanyCode=CompanyCode;
                //ADD or Update condition
                switch (FormType)
                {
                    case "Create":
                        model.WFStatusDateTime = DateTime.UtcNow;
                        model.Id = RestClient.Add(model, null, CompanyCode, LoggedInUserId, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList, UploadedDataFile, IsDataUploadedByFile);
                        break;
                    case "Edit":
                    case "Change":
                        var OverwriteExistingData = false;
                        if (rbGrp == "4")//This value is set in Client side which are as follows: 3 Append to existing data and 4 Overwrite existing data
                        {
                            OverwriteExistingData = true;
                        }
                        RestClient.Update(model, null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList, CompanyCode, LoggedInUserId, OverwriteExistingData, FormType, UploadedDataFile, IsDataUploadedByFile);
                        break;
                }
                return Json(new { success = true, Id = model.Id, ErrorMessage =string.Empty }, JsonRequestBehavior.AllowGet);
                #endregion Save valid ReferenceData 
            }
            catch (Exception ex)
            {
                string redirectUrl = ex.Data["RedirectToUrl"] as string;
                if (ex.Data["ErrorCode"]!=null)
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)Globals.ExceptionType.Type1:
                            //redirect user to gneric error page
                            // return Redirect(ex.Data["RedirectToUrl"] as string);
                            return Json(new { success = false, ErrorMessage = ex.Data["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                        case (int)Globals.ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller name and method name) from where request was initiated 
                            ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                            //return View(model);
                            model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                            return Json(model, JsonRequestBehavior.AllowGet);
                        case (int)Globals.ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                            return Json(new { success = false, ErrorMessage = ex.Data["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                        case (int)Globals.ExceptionType.Type4:
                            ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                            model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                            // return View(model);
                            return Json(model, JsonRequestBehavior.AllowGet);
                        default:
                            return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ControllerActionFilter]
        public ActionResult DownloadTemplate(string RefType)
        {
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string filename = "RefData_" + RefType + ".xlsx";
            Globals.GenerateTemplate("LReferences", RefType);//generates Templates
            return File(path + "/" + filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

        //private string ReadAndValidateExcel(string LRefType)
        //{
        //    //Earlier this was a long method, whose logic was changed during code maintenance activity. This method is kept here for some time for testing and stabilization 
        //    //purpose. Once code is stable, this method can be removed and it's calling method can directly call "ReadAndValidateExcelColumnVise" from globals.
        //    var result = Globals.ReadAndValidateExcelColumnVise("LReferenceData", LRefType);
        //    return result;
        //}

        [ControllerActionFilter]
        /// Description: This method is used to download the reference data grid in Excel format.
        public ActionResult DownloadReferenceDataGrid(int ReferenceId, string LRefType, string OutputFilename)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            string Filename = "";
            var model = RDRestClient.DownloadReferenceDataGrid(ReferenceId, LRefType, "LReferenceData", CompanyCode, OutputFilename);
            if (model != null)
            {
                Filename = model.Name;
            }
            string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketReferenceDataFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + Filename;           
            var fileData = Globals.DownloadFromS3(S3TargetPath);
            return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);            
        }
    }
}

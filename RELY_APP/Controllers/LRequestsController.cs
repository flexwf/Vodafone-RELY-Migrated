using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebAPP.Controllers;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LRequestsController : Controller
    {
        ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();

        ILRequestsRestClient RestClient = new LRequestsRestClient();
        IRRequestSystemsRestClient RRSRC = new RRequestSystemsRestClient();
        ILProductsRestClient PRC = new LProductsRestClient();
        FilesUploadHelper filesUploadHelper;
        String LocalStoragePath = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "/";

        string Workflow = Convert.ToString(System.Web.HttpContext.Current.Session["Workflow"]);
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        string UserRole = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
        string UserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        //string LoggedRoleId = System.Web.HttpContext.Current.Session["UserRoleId"] as string;
        int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
        public string StorageRoot
        {
            get { return Path.Combine((LocalStoragePath)); }
        }
        private string UrlBase = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "/";
        String DeleteURL = "/FileUpload/DeleteFile/?file=";
        String DeleteType = "GET";
    
        // GET: LRequests
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Request";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult Change(int Id/*, int WorkFlowId, int StepId*/)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            ViewBag.FormType = "Change";
            System.Web.HttpContext.Current.Session["Title"] = "Change Request";

            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//To Empty the List of Uploaded Files,in case session already has some file in the variable

            LRequestViewModel model = RestClient.GetById(Id);
            ViewBag.Title = "Change Request ( " + model.Name + " )";
            ViewBag.SystemId = GetSystem(Convert.ToString(model.SystemId));

            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]));
            ViewBag.StepId = StepId;
            ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);

            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LRequests", model.WFType);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true };
            //selectList is assigned to varchar type attribute column , in case of DropDownId exist dropdown will be displayed on screen.
            ViewBag.AttributeC01 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC02 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC03 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC04 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC05 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC06 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC07 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC08 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC09 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC10 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            return View("Edit", model);
        }

        [CustomAuthorize]
        public JsonResult GetAttributesForProductGrid(string SelecterType)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            ILCompanySpecificColumnsRestClient LCSRDC = new LCompanySpecificColumnsRestClient();
            var CompanySpecificData = LCSRDC.GetAttributesForGrid(CompanyCode, "LProducts", SelecterType).ToList();
            ViewBag.LReferenceDataColumnList = CompanySpecificData;
            return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);
        }
        [ControllerActionFilter]
        public ActionResult Edit(int Id/*, int WorkFlowId, int StepId*/)
        {
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//To Empty the List of Uploaded Files,in case session already has some file in the variable
            ViewBag.FormType = "Edit";
            System.Web.HttpContext.Current.Session["Title"] = "Edit Request";

            LRequestViewModel model = RestClient.GetById(Id);
            //ViewBag.Title = "Edit Request ( " + model.Name + " )";
            ViewBag.Title = "Edit Request (" + model.WFType + ")";
            ViewBag.SystemId = GetSystem(Convert.ToString(model.SystemId));
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LRequests", model.WFType);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            string WFType = model.WFType;
            //calculating WorkFlowId from WFType of transaction
            IRWorkFlowsRestClient RRC = new RWorkFlowsRestClient();
            var WFData = RRC.GetByType(WFType, CompanyCode);
            int WFId = WFData.Id;
            WorkFlowName = WFData.Name;
            System.Web.HttpContext.Current.Session["WorkFlow"] = WorkFlowName;
            /*------Commented by RS as to hide workflowid and stepid during any transaction Actions (Create, edit,review,change)-------------------*/
            //ViewBag.WorkFlowId = WorkFlowId;
            //ViewBag.StepId = StepId;
            /*--------------------------------------------------END--------------------------------------------------------------------------------*/
            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, WFId);
            ViewBag.StepId = StepId;
            ViewBag.WorkFlowId = WFId;
            //if (!string.IsNullOrEmpty(model.WFComments)) //replacing newlines characters to space as its creating issue in Javascript
            //{
            //    //model.WFComments = model.WFComments.Replace("\r\n", " ");
            //}
            //selectList is assigned to varchar type attribute column , in case of DropDownId exist dropdown will be displayed on screen.
            ViewBag.AttributeC01 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC02 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC03 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC04 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC05 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC06 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC07 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC08 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC09 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC10 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");

            //AttachedFileDetails.OriginalFileName;
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Edit");



            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LRequests", Id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page fr Upload Utility
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true, ExistingFilesList = existingFilesList };

            ViewBag.SectionItemsParameters = new SectionItemsParametersViewModel { Width = "100px", Height = "250px", Disabled = false };
            /*Start Namita changes*/
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LRequests";
            /*End Namita Changes*/
            return View(model);
        }

        [ControllerActionFilter]
        public ActionResult Review(int Id/*, int WorkFlowId, int StepId*/)
        {
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//To Empty the List of Uploaded Files,in case session already has some file in the variable
            System.Web.HttpContext.Current.Session["Title"] = "Review Request";

            LRequestViewModel model = RestClient.GetById(Id);
            ViewBag.Title = "Review Request (" + model.WFType + ")";
            ViewBag.SystemId = GetSystem(Convert.ToString(model.SystemId));
            string WFType = model.WFType;
            //calculating WorkFlowId from WFType of transaction
            IRWorkFlowsRestClient RRC = new RWorkFlowsRestClient();
            var WFData = RRC.GetByType(WFType,CompanyCode);
            int WFId = WFData.Id;
            WorkFlowName = WFData.Name;
            System.Web.HttpContext.Current.Session["WorkFlow"] = WorkFlowName;
            /*------Commented by RS as to hide workflowid and stepid during any transaction Actions (Create, edit,review,change)-------------------*/
            //ViewBag.WorkFlowId = WorkFlowId;
            //ViewBag.StepId = StepId;
            /*----------------------------------------------------------(END)---------------------------------------------------------------------*/
            //int WFId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            ViewBag.WorkFlowId = WFId;
            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, WFId);
            ViewBag.StepId = StepId;

            ViewBag.FormType = "Review";
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LRequests", model.WFType);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            //if (!string.IsNullOrEmpty(model.WFComments)) //replacing newlines characters to space as its creating issue in Javascript
            //{
            //    model.WFComments = model.WFComments.Replace("\r\n", " ");
            //}
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Review");

            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LRequests", Id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page for Upload Utility
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true, ExistingFilesList = existingFilesList };

            /*Start Namita changes*/
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LRequests";
            /*End Namita Changes*/
            //selectList is assigned to varchar type attribute column , in case of DropDownId exist dropdown will be displayed on screen.
            ViewBag.AttributeC01 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC02 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC03 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC04 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC05 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC06 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC07 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC08 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC09 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC10 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            return View(model);
        }

        [ControllerActionFilter]
        public ActionResult Create(/*int WorkFlowId,int StepId*/)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            // IWStepRestClient WSRC = new WStepRestClient();
            // var wcolumns = WSRC.GetWFColumnsFromWStep(LoggedInUserId, UserRoleId, WorkFlowId, "Create", CompanyCode);

            /*---------(START)Commented by RS as to hide workflowid and stepid during any transaction Actions (Create, edit,review,change)-------------------*/
            //var WFDetails = RestClient.GetWFDetails(WorkFlowId);
            //string WFType = WFDetails.WFType;
            //var SystemId = RRSRC.GetByName(WFType).Id;
            //ViewBag.FormType = "Create";
            //System.Web.HttpContext.Current.Session["Title"] = "Create Request";
            //ViewBag.Title = "Create Request";
            //ViewBag.SystemId = GetSystem(SystemId.ToString());
            //ViewBag.WorkFlowId = WorkFlowId;
            //ViewBag.StepId = StepId;
            /*------------------------------ (END)--------------------------------------------------------------------------------------------------*/

            LRequestViewModel model = new LRequestViewModel();
            int WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            ViewBag.WorkFlowId = WorkFlowId;
            var WFDetails = RestClient.GetWFDetails(ViewBag.WorkFlowId, CompanyCode);
            string WFType = WFDetails.WFType;
            var SystemId = RRSRC.GetByName(WFType).Id;
            ViewBag.FormType = "Create";
            System.Web.HttpContext.Current.Session["Title"] = "Create Request";
            ViewBag.Title = "Create Request (" + WFType + ")";
            ViewBag.SystemId = GetSystem(SystemId.ToString());
            model.SystemId = SystemId;
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            IWStepRestClient WSRC = new WStepRestClient();
            var StepDetails = WSRC.GetWFColumnsFromWStep(LoggedInUserId, UserRoleId, WorkFlowId, "Create", CompanyCode);
            int OrdinalValue = StepDetails.Ordinal;

            int StepId = Globals.GetStepId(OrdinalValue, WorkFlowId);
            ViewBag.StepId = StepId;

            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LRequests", WFDetails.WFType);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;

            //Below line will decide how to display UploadDocument PartialView on the page for Upload Utility
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true, ExistingFilesList = null };
            /*Start Namita changes*/
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LRequests";
            /*End Namita Changes*/
            return View(model);
        }

        #region Upload Excel 
        [ControllerActionFilter]
        public ActionResult Upload()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Upload";
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            LRequestViewModel model = new LRequestViewModel();
            int WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            ViewBag.WorkFlowId = WorkFlowId;
            var WFDetails = RestClient.GetWFDetails(ViewBag.WorkFlowId, CompanyCode);
            string WFType = WFDetails.WFType;
            var SystemId = RRSRC.GetByName(WFType).Id;
            ViewBag.FormType = "Upload";
            System.Web.HttpContext.Current.Session["Title"] = "Upload Request";
            ViewBag.Title = "Upload Request (" + WFType + ")";
            ViewBag.SystemId = GetSystem(SystemId.ToString());
            model.SystemId = SystemId;
            IWStepRestClient WSRC = new WStepRestClient();
            var StepDetails = WSRC.GetWFColumnsFromWStep(LoggedInUserId, UserRoleId, WorkFlowId, "Upload", CompanyCode);
            int OrdinalValue = StepDetails.Ordinal;
            int StepId = Globals.GetStepId(OrdinalValue, WorkFlowId);
            ViewBag.StepId = StepId;

            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LRequests", WFDetails.WFType);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;

            //Below line will decide how to display UploadDocument PartialView on the page for Upload Utility
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true, ExistingFilesList = null };
            /*Start Namita changes*/
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LRequests";
            /*End Namita Changes*/
            return View(model);
        }
        //This method is called when user licks on upload button after adding payee file
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase File1,string AttachedComments, bool SaveFileInBucket, bool IsDataUpload)
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
                    // As directed by JS the file names would have date time stamp as suffix
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["File1"].FileName);
                    string name = System.IO.Path.GetFileNameWithoutExtension(Request.Files["File1"].FileName);
                    string FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                   // var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        //providing Random Name to file saved in content --> PayeeFiles
                        fileLocation = string.Format("{0}/{1}", ConfigurationManager.AppSettings["LocalTempUploadFolder"], FileNames);

                        Request.Files["File1"].SaveAs(fileLocation);

                        string filename = FileNames;
                        //string filename = "TestFile.txt";
                        string fullpath = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\" + filename;//
                        string localpath = fullpath;
                        string S3BucketCopaDimensionFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"]; 
                        string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketCopaDimensionFolder + "/" + filename;
                        Globals.UploadFileToS3(localpath, S3TargetPath);


                        #region Loading sheets for excel in dataset
                        //connection string
                        string excelConnectionString = string.Empty;
                        // This line is added to make a connection with the excel sheet saved  to read data from it
                        excelConnectionString = string.Format(ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"], fileLocation);
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        //OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        //Get Payee Sheet Column list from OLEDB schema
                        DataTable PayeeSheetColumns = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, "RELY$", null });
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
                            //check whether Rely sheet has data or not
                            string dataReader = "SELECT count(*) from [RELY$]";
                            command_reader = new OleDbCommand(dataReader, excelConnection);
                            int sheetRowCount = (int)command_reader.ExecuteScalar();
                            //if (sheetRowCount == 0)
                            //{
                            //    TempData["Message"] = "Uploaded file contains no data.";
                            //    excelConnection.Dispose();
                            //    if (System.IO.File.Exists(fileLocation))
                            //        System.IO.File.Delete(fileLocation);
                            //    command_reader.Dispose();
                            //    return View();
                            //}
                            command_reader.Dispose();
                            excelConnection.Dispose();
                        }
                        catch (Exception)
                        {
                            TempData["Message"] = "Uploaded file does not seem to match the template. Kindly download fresh template and try again.";
                            // excelConnection1.Dispose();
                            excelConnection.Dispose();
                            if (System.IO.File.Exists(fileLocation))
                                System.IO.File.Delete(fileLocation);
                            command_reader.Dispose();
                            return View();
                        }

                        #endregion
                        //var CompPayeeColumn = PayeeColsClient.GetLRequestColumnsByCompanyCodeForGrid(CompanyCode);   //need to be discussed with Jas commented by SSC
                        //if (CompPayeeColumn.Count() == 0)
                        //{
                        //    TempData["Message"] = "Configuration issue with Company Specific columns, Contact Admin";
                        //    TempData["PayeeModelList"] = null;
                        //    //excelConnection1.Dispose();
                        //    excelConnection.Dispose();
                        //    if (System.IO.File.Exists(fileLocation))
                        //        System.IO.File.Delete(fileLocation);

                        //    return View();
                        //}
                        #region Validating Template First                       
                        //upload file to S3
                        var resultList = new List<FilesUploadViewModel>();
                        var data = new FilesUploadViewModel();
                        data.FileName = File1.FileName;
                        resultList.Add(data);
                        var currentContext = HttpContext;
                        filesUploadHelper.UploadAndShowResults(HttpContext, resultList, false);

                        #endregion
                        //call RestClient for bulk upload

                        var Result = RestClient.UploadPPM(FileNames, UserRoleId.ToString(), CompanyCode, LoggedInUserId.ToString(),null);
                        //ViewBag.ReturnMessage = "Your file has been successfully validated and added in the grid. Please press Upload button under Actions column to import this file in RELY DB. ";
                        ViewBag.ReturnMessage = "Validation completed please check status of your batch in the grid.";


                        System.Web.HttpContext.Current.Session["File1"] = null;
                        File1 = null;
                        return View();
                    }

                }
                System.Web.HttpContext.Current.Session["File1"] = null;
                File1 = null;
                return RedirectToAction("Index", "GenericGrid");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Record could not be validated";
                System.Web.HttpContext.Current.Session["File1"] = null;
                File1 = null;
                throw ex;
            }
        }

        private Boolean ValidatePayeeSheetHeader(List<string> columnList)
        {	//Checking Header column	
            if (!columnList.Contains("Product ID")) return false;
            if (!columnList.Contains("Authorization#")) return false;
            if (!columnList.Contains("Segment")) return false;
            if (!columnList.Contains("Gate")) return false;
            if (!columnList.Contains("Product ID")) return false;
            if (!columnList.Contains("System")) return false;
            if (!columnList.Contains("Survey")) return false;
            if (!columnList.Contains("Business Category")) return false;
            if (!columnList.Contains("S15 impact")) return false;
            if (!columnList.Contains("Supported by existing systems")) return false;
            if (!columnList.Contains("Type of Product")) return false;
            if (!columnList.Contains("Create new Local POB Name")) return false;
            if (!columnList.Contains("POB Indicator")) return false;
            if (!columnList.Contains("Usage indicator")) return false;
            if (!columnList.Contains("Bundle indicator")) return false;
            if (!columnList.Contains("Special Indicator")) return false;
            if (!columnList.Contains("Article number")) return false;
            if (!columnList.Contains("SSP")) return false;
            //if (!columnList.Contains("SSP ID")) return false;
            if (!columnList.Contains("Global POB 1: Acquisit")) return false;
            if (!columnList.Contains("Global POB 2: Retention")) return false;
            if (!columnList.Contains("Copa Dimesion - Call Origin/Destination")) return false;
            if (!columnList.Contains("Copa Dimesion : Bearer Technology")) return false;
            if (!columnList.Contains("Global POB 3 : Others")) return false;
            return true;
        }

        public JsonResult GetGridDataFields()
        {
            var data = RestClient.GetGridDataFields(CompanyId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetXUploadRequestsCountByBatchNumber(int Id)
        {
            //var model = GetBatchDetailsById(Id);
            int BatchNumber = Id;
            int count = RestClient.GetXUploadLRequestCountByBatchNumber(CompanyCode, BatchNumber);
            return Json(count, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetXUploadRequestsByBatchNumber(int BatchNo, string today, string sortdatafield, string sortorder, int? pagesize, int? pagenum)
        {
            // var model = GetBatchDetailsById(Id);
            int BatchNumber = BatchNo;
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            int PageSize = 200;
            if (pagesize != null)
            {
                PageSize = (int)pagesize;
            }
            int PageNum = 0;
            if (pagenum != null)
            {
                PageNum = (int)pagenum;
            }
            var data = RestClient.GetXUploadLRequestByBatchNumber(CompanyId, BatchNumber, sortdatafield, sortorder, PageSize, pagenum, FilterQuery);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult DeletePayeeUploadBatch(int Id)
        //{
        //    RestClient.DeletePayeeUploadBatch(Id);
        //    return Json(string.Empty, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetByUserForRequestUploadGrid()
        {
            var Data = RestClient.GetByUserForRequestUploadGrid(CompanyCode, UserId);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult UploadValidatedRequestBatch(int Id)
        {
            var model = GetBatchDetailsById(Id);
            int BatchNumber = model.XBatchNumber;
            RestClient.UploadValidatedRequestBatch(CompanyCode, BatchNumber, UserId, Convert.ToInt32(UserRoleId));
            return Json(String.Empty, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DownloadErrorFile(int Id)
        {
            var model = GetBatchDetailsById(Id);
            int BatchNumber = model.XBatchNumber;
            //call to API for getting filename.
            string FileName = RestClient.DownloadRequestUploadErrors(CompanyCode, BatchNumber);
            Thread.Sleep(10000);
            //// S3BucketRootFolder = ConfigurationManager.AppSettings["SOSBucketRootFolder"];
            ////string S3TargetPath = S3BucketRootFolder + CompanyCode + "/upload/payees/" + FileName;

            string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + FileName;
            var FileData = Globals.DownloadFromS3(S3TargetPath);
            return File(FileData, "application/unknown", FileName);

            //string FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/payees/"+ FileName;
            //DirectoryInfo dir = new DirectoryInfo(FilePath);
            //dir.Refresh();
            //if (System.IO.File.Exists(FilePath))
            //{
            //    return File(FilePath, "application/octet-stream", FileName);
            //}
            //else
            //{
            //    return null;
            //}
        }

        [HttpGet]
        public ActionResult DownloadDocument(int Id) //Sachin
        {
            var model = GetBatchDetailsById(Id);
            string FileName = model.LbfFileName;
            //string S3BucketRootFolder = ConfigurationManager.AppSettings["SOSBucketRootFolder"];
            //string S3TargetPath = S3BucketRootFolder + CompanyCode + "/upload/payees/" + FileName;
            string OriginalFileName = FileName.Split('.')[0];
            string extn = FileName.Split('.')[1];

            string S3BucketPPMDataFilesFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketPPMDataFilesFolder + "/" + FileName;
            //byte[] bytedata = Globals.DownloadFromS3(S3TargetPath);

            //var FilePath = string.Format("{0}{1}", ConfigurationManager.AppSettings["SOSBucketRootFolder"], CompanyCode + "/upload/payees");
            var FileData = Globals.DownloadFromS3(S3TargetPath);
            return File(FileData, "application/unknown", FileName);
        }

        private LBatchViewModelForRequestGrid GetBatchDetailsById(int Id)
        {
            LBatchViewModelForRequestGrid model = RestClient.GetDetailsById(CompanyCode, Id);
            return model;
        }
        public JsonResult DeleteRequestUploadBatch(int Id)
        {
            RestClient.DeleteRequestUploadBatch(Id);
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [ControllerActionFilter]
        public ActionResult SaveRequest(LRequestViewModel model, string GridArray, string Comments, string ContinueFlag, int WorkFlowId, string FormType, string SSPDate1, string SSPDate2, int StepId,string CheckBoxAttributeValues)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            var l_attributesList = CheckBoxAttributeValues.Split('|');
            foreach (var attribute in l_attributesList)
            {
                string[] dataList = attribute.Split(':');
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
                        if (String.IsNullOrEmpty(AttributeValue) || AttributeValue.Equals("undefined"))
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
            try
            {
                int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
                //if (!FormType.Equals("Create"))
                {
                    var isAuthorized = Globals.CheckActionAuthorization(FormType, UserRoleId, LoggedInUserId, WorkFlowId, StepId);
                    if (!isAuthorized)
                    {
                        model.ErrorMessage = "Sorry , You are not authorised to perform this action .Please contact L2 Admin";
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
                //Date manipulations for Effective Start and End Date
                Nullable<DateTime> SSPStartDate = null;
                Nullable<DateTime> SSPEndDate = null;
                if (!(string.IsNullOrEmpty(SSPDate1) || SSPDate1.Equals("undefined", StringComparison.InvariantCultureIgnoreCase)))
                {
                    SSPDate1 = SSPDate1 + " 13:00:00";//This is just a workaround. due to some time/offset difference, db was saving dates with 2hrs difference. 
                    SSPStartDate = DateTime.ParseExact(SSPDate1, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                if (!(string.IsNullOrEmpty(SSPDate2) || SSPDate2.Equals("undefined", StringComparison.InvariantCultureIgnoreCase)))
                {
                    SSPDate2 = SSPDate2 + " 23:59:59";
                    SSPEndDate = DateTime.ParseExact(SSPDate2, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                model.AttributeD01 = SSPStartDate;
                model.AttributeD02 = SSPEndDate;

                if (string.IsNullOrEmpty(ContinueFlag))
                {
                    List<string> ExistingNames = RestClient.GetRequestNamesByOpco(CompanyCode);//get existing Request Names for OpCo
                    string WarningMessage = null;
                    if (ExistingNames != null)
                    {
                        //check whether the provided name exist in Existing list or not
                        if (ExistingNames.Contains(model.Name))
                        {
                            WarningMessage = "You are saving this request with the same name of an existing one. Record was not saved, yet.Click “save”, again, to use this duplicate name Change the name and then save, to have a unique request name";
                        }
                    }
                    if (!string.IsNullOrEmpty(WarningMessage))
                    {
                        model.ErrorMessage = WarningMessage;
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
                if (FormType.Equals("Change"))//in case of Change, new version is created.
                {
                    model.Id = 0;
                }
                model.AttributeC20 = FormType; // using AttributeC20 for checking FormType while saving data
                //call restclient's add method
                string result = "";
                if (model.Id == 0)
                {
                    model.Version = 1;
                    model.WFStatusDateTime = DateTime.UtcNow;
                    result = Add(model, Comments, WorkFlowId);
                }
                else
                {
                    model.Version = 1;
                    result = Update(model, Comments, WorkFlowId);//should be update
                }
                //insert entry to LAudit table                
                /* LAuditViewModel audit = new LAuditViewModel();
                 audit.RelyProcessName = WorkFlowName;
                 audit.VFProcessName = WorkFlowName;
                 audit.ControlCode = "Audit";
                 audit.ControlDescription = null;
                 audit.Action = FormType;
                 audit.ActionType = FormType;//any one of these 3 Create/Update/Delete
                 audit.ActionedById = LoggedInUserId;
                 audit.ActionedByRoleId = UserRoleId;
                 audit.ActionDateTime = DateTime.Now;
                 audit.OldStatus = null;
                 audit.NewStatus = model.WFStatus; //when there is no WF, use Status Column
                 audit.EntityType = "LRequests";//BasetableName, hardcoded when no WF
                 audit.EntityId = model.Id;
                 audit.EntityName = model.Name ;
                 audit.WorkFlowId = WorkFlowId;
                 audit.CompanyCode = CompanyCode;
                 audit.Comments = Comments;
                 audit.StepId = null;
                 audit.ActionLabel = FormType;
                 audit.SupportingDocumentId = null;
                 ILAuditRestClient ARC = new LAuditRestClient();
                 ARC.Add(audit, null);*/
                //in case of creating new Request, Add methd is returning Id in format (Id=1), therefore, manipulating string from the received value
                if (result.Contains("Id="))
                {
                    model.Id = Convert.ToInt32(result.Substring(3));
                }
                else
                {
                    model.ErrorMessage = result;

                    return Json(new { success = true, model = model, Id = model.Id, ErrorMessage = model.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }
                model = RestClient.GetById(model.Id);
                string ProductResult = "";
                if (!string.IsNullOrEmpty(GridArray))
                {
                    if (FormType.Equals("Change"))
                    {
                        ProductResult = AddProductDataForChange(GridArray, model.Id, FormType);//call to method for updating selected Products for the request
                    }
                    else
                    {
                        ProductResult = UpdateProductData(GridArray, model.Id, FormType, model.WFComments);//call to method for updating selected Products for the request
                    }

                    if (ProductResult != "Success")
                    {
                        model.ErrorMessage = model.ErrorMessage + Environment.NewLine + ProductResult;
                    }
                }
                ViewBag.WorkFlowId = WorkFlowId;
                ViewBag.EntityId = model.Id;
                return Json(new { success = true, model = model, Id = model.Id, ErrorMessage = model.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string redirectUrl = ex.Data["RedirectToUrl"].ToString();
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)Globals.ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(redirectUrl);
                    case (int)Globals.ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller name and method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        //return View(model);
                        model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                        return Json(model, JsonRequestBehavior.AllowGet);
                    case (int)Globals.ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)Globals.ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                        // return View(model);
                        return Json(model, JsonRequestBehavior.AllowGet);
                    default:
                        throw ex;
                }
            }
        }

        [ControllerActionFilter]
        public string Add(LRequestViewModel model, string Comments, int WorkFlowId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();

            IWStepRestClient WSRC = new WStepRestClient();
            var wcolumns = WSRC.GetWFColumnsFromWStep(LoggedInUserId, UserRoleId, WorkFlowId, "Create", CompanyCode);
            string wcName = null;
            int wcOrdinal = 0;
            if (wcolumns != null)
            {
                wcName = wcolumns.Name;
                wcOrdinal = wcolumns.Ordinal;
            }
            model.CompanyCode = CompanyCode;
            model.CreatedDateTime = DateTime.Now;
            model.UpdatedDateTime = DateTime.Now;
            model.WFStatusDateTime = DateTime.Now;
            model.CreatedById = LoggedInUserId;
            model.UpdatedById = LoggedInUserId;
            model.Status = wcName;
            model.WFRequesterRoleId = UserRoleId;
            model.WFRequesterId = LoggedInUserId;
            model.WFCurrentOwnerId = LoggedInUserId;
            model.WFStatus = "Saved";
            model.WFOrdinal = wcOrdinal;
            IRWorkFlowsRestClient RRC = new RWorkFlowsRestClient();
            var Workflow = RRC.GetById(WorkFlowId);
            model.WFType = Workflow.WFType;

            //Upload Utility Section to move files to specified location and  save FileList  in db starts
            string FileList = "";
            string OriginalFileList = "";
            string FilePath = "";
            string SupportingDocumentsDescription = "";
            if (System.Web.HttpContext.Current.Session["UploadedFilesList"] != null)
            {
                var AttachedSupportingFiles = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
                FileList = string.Join(",", AttachedSupportingFiles.Select(p => p.FileName));
                OriginalFileList = string.Join(",", AttachedSupportingFiles.Select(p => p.OriginalFileName));
                SupportingDocumentsDescription = string.Join(",", AttachedSupportingFiles.Select(p => p.Description));
                FilePath = AttachedSupportingFiles.ElementAt(0).FilePath;
                //foreach (var files in AttachedSupportingFiles)//Code to move file 
                //{
                //    var Source = files.FilePath+"/"+files.FileName;
                //    var Destination = "/"+Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]).ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                //    var DestinationCompleteFilePath = Destination+"/"+ files.FileName;
                //   var sucess= Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                //    if(sucess)
                //    FilePath = Destination;
                //}
                System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//Clear Session now
            }
            //Section Ends
            Comments = Globals.GenerateWFComments(loggedInUser, UserRoleName, Comments, model.WFComments, "Create", wcName);//Step valeu will be populated thru WFs.
                                                                                                                            // model.WFComments = Comments;
            try
            {
                model.Id = RestClient.Add(model, null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList);
                return ("Id=" + model.Id);
            }
            catch (Exception ex)
            {
                var returnMessage = ex.Data["ErrorMessage"].ToString();
                return returnMessage;
            }


        }

        [ControllerActionFilter]
        public string Update(LRequestViewModel model, string Comments, int WorkFlowId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();

            string FileList = "";
            string OriginalFileList = "";
            string FilePath = "";
            string SupportingDocumentsDescription = "";
            if (System.Web.HttpContext.Current.Session["UploadedFilesList"] != null)
            {
                var AttachedSupportingFiles = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
                FileList = string.Join(",", AttachedSupportingFiles.Select(p => p.FileName));
                OriginalFileList = string.Join(",", AttachedSupportingFiles.Select(p => p.OriginalFileName));
                SupportingDocumentsDescription = string.Join(",", AttachedSupportingFiles.Select(p => p.Description));
                FilePath = AttachedSupportingFiles.ElementAt(0).FilePath;
                foreach (var files in AttachedSupportingFiles)//Code to move file 
                {
                    var Source = files.FilePath + "/" + files.FileName;
                    var Destination = "/" + Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]).ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                    var DestinationCompleteFilePath = Destination + "/" + files.FileName;
                    var sucess = Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                    if (sucess)
                        FilePath = Destination;
                }
                System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//Clear Session now
            }
            IWStepRestClient WSRC = new WStepRestClient();
            var wcolumns = WSRC.GetWFColumnsFromWStep(LoggedInUserId, UserRoleId, WorkFlowId, "Create", CompanyCode);
            string wcName = null;
            int wcOrdinal = 0;
            if (wcolumns != null)
            {
                wcName = wcolumns.Name;
                wcOrdinal = wcolumns.Ordinal;
            }
            model.Status = "Edit";
            model.UpdatedById = LoggedInUserId;
            model.WFCurrentOwnerId = LoggedInUserId;
            model.UpdatedDateTime = DateTime.Now;
            model.CompanyCode = CompanyCode;
            Comments = Globals.GenerateWFComments(loggedInUser, UserRoleName, Comments, model.WFComments, "Create", wcName);//Step value will be populated thru WFs.
                                                                                                                            // model.WFComments = Comments;
            try
            {
                RestClient.Update(model, model.Id, null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList);
                return ("Id=" + model.Id);
            }
            catch (Exception ex)
            {
                var returnMessage = ex.Data["ErrorMessage"].ToString();
                return returnMessage;
            }


        }

        [ControllerActionFilter]
        public string AddProductDataForChange(string GridArray, int RequestId, string FormType)
        {
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            string returnMessage = "";
            string[] ProductIdList = GridArray.Split(',');
            List<LProductViewModel> modelList = new List<LProductViewModel>();
            for (var i = 0; i < ProductIdList.Length; i = i + 1)
            {
                LProductViewModel model = PRC.GetById(Convert.ToInt32(ProductIdList[i]));
                //when Change request, create new Version of Product
                model.Id = 0;
                model.Version++;
                model.CreatedDateTime = System.DateTime.Now;
                model.CreatedById = LoggedInUserId;
                //Upload Utility Section to move files to specified location and  save FileList  in db starts
                string FileList = "";
                string OriginalFileList = "";
                string FilePath = "";
                string SupportingDocumentsDescription = "";
                if (System.Web.HttpContext.Current.Session["UploadedFilesList"] != null)
                {
                    var AttachedSupportingFiles = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
                    FileList = string.Join(",", AttachedSupportingFiles.Select(p => p.FileName));
                    OriginalFileList = string.Join(",", AttachedSupportingFiles.Select(p => p.OriginalFileName));
                    SupportingDocumentsDescription = string.Join(",", AttachedSupportingFiles.Select(p => p.Description));
                    FilePath = AttachedSupportingFiles.ElementAt(0).FilePath;
                    foreach (var files in AttachedSupportingFiles)//Code to move file 
                    {
                        var Source = files.FilePath + "/" + files.FileName;
                        var Destination = "/" + Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]).ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                        var DestinationCompleteFilePath = Destination + "/" + files.FileName;
                        var sucess = Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                        if (sucess)
                            FilePath = Destination;
                    }
                }
                //Section Ends
                try
                {
                    PRC.Add(model, null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList);//DB call for new record insertion
                }
                catch (Exception ex)
                {
                    returnMessage = ex.Data["ErrorMessage"].ToString();
                }

            }
            return returnMessage;

        }

        [ControllerActionFilter]
        public string UpdateProductData(string GridArray, int RequestId, string FormType, string Comments)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());

            string returnMessage = "";
            string[] ProductIdList = GridArray.Split(',');
            List<LProductViewModel> modelList = new List<LProductViewModel>();
            for (var i = 0; i < ProductIdList.Length; i = i + 1)
            {
                LProductViewModel Product = PRC.GetById(Convert.ToInt32(ProductIdList[i]));
                LProductViewModel newClonedProduct = null;
                if (Product.WFStatus.Equals("Completed"))//clone for Completed Products
                {
                    Product.RequestId = RequestId;
                    newClonedProduct = PRC.CloneProduct(Product.Id, LoggedInUserId, UserRoleId, CompanyCode, "Request", RequestId);
                    Product = newClonedProduct;
                }
                else//otherwise, update the existig product version(This products will be the one that has been created on the fly.)
                {
                    Product.RequestId = RequestId;
                    Product.UpdatedDateTime = System.DateTime.Now;
                    Product.UpdatedById = LoggedInUserId;
                    //Product.WFComments = "Edited Request " + RequestId + " which includes this product.  " + Comments; // Edited Request XXX which includes this product
                    modelList.Add(Product);
                }


                //moving this line in above else section. When Product is cloned, no need to Update Product again as RequestId is attached at that time only.
               // modelList.Add(Product);
            }
            try
            {
                PRC.UpdateForRequest(modelList, null);
                returnMessage = "Success";
            }
            catch (Exception ex)
            {
                returnMessage = ex.Data["ErrorMessage"].ToString();
                //return errormessage;
            }
            return returnMessage;

        }

        [ControllerActionFilter]
        public JsonResult GetByCompanyCode()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetByCompanyCode(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        private SelectList GetSystem(string Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RRSRC.GetByCompanyCode(CompanyCode);
            var x = new SelectList(ApiData, "Id", "Name", Selected);
            return x;
        }
        //public JsonResult GetByCompanyCode(string UserName, string WorkFlow)
        //{  
        //    var ApiData = RestClient.GetByCompanyCode(CompanyCode);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetByLRequestId(int Id, string UserName, string WorkFlow)
        //{
        //    var ApiData = RestClient.GetByLRequestId(Id);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}

        // [ControllerActionFilter]
        public JsonResult GetById(int RequestId)
        {
            var ApiData = RestClient.GetById(RequestId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        //DeleteProduct
        [ControllerActionFilter]
        public JsonResult DeleteProduct(int Id)
        {
            try
            {
                PRC.DetachProductFromRequest(Id, null);
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }
        //Products on Fly
        [ControllerActionFilter]
        public JsonResult GetLatestProductCreatedOnFly(int RequestId)
        {
            ILProductsRestClient LPRC = new LProductsRestClient();
            var LatestProduct = LPRC.GetLatestProductByRequestId(RequestId);
            return Json(LatestProduct, JsonRequestBehavior.AllowGet);
        }
        [ControllerActionFilter]
        public JsonResult GetLatestPOBCreatedOnFly(int ProductId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            ILProductsRestClient LPRC = new LProductsRestClient();
            var LatestPOB = LPRC.GetLatestPOBByProductId(CompanyCode, ProductId);
            return Json(LatestPOB, JsonRequestBehavior.AllowGet);
        }


        [ControllerActionFilter]
        public JsonResult GetSurveyDetails(string EntityType, int? SurveyId, int RequestId)
        {
            var ApiData = RestClient.GetSurveySummary(SurveyId, RequestId, EntityType);
            return Json(ApiData, JsonRequestBehavior.AllowGet);

        }

        public JsonResult CheckFeasibilityOfAccMemo(int RequestId)
        {
            var Result = RestClient.CheckFeasibilityOfAccMemo(RequestId);
            //if (Result != "Success")
            //{
            //    Result = "More than one survey type is used in attcahed products with this Request";
            //}

            return Json(Result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetRequestLevelAccMemo(int RequestId)
        {
            try
            {
                string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

                int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
                string FileName = RestClient.GetRequestLevelAccMemo(RequestId, LoggedInUserId, CompanyCode);
               
                string TempFolder = ConfigurationManager.AppSettings["S3BucketRootFolder"];
                
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/downloads/consolidatedaccountingmemo/" + FileName;
              
                var fileData = Globals.DownloadFromS3(S3TargetPath);
               
                return File(fileData, "application/doc", FileName);
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
                    var OutputJson = new { ErrorMessage = "Unable to Download", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "" };
                    
                    throw ex;
                  
                }
            }
        }


    }


    }
 
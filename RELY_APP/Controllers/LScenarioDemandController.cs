using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{

    [SessionExpire]
    [HandleCustomError]

    public class LScenarioDemandController : Controller
    {
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();
        //int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
        //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;

        ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
        ILScenarioDemandRestClient RestClient = new LScenarioDemandRestClient();
        // GET: ScenarioDemand
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Scenario Demand";
            return View();
        }

        [ControllerActionFilter]
        public JsonResult GetScenarioDemand()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetByCompanyCode(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult Create(string QuestionCode,int ProductId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();

            var model = new LScenarioDemandViewModel();
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LScenarioDemand", CompanyCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            ViewBag.FormType = "Create";
            System.Web.HttpContext.Current.Session["Title"] = "Create Scenario Demand";
            ViewBag.Title = "Create Scenario Demand";

            model.QuestionCode = QuestionCode;
            model.ProductId = ProductId;
            ILProductsRestClient PRC = new LProductsRestClient();
            var Product =PRC.GetById(ProductId);
            if(Product !=null)
                model.RequestId =  Product.RequestId;
            ILUsersRestClient URC = new LUsersRestClient();
            var UserDetails = URC.GetByEmail(loggedInUser);
            if (UserDetails != null)
                model.PointOfContact = UserDetails.FirstName + " " + UserDetails.LastName;
            model.ContactEmail = loggedInUser;
            ViewBag.BusinessAreaId = GetDropDownValue("BusinessArea", null);
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.StepId = 0;
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, 0, "Create");
            //Workflow is to be hardcoded in case for create of ScenarioDdaemand
            var WFDeatails = GRC.GetWorkflowDetails("ScenarioDemand", CompanyCode);
            ViewBag.WorkflowId = WFDeatails.Id;
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true, ExistingFilesList = null };
            return View(model);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize]
        public ActionResult SaveData(LScenarioDemandViewModel model, int collength, int WorkFlowId, string FormType,string ImplementationDate)
        {
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());

            try
            {
                //Date manipulations
                Nullable<DateTime> ImpDate = null;
                if (!string.IsNullOrEmpty(ImplementationDate))
                {
                    ImplementationDate = ImplementationDate + " 13:00:00";//This is just a workaround. due to some time/offset difference, db was saving dates with 2hrs difference. 
                    ImpDate = DateTime.ParseExact(ImplementationDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                model.ImplementationDate = ImpDate;
                string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
                //ToString be removed after table changes are made
                // model.ContactLName = "Dummy";
                // model.ContactFName = "Dummy";
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
                    System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//Clear Session now
                }
                //Section Ends
                ILRolesRestClient RRC = new LRolesRestClient();
                //in future, role changes, we need to change this hardcoded RoleName.
                var RoleDetails = RRC.GetByRoleName("Scenario Demand Editor", CompanyCode);
                IWStepRestClient WSRC = new WStepRestClient();
                var wcolumns = WSRC.GetWFColumnsFromWStep(LoggedInUserId, RoleDetails.Id, WorkFlowId, "Create", CompanyCode);
                string wcName = null;
                int wcOrdinal = 0;
                if (wcolumns != null)
                {
                    wcName = wcolumns.Name;
                    wcOrdinal = wcolumns.Ordinal;
                }
                //call restcliet's add method
                if (FormType.Equals("Edit"))
                {
                    model.UpdatedById = LoggedInUserId;
                    model.UpdatedDateTime = DateTime.Now;
                    model.Status = FormType;
                    model.Version = 1;
                    RestClient.Update(model, null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList);
                }
                else
                {
                    model.WFRequesterRoleId = UserRoleId;
                    model.WFRequesterId = LoggedInUserId;
                    model.WFCurrentOwnerId = LoggedInUserId;
                    model.WFStatus = "Saved";
                    model.WFOrdinal = wcOrdinal;
                    IRWorkFlowsRestClient RWFRC = new RWorkFlowsRestClient();
                    var Workflow = RWFRC.GetById(WorkFlowId);
                    model.WFType = Workflow.WFType;

                    model.CompanyCode = CompanyCode;
                    model.CreatedDateTime = DateTime.UtcNow;
                    model.UpdatedDateTime = DateTime.UtcNow;
                    model.CreatedById = LoggedInUserId;
                    model.UpdatedById = LoggedInUserId;
                    model.Status = "New";
                    if (model.Id > 0)
                    {
                        model.Version = 1;
                        RestClient.Update(model, null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList);
                    }
                    else
                    {
                        model.Version = 1;
                        model.WFStatusDateTime = DateTime.UtcNow;
                        model.Id = RestClient.Add(model, null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList);
                    }

                }
                return Json(new { success = true, Id = model.Id, ErrorMessage = model.ErrorMessage }, JsonRequestBehavior.AllowGet);
                //return View(model);
            }
            catch (Exception ex)
            {
                string redirectUrl = ex.Data["RedirectToUrl"].ToString();
                // if (ex.Data["ErrorCode"].GetType() == typeof(int))
                //{
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)Globals.ExceptionType.Type1:
                        //redirect user to gneric error page
                        // return Redirect(ex.Data["RedirectToUrl"] as string
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                        //return Redirect(redirectUrl);
                        return Json(model, JsonRequestBehavior.AllowGet);
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
        public ActionResult Edit(int Id/*, int WorkflowId, int StepId*/,string FormType)
        {
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            System.Web.HttpContext.Current.Session["Title"] = FormType + " Scenario Demand";
            ViewBag.Title = FormType + " Scenario Demand";
            ViewBag.FormType = FormType;
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LScenarioDemand", CompanyCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            LScenarioDemandViewModel model = RestClient.GetById(Id);
            ViewBag.BusinessAreaId = GetDropDownValue("BusinessArea", model.BusinessAreaId);
            //ViewBag.WorkflowId = WorkflowId;
            ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]));
            ViewBag.StepId = StepId;

            ILProductsRestClient PRC = new LProductsRestClient();
            var Product = PRC.GetById(model.ProductId);
            if (Product != null)
                model.RequestId = Product.RequestId;

            ILUsersRestClient URC = new LUsersRestClient();
            var UserDetails = URC.GetById(model.CreatedById);
            if (UserDetails != null)
            {
                model.PointOfContact = UserDetails.FirstName + " " + UserDetails.LastName;
                model.ContactEmail = UserDetails.LoginEmail;
            }
            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LScenarioDemand", Id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true, ExistingFilesList = existingFilesList };
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, FormType);
            //ViewBag.StepId = StepId;
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LScenarioDemand";
            return View("Edit", model);
        }

       // [ControllerActionFilter]
        public ActionResult Change(int Id, string FormType)
        {
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            System.Web.HttpContext.Current.Session["Title"] = FormType + " Scenario Demand";
            ViewBag.Title = FormType + " Scenario Demand";
            ViewBag.FormType = FormType;
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LScenarioDemand", CompanyCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            LScenarioDemandViewModel model = RestClient.GetById(Id);
            ViewBag.BusinessAreaId = GetDropDownValue("BusinessArea", model.BusinessAreaId);
            //ViewBag.WorkflowId = WorkflowId;
            ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]));
            ViewBag.StepId = StepId;

            ILProductsRestClient PRC = new LProductsRestClient();
            var Product = PRC.GetById(model.ProductId);
            if (Product != null)
                model.RequestId = Product.RequestId;

            ILUsersRestClient URC = new LUsersRestClient();
            var UserDetails = URC.GetById(model.CreatedById);
            if (UserDetails != null)
            {
                model.PointOfContact = UserDetails.FirstName + " " + UserDetails.LastName;
                model.ContactEmail = UserDetails.LoginEmail;
            }
            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LScenarioDemand", Id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true, ExistingFilesList = existingFilesList };
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, FormType);
            //ViewBag.StepId = StepId;
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LScenarioDemand";
            return View("Edit", model);
        }

        // [ControllerActionFilter]
        public ActionResult Review(int Id,/* int WorkflowId, int StepId,*/ string FormType)
        {
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            System.Web.HttpContext.Current.Session["Title"] = FormType + " Scenario Demand";
            ViewBag.Title = FormType + " Scenario Demand";
            ViewBag.FormType = FormType;
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LScenarioDemand", CompanyCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            LScenarioDemandViewModel model = RestClient.GetById(Id);
            ViewBag.BusinessAreaId = GetDropDownValue("BusinessArea", model.BusinessAreaId);
            //ViewBag.WorkflowId = WorkflowId;
            ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]));
            ViewBag.StepId = StepId;
            ILProductsRestClient PRC = new LProductsRestClient();
            var Product = PRC.GetById(model.ProductId);
            if (Product != null)
                model.RequestId = Product.RequestId;

            ILUsersRestClient URC = new LUsersRestClient();
            var UserDetails = URC.GetById(model.CreatedById);
            if (UserDetails != null)
            {
                model.PointOfContact = UserDetails.FirstName + " " + UserDetails.LastName;
                model.ContactEmail = UserDetails.LoginEmail;
            }
            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LScenarioDemand", Id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true, ExistingFilesList = existingFilesList };
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, FormType);
            //ViewBag.StepId = StepId;
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LScenarioDemand";
            return View("Review", model);
        }


        [ControllerActionFilter]
        public SelectList GetDropDownValue(string DropdownFor, int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            ILDropDownValuesRestClient LDDVRC = new LDropDownValuesRestClient();
            ILDropDownsRestClient LDDRC = new LDropDownsRestClient();
            var DropDownObj = LDDRC.GetByName(DropdownFor, CompanyCode);
            if (DropDownObj != null)
            {
                int DropdownId = DropDownObj.Id;
                var LDropDownValues = LDDVRC.GetByDropDownId(DropdownId).Select(p => new { p.Id, p.Description, p.Value });
                var x = new SelectList(LDropDownValues, "Id", "Description", Selected);
                return x;
            }

            return new SelectList(String.Empty);

        }

       
    }
}
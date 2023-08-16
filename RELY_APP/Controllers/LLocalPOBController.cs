
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Reflection;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LLocalPOBController : Controller
    {
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();
        //int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
        //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"].ToString();

        ILLocalPOBRestClient RestClient = new LLocalPOBRestClient();
        ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
        // GET: LLocalPOB

        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Local POBs";
            return View();
        }
        
        //change- Same as edit. WFSTatus= Saved, Ordinal
        [ControllerActionFilter]
        public ActionResult Edit(int id , string Source/*, int WorkFlowId,int StepId*/)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"].ToString();
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//To Empty the List of Uploaded Files,in case session already has some file in the variable
            ViewBag.FormType = "Edit";
            System.Web.HttpContext.Current.Session["Title"] = "Edit LocalPOB";
            
            LLocalPOBViewModel model = RestClient.GetById(id);
            ViewBag.Title = "Edit LocalPOB ( " + model.Name + " )";
            ViewBag.EntityType = "LLocalPobs";

            //ViewBag.WorkFlowId = WorkFlowId;
            //ViewBag.StepId = StepId;
            //when source is Request, calculating WorkflowId based on WFType instead of Session
            int WorkFlowId = 0;
            if ("Product".Equals(Source))
            {
                IRWorkFlowsRestClient RRC = new RWorkFlowsRestClient();
                var Workflow = RRC.GetByType(model.WFType, CompanyCode);
                WorkFlowId = Workflow.Id;
                ViewBag.WorkFlowId = WorkFlowId;
            }
            else
            {
                WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]); ;
                ViewBag.WorkFlowId = WorkFlowId;
            }

            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, WorkFlowId);
            ViewBag.StepId = StepId;

            ViewBag.LocalPobTypeId = GetLocatPobType(model.LocalPobTypeId);
            model.Source = Source;
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LLocalPobs", model.LocalPobTypeName);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            //Get StartDate of prevoius Version
            //if (model.Version > 1)
                ViewBag.PreviousVersionStartDate = RestClient.GetPreviousVersionStartDate(model.Id);

            //if (!string.IsNullOrEmpty(model.WFComments)) //replacing newlines characters to space as its creating issue in Javascript
            //{
            //    model.WFComments = model.WFComments.Replace("\r\n", " ");
            //}
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Edit");


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
            ViewBag.AttributeC11 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC12 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC14 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC15 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC16 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC17 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC18 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC19 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC20 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LLocalPobs", id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }

            /*Start Namita changes*/
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LLocalPobs";
            /*End Namita Changes*/
            //Below line will decide how to display UploadDocument PartialView on the page fr Upload Utility
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true ,ExistingFilesList=existingFilesList};
            return View(model);
        }

        //change- Same as edit. WFSTatus= Saved, Ordinal
        [ControllerActionFilter]
        public ActionResult Change(int id, string Source/*, int WorkFlowId, int StepId*/)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"].ToString();
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//To Empty the List of Uploaded Files,in case session already has some file in the variablerevi
            ViewBag.FormType = "Change";
            System.Web.HttpContext.Current.Session["Title"] = "Change LocalPOB";
            
            LLocalPOBViewModel model = RestClient.GetById(id);
            ViewBag.Title = "Change LocalPOB ( " + model.Name + " )";
            /*------Commented by RS as to hide workflowid and stepid during any transaction Actions (Create, edit,review,change)-------------------*/
            //ViewBag.WorkFlowId = WorkFlowId;
            //ViewBag.StepId = StepId;
            /*-----------------------------------------------END-----------------------------------------------------------------------------------------*/

            //when source is Request, calculating WorkflowId based on WFType instead of Session
            int WorkFlowId = 0;
            if ("Product".Equals(Source))
            {
                IRWorkFlowsRestClient RRC = new RWorkFlowsRestClient();
                var Workflow = RRC.GetByType(model.WFType, CompanyCode);
                WorkFlowId = Workflow.Id;
                ViewBag.WorkFlowId = WorkFlowId;
            }
            else
            {
                WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]); ;
                ViewBag.WorkFlowId = WorkFlowId;
            }

            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, WorkFlowId);
            ViewBag.StepId = StepId;



            ViewBag.LocalPobTypeId = GetLocatPobType(model.LocalPobTypeId);
            model.Source = Source;
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LLocalPobs", model.LocalPobTypeName);
            ViewBag.CompanySpecificColumns = CompanySpecificData;

            //if (!string.IsNullOrEmpty(model.WFComments)) //replacing newlines characters to space as its creating issue in Javascript
            //{
            //    model.WFComments = model.WFComments.Replace("\r\n", " ");
            //}
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Change");
            
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
            ViewBag.AttributeC11 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC12 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC14 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC15 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC16 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC17 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC18 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC19 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC20 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LLocalPobs", id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page fr Upload Utility
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true ,ExistingFilesList=existingFilesList};
            return View("Edit", model);
        }

        [ControllerActionFilter]
        private SelectList GetLocatPobType(Nullable<int> selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            IRLocalPobTypeRestClient LPTRC = new RLocalPobTypeRestClient();
            var ApiData = LPTRC.GetByCompanyCode(CompanyCode);
            var x = new SelectList(ApiData, "Id", "Name", selected);
            return x;
        }

        [ControllerActionFilter]
        public JsonResult GetLPobType(Nullable<int> selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            IRLocalPobTypeRestClient LPTRC = new RLocalPobTypeRestClient();
            var ApiData = LPTRC.GetByCompanyCode(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetAttributes(int LocalPobTypeId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            IRLocalPobTypeRestClient LPRC = new RLocalPobTypeRestClient();
            var obj = LPRC.GetById(LocalPobTypeId);
           // ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LLocalPobs", obj.Name);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult Review(int id, string Source /*, int WorkFlowId,int StepId*/)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"]as string;
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//To Empty the List of Uploaded Files,in case session already has some file in the variable
            ViewBag.FormType = "Review";
            System.Web.HttpContext.Current.Session["Title"] = "Review Local POB";
            
            LLocalPOBViewModel model = RestClient.GetById(id);
            ViewBag.Title = "Review LocalPOB ( " + model.Name + " )";
            /*------Commented by RS as to hide workflowid and stepid during any transaction Actions (Create, edit,review,change)-------------------*/
            //ViewBag.WorkFlowId = WorkFlowId;
            //ViewBag.StepId = StepId;
            /*-------------------------------------------------------End----------------------------------------------------------------------------*/

            ViewBag.EntityType = "LLocalPobs";
            //when source is Request, calculating WorkflowId based on WFType instead of Session
            int WorkFlowId = 0;
            if ("Product".Equals(Source))
            {
                IRWorkFlowsRestClient RRC = new RWorkFlowsRestClient();
                var Workflow = RRC.GetByType(model.WFType, CompanyCode);
                WorkFlowId = Workflow.Id;
                ViewBag.WorkFlowId = WorkFlowId;
            }
            else
            {
                WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]); ;
                ViewBag.WorkFlowId = WorkFlowId;
            }

            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, WorkFlowId);
            ViewBag.StepId = StepId;

            ViewBag.LocalPobTypeId = GetLocatPobType(model.LocalPobTypeId);
            model.Source = Source;


            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LLocalPobs", model.LocalPobTypeName);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Review");

            //if (!string.IsNullOrEmpty(model.WFComments)) //replacing newlines characters to space as its creating issue in Javascript
            //{
            //    model.WFComments = model.WFComments.Replace("\r\n", " ");
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
            ViewBag.AttributeC11 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC12 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC14 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC15 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC16 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC17 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC18 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC19 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC20 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LLocalPobs", id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page fr Upload Utility
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true ,ExistingFilesList=existingFilesList};
            /*Start Namita changes*/
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LLocalPobs";
            /*End Namita Changes*/

            return View(model);
        }

        [ControllerActionFilter]
        public ActionResult Create(/*int WorkFlowId,*/ string Source,int? ProductId,string PobStDt,string PobEnDt) //--commented by RS as per requirement to hide workflowid
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            string FormType = "";
            if ("Product".Equals(Source))
                FormType = "CreateOnFly";//new action has been introduced for Creating LPob on fly.
            else
                FormType = "Create";
            ViewBag.EntityType = "LLocalPobs";

            ViewBag.FormType = FormType;
            System.Web.HttpContext.Current.Session["Title"] = "Create Local POB";
            ViewBag.Title = "Create Local POB";
            // ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;
            string LocalPobType = "Finance";
            IRLocalPobTypeRestClient LPTRC = new RLocalPobTypeRestClient();
            var TypeDetails = LPTRC.GetByName(LocalPobType);
            ViewBag.LocalPobTypeId = GetLocatPobType(TypeDetails.Id);
            LLocalPOBViewModel model = new LLocalPOBViewModel();
            int WorkFlowId = 0;int StepId = 0;int OrdinalValue = 0;
            if ("Product".Equals(Source))
            {
                IRWorkFlowsRestClient RRC = new RWorkFlowsRestClient();
                var Workflow = RRC.GetByType("LLocalPobs", CompanyCode);//Type is being hardcoded here, Since in Create, we dont have WFtype defined for the transaction.
                WorkFlowId = Workflow.Id;
                ViewBag.WorkFlowId = WorkFlowId;
                OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            }
            else
            {
                WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
                ViewBag.WorkFlowId = WorkFlowId;
                int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
                int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
                IWStepRestClient WSRC = new WStepRestClient();
                var StepDetails = WSRC.GetWFColumnsFromWStep(LoggedInUserId, UserRoleId, WorkFlowId, FormType, CompanyCode);
                OrdinalValue = StepDetails.Ordinal;
            }

            StepId = Globals.GetStepId(OrdinalValue, WorkFlowId);
            ViewBag.StepId = StepId;

            // string SelectorType = "Finance";//this will be localPobTypeId that has been selected
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LLocalPobs", LocalPobType);
           ViewBag.CompanySpecificColumns = CompanySpecificData;
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
            ViewBag.AttributeC11 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC12 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC14 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC15 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC16 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC17 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC18 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC19 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
            ViewBag.AttributeC20 = new SelectList(new List<LDropDownValueViewModel>(), "DropDownId", "Value");
           
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true ,ExistingFilesList = null};
            ViewBag.ProductId = ProductId;
            ViewBag.PobStDt = PobStDt;
            ViewBag.PobEnDt = PobEnDt;
            /*Start Namita changes*/
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LLocalPobs";
            /*End Namita Changes*/

            model.Source = Source;
            model.LocalPobTypeId = TypeDetails.Id;
            return View(model);
        }

        [HttpPost]
        [ControllerActionFilter]
        [ValidateAntiForgeryToken]
        //string EffectiveStartDate, string EffectiveEndDate,
        public ActionResult Create(LLocalPOBViewModel model , string Comments,string FormType,int WorkFlowId,int? ProductId,int StepId, string CheckBoxAttributeValues,string PobStDt,string PobEnDt,string StartDate)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();
            string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
            var l_attributesList = CheckBoxAttributeValues.Split('|');
            foreach(var attribute in l_attributesList)
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
               // if (!FormType.Equals("Create"))
                {
                    var isAuthorized = Globals.CheckActionAuthorization(FormType, UserRoleId, LoggedInUserId, WorkFlowId, StepId);
                    if (!isAuthorized)
                    {
                        model.ErrorMessage = "Sorry , You are not authorised to perform this action .Please contact L2 Admin";
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
                string Action = FormType;
                
                ViewBag.FormType = FormType;
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
                    //    var Source = files.FilePath + "/" + files.FileName;
                    //    var Destination = "/" + Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]).ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                    //    var DestinationCompleteFilePath = Destination + "/" + files.FileName;
                    //   var sucess = Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                    //    if (sucess)
                    //        FilePath = Destination;
                    //}
                    System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//Clear Session now
                }
                //Section Ends
                //not used anymore as EffectiveStartDate,EndDate are separate fields now
                //Date manipulations for Effective Start and End Date
                //Nullable<DateTime> ValidityStartDate = null;
                //Nullable<DateTime> ValidityEndDate = null;
                //if (!string.IsNullOrEmpty(EffectiveStartDate))
                //{
                //    EffectiveStartDate = EffectiveStartDate + " 13:00:00";//This is just a workaround. due to some time/offset difference, db was saving dates with 2hrs difference. 
                //    ValidityStartDate = DateTime.ParseExact(EffectiveStartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                //}
                //if (!string.IsNullOrEmpty(EffectiveEndDate))
                //{
                //    EffectiveEndDate = EffectiveEndDate + " 23:59:59";
                //    ValidityEndDate = DateTime.ParseExact(EffectiveEndDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                //}
                //model.AttributeD01 = ValidityStartDate;
                //model.AttributeD02 = ValidityEndDate;
                //Get WFColumn details from WSteps
                IWStepRestClient WSRC = new WStepRestClient();
               
                if (FormType.Equals("Create") || FormType.Equals("CreateOnFly"))
                {
                    model.Version = 1;//While creating new record, Version will always be 1, so hardcoded it.
                    model.CompanyCode = CompanyCode;
                    //Comments = Globals.GenerateWFComments(loggedInUser, UserRoleName, Comments, model.WFComments, Action, wcolumns.Name);
                   // model.WFComments = Comments;
                    model.CreatedById = LoggedInUserId;
                    model.UpdatedById = LoggedInUserId;
                    model.CreatedDateTime = DateTime.Now;
                    model.UpdatedDateTime = DateTime.Now;
                    //Get PobCatelogueId
                    int PobCatalogueId = RestClient.GetMaxPobCatelogueId(CompanyCode);
                    model.PobCatalogueId = PobCatalogueId;
                    var StepDetails = WSRC.GetWFColumnsFromWStep(LoggedInUserId, UserRoleId, WorkFlowId, FormType, CompanyCode);
                    model.Status = StepDetails.Name;
                    /*To check POB Created on Fly or not*/
                    if (model.Source == "Product")
                    {
                        
                        model.WFStatus = "Parked";
                        model.WFOrdinal = 0;
                    }
                    else
                    {
                        model.WFStatus = "Saved";
                        model.WFOrdinal = StepDetails.Ordinal;
                    }
                    model.WFRequesterId = LoggedInUserId;
                    model.WFCurrentOwnerId = LoggedInUserId;
                    model.WFRequesterRoleId = UserRoleId;
                    IRWorkFlowsRestClient RRC = new RWorkFlowsRestClient();
                    var Workflow = RRC.GetById(WorkFlowId);
                    model.WFType = Workflow.WFType;
                    model.EffectiveStartDate = new DateTime(2010, 01, 01, 13, 00, 00);
                    model.EffectiveEndDate = new DateTime(2099, 12, 31, 13, 00, 00);
                }
                else if (FormType.Equals("Edit") || FormType.Equals("Change"))
                {//need updation for Edit
                    model.Status = "Edit";
                    model.UpdatedById = LoggedInUserId;
                    model.WFCurrentOwnerId = LoggedInUserId;
                    model.WFRequesterRoleId = UserRoleId;
                    model.UpdatedDateTime = DateTime.Now;

                    DateTime StartDate1 = new DateTime();
                    if (!string.IsNullOrEmpty(StartDate))
                    {
                        StartDate = StartDate + " 13:00:00"; //This is just a workaround, not fix. due to some time/offset difference, db was saving dates with 2hrs difference. so, taking 3hrs instead of 00hrs
                        StartDate1 = DateTime.ParseExact(StartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    model.EffectiveStartDate = StartDate1;
                    model.EffectiveEndDate = new DateTime(2099, 12, 31, 13, 00, 00);
                    // if (!string.IsNullOrEmpty(Comments))
                    //   model.WFComments= Globals.GenerateWFComments(loggedInUser, UserRoleName, Comments, model.WFComments, Action, wcolumns.Name);

                }
                 
                
                if (model.Id == 0)
                {
                    model.WFStatusDateTime = DateTime.UtcNow;
                   model.Id = RestClient.Add(model, null,FileList, SupportingDocumentsDescription, FilePath, OriginalFileList,ProductId,PobStDt,PobEnDt);
                }
                else
                {
                    RestClient.Update(model, model.Id, null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList, FormType, ProductId);
                }
                
                model.ErrorMessage = "";
                return Json(new { success = true, Id = model.Id, ErrorMessage = model.ErrorMessage }, JsonRequestBehavior.AllowGet);
               // return RedirectToAction("Index","LLocalPOB");
            }
            catch(Exception ex)
            {
                string redirectUrl = ex.Data["RedirectToUrl"].ToString();
                // if (ex.Data["ErrorCode"].GetType() == typeof(int))
                //{
                //ViewBag.SpecialIndicator = GetSpecialIndicator("");
                switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)Globals.ExceptionType.Type1:
                            //redirect user to gneric error page
                            // return Redirect(ex.Data["RedirectToUrl"] as string);
                            return Redirect(redirectUrl);
                        case (int)Globals.ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller name and method name) from where request was initiated 
                            ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        // return View(model);
                        model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                        return Json(model, JsonRequestBehavior.AllowGet);
                    case (int)Globals.ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                            return Redirect(ex.Data["RedirectToUrl"] as string);
                        case (int)Globals.ExceptionType.Type4:
                            ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        // return View(model);
                        model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                        return Json(model, JsonRequestBehavior.AllowGet);
                    default:
                            throw ex;
                    }
            }
        }

        [ControllerActionFilter]
        public JsonResult GetLocalPOBs()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //  var CompanyCode = "DE";being used from session variable
            var ApiData = RestClient.GetByCompanyCode(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetById(int LocalPobId)
        {
            var ApiData = RestClient.GetById(LocalPobId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetLLocalPOBForProduct(int PobCatalogueId,int ProductId)
        {
            var ApiData = RestClient.GetLLocalPOBForProduct(PobCatalogueId, ProductId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetByIdWithCurrentSSP(int LocalPobId)
        //{
        //    var ApiData = RestClient.GetByIdWithCurrentSSP(LocalPobId);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}

        [ControllerActionFilter]
        public JsonResult GetVersions(string Name,int TypeId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetVersions(Name,CompanyCode,TypeId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //This method is used for dropdown of SpecialIndicator
        [ControllerActionFilter]
        private SelectList GetSpecialIndicator(string selected)
        {
            string[] SI = { "D", "I", "DI" ,"B", "N" };
            var x = new SelectList(SI,selected);
            return x;
        }

        [ControllerActionFilter]
        public JsonResult GetDropDownValue(int DropdownId, int TransactionId, string ColumnName, string FormType,string DefaultValue)
        {
            ILDropDownValuesRestClient LDDVRC = new LDropDownValuesRestClient();
            var SelectedValue = "";
            if (TransactionId == -1) //while creating, -1 is being used as transactionId. Since 0 is possible value for LocalPob
            {
                SelectedValue = DefaultValue;
            }
            else
            {
                switch (FormType)
                {
                    case "LocalPobs":
                        var ApiDataLPob = RestClient.GetById(TransactionId);
                        SelectedValue = GetSelectedDropDownValue(ApiDataLPob, ColumnName);
                        break;

                    case "Products":
                        ILProductsRestClient PRC = new LProductsRestClient();
                        var ApiDataProducts = PRC.GetById(TransactionId);
                        SelectedValue = GetSelectedDropDownValue(ApiDataProducts, ColumnName);
                        break;
                    case "Requests":
                        ILRequestsRestClient RRC = new LRequestsRestClient();
                        var ApiDataRequests = RRC.GetById(TransactionId);
                        SelectedValue = GetSelectedDropDownValue(ApiDataRequests, ColumnName);
                        break;
                    case "LScenarioDemand":
                        ILScenarioDemandRestClient SDRC = new LScenarioDemandRestClient();
                        var ApiScenarioDemandData = SDRC.GetById(TransactionId);
                        SelectedValue = GetSelectedDropDownValue(ApiScenarioDemandData, ColumnName);
                        break;
                    case "AccountingScenarios":
                        ILAccountingScenariosRestClient ASRC = new LAccountingScenariosRestClient();
                        var ApiAccountingScenarioData = ASRC.GetById(TransactionId);
                        SelectedValue = GetSelectedDropDownValue(ApiAccountingScenarioData, ColumnName);
                        break;
                }
            }
            if (SelectedValue == null)
            {
                SelectedValue = string.Empty;
                
            }
            var LDropDownValues = LDDVRC.GetByDropDownId(DropdownId).Select(p => new { p.Id, p.Description, p.Value, SelectedValue = SelectedValue });
            ILDropDownsRestClient DRC = new LDropDownsRestClient();
            var DropDown =  DRC.GetById(DropdownId);
            return Json(new { model = LDropDownValues, DropdownName = DropDown.Name }, JsonRequestBehavior.AllowGet);
        }

        //Get slected dropdownvalue from model
        [ControllerActionFilter]
        private string GetSelectedDropDownValue(dynamic Model, string ColumnName)
        {
            switch (ColumnName)
            {
                case "AttributeC01":
                    return Model.AttributeC01;
                case "AttributeC02":
                    return Model.AttributeC02;
                case "AttributeC03":
                    return Model.AttributeC03;
                case "AttributeC04":
                    return Model.AttributeC04;
                case "AttributeC05":
                    return Model.AttributeC05;
                case "AttributeC06":
                    return Model.AttributeC06;
                case "AttributeC07":
                    return Model.AttributeC07;
                case "AttributeC08":
                    return Model.AttributeC08;
                case "AttributeC09":
                    return Model.AttributeC09;
                case "AttributeC10":
                    return Model.AttributeC10;
                default:
                    return string.Empty;
            }
        }

        public ActionResult LPobLibrary()
        {
            System.Web.HttpContext.Current.Session["Title"] = "LocalPOB Library";
            return View();
        }
        public JsonResult GetAllLPobsDetails(string sortdatafield, string sortorder, int pagesize, int pagenum)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetAllLPobsDetails(CompanyCode, sortdatafield, sortorder, pagesize, pagenum, FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllLPobsCounts()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetAllLocalPOBCounts(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CloneLPob(int LPobId,string Source)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            LLocalPOBViewModel ApiData = RestClient.CloneLPob(LPobId, LoggedInUserId, UserRoleId, CompanyCode, Source);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AttachGpob(int PobCatalogueId,string GpobMappingStartDate,int GpobId,string GpobType)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            
            try
            {
                var LatestGpobMappedStartDate = RestClient.AttachGpob(PobCatalogueId, GpobMappingStartDate, GpobId, CompanyCode, GpobType);
                if (LatestGpobMappedStartDate.Year == 0001)
                {
                    var OutputJson1 = new { ErrorMessage = "", LatestCopaMappedStartDate = String.Empty, RedirectToUrl = "" };
                    return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                }
                var OutputJson = new { ErrorMessage = "", LatestGpobMappedStartDate = LatestGpobMappedStartDate, RedirectToUrl = "" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
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
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Global POB could not be attached";
                    throw ex;
                }
            }
        }
        public JsonResult GetGPOBDataForGrid(int PobCatalogueId,string GpobType)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var Data = RestClient.GetGPOBDataForGrid(PobCatalogueId, CompanyCode, GpobType);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AttachCopa(int PobCatalogueId, string CopaMappingStartDate, int CopaId, int CopaClass)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            try
            {
                var LatestCopaMappedStartDate = RestClient.AttachCopa(PobCatalogueId, CopaMappingStartDate, CopaId, CompanyCode,CopaClass);
                if(LatestCopaMappedStartDate.Year == 0001)
                {
                    var OutputJson1 = new { ErrorMessage = "", LatestCopaMappedStartDate = String.Empty, RedirectToUrl = "" };
                    return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                }
                var OutputJson = new { ErrorMessage = "", LatestCopaMappedStartDate = LatestCopaMappedStartDate, RedirectToUrl = "" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
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
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Copa could not be attached";
                    throw ex;
                }
            }
        }
        public JsonResult GetCopaDataForGrid(int PobCatalogueId, int CopaClass)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var Data = RestClient.GetCopaDataForGrid(PobCatalogueId, CompanyCode, CopaClass);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLatestMappedCopaStartDate(int PobCatalogueId,int CopaClass)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            DateTime Data = RestClient.GetLatestMappedCopaStartDate(PobCatalogueId, CopaClass);
            if (Data.Year == 0001)
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLatestMappedGpobStartDate(int PobCatalogueId,string GpobType)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            DateTime Data = RestClient.GetLatestMappedGpobStartDate(PobCatalogueId,GpobType);
            if (Data.Year == 0001)
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteMappingRow(int RowId, string Type)
        {
            RestClient.DeleteMappingRow(RowId, Type);
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
    }
}
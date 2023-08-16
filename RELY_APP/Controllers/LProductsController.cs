using Newtonsoft.Json;
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LProductsController : Controller
    {
        //Global Variable
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();
        //int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
        //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;

        //  IRProductSystemsRestClient RPSRC = new RProductSystemsRestClient();
        //  IRProductCategoriesRestClient PCRC = new RProductCategoriesRestClient();
        IRSysCatRestClient SCRC = new RSysCatRestClient();
        ILLocalPOBRestClient LPRC = new LLocalPOBRestClient();
        ILProductsRestClient RestClient = new LProductsRestClient();
        ILProductPobsRestClient PPRC = new LProductPobsRestClient();
        ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
        IProductHistoryGridRestClient ProductHistoryRestClient = new ProductHistoryGridRestClient();
        ILReconBucketRestclient RBRC = new LReconBucketRestclient();

        // GET: LProducts

        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Local Products ";
            return View();
        }
        public JsonResult GetByCompanyCodeForChangeSurveyCounts(int CurrentProductId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetByCompanyCodeForChangeSurveyCount(CompanyCode, CurrentProductId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetByCompanyCodeForChangeSurvey(int CurrentProductId, string sortdatafield, string sortorder, int? pagesize, int? pagenum)
        {
            //updated int pagesize to int? as sometimes it was throwing exception due to some unknown reason.
            //and whenever pagesize is null, setting it to default value. This is a workaround for the bug.
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
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
            var ApiData = RestClient.GetByCompanyCodeForChangeSurvey(CompanyCode, CurrentProductId,sortdatafield,sortorder, PageSize, PageNum,FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetByCompanyCode()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetByCompanyCode(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductsListForRequestCount()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetProductsListForRequestCount(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //new method for getting Products list for Request page
        public JsonResult GetProductsListForRequest(string sortdatafield, string sortorder, int pagesize, int pagenum)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetProductsListForRequest(CompanyCode, sortdatafield, sortorder, pagesize, pagenum, FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetByRequestId(int RequestId)
        {
            var ApiData = RestClient.GetByRequestId(RequestId, "LRequests");
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //Add by Ankit
        [ControllerActionFilter]
        public JsonResult GetProductRequestHistory(int ProductId)
        {
            var ApiData = RestClient.GetProductRequestHistory(ProductId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetProductHistory(string SelecterType, int ProductId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetProductHistory(SelecterType, CompanyCode, ProductId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        /*
        //This method loads the data in dropdown for Product System
        private SelectList GetSystem()
        {
            var ApiData = RPSRC.GetByCompanyCode(CompanyCode);
            var x = new SelectList(ApiData, "Id", "Name");
            return x;
        }

        //This method loads the data in dropdown for Product Category
        private SelectList GetCategory()
        {
            //this code has to be updated to get the data from API.
            var ApiData = PCRC.GetByCompanyCode(CompanyCode);
            var x = new SelectList(ApiData, "Id", "Name");
            return x;
        }*/
        [ControllerActionFilter]
        private SelectList GetSysCat(int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //this code has to be updated to get the data from API.
            var ApiData = SCRC.GetSysCatforDropDown(CompanyCode);
            var x = new SelectList(ApiData, "Id", "SysCat", Selected);
            return x;
        }

        [ControllerActionFilter]
        private SelectList GetSurveys(int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //this code has to be updated to get the data from API.
            ILFinancialSurveysRestClient FSRC = new LFinancialSurveysRestClient();
            var ApiData = FSRC.GetSurveyforDropDown(CompanyCode);
            var x = new SelectList(ApiData, "Id", "SurveyName", Selected);
            return x;
        }

       
        [ControllerActionFilter]
        public JsonResult GetAttributesForProducts(int SysCatId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            IRSysCatRestClient RSRC = new RSysCatRestClient();
            var SysCatObj = RSRC.GetById(SysCatId);

            //data should fetch on the basis of SysCode instead of SysCat
            //var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LProducts", SysCatObj.SysCat);
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LProducts", SysCatObj.SysCatCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
           return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetAttributesforProductPob(int SysCatId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            IRSysCatRestClient RSRC = new RSysCatRestClient();
            var SysCatObj = RSRC.GetById(SysCatId);

            //data should fetch on the basis of SysCode instead of SysCat
            //var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LProductPobs", SysCatObj.SysCat);
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LProductPobs", SysCatObj.SysCatCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);
        }

        //this method is not in use as Cloning is being handled thru SP
        /* public ActionResult Change(int id, string Source, int WorkFlowId,int StepId)
         {
             ViewBag.FormType = "Change";
             System.Web.HttpContext.Current.Session["Title"] = "Change Product";
             LProductViewModel model = RestClient.GetById(id);
             ViewBag.SysCatId = GetSysCat(model.SysCatId);
             ViewBag.LocalPob = GetLocalPob();
             ViewBag.WorkFlowId = WorkFlowId;
             ViewBag.StepId = StepId;
             model.Source = Source;
             IRSysCatRestClient RSRC = new RSysCatRestClient();
             var SysCatObj = RSRC.GetById(model.SysCatId);
             var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LProducts", SysCatObj.SysCat);
             ViewBag.CompanySpecificColumns = CompanySpecificData;

             var CompanySpecificDataForPob = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LProductPobs", SysCatObj.SysCat);
             ViewBag.CompanySpecificColumnsForPob = CompanySpecificDataForPob;
             if (!string.IsNullOrEmpty(model.WFComments)) //replacing newlines characters to space as its creating issue in Javascript
             {
                 model.WFComments = model.WFComments.Replace("\r\n", " ");
             }
             //Below line will decide how to display UploadDocument PartialView on the page
             ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true };

             return View("Edit",model);
         }*/
        //[ControllerActionFilter]
        public JsonResult GetById(int Id) { 

            var ApiData = RestClient.GetById(Id);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult Edit(int id, string Source/*,int WorkFlowId,int StepId*/)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//Clear Session now
            ViewBag.FormType = "Edit";
            System.Web.HttpContext.Current.Session["Title"] = "Edit Product";
            
            LProductViewModel model = RestClient.GetById(id);
            ViewBag.Title = "Edit Product ( " + model.Name + " )";
            ViewBag.SysCatId = GetSysCat(model.SysCatId);
            ViewBag.SurveyId = GetSurveys(model.SurveyId);
            ViewBag.EntityType = "LProducts";
            // ViewBag.LocalPob = GetLocalPob();

            /*------Commented by RS as to hide workflowid and stepid during any transaction Actions (Create, edit,review,change)-------------------*/
            //ViewBag.WorkFlowId = WorkFlowId;
            //ViewBag.StepId = StepId;
            /*------Commented by RS as to hide workflowid and stepid during any transaction Actions (Create, edit,review,change)-------------------*/
            //when source is Request, calculating WorkflowId based on WFType instead of Session
            int WorkFlowId = 0; int StepId = 0; int OrdinalValue = 0;
            if ("Request".Equals(Source))
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

             OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
             StepId = Globals.GetStepId(OrdinalValue,WorkFlowId);
             ViewBag.StepId = StepId;

            model.Source = Source;
            IRSysCatRestClient RSRC = new RSysCatRestClient();
            var SysCatObj = RSRC.GetById(model.SysCatId);
            model.SysCat = SysCatObj.SysCat;
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LProducts", SysCatObj.SysCatCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            
            var CompanySpecificDataForPob = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LProductPobs", SysCatObj.SysCatCode);
            ViewBag.CompanySpecificColumnsForPob = CompanySpecificDataForPob;
             
            var ColumnsDataForHistoryGrid = LCSCRC.GetColumnsForProductHistory(CompanyCode,SysCatObj.SysCatCode);
            ViewBag.ColumnsDataForHistoryGrid = ColumnsDataForHistoryGrid;


            string TypeOfProductColumn = LCSCRC.GetColumnNameByLabel(CompanyCode, "LProducts", model.SysCatId, "Type Of Product");
            PropertyInfo propertyInfo = model.GetType().GetProperty(TypeOfProductColumn);
            if (propertyInfo != null)
            {
                //disable SSP
               string TypeOfProduct = propertyInfo.GetValue(model) as string;
                if("Discount SOC".Equals(TypeOfProduct) || "Marker SOC".Equals(TypeOfProduct)){
                    ViewBag.DisableSSP = "disable";
                }
            }

                int ProductId = model.Id;
            //Get No of Columns for Product History Grid
            //int ProductHistorycolumnCount = ProductHistoryRestClient.GetColumnsCountForProductHistory(ProductId);
            //ViewBag.ProductHistorycolumnCount = ProductHistorycolumnCount;

            //Calling method to get columns for Recon-Grid
            var ReconGridColumns = RBRC.GetColumnsRecon(model.ProductCode, model.SysCatId, CompanyCode);
            //adding columns received from method into view Bag to get on view
            ViewBag.LReconColumns = ReconGridColumns;

            //Get StartDate of prevoius Version
            if(model.Version > 1)
                ViewBag.PreviousVersionStartDate = RestClient.GetPreviousVersionStartDate(ProductId);

            //if (!string.IsNullOrEmpty(model.WFComments)) //replacing newlines characters to space as its creating issue in Javascript
            //{
            //    model.WFComments = model.WFComments.Replace("\r\n", " ");
            //}
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Edit");
            ViewBag.StepId = StepId;
            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LProducts", id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true ,ExistingFilesList=existingFilesList};
            /*Start Namita changes-----Accessing these values in Partial View*/
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LProducts";
           
            return View("Edit",model);
        }

        [ControllerActionFilter]
        public ActionResult Review(int id, string Source/*, int WorkFlowId,int StepId*/)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//Clear Session now
            ViewBag.FormType = "Review";
            System.Web.HttpContext.Current.Session["Title"] = "Review Product";
            
            LProductViewModel model = RestClient.GetById(id);
            ViewBag.Title = "Review Product ( " + model.Name + " )";
            ViewBag.SysCatId = GetSysCat(model.SysCatId);
            //ViewBag.SurveyId = GetSurveys(model.SurveyId);
            ViewBag.SurveyId = GetSurveys(model.SurveyId);
            //LFinancialSurveysViewModel surveyModel = null;
            //if (model.SurveyId != null)
            //{
            //    ILFinancialSurveysRestClient FSRC = new LFinancialSurveysRestClient();
            //    surveyModel = FSRC.GetLFinancialSurveyById(model.SurveyId);
            //    model.SurveyName = surveyModel.SurveyName;
            //}

            //if (surveyModel !=null)
            //    ViewBag.SurveyName = surveyModel.SurveyName;
            // ViewBag.LocalPob = GetLocalPob();

            /*------Commented by RS as to hide workflowid and stepid during any transaction Actions (Create, edit,review,change)-------------------*/
            //ViewBag.WorkFlowId = WorkFlowId;
            //ViewBag.StepId = StepId;
            /*------Commented by RS as to hide workflowid and stepid during any transaction Actions (Create, edit,review,change)-------------------*/
            //when source is Request, calculating WorkflowId based on WFType instead of Session
            int WorkFlowId = 0;
            if ("Request".Equals(Source))
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


            model.Source = Source;
            IRSysCatRestClient RSRC = new RSysCatRestClient();
            var SysCatObj = RSRC.GetById(model.SysCatId);
            model.SysCat = SysCatObj.SysCat;

            //Get No of Columns for Product History Grid
            int ProductId = model.Id;
            //int ProductHistorycolumnCount = ProductHistoryRestClient.GetColumnsCountForProductHistory(ProductId);
            //ViewBag.ProductHistorycolumnCount = ProductHistorycolumnCount;

            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LProducts", SysCatObj.SysCatCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            var ColumnsDataForHistoryGrid = LCSCRC.GetColumnsForProductHistory(CompanyCode, SysCatObj.SysCatCode);
            ViewBag.ColumnsDataForHistoryGrid = ColumnsDataForHistoryGrid;
            var CompanySpecificDataForPob = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LProductPobs", SysCatObj.SysCatCode);
            ViewBag.CompanySpecificColumnsForPob = CompanySpecificDataForPob;

            //Calling method to get columns for Recon-Grid
            var ReconGridColumns = RBRC.GetColumnsRecon(model.ProductCode, model.SysCatId, CompanyCode);
            //adding columns received from method into view Bag to get on view
            ViewBag.LReconColumns = ReconGridColumns;
            
            var ColumnsDataForReconGrid = LCSCRC.GetColumnsForProductHistory(CompanyCode, SysCatObj.SysCatCode);
            ViewBag.ColumnsDataForHistoryGrid = ColumnsDataForHistoryGrid;

            //if (!string.IsNullOrEmpty(model.WFComments)) //replacing newlines characters to space as its creating issue in Javascript
            //{
            //    model.WFComments = model.WFComments.Replace("\r\n", " ");
            //}
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Review");
            ViewBag.StepId = StepId;
            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LProducts", id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true ,ExistingFilesList=existingFilesList};
            /*Start Namita changes-----Accessing these values in Partial View*/
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LProducts";
            ViewBag.productcode = model.ProductCode;
            return View(model);
        }

        [ControllerActionFilter]
        public ActionResult Create(string Source,int? RequestId,string RequestName/*,int StepId,int WorkFlowId*/)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            System.Web.HttpContext.Current.Session["Title"] = "Create Product";
            ViewBag.Title = "Create Product";
            string FormType = "";
            if ("Request".Equals(Source))
            {
                FormType = "CreateOnFly";//new action has been introduced for Creating LPob on fly.
                ViewBag.FormType = FormType;
            }
            else
            {
                FormType = "Create";
                ViewBag.FormType = FormType;
            }
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//Clear Session now
            //  var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode,"LProducts","");
            //  ViewBag.CompanySpecificColumns = CompanySpecificData;
            ViewBag.SysCatId = GetSysCat(null);
            ViewBag.SurveyId = GetSurveys(null);
            //ViewBag.LocalPob = GetLocalPob();
            /*------Commented by RS as to hide workflowid and stepid during any transaction Actions (Create, edit,review,change)-------------------*/
            //ViewBag.WorkFlowId = WorkFlowId;
            //ViewBag.StepId = StepId;
            /*------Commented by RS as to hide workflowid and stepid during any transaction Actions (Create, edit,review,change)-------------------*/
            LProductViewModel model = new LProductViewModel();
            
            int WorkFlowId = 0; int StepId = 0; int OrdinalValue = 0;
            if ("Request".Equals(Source))
            {
                IRWorkFlowsRestClient RRC = new RWorkFlowsRestClient();
                var Workflow = RRC.GetByType("LProducts", CompanyCode);//Type is being hardcoded here, Since in Create, we dont have WFtype defined for the transaction.
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


            if (RequestId.HasValue)
            {
                model.RequestId = RequestId.Value;
                model.RequestName = RequestName;
            }
            model.Source = Source;
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true ,ExistingFilesList=null};
            /*Start Namita changes-----Accessing these values in Partial View*/
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LProducts";
            return View(model);
        }

        [ControllerActionFilter]
        public string SaveProductPobData(string GridArray, int ProductId, int collength,string FormType,string TypeOfProduct)
        {
           // string GrdArray1 = GridArray.TrimEnd('}');
           // string GrdArray2 = GrdArray1.TrimStart('{');
            string GrdAry = GridArray.TrimEnd(',');
            GrdAry = GrdAry.TrimStart(',');
            string returnMessage = "";
            string[] keyValuePair = GrdAry.Split(',');
            List<LProductPobViewModel> modelList = new List<LProductPobViewModel>();
            int i = 0;
            //for (var i = 0; i < keyValuePair.Length; i = i + collength)
            {
                var ProductPob = new LProductPobViewModel
                {
                    ProductId = ProductId,

                   
                };

                for (var j = 0; j < keyValuePair.Length; j++)
                {
                    string[] dataList = keyValuePair[i + j].Split(':');
                    var AttributeName = dataList[0]; var AttributeValue = dataList[1];
                    PropertyInfo propertyInfo = ProductPob.GetType().GetProperty(AttributeName);
                    if (propertyInfo != null)
                    {
                        try
                        {
                            if (propertyInfo.PropertyType.FullName.Contains("System.DateTime") && !(String.IsNullOrEmpty(AttributeValue) || AttributeValue.Equals("undefined")))
                            {
                                DateTime dt = DateTime.ParseExact(AttributeValue + " 13:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                propertyInfo.SetValue(ProductPob, dt, null);
                            }
                            else
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
                                        propertyInfo.SetValue(ProductPob, Convert.ChangeType(AttributeValue, Nullable.GetUnderlyingType(propertyInfo.PropertyType)), null);
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
                                        propertyInfo.SetValue(ProductPob, Convert.ChangeType(AttributeValue, propertyInfo.PropertyType), null);
                                    }
                                }
                            }
                        }
                        catch(Exception ex)
                        {

                        }
                    }
                }
                if (FormType.Equals("Change"))
                {
                    ProductPob.Id = 0; //in case of Change, new version is created.
                }
               // if (ProductPob.Id > 0) { }
                //else
                {
                    modelList.Add(ProductPob);
                }
            }
            try
            {
                if(modelList.Count() > 0)
                PPRC.Add(modelList, TypeOfProduct, null);
                returnMessage = "Success";
            }
            catch (Exception ex)
            {
                returnMessage = ex.Data["ErrorMessage"].ToString();
                //return errormessage;
            }
            return returnMessage;

        }

        //not in use
        /*     //this method will save the details of SSP
             public string SaveSSPDetails(string SSPData,int ProductId )
              //need to update the signature later
             {
                 ILProductSSPRestClient PSRC = new LProductSSPRestClient();
                 string returnMessage = "";
                 string[] dataList = SSPData.Split(',');
                 List<LProductSSPViewModel> modelList = new List<LProductSSPViewModel>();
                 for (var i = 0; i < dataList.Length; i = i + 4)
                 {
                     decimal SSP = Convert.ToDecimal(dataList[i]);
                     string strValidtyStart = dataList[i + 1] + " 00:00:00";
                     string strValidtyEnd = dataList[i + 2] + " 23:59:59";
                     int Id = Convert.ToInt32(dataList[i + 3]);
                     //Parsing ValidtyStart and ValidityEnd strings to proper Date Time Format
                     var ValidityStartDate = DateTime.ParseExact(strValidtyStart, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                     var ValidityEndDate = DateTime.ParseExact(strValidtyEnd, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                     var LLocalPobSsp = new LProductSSPViewModel
                     {
                         ProductId = ProductId,
                         Amount = SSP,
                         EffectiveStartDate = ValidityStartDate,
                         EffectiveEndDate = ValidityEndDate,
                     };
                     modelList.Add(LLocalPobSsp);
                 }
                 try
                 {
                     PSRC.Add(modelList, ProductId, null);
                     returnMessage = "Success";
                 }
                 catch(Exception ex)
                 {
                     returnMessage = ex.Data["ErrorMessage"].ToString();
                     //return errormessage;
                 }
                 return returnMessage;

             }

             //this method will save the details of Duration
             public string SaveDurationDetails(string DurationData, int ProductId)
             //need to update the signature later
             {
                 ILProductContractDurationRestClient PCDRC = new LProductContractDurationRestClient();
                 string returnMessage = "";
                 string[] dataList = DurationData.Split(',');
                 List<LProductContractDurationViewModel> modelList = new List<LProductContractDurationViewModel>();
                 for (var i = 0; i < dataList.Length; i = i + 4)
                 {
                     string Duration = Convert.ToString(dataList[i]);
                     string strValidtyStart = dataList[i + 1] + " 00:00:00";
                     string strValidtyEnd = dataList[i + 2] + " 23:59:59";
                     int Id = Convert.ToInt32(dataList[i + 3]);
                     //Parsing ValidtyStart and ValidityEnd strings to proper Date Time Format
                     var ValidityStartDate = DateTime.ParseExact(strValidtyStart, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                     var ValidityEndDate = DateTime.ParseExact(strValidtyEnd, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                     var LLocalProductContractDuration = new LProductContractDurationViewModel
                     {
                         ProductId = ProductId,
                         Duration = Duration,
                         EffectiveStartDate = ValidityStartDate,
                         EffectiveEndDate = ValidityEndDate,
                     };
                     modelList.Add(LLocalProductContractDuration);
                 }
                 try
                 {
                     PCDRC.Add(modelList, ProductId, null);
                     returnMessage = "Success";
                 }
                 catch(Exception ex)
                 {
                     returnMessage = ex.Data["ErrorMessage"].ToString();
                 }
                 return returnMessage;

             }
             */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult SaveProducts(LProductViewModel model, string GridArray, int collength,string Comments,int WorkFlowId,string FormType,string SSPDate1, string SSPDate2,string EstDate1,string EstDate2,bool DuplicityCheckFlag,int StepId, string CheckBoxAttributeValues)
        {
            LCompanySpecificColumnsRestClient LCRC = new LCompanySpecificColumnsRestClient();
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            var l_attributesList = CheckBoxAttributeValues.Split('|');
            string TypeOfProduct = null;
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
                if (!( /*FormType.Equals("Create") ||*/ FormType.Equals("EditOnCreate"))) //as we dont have stepid while create,skipping authorization.EditOnCreate is being used for Editing just after Product creatioon,keeping on same page
                {
                    var isAuthorized = Globals.CheckActionAuthorization(FormType, UserRoleId, LoggedInUserId, WorkFlowId, StepId);
                    if (!isAuthorized)
                    {
                        model.ErrorMessage = "Sorry , You are not authorised to perform this action .Please contact L2 Admin";
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }

                if (FormType.Equals("EditOnCreate"))
                {
                    FormType = "Edit";
                    model.CreatedDateTime = DateTime.UtcNow;
                }
                if (DuplicityCheckFlag)
                {
                    string ColumnName = LCRC.GetColumnNameByLabel(CompanyCode, "LProducts", model.SysCatId, "Business Category");
                    //List<string> ExistingPCs = RestClient.GetProductCodesByOpcoSysCat(CompanyCode, model.SysCatId, model.Id);//get existing Product Codes for SysCat
                    if (!string.IsNullOrEmpty(model.ProductCode))
                    {
                        string WarningMessage = null;
                        string IsDuplicate = RestClient.CheckProductDuplicacy(CompanyCode, model.SysCatId, Convert.ToString(model.Id), model.ProductCode, GetColumnValue(model, ColumnName));
                        if ("Duplicate".Equals(IsDuplicate))
                        {
                            WarningMessage = "ProductCode already exists, Please change and continue";

                        }
                        //if (ExistingPCs != null)
                        //{
                        //    //check whether the provided name exist in Existing list or not
                        //    if (ExistingPCs.Contains(model.ProductCode))//PC comparison needs to be case-sensitive
                        //    {
                        //        WarningMessage = "ProductCode already exists, Please change and continue";
                        //    }
                        //}
                        if (!string.IsNullOrEmpty(WarningMessage))
                        {
                            model.ErrorMessage = WarningMessage;
                            return Json(model, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                //Date manipulations for Effective Start and End Date
                DateTime SSPStartDate = new DateTime(2010, 01, 01, 13, 00, 00);//default to today's date
                DateTime SSPEndDate = new DateTime(2099, 12, 31,13,00,00);//defaul to 2099-12-31
               
                if (!string.IsNullOrEmpty(SSPDate1))
                {
                    SSPDate1 = SSPDate1 + " 13:00:00"; //This is just a workaround, not fix. due to some time/offset difference, db was saving dates with 2hrs difference. so, taking 3hrs instead of 00hrs
                    SSPStartDate = DateTime.ParseExact(SSPDate1, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(SSPDate2))
                {
                    SSPDate2 = SSPDate2 + " 23:59:59";
                    SSPEndDate = DateTime.ParseExact(SSPDate2, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                model.EffectiveStartDate = SSPStartDate;
                model.EffectiveEndDate = SSPEndDate;
                Nullable<DateTime> EstStartDate = null;
                Nullable<DateTime> EstEndDate = null;
                if (!string.IsNullOrEmpty(EstDate1))
                {
                    EstDate1 = EstDate1 + " 13:00:00";//due to some time/offset difference, db was saving dates with 2hrs difference. so, taking 3hrs instead of 00hrs
                    EstStartDate = DateTime.ParseExact(EstDate1, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(EstDate2))
                {
                    EstDate2 = EstDate2 + " 23:59:59";
                    EstEndDate = DateTime.ParseExact(EstDate2, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                model.AttributeD03 = EstStartDate;
                model.AttributeD04 = EstEndDate;
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
                IWStepRestClient WSRC = new WStepRestClient();
                var wcolumns = WSRC.GetWFColumnsFromWStep(LoggedInUserId, UserRoleId, WorkFlowId, FormType, CompanyCode);
                string wcName = null;
                int wcOrdinal = 0;
                if (wcolumns != null)
                {
                    wcName = wcolumns.Name;
                    wcOrdinal = wcolumns.Ordinal;
                }
                model.AttributeC20 = FormType; // using AttributeC20 for checking FormType while saving data
                string TypeOfProductColumn = LCRC.GetColumnNameByLabel(CompanyCode, "LProducts", model.SysCatId, "Type Of Product");
                //5th June 2020 - special treatment when Type of Product is MarkerSOC or DIscount SOC
                /* In Option 2, I want to highlight two more points:-
                Pob Catalogue Id :- Whenever a ‘Discount SOC’ product is created, automatically attach Pob catalogue id “0” to it. AND do not let user attach any other Pob Catalogue id.
                Pob Catalogue Id :- Whenever a “Marker SOC” product is created then automatically attach “99999” Pob catalogue Id and do not let user attach any other Pob Catalogue Id.
                SspId:- Whenever a “Discount SOC” and “Marker SOC” product is created then attach “0” SSP Id with them and do not let user attach any other Ssp Id.*/
                PropertyInfo propertyInfo = model.GetType().GetProperty(TypeOfProductColumn);
                if (propertyInfo != null)
                {
                    TypeOfProduct = propertyInfo.GetValue(model) as string;
                    if (!string.IsNullOrEmpty(TypeOfProduct))
                    {
                        switch (TypeOfProduct)
                        {
                            case "Discount SOC":
                                model.SspId = 0;
                                GridArray = "PobCatalogueId:0,EffectiveStartDate:01/01/2010,EffectiveEndDate:31/12/2099";
                                break;
                            case "Marker SOC":
                                model.SspId = 0;
                                GridArray = "PobCatalogueId:99999,EffectiveStartDate:01/01/2010,EffectiveEndDate:31/12/2099";
                                break;
                        }
                    }
                }
                //call restcliet's add method
                if (FormType.Equals("Edit"))
                {
                    model.UpdatedById = LoggedInUserId;
                    model.UpdatedDateTime = DateTime.UtcNow;
                    model.Status = "Edit";
                    
                    RestClient.Update(model, null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList);
                }
                else
                {
                    model.WFRequesterRoleId = UserRoleId;
                    model.WFRequesterId = LoggedInUserId;
                    model.WFCurrentOwnerId = LoggedInUserId;
                    /*To check Product Created on Fly or not.*/
                    if (model.Source == "Request")
                    {
                        model.WFStatus = "Parked";
                        model.WFOrdinal = 0;
                    }
                    else { 
                        model.WFStatus = "Saved";
                        model.WFOrdinal = wcOrdinal;
                    }
                    IRWorkFlowsRestClient RRC = new RWorkFlowsRestClient();
                    var Workflow = RRC.GetById(WorkFlowId);

                    model.WFType = Workflow.WFType;//must match with the WFType in RworkFlow
                    
                    model.CompanyCode = CompanyCode;
                    model.CreatedDateTime = System.DateTime.UtcNow;
                    model.UpdatedDateTime = System.DateTime.UtcNow;
                    model.CreatedById = LoggedInUserId;
                    model.UpdatedById = LoggedInUserId;
                    model.Status = wcName;
                    if (FormType.Equals("Create") || FormType.Equals("CreateOnFly"))
                    {
                        model.Version = 1;
                    }
                    if (FormType.Equals("Change"))//in case of Change, new version is created.
                    {
                        // model.Id = 0;
                        model.Version = model.Version + 1;
                    }
                    model.WFStatusDateTime = DateTime.UtcNow;
                    model.Id = RestClient.Add(model, null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList);

                }
                //call Add to ProductPob
                string ProductMessage = "Product created sucessfully";
                string ProductPobResult = "";
                if (!string.IsNullOrEmpty(GridArray))
                {
                    ProductPobResult = SaveProductPobData(GridArray, model.Id, collength,FormType, TypeOfProduct);// call to method for Posting the SSP table details
                                                                                   //will check for SSPResult != "Success" , then concate the errormessage with the SSPResult message
                    if (ProductPobResult != "Success")
                    {
                        model.ErrorMessage = model.ErrorMessage + Environment.NewLine + ProductPobResult;
                    }
                }
                return Json(new{ success= true, Id = model.Id, model = model,Product = model.Name + "-" + model.ProductCode, ErrorMessage = model.ErrorMessage },JsonRequestBehavior.AllowGet);
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
        private string GetColumnValue(dynamic Model, string ColumnName)
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


        [ControllerActionFilter]
        public JsonResult UpdateSurvey(int EntityId, int SurveyId, bool IsCopySurvey, int? SourceProductId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());

            try
            {
               var model = RestClient.UpdateSurveyNew(EntityId, SurveyId, IsCopySurvey, SourceProductId,LoggedInUserId,CompanyCode,null);
                return Json(new{model = model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string redirectUrl = ex.Data["RedirectToUrl"].ToString();
                return Json(new { ErrorMessage = ex.Data["ErrorMessage"].ToString() }, JsonRequestBehavior.AllowGet);
                    
            }

        }

        [ControllerActionFilter]
        public JsonResult GetProductPobs(int? ProductId)
        {
            try
            {
                var ApiData  = PPRC.GetByProductId(ProductId);
                return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        [ControllerActionFilter]
        public JsonResult GetProductObligations(int ProductId)
        {
            try
            {
                var  ApiData = PPRC.GetByProductIdForProductGrid(ProductId);
                ViewBag.ExistingObligationsCount = ApiData.Count();
                return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        [ControllerActionFilter]
        public JsonResult DeleteProductObligations(int Id)
        {
            try
            {
                PPRC.Delete(Id,  null);
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        //Get data for Product History Grid
        [ControllerActionFilter]
        public JsonResult GetDataForProductHistory(int ProductId)
        {
            var ApiData = ProductHistoryRestClient.GetDataForProductHistory(ProductId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ProductLibrary()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Products Library";
            return View();
        }
        
        public JsonResult GetAllProductCounts()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetAllProductsCounts(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProductSDetails(string sortdatafield, string sortorder, int pagesize, int pagenum)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetAllProductsDetails( CompanyCode, sortdatafield,  sortorder, pagesize, pagenum, FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLocalPobCounts()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = LPRC.GetCompletedListCount(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetLocalPob(string sortdatafield, string sortorder, int pagesize, int pagenum,string UnderlyingProductId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            //manipulating datafield as this cause ambiguity in SP
            if (!String.IsNullOrEmpty(FilterQuery))
            {
                FilterQuery = FilterQuery.Replace("Name", "aa.Name");
            }
            var ApiData = LPRC.GetCompletedList(CompanyCode, sortdatafield, sortorder, pagesize, pagenum, FilterQuery, UnderlyingProductId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
       

        /// <summary>
        /// Created by Rakhi Singh on 18 june 2018
        /// Function to get data for Recon Columns of Recon-Grid
        /// </summary>
        /// <param name="ProductCode"></param>
        /// <param name="SysCatId"></param>
        /// <param name="CompanyCode"></param>
        /// <returns>data for grid</returns>
        public JsonResult GetDataForRecon(string ProductCode, int SysCatId, string CompanyCode)
        {
            var ApiData = RBRC.GetDataForRecon(ProductCode, SysCatId, CompanyCode);
            
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MissingProductsReport()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Missing Products Report";
            return View();
        }

        public JsonResult GetMissingProducts()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            var ApiData = RBRC.GetMissingProducts(CompanyCode);

            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadProductHistoryDetails(int ProductId)
        {
            var ApiData = ProductHistoryRestClient.DownloadGetDataForProductHistory(ProductId);
            var ApiColumncounts = ProductHistoryRestClient.DownloadGetColumnsCountForProductHistory(ProductId);
            DataTable dtData = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "ProductHistoryDetails_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dtData, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);

        }
        [CustomAuthorize]
        public JsonResult GetProductCountAttachedToRequest(int RequestId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            var ApiData = RestClient.GetProductCountAttachedToRequest(RequestId,CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

       
    }
}
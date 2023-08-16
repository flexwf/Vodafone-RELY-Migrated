using Newtonsoft.Json;
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{

    
    [HandleCustomError]
    [SessionExpire]
    public class LAccountingScenariosController : Controller
    {
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();
        //int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
        //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;

        ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
        ILAccountingScenariosRestClient RestClient = new LAccountingScenariosRestClient();
        // GET: AccountingScenarios
        [ControllerActionFilter]
        public ActionResult Index()
        {
            return View();
        }
        //Save comments in LFSResponses.Comments//
        [ControllerActionFilter]
        public JsonResult SaveResponseComments(int ResponseId,string Comments)
        {
            RestClient.UpdateResponseComments(ResponseId,Comments);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //Get Matrix grid//
        [ControllerActionFilter]
        public JsonResult GetAccountingScenarioMatrix(int EntityId,string EntityType,string TabName)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetAccountingScenarioMatrix(EntityId,EntityType, TabName, CompanyCode);
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created By: Rakhi Singh
        /// Created Date: 29 june
        /// Description: It downloads the data of overview tab in accountsceneriomatrix
        /// </summary>
        /// <param name="EntityId"></param>
        /// <param name="EntityType"></param>
        /// <returns></returns>
        public ActionResult DownloadOverviewAccountingScenarioMatrix(int EntityId, string EntityType)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var OverViewTab = RestClient.DownloadGAccountScenerioMatrix(EntityId, EntityType, "Overview", CompanyCode);
           
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(OverViewTab, (typeof(DataTable)));
            if (dt.Columns.Count > 0)
            {
                //line to remove unwanted columns in grid on front-view
                dt.Columns.Remove("EntityId");
                dt.Columns.Remove("EntityType");
                dt.Columns.Remove("QuestionCode");

                //line that sets the arrangement of columns on front-view
                dt.Columns["Category"].SetOrdinal(0);
                dt.Columns["Situation"].SetOrdinal(1);
            }
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "OverviewAccountingScenerio_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
        }


        //Get Accounting Scenario Matrix
        [ControllerActionFilter]
        public ActionResult AccountingScenarioMatrix(int EntityId,string EntityType/*, int WorkFlowId, int StepId*/)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Accounting Scenario Matrix";
            ViewBag.Title = "Accounting Scenario Matrix";
            LAccountingScenarioViewModel model = new LAccountingScenarioViewModel();
            //ViewBag.WorkFlowId = WorkFlowId;
            //ViewBag.StepId = StepId;

            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]));
            ViewBag.StepId = StepId;
            ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);

            ViewBag.EntityId = EntityId;
            ViewBag.EntityType = EntityType;
            return View();
        }

        //Get Accounting Scenario Matrix
        [ControllerActionFilter]
        public ActionResult ChangeAccountingScenario(int? ResponseId,/*int WorkflowId,*/int ProductId)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Change Accounting Scenario";
            ViewBag.Title = "Change Accounting Scenario";
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetAccountingScenarioByResponseId(ResponseId.Value,CompanyCode);
            ViewBag.AccountingScenarioMatrixModel = ApiData;
            ViewBag.ResponseId = ResponseId;
            //ViewBag.WorkflowId = WorkflowId;
            ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            ViewBag.ProductId = ProductId;
            if (ApiData != null)
                ViewBag.QuestionCode = ApiData.QuestionCode;
            return View();
        }

        //method to load grid
        //Section 2: ‘One of the Scenario from reccomended list’
        [ControllerActionFilter]
        public JsonResult GetRecommendedScenarios(int ResponseId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetRecommendedAccountingScenarioByResponseId(ResponseId, CompanyCode);
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }
        //
        [ControllerActionFilter]
        public JsonResult GetAccountingScenarios()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetAccountingScenarioByCompanyCode(CompanyCode);
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

        //Save Selected Answer Bank
        [ControllerActionFilter]
        public ActionResult DeleteResponse(int ResponseId)
        {
            RestClient.DeleteResponse(ResponseId);
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }
        //Save Selected Answer Bank

        [ControllerActionFilter]
        public ActionResult SaveRecommendedScenario(int AnswerBankId,int ResponseId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            RestClient.SaveRecommendedAccountingScenario(AnswerBankId, ResponseId, CompanyCode, LoggedInUserId);
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }

        //Save Selected Acc Scenario
        [ControllerActionFilter]
        public ActionResult SaveAccountingScenario(int AccountingScenarioId, int ResponseId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            RestClient.SaveAccountingScenario(AccountingScenarioId,ResponseId,CompanyCode,LoggedInUserId);
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }

        //
        //Manual Tab
        [ControllerActionFilter]
        public JsonResult GetManualAccountingScenario(int EntityId, string EntityType)
        {
            var ManualTab = RestClient.GetManualAccountingScenario(EntityId,EntityType);
            return Json(ManualTab,JsonRequestBehavior.AllowGet);
        }

        //Get Products For Matrix
        [ControllerActionFilter]
        public JsonResult GetProductAccountingScenarioMatrix(int EntityId,string EntityType)
        {
            ILProductsRestClient LPRC = new LProductsRestClient();
            var ApiData = LPRC.GetByRequestId(EntityId,EntityType);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //Overview Tab
        [ControllerActionFilter]
        public JsonResult GetOverviewAccountingScenario(int EntityId, string EntityType)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var OverViewTab = RestClient.GetAccountingScenarioMatrix(EntityId, EntityType, "Overview", CompanyCode);
            return Json(OverViewTab, JsonRequestBehavior.AllowGet);
        }

        //Save Manual Accounting Scenario//There is a bug that Grid array is having count 1.SS is checkin in by this method for time being. But this method will stop working when row counts will be in hundreds
        [ControllerActionFilter]
        public ActionResult SaveManualAccountingScenario(Object[] FormData)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            RestClient.SaveManualAccountingScenario(FormData,CompanyCode);
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }

        [ControllerActionFilter]
        public ActionResult Create(/*int WorkflowId*/)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Accounting Scenario";
            ViewBag.Title = "Create Accounting Scenario";
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"].ToString();
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LAccountingScenarios", CompanyCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            ViewBag.FormType = "Create";
            //ViewBag.WorkflowId = WorkflowId;          
            ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            ViewBag.BusinessAreaId = GetDropDownValue("BusinessArea",null);
            ViewBag.NatureId = GetDropDownValue("Nature", null);
            ViewBag.EventId = GetDropDownValue("Event", null);
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.StepId = 0;
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, 0, "Edit");
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true,ExistingFilesList=null };
            //Accessing these valuse in Partial View
           // ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LAccountingScenarios";
            return View(new LAccountingScenarioViewModel());
        }

        [ControllerActionFilter]
        public ActionResult Edit(int Id/*, int WorkflowId, int StepId*/)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Edit Accounting Scenario";
            
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"].ToString();
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LAccountingScenarios", CompanyCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            LAccountingScenarioViewModel model = RestClient.GetById(Id);
            ViewBag.Title = "Edit Accounting Scenario ( " + model.Reference + " )";
            ViewBag.BusinessAreaId = GetDropDownValue("BusinessArea", model.BusinessAreaId);
            ViewBag.FormType = "Edit";
            //ViewBag.WorkflowId = WorkflowId;
            ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]));
            ViewBag.StepId = StepId;
            ViewBag.NatureId = GetDropDownValue("Nature", null);
            ViewBag.EventId = GetDropDownValue("Event", null);
            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LAccountingScenarios", Id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true ,ExistingFilesList=existingFilesList};
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Edit");
            //ViewBag.StepId = StepId;
            if (!string.IsNullOrEmpty(model.AttributeM01)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM01 = model.AttributeM01.Replace("\n", " ");
                model.AttributeM01 = model.AttributeM01.Replace("\r", " ");
            }

            if (!string.IsNullOrEmpty(model.AttributeM02)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM02 = model.AttributeM02.Replace("\n", " ");
                model.AttributeM02 = model.AttributeM02.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeM03)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM03 = model.AttributeM03.Replace("\n", " ");
                model.AttributeM03 = model.AttributeM03.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeM04)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM04 = model.AttributeM04.Replace("\n", " ");
                model.AttributeM04 = model.AttributeM04.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC01)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC01 = model.AttributeC01.Replace("\n", " ");
                model.AttributeC01 = model.AttributeC01.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC02)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC02 = model.AttributeC02.Replace("\n", " ");
                model.AttributeC02 = model.AttributeC02.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC03)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC03 = model.AttributeC03.Replace("\n", " ");
                model.AttributeC03 = model.AttributeC03.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC02)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC04 = model.AttributeC04.Replace("\n", " ");
                model.AttributeC04 = model.AttributeC04.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC05)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC05 = model.AttributeC05.Replace("\n", " ");
                model.AttributeC05 = model.AttributeC05.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC06)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC06 = model.AttributeC06.Replace("\n", " ");
                model.AttributeC06 = model.AttributeC06.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC07)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC07 = model.AttributeC07.Replace("\n", " ");
                model.AttributeC07 = model.AttributeC07.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC08)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC08 = model.AttributeC08.Replace("\n", " ");
                model.AttributeC08 = model.AttributeC08.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC09)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC09 = model.AttributeC09.Replace("\n", " ");
                model.AttributeC09 = model.AttributeC09.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC10)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC10 = model.AttributeC10.Replace("\n", " ");
                model.AttributeC10 = model.AttributeC10.Replace("\r", " ");

            }
            /*Start Namita changes*/
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LAccountingScenarios";
            /*End Namita Changes*/
            return View("Edit",model);
        }

        [ControllerActionFilter]
        public ActionResult Change(int Id/*, int WorkflowId, int StepId*/)
        {
            System.Web.HttpContext.Current.Session["Title"] = " Change Accounting Scenario";
            
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"].ToString();
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LAccountingScenarios", CompanyCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            LAccountingScenarioViewModel model = RestClient.GetById(Id);
            ViewBag.Title = "Change Accounting Scenario ( " + model.Reference + " )";
            ViewBag.BusinessAreaId = GetDropDownValue("BusinessArea", model.BusinessAreaId);
            ViewBag.FormType = "Change";
            //ViewBag.WorkflowId = WorkflowId;
            ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]));
            ViewBag.StepId = StepId;

            ViewBag.NatureId = GetDropDownValue("Nature", null);
            ViewBag.EventId = GetDropDownValue("Event", null);
            if (!string.IsNullOrEmpty(model.AttributeM01)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM01 = model.AttributeM01.Replace("\n", " ");
                model.AttributeM01 = model.AttributeM01.Replace("\r", " ");
            }

            if (!string.IsNullOrEmpty(model.AttributeM02)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM02 = model.AttributeM02.Replace("\n", " ");
                model.AttributeM02 = model.AttributeM02.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeM03)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM03 = model.AttributeM03.Replace("\n", " ");
                model.AttributeM03 = model.AttributeM03.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeM04)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM04 = model.AttributeM04.Replace("\n", " ");
                model.AttributeM04 = model.AttributeM04.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC01)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC01 = model.AttributeC01.Replace("\n", " ");
                model.AttributeC01 = model.AttributeC01.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC02)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC02 = model.AttributeC02.Replace("\n", " ");
                model.AttributeC02 = model.AttributeC02.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC03)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC03 = model.AttributeC03.Replace("\n", " ");
                model.AttributeC03 = model.AttributeC03.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC02)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC04 = model.AttributeC04.Replace("\n", " ");
                model.AttributeC04 = model.AttributeC04.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC05)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC05 = model.AttributeC05.Replace("\n", " ");
                model.AttributeC05 = model.AttributeC05.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC06)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC06 = model.AttributeC06.Replace("\n", " ");
                model.AttributeC06 = model.AttributeC06.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC07)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC07 = model.AttributeC07.Replace("\n", " ");
                model.AttributeC07 = model.AttributeC07.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC08)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC08 = model.AttributeC08.Replace("\n", " ");
                model.AttributeC08 = model.AttributeC08.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC09)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC09 = model.AttributeC09.Replace("\n", " ");
                model.AttributeC09 = model.AttributeC09.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC10)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC10 = model.AttributeC10.Replace("\n", " ");
                model.AttributeC10 = model.AttributeC10.Replace("\r", " ");

            }

            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LAccountingScenarios", Id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true ,ExistingFilesList=existingFilesList};
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Change");
            //ViewBag.StepId = StepId;
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LAccountingScenarios";
            return View("Create", model);
        }

        [ControllerActionFilter]
        public ActionResult Review(int Id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Review Accounting Scenario";
            
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"].ToString();
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LAccountingScenarios", CompanyCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            LAccountingScenarioViewModel model = RestClient.GetById(Id);
            ViewBag.Title = "Review Accounting Scenario ( " + model.Reference + " )";
            ViewBag.BusinessAreaId = GetDropDownValue("BusinessArea", model.BusinessAreaId);
            ViewBag.FormType = "Review";
            //ViewBag.WorkflowId = WorkflowId;
            ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]));
            ViewBag.StepId = StepId;
            ViewBag.NatureId = GetDropDownValue("Nature", null);
            ViewBag.EventId = GetDropDownValue("Event", null);
            if (!string.IsNullOrEmpty(model.AttributeM01)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM01 = model.AttributeM01.Replace("\n", " ");
                model.AttributeM01 = model.AttributeM01.Replace("\r", " ");
            }

            if (!string.IsNullOrEmpty(model.AttributeM02)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM02 = model.AttributeM02.Replace("\n", " ");
                model.AttributeM02 = model.AttributeM02.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeM03)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM03 = model.AttributeM03.Replace("\n", " ");
                model.AttributeM03 = model.AttributeM03.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeM04)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM04 = model.AttributeM04.Replace("\n", " ");
                model.AttributeM04 = model.AttributeM04.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC01)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC01 = model.AttributeC01.Replace("\n", " ");
                model.AttributeC01 = model.AttributeC01.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC02)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC02 = model.AttributeC02.Replace("\n", " ");
                model.AttributeC02 = model.AttributeC02.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC03)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC03 = model.AttributeC03.Replace("\n", " ");
                model.AttributeC03 = model.AttributeC03.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC02)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC04 = model.AttributeC04.Replace("\n", " ");
                model.AttributeC04 = model.AttributeC04.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC05)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC05 = model.AttributeC05.Replace("\n", " ");
                model.AttributeC05 = model.AttributeC05.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC06)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC06 = model.AttributeC06.Replace("\n", " ");
                model.AttributeC06 = model.AttributeC06.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC07)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC07 = model.AttributeC07.Replace("\n", " ");
                model.AttributeC07 = model.AttributeC07.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC08)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC08 = model.AttributeC08.Replace("\n", " ");
                model.AttributeC08 = model.AttributeC08.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC09)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC09 = model.AttributeC09.Replace("\n", " ");
                model.AttributeC09 = model.AttributeC09.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC10)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC10 = model.AttributeC10.Replace("\n", " ");
                model.AttributeC10 = model.AttributeC10.Replace("\r", " ");

            }

            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LAccountingScenarios", Id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true,ExistingFilesList=existingFilesList };

            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Review");
            //ViewBag.StepId = StepId;
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LAccountingScenarios";

            return View("Review", model);
        }

        [CustomAuthorize]
        //New method for reviewing AS from Product Accounting Scenario Helper
        public ActionResult ReviewForSurvey(int Id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Review Accounting Scenario";

            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"].ToString();
            var CompanySpecificData = LCSCRC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LAccountingScenarios", CompanyCode);
            ViewBag.CompanySpecificColumns = CompanySpecificData;
            LAccountingScenarioViewModel model = RestClient.GetById(Id);
            ViewBag.Title = "Review Accounting Scenario ( " + model.Reference + " )";
            ViewBag.BusinessAreaId = GetDropDownValue("BusinessArea", model.BusinessAreaId);
            ViewBag.FormType = "Review";
            //ViewBag.WorkflowId = WorkflowId;
            //ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            //int StepId = Globals.GetStepId(OrdinalValue, Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]));
            //ViewBag.StepId = StepId;
            ViewBag.NatureId = GetDropDownValue("Nature", null);
            ViewBag.EventId = GetDropDownValue("Event", null);
            if (!string.IsNullOrEmpty(model.AttributeM01)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM01 = model.AttributeM01.Replace("\n", " ");
                model.AttributeM01 = model.AttributeM01.Replace("\r", " ");
            }

            if (!string.IsNullOrEmpty(model.AttributeM02)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM02 = model.AttributeM02.Replace("\n", " ");
                model.AttributeM02 = model.AttributeM02.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeM03)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM03 = model.AttributeM03.Replace("\n", " ");
                model.AttributeM03 = model.AttributeM03.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeM04)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeM04 = model.AttributeM04.Replace("\n", " ");
                model.AttributeM04 = model.AttributeM04.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC01)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC01 = model.AttributeC01.Replace("\n", " ");
                model.AttributeC01 = model.AttributeC01.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC02)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC02 = model.AttributeC02.Replace("\n", " ");
                model.AttributeC02 = model.AttributeC02.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC03)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC03 = model.AttributeC03.Replace("\n", " ");
                model.AttributeC03 = model.AttributeC03.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC02)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC04 = model.AttributeC04.Replace("\n", " ");
                model.AttributeC04 = model.AttributeC04.Replace("\r", " ");
            }
            if (!string.IsNullOrEmpty(model.AttributeC05)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC05 = model.AttributeC05.Replace("\n", " ");
                model.AttributeC05 = model.AttributeC05.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC06)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC06 = model.AttributeC06.Replace("\n", " ");
                model.AttributeC06 = model.AttributeC06.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC07)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC07 = model.AttributeC07.Replace("\n", " ");
                model.AttributeC07 = model.AttributeC07.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC08)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC08 = model.AttributeC08.Replace("\n", " ");
                model.AttributeC08 = model.AttributeC08.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC09)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC09 = model.AttributeC09.Replace("\n", " ");
                model.AttributeC09 = model.AttributeC09.Replace("\r", " ");

            }
            if (!string.IsNullOrEmpty(model.AttributeC10)) //replacing newlines characters to space as its creating issue in Javascript
            {
                model.AttributeC10 = model.AttributeC10.Replace("\n", " ");
                model.AttributeC10 = model.AttributeC10.Replace("\r", " ");

            }

            //supporting documents details that are already attached
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var DocumentsList = LSDRC.GetByEntityType("LAccountingScenarios", Id);
            List<string> existingFilesList = new List<string>();
            foreach (var document in DocumentsList)
            {
                existingFilesList.Add(document.OriginalFileName);
            }
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true, ExistingFilesList = existingFilesList };

            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            //ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, UserRoleId, LoggedInUserId, StepId, "Review");
            ViewBag.BottomButtons = null;
            //ViewBag.StepId = StepId;
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LAccountingScenarios";

            return View( model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult SaveData(LAccountingScenarioViewModel model, string GridArray, int collength, int WorkFlowId, string FormType, string CheckBoxAttributeValues)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
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
                //need to uncomment after workflow configuration
                 var wcolumns = WSRC.GetWFColumnsFromWStep(LoggedInUserId, UserRoleId, WorkFlowId, "Create", CompanyCode);
               // WStepsViewModel wcolumns = null;
                string wcName = null;
                int wcOrdinal = 0;
                if (wcolumns != null)
                {
                    wcName = wcolumns.Name;
                    wcOrdinal = wcolumns.Ordinal;
                }
                model.AttributeC20 = FormType; // using AttributeC20 for checking FormType while saving data
                /*if(FormType.Equals("Change"))
                {
                    model.Id = 0;
                }*/
                //call restcliet's add method
                if (FormType.Equals("Edit") || FormType.Equals("Change"))
                {
                    model.UpdatedById = LoggedInUserId;
                    model.UpdatedDateTime = DateTime.Now;
                    model.Status = FormType;
                    model.WFStatus = "Saved";
                    model.WFOrdinal = wcOrdinal;
                    RestClient.Update(model, null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList);
                }
                else
                {
                    model.WFRequesterRoleId = UserRoleId;
                    model.WFRequesterId = LoggedInUserId;
                    model.WFCurrentOwnerId = LoggedInUserId;
                    /*To check Product Created on Fly or not.*/
                        model.WFStatus = "Saved";
                        model.WFOrdinal = wcOrdinal;
                    IRWorkFlowsRestClient RRC = new RWorkFlowsRestClient();
                    var Workflow = RRC.GetById(WorkFlowId);
                    model.WFType = Workflow.WFType;

                    model.CompanyCode = CompanyCode;
                    model.CreatedDateTime = System.DateTime.Now;
                    model.UpdatedDateTime = System.DateTime.Now;
                    model.CreatedById = LoggedInUserId;
                    model.UpdatedById = LoggedInUserId;
                    //model.Status = wcName;
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
                
                //call Add to ProductPob
                string ProductPobResult = "";
                if (!string.IsNullOrEmpty(GridArray))
                {
                    ProductPobResult = SaveLifeEventsData(GridArray, model.Id, collength, FormType);
                    if (ProductPobResult != "Success")
                    {
                        model.ErrorMessage = model.ErrorMessage + Environment.NewLine + ProductPobResult;
                    }
                }
                
                //model.ErrorMessage = "";
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
        public string SaveLifeEventsData(string GridArray, int AccountingScenarioId, int collength, string FormType)
        {
            ILASLifeCycleEventsRestClient LERC = new LASLifeCycleEventsRestClient();
            string GrdAry = GridArray.TrimEnd(',');
            string returnMessage = "";
            string[] keyValuePair = GrdAry.Split(',');
            List<LASLifecycleEventViewModel> modelList = new List<LASLifecycleEventViewModel>();
            for (var i = 0; i < keyValuePair.Length; i = i + collength)
            {
                var ASmodel = new LASLifecycleEventViewModel
                {
                    AccountingScenarioId = AccountingScenarioId,


                };

                for (var j = 0; j < collength; j++)
                {
                    string[] dataList = keyValuePair[i + j].Split(':');
                    var AttributeName = dataList[0]; var AttributeValue = dataList[1];

                    PropertyInfo propertyInfo = ASmodel.GetType().GetProperty(AttributeName);
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
                                propertyInfo.SetValue(ASmodel, Convert.ChangeType(AttributeValue, Nullable.GetUnderlyingType(propertyInfo.PropertyType)), null);
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
                                propertyInfo.SetValue(ASmodel, Convert.ChangeType(AttributeValue, propertyInfo.PropertyType), null);
                            }
                        }
                    }
                }
                    modelList.Add(ASmodel);
            }
            try
            {
                if (modelList.Count() > 0)
                    LERC.Add(modelList, null);
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
        public JsonResult BindLifecycleEventsDropdown(string DropdownFor)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            ILDropDownValuesRestClient LDDVRC = new LDropDownValuesRestClient();
            ILDropDownsRestClient LDDRC = new LDropDownsRestClient();
            var DropDownObj = LDDRC.GetByName(DropdownFor, CompanyCode);
            if (DropDownObj != null)
            {
                int DropdownId = DropDownObj.Id;
                var LDropDownValues = LDDVRC.GetByDropDownId(DropdownId).Select(p => new { p.Id, p.Description, p.Value });

                return Json(LDropDownValues, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        [ControllerActionFilter]
        public SelectList GetDropDownValue(string  DropdownFor,int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            ILDropDownValuesRestClient LDDVRC = new LDropDownValuesRestClient();
            ILDropDownsRestClient LDDRC = new LDropDownsRestClient();
            var DropDownObj = LDDRC.GetByName(DropdownFor,CompanyCode);
            if (DropDownObj != null)
            {
                int DropdownId = DropDownObj.Id;
                var LDropDownValues = LDDVRC.GetByDropDownId(DropdownId).Select(p => new { p.Id, p.Description, p.Value });
                var x = new SelectList(LDropDownValues, "Id", "Description", Selected);
                return x;
            }
            
            return new SelectList(String.Empty);

        }

        [ControllerActionFilter]
        public JsonResult GetLifeeventsGrid(int ASId)
        {
            ILASLifeCycleEventsRestClient LERC = new LASLifeCycleEventsRestClient();
            var ApiData = LERC.GetByASId(ASId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetByQuestionCodeCompanyCode(string QuestionCode)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetByQuestionCodeCompanyCode(QuestionCode,CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //This Method will return Acc Scenarios based on Company Code
        public JsonResult GetByCompanyCode()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var ApiData = RestClient.GetByCompanyCode(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompletedList()
        {
            //string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

            var ApiData = RestClient.GetCompletedList(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        ///Author: Created by Rakhi Singh (28/06/18)
        ///Description: this method download the grid of LifeEvent.
        public ActionResult DownloadLifeEventDetails(int ASId)
        {

            ILASLifeCycleEventsRestClient LERC = new LASLifeCycleEventsRestClient();           
            var ApiData = LERC.DownloadLifeEventDetails(ASId);
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));
            if (dt.Columns.Count>0)
            {
                dt.Columns["EventValue"].ColumnName = "Events";
                dt.Columns["NatureValue"].ColumnName = "Nature";
                dt.Columns["Ordinal"].SetOrdinal(0);
                dt.Columns["Events"].SetOrdinal(1);
                dt.Columns["Nature"].SetOrdinal(2);
                dt.Columns["Notes"].SetOrdinal(3);
            }
           
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "LifeCycleEvent_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);

        }
        


    }
}
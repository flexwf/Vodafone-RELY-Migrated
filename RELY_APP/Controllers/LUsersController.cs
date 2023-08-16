using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LUsersController : Controller
    {
        ILUsersRestClient RestClient = new LUsersRestClient();
        //Global variables
        //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
        //string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        //string UserName = Convert.ToString(System.Web.HttpContext.Current.Session["LoginEmail"]);

        // GET: sers//
        [ControllerActionFilter]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ControllerActionFilter]
        public ActionResult Create(int? id, string FormType/*, Nullable<int> StepId*/)
        {
            //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            //int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
            //string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);


            //var model = new LUserViewModel();
            //System.Web.HttpContext.Current.Session["Title"] = FormType + " User";

            //if (TransactionId.HasValue)
            //{
            //    model = RestClient.GetById(TransactionId.Value);
            //}
            //if (FormType=="Create")
            //{
            //    ViewBag.Title = FormType + " User";
            //}
            //else
            //{
            //    ViewBag.Title = FormType + " User ( " + model.FirstName + " )";

            //}
            //ViewBag.FormType = FormType;
            ////bottom buttons
            //IGenericGridRestClient GRC = new GenericGridRestClient();
            ////if (!StepId.HasValue)
            ////    StepId = 0;
            ////ViewBag.StepId = StepId;
            ////ViewBag.WorkflowId =GRC.GetWorkflowDetails(WorkFlowName,CompanyCode).Id;

            //ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            //int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            ////int StepId = Globals.GetStepId();
            //int StepId = Globals.GetStepId(OrdinalValue, Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]));
            //ViewBag.StepId = StepId;


            //ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, LoggedInRoleId, LoggedInUserId, ViewBag.StepId, FormType);

            var model = SetViewBagDetailsAndGetModel(FormType,id); ;
            return View(model);
        }


        [HttpGet]
       // [ControllerActionFilter]
        public ActionResult Edit(int? id, string FormType)
        {
            var model = SetViewBagDetailsAndGetModel(FormType, id);
            return View("Create", model);
        }
        [HttpGet]
        // [ControllerActionFilter]
        public ActionResult Review(int? id, string FormType)
        {
            var model = SetViewBagDetailsAndGetModel(FormType, id);
            return View("Create",model);
        }
        [HttpGet]
        // [ControllerActionFilter]
        public ActionResult Change(int? id, string FormType)
        {
            var model = SetViewBagDetailsAndGetModel(FormType, id);
            return View("Create", model);
        }

        private LUserViewModel SetViewBagDetailsAndGetModel(string FormType,int? TransactionId)
        {
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var model = new LUserViewModel();
            System.Web.HttpContext.Current.Session["Title"] = FormType + " User";
            if (TransactionId.HasValue)
            {
                model = RestClient.GetById(TransactionId.Value);
            }
            if (FormType == "Create")
            {
                ViewBag.Title = FormType + " User";
            }
            else
            {
                ViewBag.Title = FormType + " User ( " + model.FirstName + " )";

            }
            ViewBag.FormType = FormType;
            //bottom buttons
            IGenericGridRestClient GRC = new GenericGridRestClient();
            ViewBag.WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            int OrdinalValue = (int)(model.WFOrdinal == null ? 0 : model.WFOrdinal);
            int StepId = Globals.GetStepId(OrdinalValue, Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]));
            ViewBag.StepId = StepId;
            ViewBag.BottomButtons = GRC.GetFormBottomButtons(WorkFlowName, CompanyCode, LoggedInRoleId, LoggedInUserId, ViewBag.StepId, FormType);
            return model;
        }

        [HttpPost]
        [ControllerActionFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LUserViewModel LUVM,string RoleList,string FormType,int WorkFlowId,int StepId)
        {
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            ViewBag.StepId = StepId;
            ViewBag.WorkflowId = WorkFlowId;           
            try
            {
                if (!FormType.Equals("Create"))
                {
                    var isAuthorized = Globals.CheckActionAuthorization(FormType, LoggedInRoleId, LoggedInUserId, WorkFlowId, StepId);
                    if (!isAuthorized)
                    {
                        LUVM.ErrorMessage = "Sorry , You are not authorised to perform this action .Please contact L2 Admin";
                        //return Json(LUVM, JsonRequestBehavior.AllowGet);
                        return RedirectToAction("Index", "GenericGrid");
                    }
                }
                LUVM.CompanyCode = CompanyCode;
                LUVM.UpdatedById = LoggedInUserId;
                LUVM.UpdatedDateTime = DateTime.UtcNow;
                //if (!string.IsNullOrEmpty(LUVM.WFComments))
                //{
                //    LUVM.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + LUVM.WFComments;
                //}
                //LUVM.FileNames = FileNames;
                var WfName = System.Web.HttpContext.Current.Session["Workflow"] as string;
                //Trim the three comns suggested by JS
                LUVM.FirstName = LUVM.FirstName.Trim();
                LUVM.LoginEmail = LUVM.LoginEmail.Trim();
                if (LUVM.Id == 0)
                {
                    LUVM.LockoutUntil = DateTime.UtcNow;
                    LUVM.WFRequesterId = LoggedInUserId;
                    LUVM.WFStatus = "Saved";
                    LUVM.WFType = "LUsers";
                    LUVM.WFRequesterRoleId = LoggedInRoleId;
                    LUVM.CreatedById = LoggedInUserId;
                    LUVM.CreatedDateTime = DateTime.UtcNow;
                    LUVM.Version = 1;
                    LUVM.WFStatusDateTime = DateTime.UtcNow;
                    RestClient.Add(LUVM, null, RoleList, WfName);
                }
                else
                {
                    RestClient.Update(LUVM,null,RoleList, FormType,LoggedInUserId,LoggedInRoleId);
                }
                //return RedirectToAction("Index", "GenericGrid");
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string ErrorMessage = "";
                // ViewBag.LuReportsToId = GetReportsTo(LUVM.LuReportsToId);
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                    //redirect user to gneric error page
                    // return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        //return View(LUVM);
                        ErrorMessage = ex.Data["ErrorMessage"].ToString();
                        return Json(ErrorMessage, JsonRequestBehavior.AllowGet);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        //return Redirect(ex.Data["RedirectToUrl"] as string);
                        return Json(new { success = false, ErrorMessage = ex.Data["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                    case (int)ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        //return View(LUVM);
                        ErrorMessage = ex.Data["ErrorMessage"].ToString();
                        return Json(ErrorMessage, JsonRequestBehavior.AllowGet);
                    default:
                        return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
                        //throw ex;
                }
            }
        }

        //get roles list in user form
        [ControllerActionFilter]
        public JsonResult GetRoles(int TransactionId)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            ILRolesRestClient LRRC = new LRolesRestClient();
            if (TransactionId == 0)
            {
                var ApiData = LRRC.GetByCompanyCode(CompanyCode);
                return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var ApiData = LRRC.GetUserRolesForEditPage(CompanyCode,TransactionId);
                return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Terminate(string Status,int Id)
        {
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            RestClient.TerminateUser(Status, Id, LoggedInUserId);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Web.Mvc;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{

    [SessionExpire]
    [HandleCustomError]
    public class WStepsController : Controller
    {
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();
        //int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
        //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;

        IWStepRestClient RestClient = new WStepRestClient();

        // GET: WSteps
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetByWorkFlowId(int WorkFlowId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            var MaxStepOrdinal = RestClient.GetMaxOrdinal(WorkFlowId, CompanyCode);
            ViewBag.MaxStepOrdinal = MaxStepOrdinal;
            TempData["MaxStepOrdinal"] = MaxStepOrdinal;
            var ApiData = RestClient.GetByWFId(WorkFlowId, CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetStepsDropDown(int WorkFlowId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            var ApiData = RestClient.GetByWFIdForDropDown(WorkFlowId, CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        //[ControllerActionFilter]
        public JsonResult SaveData(object[] GridData,int WorkFlowId)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            try
            {
                IRWorkFlowsRestClient WFRC = new RWorkFlowsRestClient();
                var WfDetails =  WFRC.GetById(WorkFlowId);
                if (WfDetails == null)
                {
                    var OutputJson2 = new { ErrorMessage = "WorkFlow Not Found", PopupMessage = "", RedirectToUrl = "" };
                    return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                }
                if (GridData != null && GridData.Length > 0)
                {
                    foreach (string[] Data in GridData)
                    {
                        var GridArray = Data;
                        var model = new WStepsViewModel
                        {
                            Id = Convert.ToInt32(GridArray[0]),
                            Name = String.IsNullOrEmpty(GridArray[1]) ? null : GridArray[1],
                            Label = String.IsNullOrEmpty(GridArray[2]) ? null : GridArray[2],
                            Banner = String.IsNullOrEmpty(GridArray[3]) ? null : GridArray[3],
                            Skip = Convert.ToBoolean(GridArray[4]),
                            SkipFunctionName = String.IsNullOrEmpty(GridArray[5]) ? null : GridArray[5],
                            Description = String.IsNullOrEmpty(GridArray[6]) ? null : GridArray[6],
                            IsReady = Convert.ToBoolean(GridArray[7]),
                            DoNotNotify = Convert.ToBoolean(GridArray[8]),
                            WorkFlowId = WorkFlowId,
                            CompanyCode = CompanyCode,
                            Ordinal = Convert.ToInt32(GridArray[9]),
                        };
                        RestClient.Add(model, null);
                    }

                }
                var OutputJson = new { Success = "Create Steps", ErrorMessage="", PopupMessage = "", RedirectToUrl = "/RWorkFlows/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // If exception generated from Api redirect control to view page where actions will be taken as per error type.
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
                    TempData["Error"] = "Record could not be updated";
                    throw ex;
                }
            }
        }
        public JsonResult DeleteById(int Id,int WorkFlowId)
        {
            try
            {
                RestClient.Delete(Id, WorkFlowId, null);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "step deleted successfully", RedirectToUrl = "/RWorkFlows/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // If exception generated from Api redirect control to view page where actions will be taken as per error type.
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
                    TempData["Error"] = "Record could not be updated";
                    throw ex;
                }
            }
        }

        [HttpPost]
        public JsonResult SwapStepOrdinals(int WorkFlowId,int Ordinal1, int Ordinal2)
        {
            try
            {
                string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
                RestClient.SwapStepOrdinals(WorkFlowId, Ordinal1, Ordinal2, CompanyCode, null);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/RWorkFlows/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // If exception generated from Api redirect control to view page where actions will be taken as per error type.
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
                    TempData["Error"] = "Record could not be updated";
                    throw ex;
                }
            }
        }
    }
}
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
//using System.Collections.Generic;
using System.Data;
//using System.Linq;
//using System.Web;
using System.Web.Mvc;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{
    public class LNotificationController : Controller
    {
        DataTable dt = new DataTable();
        IWStepRestClient WStepRestClient = new WStepRestClient();
        ILRolesRestClient RolesRestClient = new LRolesRestClient();
        LNotificationRestClient RestClient = new LNotificationRestClient();
        
        // GET: LNotification
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Notifications";
            //ViewBag.Name = GetStepName(null);
            //ViewBag.OriginatingStepName = GetStepName(null);
            ViewBag.RoleName = GetRoleName(null);
            ViewBag.TemplateName = GetTemplateName(null);
            ViewBag.Workflow = GetWorkflowListByCompanyCode(null);
            return View();
        }

        //function that add system step names into dropdownlist
        [ControllerActionFilter]
        private SelectList GetStepName(int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //this code has to be updated to get the data from API.
            // var ApiData = RestClient.GetByCompanyCode(CompanyCode);
             var StepName = WStepRestClient.GetLandingStepsName(CompanyCode);
            //var StepName = WStepRestClient.GetAll();
            
             var x = new SelectList(StepName, "Id", "Name", Selected);
            return x;
        }

        //method to populate dropdown of rolename
         [ControllerActionFilter]
        private SelectList GetRoleName(int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //this code has to be updated to get the data from API.
            var ApiData = RolesRestClient.GetByCompanyCode(CompanyCode);
            var x = new SelectList(ApiData, "Id", "RoleName", Selected);
            return x;
        }

        // desc: method to populate dropdown of template
        [ControllerActionFilter]
        private SelectList GetTemplateName(int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //this code has to be updated to get the data from API.
            var ApiData = RestClient.GetEmailTemplateByCompanyCode(CompanyCode);
            var x = new SelectList(ApiData, "Id", "TemplateName", Selected);
            return x;
        }

        // desc: method to load data into grid
        public JsonResult GetLNotification()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var CompanySpecificData = RestClient.GetByCompanyCode(CompanyCode);
            return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);
        }

        // method to create new data and show in grid
        [HttpPost]
        public JsonResult Create(LNotificationViewModel lnvm, int tempid,int roleid, int stepid,int OriginatingStepId)
        {
            
            lnvm.LandingStepId =  stepid;
            lnvm.RecipientRoleId = roleid;
            lnvm.TemplateId = tempid;
            lnvm.OriginatingStepId = OriginatingStepId;
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            try
            {
                lnvm.CompanyCode = CompanyCode;
                RestClient.Add(lnvm, null);//the value for redirectUrl may be updated if required      
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Notification saved successfully", RedirectToUrl = "/LNotification/Index" };             
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    TempData["Error"] = ex.Data["ErrorMessage"] as string;
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page and because of this error message is left blank.
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/LNotification/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/LNotification/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Notification could not be created";
                    throw ex;
                }
            }
        }

        // method to update selected data
        [HttpPost]
        public JsonResult Update(LNotificationViewModel lnvm, int id, int tempid, int roleid, int stepid,int OriginatingStepId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            try
            {
                var ApiData = RestClient.GetById(id);

                ApiData.LandingStepId = stepid;
                ApiData.OriginatingStepId = OriginatingStepId;
                ApiData.RecipientRoleId = roleid;
                ApiData.TemplateId = tempid;
                ApiData.CompanyCode = CompanyCode;
                ApiData.Description = lnvm.Description;
                ApiData.RemindAfterDays = lnvm.RemindAfterDays;
                ApiData.Type = lnvm.Type;
                //ApiData.Description = Prodcat.Description;
                //ApiData.Name = Prodcat.Name;
                RestClient.Update(ApiData, null);//the value for redirectUrl may be updated if required              
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Notification updated successfully", RedirectToUrl = "/LNotification/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    TempData["Error"] = ex.Data["ErrorMessage"] as string;
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page and because of this error message is left blank.
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/LNotification/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/LNotification/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Notification could not be updated";
                    throw ex;
                }
            }
        }
        
        // Function to get the details of the data choosen for updation
        [HttpGet]
        public JsonResult Edit(int id)
        {
            var ApiData = RestClient.GetById(id);
            ViewBag.Name = GetStepName(ApiData.LandingStepId);
            ViewBag.OriginatingStepName = GetStepName(ApiData.LandingStepId);
            ViewBag.RoleName = GetRoleName(ApiData.RecipientRoleId);
            ViewBag.TemplateName = GetTemplateName(ApiData.TemplateId);

            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        // method to delete selected data
        public JsonResult Delete(int Id)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            try
            {
                RestClient.Delete(Id, null, CompanyCode);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Notification deleted successfully", RedirectToUrl = "" };
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
                    TempData["Error"] = "Notification could not be deleted";
                    throw ex;
                }
            }
            
        }


        public SelectList GetWorkflowListByCompanyCode(int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            IRWorkFlowsRestClient WFRC = new RWorkFlowsRestClient();
            var ApiData = WFRC.GetRWorkFlow(CompanyCode);
            var x = new SelectList(ApiData, "Id", "Name", Selected);
            return x;
            // return Json(TableNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStepsNameByWorkFlowId(int WorkFlowId)
        {

            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            IWStepRestClient WSRC = new WStepRestClient();
            var ApiData = WSRC.GetByWFIdForDropDown(WorkFlowId, CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        /*--------------------------------------------------Below code is commented by Rakhi Singh on 12/09/18--------------------------------------------*/

        //[HttpPost]
        //public JsonResult EditLNotification(object[] GridData)
        //{
        //    string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //    int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //    try
        //    {
        //        foreach (string[] Data in GridData)
        //        {
        //            var data = Data;
        //            var GridArray = data;
        //            var model = new LNotificationViewModel
        //            {
        //                Id = Convert.ToInt32(GridArray[0]),
        //                Type = string.IsNullOrEmpty(GridArray[1]) ? null : GridArray[1],
        //                Description = string.IsNullOrEmpty(GridArray[2]) ? null : GridArray[2],
        //                RemindAfterDays = Convert.ToInt32(GridArray[3]),
        //                LandingStepId = Convert.ToInt32(GridArray[4]),
        //                RecipientRoleId = Convert.ToInt32(GridArray[5]),
        //                TemplateId = Convert.ToInt32(GridArray[6])
        //                //Name = GridArray[1],


        //            };

        //            model.CompanyCode = CompanyCode;

        //            //model.CreatedById = LoggedInUserId;
        //            //model.UpdatedById = LoggedInUserId;
        //            //model.UpdatedDateTime = DateTime.Now;
        //            //model.CreatedDateTime = DateTime.Now;
        //            RestClient.Add(model, null);
        //        }
        //        var OutputJson = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/LNotification/Index" };
        //        return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        // If exception generated from Api redirect control to view page where actions will be taken as per error type.
        //        if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
        //        {
        //            switch ((int)ex.Data["ErrorCode"])
        //            {
        //                case (int)ExceptionType.Type1:
        //                    //redirect user to gneric error page
        //                    var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
        //                    return Json(OutputJson1, JsonRequestBehavior.AllowGet);
        //                case (int)ExceptionType.Type2:
        //                    //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
        //                    var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
        //                    return Json(OutputJson2, JsonRequestBehavior.AllowGet);
        //                case (int)ExceptionType.Type3:
        //                    //Send Ex.Message to the error page which will be displayed as popup
        //                    var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
        //                    return Json(OutputJson3, JsonRequestBehavior.AllowGet);
        //                case (int)ExceptionType.Type4:
        //                    var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "" };
        //                    return Json(OutputJson4, JsonRequestBehavior.AllowGet);
        //            }
        //            var OutputJson = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
        //            return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
        //            TempData["Error"] = "Record could not be updated";
        //            throw ex;
        //        }

        //    }
        //}

        //public JsonResult BindStepNameDropDown()
        //{
        //    string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //    var StepName = WStepRestClient.GetLandingStepsName(CompanyCode);
        //    return Json(StepName, JsonRequestBehavior.AllowGet);

        //}

        //public JsonResult BindRoleNameDropDown()
        //{
        //    string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //    var CompanySpecificData = RolesRestClient.GetByCompanyCode(CompanyCode);
        //    return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult BindTemplateNameDropDown()
        //{
        //    string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //    var CompanySpecificData = RestClient.GetEmailTemplateByCompanyCode(CompanyCode);
        //    return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);
        //}


    }
}
using RELY_APP.Helper;
using System;
using RELY_APP.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RELY_APP.Utilities.Globals;
using RELY_APP.Utilities;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class GAuthorizableObjectsController : Controller
    {
        GAuthorizableObjectsRestClient RestClient = new GAuthorizableObjectsRestClient();
        // GET: GAuthorizableObjects
        [ControllerActionFilter]
        public ActionResult Index()
        {

            System.Web.HttpContext.Current.Session["Title"] = "Manage AuthorizableObjects";
            ViewBag.Title = "Manage AuthorizableObjects";
            return View();
        }

        [ControllerActionFilter]
        public JsonResult GetAuthorizableObjects()
        {
            var ApiData = RestClient.GetAll();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ControllerActionFilter]
        [ControllerActionFilter]
        public JsonResult EditAuthorizableObjects(string[] GridData)
        {
            //Add Update Types
            try
            {
                RestClient.Add(GridData, null);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "AuthorizableObjects Updated Successfully", RedirectToUrl = "/GAuthorizableObjects/Index" };
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
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/GAuthorizableObjects/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/GAuthorizableObjects/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                    }
                    var OutputJson = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "Record Duplicate", RedirectToUrl = "/GAuthorizableObjects/Index" };
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

        [ControllerActionFilter]
        public JsonResult DeleteAuthorizableObjects(int Id)
        {
            try
            {
                RestClient.Delete(Id, null);
                var OutputJson = new { ErrorMessage = "AuthorizableObject Deleted ", PopupMessage = "AuthorizableObject Deleted Successfully", RedirectToUrl = "" };
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
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/GAuthorizableObjects/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/GAuthorizableObjects/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                    }
                    var OutputJson = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "Record Duplicate", RedirectToUrl = "/GAuthorizableObjects/Index" };
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
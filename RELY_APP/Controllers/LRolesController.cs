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
    public class LRolesController : Controller
    {
        ILRolesRestClient RestClient = new LRolesRestClient();      

        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Roles";
            ViewBag.Title = "Manage Roles";
            return View();
        }       
       
        public JsonResult GetRoles()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            var ApiData = RestClient.GetByCompanyCode(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Create(LRoleViewModel LRVM)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;            
            try
            {               
                LRVM.CompanyCode = CompanyCode;
                RestClient.Add(LRVM, null);

                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Role Created Successfully", RedirectToUrl = "/LRoles/Index" };

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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/LRoles/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/LRoles/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;                       
                    }                    
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Role could not be created";
                    throw ex;
                }
            }
        }

        //Below code is commented by Rakhi Singh on 27th july 2018 during code review by Vikas.
        //[ControllerActionFilter]
        //public JsonResult GetAll(string UserName, string WorkFlow)
        //{

        //    var ApiData = RestClient.GetAll();
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}

        //[ControllerActionFilter]
        ////Get:RProductSystems
        //public ActionResult Create()
        //{
        //    System.Web.HttpContext.Current.Session["Title"] = "Create Roles";
        //    return View();
        //}
        // POST: LRoles/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ControllerActionFilter]
        ////Post:Method Use to post data entered by user into db
        //public ActionResult Create(LRoleViewModel LRVM)
        //{
        //    string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //    try
        //    {
        //        LRVM.CompanyCode = CompanyCode;
        //        RestClient.Add(LRVM, null);//the value for redirectUrl may be updated if required
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewData["ErrorMessage"] = ex.Message;
        //        return View(LRVM);
        //    }
        //}

        // GET: LRoles/Edit
        //[ControllerActionFilter]
        //Get:Method use to Get Data Requested by User
        //[HttpGet]
        //[ControllerActionFilter]
        //public ActionResult Edit(String CompanyCode, string Name)
        //{
        //    System.Web.HttpContext.Current.Session["Title"] = "Edit Roles";
        //    LRoleViewModel LRVM = RestClient.GetByCode(CompanyCode, Name);
        //    if (LRVM == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(LRVM);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ControllerActionFilter]
        //// POST: LRoles/Edit
        ////Post:Method used to Edit data into db
        //public ActionResult Edit(LRoleViewModel LRVM)
        //{
        //    string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //    try
        //    {
        //        LRVM.CompanyCode = CompanyCode;
        //        RestClient.Update(LRVM, null);//the value for redirectUrl may be updated if required
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewData["ErrorMessage"] = ex.Message;
        //        return View(LRVM);
        //    }
        //}
        

        //[ControllerActionFilter]
        //public JsonResult DeleteRoles(int Id)
        //{
        //    try
        //    {
        //        RestClient.Delete(Id, null);
        //        var OutputJson = new { ErrorMessage = "Roles  Deleted ", PopupMessage = "Role Deleted Successfully", RedirectToUrl = "" };
        //        return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        //If exception generated from Api redirect control to view page where actions will be taken as per error type.
        //        if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
        //        {
        //            TempData["Error"] = ex.Data["ErrorMessage"] as string;
        //            switch ((int)ex.Data["ErrorCode"])
        //            {
        //                case (int)ExceptionType.Type1:
        //                    //redirect user to gneric error page
        //                    var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
        //                    return Json(OutputJson1, JsonRequestBehavior.AllowGet);
        //                case (int)ExceptionType.Type2:
        //                    //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
        //                    var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/LRoles/Index" };
        //                    return Json(OutputJson2, JsonRequestBehavior.AllowGet);
        //                case (int)ExceptionType.Type3:
        //                    //Send Ex.Message to the error page which will be displayed as popup
        //                    var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
        //                    return Json(OutputJson3, JsonRequestBehavior.AllowGet);
        //                case (int)ExceptionType.Type4:
        //                    var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/LRoles/Index" };
        //                    return Json(OutputJson4, JsonRequestBehavior.AllowGet);
        //            }
        //            var OutputJson = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "Record Duplicate", RedirectToUrl = "/LRoles/Index" };
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
        //public JsonResult GetByCompanyCode()
        //{

        //    var ApiData = RestClient.GetByCompanyCode();
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}
    }
}
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
    public class RSysCatController : Controller
    {
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        IRSysCatRestClient RestClient = new RSysCatRestClient();

        IRProductSystemsRestClient ProductSysRestClient = new RProductSystemsRestClient();
        IRProductCategoriesRestClient ProdCatRestClient = new RProductCategoriesRestClient();
        // GET: RSysCat
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] =  "Manage SysCat";
            ViewBag.Title = "Manage SysCat";
            ViewBag.System = GetSysName(null);
            ViewBag.Category = GetCatName(null);           
            return View();
        }

        [ControllerActionFilter]
        public JsonResult GetRSysCat()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var CompanySpecificData = RestClient.GetByCompanyCode(CompanyCode);

            return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);
        }

        //function that add system names into dropdownlist
        [ControllerActionFilter]
        private SelectList GetSysName(int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //this code has to be updated to get the data from API.
            var ApiData = ProductSysRestClient.GetByCompanyCode(CompanyCode);
            var x = new SelectList(ApiData, "Id", "Name", Selected);
            return x;
        }
       
        //function that add category names into dropdownlist
        [ControllerActionFilter]
        private SelectList GetCatName(int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //this code has to be updated to get the data from API.
            var ApiData = ProdCatRestClient.GetByCompanyCode(CompanyCode);
            var x = new SelectList(ApiData, "Id", "Name", Selected);
            return x;
        }

        // function to create syscat and load in grid
        [HttpPost]
        //[ControllerActionFilter]
        public JsonResult Create(RSysCatViewModel rsyscat, int sysid, int catid)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);           
            try
            {               
                rsyscat.CompanyCode = CompanyCode;
                rsyscat.CategoryId = catid;
                rsyscat.SystemId = sysid;
                RestClient.Add(rsyscat, null);
               
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "SysCat Created Successfully", RedirectToUrl = "/RSysCat/Index" };
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

        // method to delete selected syscat
        //[ControllerActionFilter]
        public JsonResult Delete(int Id)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            try
            {
                RestClient.Delete(Id, null, CompanyCode);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "SysCat Deleted Successfully", RedirectToUrl = "/RSysCat/Index" };
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




        /*----------------------------Below code is commented by Rakhi Singh as per requirement on 12/09/18-------------------------------------------*/
        //[ControllerActionFilter]
        //public JsonResult BindProductCategoryDropdown()
        //{
        //    string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        //    var CompanySpecificData = ProdCatRestClient.GetByCompanyCode(CompanyCode);

        //    return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);

        //}

        //[ControllerActionFilter]
        //public JsonResult BindProductSystemDropdown()
        //{
        //    string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        //    var CompanySpecificData = ProductSysRestClient.GetByCompanyCode(CompanyCode);

        //    return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);

        //}
        //[HttpPost]
        //[ControllerActionFilter]
        //public JsonResult EditRSysCat(object[] GridData)
        //{
        //    string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        //    //Add Update Types
        //    try
        //    {
        //        foreach (string[] Data in GridData)
        //        {
        //            var data = Data;
        //            var GridArray = data;
        //            var model = new RSysCatViewModel
        //            {
        //                Id = Convert.ToInt32(GridArray[0]),
        //                SystemId = Convert.ToInt32(GridArray[1]),
        //                CategoryId = Convert.ToInt32(GridArray[2]),
        //                SysCat=GridArray[3],
        //                CompanyCode = CompanyCode,
        //                SysCatCode = GridArray[4]
        //            };

        //            RestClient.Add(model, null);
        //        }

        //        //var OutputJson = new { ErrorMessage = "Create Types", PopupMessage = "", RedirectToUrl = "/RSysCat/Index" };
        //        //return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //        var OutputJson = new { Success = "Create Types", ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/RSysCat/Index" };
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
    }
}
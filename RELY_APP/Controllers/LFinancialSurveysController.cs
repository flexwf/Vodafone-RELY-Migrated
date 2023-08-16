using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
//using System.Collections.Generic;
//using System.Configuration;
using System.Data;
//using System.Data.OleDb;
//using System.Linq;
//using System.Reflection;
//using System.Web;
using System.Web.Mvc;
//using System.Web.UI.WebControls;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LFinancialSurveysController : Controller
    {
        DataTable dt = new DataTable();
        //int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();

        IRLFSSurveyLevelRestClient LFSSurveyLevelRestClient = new LFSSurveyLevelRestClient();
        ILFinancialSurveysRestClient RestClient = new LFinancialSurveysRestClient();
        // GET: LFinancialSurveys
        [CustomAuthorize]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Financial Survey";
            ViewBag.Title = "Manage Financial Survey";
            ViewBag.Name = GetSurveyLevelNameDropdown(null);
            return View();
        }

        //function that add financial survey names into dropdownlist
        [ControllerActionFilter]
        private SelectList GetSurveyLevelNameDropdown(int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //this code has to be updated to get the data from API.           
            var SurveyName = LFSSurveyLevelRestClient.GetByCompanyCode(CompanyCode);          
            var x = new SelectList(SurveyName, "Id", "Name", Selected);
            return x;
        }
        
        //function to show data on grid
        [CustomAuthorize]
        public JsonResult GetLFinancialSurvey()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var CompanySpecificData = RestClient.GetByCompanyCode(CompanyCode);
            return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);
        }

        //Function to create the new data
        [HttpPost]
        public JsonResult Create(LFinancialSurveysViewModel LFSVM, int surveylevelid)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            try
            {
                LFSVM.CompanyCode = CompanyCode;
                LFSVM.CreatedById = LoggedInUserId;
                LFSVM.UpdatedById = LoggedInUserId;
                LFSVM.UpdatedDateTime = DateTime.Now;
                LFSVM.CreatedDateTime = DateTime.Now;
                LFSVM.SurveyLevelId = surveylevelid;
                RestClient.Add(LFSVM, null);//the value for redirectUrl may be updated if required              
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Financial Survey saved successfully", RedirectToUrl = "/LFinancialSurvey/Index" };

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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/LFinancialSurvey/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/LFinancialSurvey/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Financial Survey could not be saved";
                    throw ex;
                }
            }
        }


        //Function to get the details of the data by id to edit 
        [HttpGet]
        public JsonResult Edit(int id)
        {
            var ApiData = RestClient.GetById(id);
            ViewBag.Name = GetSurveyLevelNameDropdown(ApiData.SurveyLevelId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //method to delete the data by id
        [CustomAuthorize]
        public JsonResult Delete(int Id)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            try
            {
                RestClient.Delete(Id, null, CompanyCode);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Financial Survey deleted successfully", RedirectToUrl = "" };
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
                    TempData["Error"] = "Financial Survey could not be deleted";
                    throw ex;
                }
            }
        }


        // method to update selected data
        [HttpPost]
        public JsonResult Update(LFinancialSurveysViewModel LFSVM, int id, int surveylevelid)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            try
            {
                var ApiData = RestClient.GetById(id);
                ApiData.SurveyLevelId = surveylevelid;              
                ApiData.CompanyCode = CompanyCode;
                ApiData.Description = LFSVM.Description;
                ApiData.SurveyName = LFSVM.SurveyName;
                ApiData.IsActive = LFSVM.IsActive;
                ApiData.CreatedById = LoggedInUserId;
                ApiData.UpdatedById = LoggedInUserId;
                ApiData.UpdatedDateTime = DateTime.Now;
                ApiData.CreatedDateTime = DateTime.Now;

                RestClient.Update(ApiData, null);//the value for redirectUrl may be updated if required              
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Financial survey updated successfully", RedirectToUrl = "/LNotification/Index" };
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
                    TempData["Error"] = "Financial survey could not be updated";
                    throw ex;
                }
            }
        }

        /*----------------------Below method is commented by Rakhi Singh as per requirement------------------------------------------*/
        //Create
        //[HttpPost]
        //[CustomAuthorize]
        //public JsonResult EditLFinancialSurvey(object[] GridData)
        //{
        //    string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //    int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //    try
        //    {
        //        foreach (string[] Data in GridData)
        //        {
        //            var data = Data;
        //            var GridArray = data;
        //            var model = new LFinancialSurveysViewModel
        //            {
        //                Id = Convert.ToInt32(GridArray[0]),
        //                SurveyLevelId = Convert.ToInt32(GridArray[1]),
        //                //Name = GridArray[1],
        //                SurveyName = GridArray[2],
        //                Description = GridArray[3],
        //                IsActive = Convert.ToBoolean(GridArray[4])
        //            };

        //            model.CompanyCode = CompanyCode;
        //            model.CreatedById = LoggedInUserId;
        //            model.UpdatedById = LoggedInUserId;
        //            model.UpdatedDateTime = DateTime.Now;
        //            model.CreatedDateTime = DateTime.Now;
        //            RestClient.Add(model, null);
        //        }
        //        //var OutputJson = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/LFinancialSurvey/Index" };
        //        //return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //        var OutputJson = new { Success = "Success", ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/LFinancialSurvey/Index" };
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

        //[CustomAuthorize]
        //public JsonResult BindSurveyLevelNameDropDown()
        //{
        //    string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //    var CompanySpecificData = LFSSurveyLevelRestClient.GetByCompanyCode(CompanyCode);

        //    return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);

        //}


    }
}
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
    public class LReferenceTypesController : Controller
    {
        ILReferenceTypesRestClient RTRestClient = new LReferenceTypesRestClient();
        
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Reference Types";
            ViewBag.Title = "Manage Reference Types";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult Create()
        {
            ViewBag.Title = "Manage Reference Types";
            System.Web.HttpContext.Current.Session["Title"] = "Create Reference Types";
            return View();
        }
        
        /// <summary>
        /// Function to get all the Reference Types.
        /// </summary>
        /// <returns></returns>
        [ControllerActionFilter]
        public JsonResult GetReferenceTypes()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var CompanySpecificData = RTRestClient.GetByCompanyCode(CompanyCode);
            return Json(CompanySpecificData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created by: Rakhi Singh on 30th july 2018
        /// Desc: Following method is used to add new LReferenceTypes.
        /// </summary>
        /// <param name="LRT"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Create(LReferenceTypesViewModel LRT)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            try
            {
                LRT.CompanyCode = CompanyCode;
                LRT.CreatedById = LoggedInUserId;
                LRT.UpdatedById = LoggedInUserId;
                LRT.CreatedDateTime = System.DateTime.Now;
                LRT.UpdatedDateTime = System.DateTime.Now;
                RTRestClient.Add(LRT, null);//the value for redirectUrl may be updated if required              
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "ReferenceType saved successfully", RedirectToUrl = "/LReferenceTypes/Index" };

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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/LReferenceTypes/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/LReferenceTypes/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Reference Type could not be saved";
                    throw ex;
                }
            }
        }
        /// <summary>
        /// Created by: Rakhi Singh on 31th july 2018
        /// Desc: Following method is used to delete LReferenceTypes.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult Delete(int Id)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            try
            {
                RTRestClient.Delete(Id, null, CompanyCode);               
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Reference Type Deleted Successfully", RedirectToUrl = "" };
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
                            var OutputJson1 = new { ErrorMessage = ex.Data["ErrorMessage"].ToString(), PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
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
                    var OutputJson = new { ErrorMessage = "Record could not be updated", PopupMessage = "", RedirectToUrl = "" };
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Reference Type could not be deleted";
                    //throw ex;
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// Created by Rakhi Singh on 31st july
        /// Description: this function is used to update the LReferenceTypes on the basis of it's id.
        /// </summary>
        /// <param name="LRT"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update(LReferenceTypesViewModel LRT, int id)
        {
            try
            {
                var ApiData = RTRestClient.GetByReferenceTypeId(id);
                ApiData.Name = LRT.Name;               
                RTRestClient.Update(ApiData, null);//the value for redirectUrl may be updated if required              
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "ReferenceType updated successfully", RedirectToUrl = "/LReferenceTypes/Index" };
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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/LReferenceTypes/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/LReferenceTypes/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "ReferenceTypes could not be updated";
                    throw ex;
                }
            }
        }
        /// <summary>
        /// Created By: Rakhi Singh
        /// Date: 31/07/18
        /// Function to get the details of the data choosen for updation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Edit(int id)
        {
            var ApiData = RTRestClient.GetByReferenceTypeId(id);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /*Below method is commented by Rakhi Singh on 31st july as per requirement*/
        //[HttpPost]
        //[ControllerActionFilter]       
        //public JsonResult EditReferenceTypes(object[] GridData)
        //{
        //    string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //    int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());

        //    //Add Update Types
        //    try
        //    {
        //        foreach (string[] Data in GridData)
        //        {
        //            var data = Data;
        //            var GridArray = data;
        //            var model = new LReferenceTypesViewModel
        //            {
        //                Id = Convert.ToInt32(GridArray[0]),
        //                Name = GridArray[1],
        //                //IsEffectiveDated = Convert.ToBoolean(GridArray[2]),
        //                IsEffectiveDated = false,
        //                CompanyCode = CompanyCode,
        //                CreatedById = LoggedInUserId,
        //                UpdatedById = LoggedInUserId,
        //                CreatedDateTime = System.DateTime.Now,
        //                UpdatedDateTime = System.DateTime.Now
        //            };
        //            RTRestClient.Add(model, null);
        //        }
        //        //if (model.Id == 0)//entry does not exist in database
        //        //{
        //        //    RestClient.Add(model, null);
        //        //}
        //        //else
        //        //{
        //        //    RestClient.Update(model, null);
        //        //}
        //        //var OutputJson = new { ErrorMessage = "Create Types", PopupMessage = "", RedirectToUrl = "/LReferenceTypes/Index" };
        //        //return Json(OutputJson, JsonRequestBehavior.AllowGet);

        //        var OutputJson = new { Success = "Create Types", ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/LReferenceTypes/Index" };
        //        return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //       // If exception generated from Api redirect control to view page where actions will be taken as per error type.
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
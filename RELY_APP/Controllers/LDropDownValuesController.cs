using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Linq;
using System.Web.Mvc;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LDropDownValuesController : Controller
    {
        ILDropDownValuesRestClient RestClient = new LDropDownValuesRestClient();       
        String DropdownName = "";
        int DropDownId = 0;

       // [ControllerActionFilter]
        public ActionResult Index(int Id, string DropDownName)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Dropdown Values For " + DropDownName;
            ViewBag.Title = "DropDown Values ( " + DropDownName + " )";
            DropDownId = Id;
            DropdownName = DropDownName;
            ViewBag.DropDownName = DropDownName;
            ViewBag.DropDownId = Id;
            return View();
        }


        //Get List of DropdownValues for that DropDownId
        // [ControllerActionFilter]
        [ControllerActionFilter]
        public JsonResult GetDropDownValues(int DropDownId)
        {
            var ApiData = RestClient.GetByDropDownId(DropDownId).ToList();
            var LastEmptyRow = new LDropDownValueViewModel { };
            // ApiData.Add(LastEmptyRow);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //GetvalueByDropDownId


        /// <summary>
        /// Created by: Rakhi Singh on 7th aug 2018
        /// Desc: Following method is used to create new LDropdown value
        /// </summary>
        /// <param name="LDVM"></param>
        /// <returns></returns>
        [HttpPost]
      //  [ValidateAntiForgeryToken]
        public JsonResult Create(LDropDownValueViewModel LDVM, int DropDownId)
        {            
            try
            {
                LDVM.DropDownId = DropDownId;
                RestClient.Add(LDVM, null);//the value for redirectUrl may be updated if required              
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "DropDown value saved successfully", RedirectToUrl = "/LDropDownValues/Index?Id=" + DropDownId + "&DropDownName=" + DropdownName };
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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = ""};
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/LDropDownValues/Index?Id=" + DropDownId + "&DropDownName=" + DropdownName };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "DropDown value could not be created";
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Created by Rakhi Singh on 7th aug 2018
        /// Description: this function is used to update the LDropdownvalue on the basis of it's id.
        /// </summary>
        /// <param name="LDVM"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update(LDropDownValueViewModel LDVM, int id)
        {
            try
            {
                var ApiData = RestClient.GetById(id);
                ApiData.Description = LDVM.Description;
                ApiData.Value = LDVM.Value;
                RestClient.Update(ApiData, null);//the value for redirectUrl may be updated if required              
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "DropDown value updated successfully", RedirectToUrl = "/LDropDownValues/Index?Id=" + DropDownId + "&DropDownName=" + DropdownName };
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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/LDropDownValues/Index?Id=" + DropDownId + "&DropDownName=" + DropdownName };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/LDropDownValues/Index?Id=" + DropDownId + "&DropDownName=" + DropdownName };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "DropDown value could not be updated";
                    throw ex;
                }
            }
        }


       // [ControllerActionFilter]
        public JsonResult Delete(int Id, int DropdownId)
        {
            try
            {
                RestClient.Delete(Id, null, DropdownId);
                //var OutputJson = new { ErrorMessage = "Create DropDown Value", PopupMessage = "", RedirectToUrl = "" };
                //return Json(OutputJson, JsonRequestBehavior.AllowGet);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "DropDown value deleted successfully", RedirectToUrl = "/LDropDownValues/Index?Id=" + DropDownId + "&DropDownName=" + DropdownName };
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
                    TempData["Error"] = "DropDown value could not be updated";
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Created By: Rakhi Singh
        /// Date: 07/08/18
        /// Function to get the details of the data choosen for updation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Edit(int Id)
        {
           // var ApiData = RestClient.GetValuebyDDid(Id,DropDownId);
           // GetById

            var ApiData = RestClient.GetById(Id);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /*Below code is commented b Rakhi Singh on 7th august 2018 as per requirement:-*/
        /*
          [ControllerActionFilter]
        public JsonResult EditDropDownValues(object[] GridData, int DropDownId)
        {
            try
            {
                foreach (string Data in GridData)
                {
                    var GridArray = Data.Split(',');
                    var model = new LDropDownValueViewModel { Id = Convert.ToInt32(GridArray[0]), Value = GridArray[1], Description = GridArray[2], DropDownId = DropDownId };
                    RestClient.Add(model, null);
                }
                //if (model.Id == 0)//entry does not exist in database
                //{
                //    RestClient.Add(model,null);
                //}
                //else
                //{
                //    RestClient.Update(model,null);
                //}
                //var OutputJson = new { ErrorMessage = "Create DropDown Value", PopupMessage = "", RedirectToUrl = "/LDropDownValues/Index?Id=" + DropDownId + "&DropDownName=" + DropdownName };
                //return Json(OutputJson, JsonRequestBehavior.AllowGet);
                var OutputJson = new { Success = "Success", ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/LDropDownValues/Index?Id=" + DropDownId + "&DropDownName=" + DropdownName };
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
                    TempData["Error"] = "Record could not be updated";
                    throw ex;
                }
            }
        }
         */

    }
}
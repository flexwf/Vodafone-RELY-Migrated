using RELY_APP.Helper;
using System;
using RELY_APP.ViewModel;
using System.Web.Mvc;
using static RELY_APP.Utilities.Globals;
using RELY_APP.Utilities;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class RProductSystemsController : Controller
    {       
        IRProductSystemsRestClient RestClient = new RProductSystemsRestClient();

        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Product Systems";
            ViewBag.Title = "Manage Product Systems";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Product Systems";
            return View();
        }

        [ControllerActionFilter]       
        public JsonResult GetProductSystems()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var ApiData = RestClient.GetByCompanyCode(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created by: Rakhi Singh on 30th july 2018
        /// Desc: Following method is used to create product system as per details send from popup fields.
        /// </summary>
        /// <param name="Prodsys"></param>
        /// <returns></returns>
        [HttpPost]
       // [ControllerActionFilter]
        public JsonResult Create(RProductSystemsViewModel Prodsys)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            try
            {
                Prodsys.CompanyCode = CompanyCode;
                RestClient.Add(Prodsys, null);//the value for redirectUrl may be updated if required              
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Product System saved successfully", RedirectToUrl = "/RProductSystems/Index" };

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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/RProductSystems/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/RProductSystems/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Product system could not be created";
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Created by Rakhi Singh on 31st july
        /// Description: this function is used to update the Product system on the basis of it's id.
        /// </summary>
        /// <param name="LPOB"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update(RProductSystemsViewModel Prodsys, int id)
        {
            try
            {
                var ApiData = RestClient.GetById(id);
                ApiData.Description = Prodsys.Description;
                ApiData.Name = Prodsys.Name;
                RestClient.Update(ApiData, null);//the value for redirectUrl may be updated if required              
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Product system updated successfully", RedirectToUrl = "/RProductSystems/Index" };
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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/RProductSystems/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/RProductSystems/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Product system could not be updated";
                    throw ex;
                }
            }
        }


        /// <summary>
        /// Created by Rakhi Singh on 31st july
        /// Description: this function is used to delete the Product system on the basis of it's id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult Delete(int Id)
        {
            //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            try
            {
                RestClient.Delete(Id, null);    
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Product system deleted successfully", RedirectToUrl = "" };
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
                    var OutputJson = new { ErrorMessage = "Product system could not be deleted", PopupMessage = "", RedirectToUrl = "" };
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Product system could not be deleted";
                    //throw ex;
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// Created By: Rakhi Singh
        /// Date: 24/07/18
        /// Function to get the details of the data choosen for updation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Edit(int id)
        {
            var ApiData = RestClient.GetById(id);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /*Below code is commented by Rakhi on 30 july 2018*/
        // POST: RProductSystems/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ControllerActionFilter]
        ////Post:Method Use to post data entered by user into db
        //public ActionResult Create(RProductSystemsViewModel RPSVM)
        //{
        //    string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        //    try
        //    {
        //        RPSVM.CompanyCode = CompanyCode;
        //        RestClient.Add(RPSVM, null);//the value for redirectUrl may be updated if required
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewData["ErrorMessage"] = ex.Message;
        //        return View(RPSVM);
        //    }
        //}
        // GET: RProductSystems/Edit
        //[ControllerActionFilter]
        //Get:Method use to Get Data Requested by User
        /* [HttpGet]
         public ActionResult Edit(String CompanyCode, string Name)
         {
             System.Web.HttpContext.Current.Session["Title"] = "Edit Product Systems";
             RProductSystemsViewModel RPSVM = RestClient.GetByCode(CompanyCode, Name);
             if (RPSVM == null)
             {
                 return HttpNotFound();
             }
             return View(RPSVM);
         }
         */
        /* [HttpPost]
         [ValidateAntiForgeryToken]
         // [ControllerActionFilter]
         // POST: RProductSystems/Edit
         //Post:Method used to Edit data into db
         public ActionResult Edit(RProductSystemsViewModel RPSVM)
         {
             try
             {
                 RPSVM.CompanyCode = CompanyCode;
                 RestClient.Update(RPSVM, null);//the value for redirectUrl may be updated if required
                 return RedirectToAction("Index");
             }
             catch (Exception ex)
             {
                 ViewData["ErrorMessage"] = ex.Message;
                 return View(RPSVM);
             }
         }*/
        //[ControllerActionFilter]
        //public ActionResult Delete(string CompanyCode,String Name)
        //{
        //    System.Web.HttpContext.Current.Session["Title"] = "Delete Systems";
        //    RProductSystemsViewModel RPSVM = RestClient.GetByCode(CompanyCode,Name);
        //    if (RPSVM == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(RPSVM);
        //}

        //// POST: RSystems/Delete/DE
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        ////[ControllerActionFilter]
        //public ActionResult DeleteConfirmed(RProductSystemsViewModel RPSVM)
        //{
        //    try
        //    {
        //        RestClient.Delete(RPSVM.CompanyCode);
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewData["ErrorMessage"] = ex.Message;
        //        return View(RPSVM);
        //    }
        //}




        //[HttpPost]
        //[ControllerActionFilter]
        //public JsonResult EditProductSystems(object[] GridData)
        //{
        //    string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        //    //Add Update Types
        //    try
        //    {
        //        foreach (string[] Data in GridData)
        //        {
        //            var data = Data;
        //            var GridArray = data;
        //            var model = new RProductSystemsViewModel
        //            {
        //                Id = Convert.ToInt32(GridArray[0]),
        //                Name = GridArray[1],
        //                Description = GridArray[2],
        //                CompanyCode = CompanyCode
        //            };
        //            RestClient.Add(model, null);
        //        }
        //        //if (model.Id == 0)//entry does not exist in database
        //        //{
        //        //    RestClient.Add(model, null);
        //        //}
        //        //else
        //        //{
        //        //    RestClient.Update(model, null);
        //        //}
        //        var OutputJson = new { Success = "Success", ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/RProductSystems/Index" };
        //        return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        //If exception generated from Api redirect control to view page where actions will be taken as per error type.
        //        if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
        //        {
        //            switch ((int)ex.Data["ErrorCode"])
        //            {
        //                case (int)ExceptionType.Type1:
        //                    //redirect user to gneric error page
        //                    var OutputJson1 = new { ErrorMessage = ex.Data["ErrorMessage"].ToString(), PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
        //                    ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
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
        //            var OutputJson = new { ErrorMessage = "Record could not be updated", PopupMessage = "", RedirectToUrl = "" };
        //            //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
        //            TempData["Error"] = "Record could not be updated";
        //            //throw ex;
        //            return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}

        //[ControllerActionFilter]
        //public JsonResult DeleteProductSystems(int Id)
        //{
        //    string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        //    try
        //    {
        //        RestClient.Delete(Id, null, CompanyCode);
        //        var OutputJson = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = "" };
        //        return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        //If exception generated from Api redirect control to view page where actions will be taken as per error type.
        //        if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
        //        {
        //            switch ((int)ex.Data["ErrorCode"])
        //            {
        //                case (int)ExceptionType.Type1:
        //                    //redirect user to gneric error page
        //                    var OutputJson1 = new { ErrorMessage = ex.Data["ErrorMessage"].ToString(), PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
        //                    ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
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
        //            var OutputJson = new { ErrorMessage = "Record could not be updated", PopupMessage = "", RedirectToUrl = "" };
        //            //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
        //            TempData["Error"] = "Record could not be updated";
        //            //throw ex;
        //            return Json(OutputJson, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}
    }
}
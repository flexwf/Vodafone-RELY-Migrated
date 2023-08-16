using RELY_APP.Helper;
using System;
using RELY_APP.ViewModel;
using System.Web.Mvc;
using System.Linq;
using RELY_APP.Utilities;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{    //Code will not work if we uncomment these two.
    //Uncommenting for Exception handling,
    [SessionExpire]
    [HandleCustomError]
    public class RProductCategoriesController : Controller
    {        
        IRProductCategoriesRestClient RestClient = new RProductCategoriesRestClient();

        //Code will not work if we uncomment filter without giving entry in GAuthorizableObjects
        // [ControllerActionFilter]
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Product Categories";
            ViewBag.Title = "Manage Product Categories";
            return View();
        }

        [ControllerActionFilter]
        // Get:RProductCategories
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Product Category";
            return View();
        }
        [ControllerActionFilter]      
        public JsonResult GetProductCategories()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var ApiData = RestClient.GetByCompanyCode(CompanyCode).ToArray();            
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created by Rakhi Singh on 30 july 2018
        /// This method is used to create product categories on the basis of details passed in the pop up fields.
        /// </summary>
        /// <param name="Prodcat"></param>
        /// <returns></returns>
        [HttpPost]
       // [ControllerActionFilter]
        public JsonResult Create(RProductCategoriesViewModel Prodcat)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            try
            {
                Prodcat.CompanyCode = CompanyCode;
                RestClient.Add(Prodcat, null);//the value for redirectUrl may be updated if required              
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Product Category saved successfully", RedirectToUrl = "/RProductCategories/Index" };
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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/RProductCategories/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/RProductCategories/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Product Category could not be created";
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Created by Rakhi Singh on 31st july
        /// Description: this function is used to update the Productcat on the basis of it's id.
        /// </summary>
        /// <param name="LPOB"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update(RProductCategoriesViewModel Prodcat, int id)
        {
            try
            {
                var ApiData = RestClient.GetById(id);
                ApiData.Description = Prodcat.Description;
                ApiData.Name = Prodcat.Name;
                RestClient.Update(ApiData, null);//the value for redirectUrl may be updated if required              
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Product category updated successfully", RedirectToUrl = "/RProductCategories/Index" };
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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/RProductCategories/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/RProductCategories/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Product category could not be updated";
                    throw ex;
                }
            }
        }


        /// <summary>
        /// Created by Rakhi Singh on 31st july
        /// Description: this function is used to delete the Product cat. on the basis of it's id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult Delete(int Id)
        {
           // string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            try
            {
                RestClient.Delete(Id, null);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Product category deleted successfully", RedirectToUrl = "" };
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
                    var OutputJson = new { ErrorMessage = "Product category could not be deleted", PopupMessage = "", RedirectToUrl = "" };
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Product category could not be deleted";
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

        /*Below code is commented by Rakhi Singh on 30th july 2018 as per requirement....................................starts*/
        // POST: RProductCategories/Create
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // [ControllerActionFilter]
        // Post:Method Use to post data entered by user into db
        //public ActionResult Create(RProductCategoriesViewModel RPCVM)
        //{
        //    try
        //    {
        //        RPCVM.CompanyCode = CompanyCode;
        //        RestClient.Add(RPCVM);
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewData["ErrorMessage"] = ex.Message;
        //        return View(RPCVM);
        //    }
        //}
        // GET: RProductCategories/Edit
        //[ControllerActionFilter]
        //Get:Method use to Get Data Requested by User
        //[HttpGet]
        //public ActionResult Edit(String CompanyCode, string Name)
        //{
        //    System.Web.HttpContext.Current.Session["Title"] = "Edit Product Category";
        //    RProductCategoriesViewModel RPCVM = RestClient.GetByCode(CompanyCode, Name);
        //    if (RPCVM == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(RPCVM);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //// [ControllerActionFilter]
        //// POST: RSystems/Edit
        ////Post:Method used to Edit data into db
        //public ActionResult Edit(RProductCategoriesViewModel RPCVM)
        //{
        //    try
        //    {
        //        RPCVM.CompanyCode = CompanyCode;
        //        RestClient.Update(RPCVM);
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewData["ErrorMessage"] = ex.Message;
        //        return View(RPCVM);
        //    }
        //}
        //Method to Update and Add new product Categories
        //[ControllerActionFilter]
        //public JsonResult EditProductCategories(RProductCategoriesViewModel model)
        //{
        //    try
        //    {
        //        if (model.Id==0)//entry does not exist in database
        //        {
        //            RestClient.Add(model,null);//the value for redirectUrl may be updated if required
        //        }
        //        else
        //        {
        //            RestClient.Update(model,null);//the value for redirectUrl may be updated if required
        //        }
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
        //Method to delete Product Categories
        //public JsonResult DeleteProductCategories(string CompanyCode, string Name)
        // {
        //     try
        //     {
        //         RestClient.Delete(CompanyCode,Name,null);//the value for redirectUrl may be updated if required
        //         return Json(string.Empty, JsonRequestBehavior.AllowGet);
        //     }
        //     catch (Exception ex)
        //     {
        //         return Json(ex.Data["ErrorMessage"], JsonRequestBehavior.AllowGet);
        //     }
        // }         

        //Folowing Edit and Delete Methods are added by Vijay
        //[HttpPost]
        //[ControllerActionFilter]
        //public JsonResult EditProdCat(object[] GridData)
        //{
        //    string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        //    //Add Update Types
        //    try
        //    {
        //        foreach (string[] Data in GridData)
        //        {
        //            var data = Data;
        //            var GridArray = data;
        //            int Id = 0;
        //            string name = "";
        //            string Desc = "";
        //            if (!String.IsNullOrEmpty(GridArray[0]))
        //                Id = Convert.ToInt32(GridArray[0]);
        //            if (!String.IsNullOrEmpty(GridArray[1]))
        //                name = GridArray[1];
        //            if (!String.IsNullOrEmpty(GridArray[2]))
        //                Desc = GridArray[2];

        //            var model = new RProductCategoriesViewModel
        //            {
        //                Id = Id,
        //                Name = name,
        //                Description = Desc,
        //                CompanyCode = CompanyCode
        //            };
        //            RestClient.Add(model, null);
        //        }

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
        //public JsonResult DeleteProdCat(int Id)
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
        /*Above code is commented by Rakhi Singh on 30th july 2018 as per requirement....................................ends*/
    }
}
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{
    public class GKeyValuesController : Controller
    {

        IGKeyValuesRestClient RestClient = new GKeyValuesRestClient();
        IGCompaniesRestClient GCompanyData = new GCompaniesRestClient();

        // GET: GKeyValues
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(GKeyValueViewModel GKVM, int compid)
        {
            try
            {
                var companycode = GCompanyData.GetCompanyCodeByCompanyId(compid, null);
                companycode = companycode.Replace('"', ' ').Trim();
                GKVM.CompanyCode = companycode;
                RestClient.Add(GKVM, null);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Key created successfully", RedirectToUrl = "/Home/L2AdminDashboard" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);              
            }
            catch (Exception ex)
            {
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

        [HttpGet]
        public JsonResult Edit(int id)
        {
            var ApiData = RestClient.GetById(id);
            var companyinfo = GCompanyData.GetByComapnyCode(ApiData.CompanyCode);
            ApiData.CompanyId = companyinfo.Id;
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Created by Rakhi Singh
        /// Method to update the selected Config and save in db and display on grid the updated value
        /// </summary>
       // [HttpPost]
        public JsonResult Update(GKeyValueViewModel GKVM, int id, int companyid)
        {
            try
            {
                var ApiData = RestClient.GetById(id);
               // ApiData.CompanyId = companyid;
                var companycode = GCompanyData.GetCompanyCodeByCompanyId(companyid,null);
                companycode = companycode.Replace('"', ' ').Trim();
                ApiData.CompanyCode = companycode;
                ApiData.Description = GKVM.Description;
                ApiData.Key = GKVM.Key;
                ApiData.Value = GKVM.Value;
                RestClient.Update(ApiData,null);
                //For the sake of consistency format of the output json is kept same as output json exception handling.
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Key updated successfully", RedirectToUrl = "/Home/L2AdminDashboard.cshtml" };
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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/Home/L2AdminDashboard.cshtml" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/Home/L2AdminDashboard.cshtml" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Key could not be updated";
                    throw ex;
                }
            }
        }

        public ActionResult Delete(int id)
        {
           
            try
            {
                RestClient.Delete(id,null);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Configuration deleted successfully", RedirectToUrl = "" };
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
                    var OutputJson = new { ErrorMessage = "Configuration could not be deleted", PopupMessage = "", RedirectToUrl = "" };
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Configuration could not be deleted";
                    //throw ex;
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// Created by Rakhi Singh
        /// Method to get counts for keyvalues detail data
        /// </summary>
        public JsonResult GetCountsForGKeyValueForConfiguration()
        {
            var SummaryCount = RestClient.CountsForGKeyValueForConfiguration();
            return Json(SummaryCount, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created by Rakhi Singh
        /// Method to get all the keyvalues detail from GKeyValue and display on grid
        /// </summary>
        [HttpGet]
        public JsonResult GetGKeyValueForConfiguration(int pagesize, int pagenum, string sortdatafield, string sortorder)
        {
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetGKeyValueForConfiguration(pagesize, pagenum, sortdatafield, sortorder, FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReleaseNotes()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var data = RestClient.GetKeyValue("ReleaseNotes",CompanyCode);
            string ReleaseNotesHTML = data.Value;
            ViewBag.ReleaseNotesHTML = ReleaseNotesHTML;
            System.Web.HttpContext.Current.Session["Title"] = " Manage Release Notes";
            return View();
        }
        public ActionResult DownloadReleaseNotes(string ReleaseNotesName)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            string ReleaseNotesRelativeFilePath = ConfigurationManager.AppSettings["ReleaseNotesPath"];
            //var ReleaseNotesRelativeFilePath = "S:\\ReleaseNotes\\RELY\\" + ReleaseNotesName + ".pdf";
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + ReleaseNotesRelativeFilePath+ "/" + ReleaseNotesName + ".pdf";
            var fileData = Globals.DownloadFromS3(S3TargetPath);
            return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ReleaseNotesName+".pdf");
             
        }
    }
}
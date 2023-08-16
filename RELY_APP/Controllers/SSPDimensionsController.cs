using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{
    public class SSPDimensionsController : Controller
    {
        SSPDimensionsRestClient SDRC = new SSPDimensionsRestClient();
        // GET: SSPDimensions
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage SSP Dimensions";
            return View();
        }

        public ActionResult Create(string Source,string EntityId)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            System.Web.HttpContext.Current.Session["Title"] = "Configure SSP Dimensions";
            ViewBag.FormType = "Create";
            ViewBag.NewSSPId = SDRC.GetSSPIdForNew(CompanyCode);
            ViewBag.Source = Source;
            ViewBag.EntityId = EntityId;
            var model = new SSPDimensionViewModel();
            model.EffectiveEndDate = new DateTime(2099,12,31,13,00,00);
            return View(model);
        }
        public ActionResult Edit(int Id)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            System.Web.HttpContext.Current.Session["Title"] = "Configure SSP Dimensions";
            ViewBag.FormType = "Edit";
            var model = SDRC.GetById(Id);
            var dt = model.EffectiveEndDate;
            ViewBag.str = dt.Day.ToString() + "/" + dt.Month.ToString() + "/" + dt.Year.ToString();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(SSPDimensionViewModel model,string EffectiveStartDate,string FormType,string Source,string EntityId)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int LoggedInRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"]);
            try
            {//Date manipulations for Effective Start and End Date
                DateTime SSPStartDate = new DateTime();
                if (!string.IsNullOrEmpty(EffectiveStartDate))
                {
                    EffectiveStartDate = EffectiveStartDate + " 13:00:00"; //This is just a workaround, not fix. due to some time/offset difference, db was saving dates with 2hrs difference. so, taking 3hrs instead of 00hrs
                    SSPStartDate = DateTime.ParseExact(EffectiveStartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                model.EffectiveStartDate = SSPStartDate;
                model.EffectiveEndDate = new DateTime(2099,12,31,13,00,00);
                model.UpdatedById = LoggedInUserId;
                model.UpdatedDateTime = DateTime.UtcNow;
                if (FormType.Equals("Create"))//add new
                {
                    model.CompanyCode = CompanyCode;
                    model.CreatedById = LoggedInUserId;
                    model.CreatedDateTime = DateTime.UtcNow;
                    SDRC.Add(model, Source, EntityId, null);
                }
                else//update
                {
                    SDRC.Update(model, null);
                }
                var OutputJson = new { Source =Source,ErrorMessage = "", PopupMessage = "SSP saved successfully", RedirectToUrl = "/SSPDimensions/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Source = Source;
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
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "/SSPDimensions/Index" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "/SSPDimensions/Index" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                        default:
                            throw ex;
                    }
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Request System could not be created";
                    throw ex;
                }
            }
        }
        [HttpGet]
        public JsonResult Delete(int id)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            try
            {
                SDRC.Delete(id, null);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "SSP deleted successfully", RedirectToUrl = "" };
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
                    TempData["Error"] = "SSP could not be deleted";
                    throw ex;
                }
            }
        }


        [HttpGet]
        public JsonResult GetDataForGrid(string EntityType, int EntityId)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var data = SDRC.GetDataForGrid(EntityType, EntityId, CompanyCode);
            return Json(data,JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetExistingSspsCount()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            int count = SDRC.GetExistingSspsCount(CompanyCode);
            return Json(count, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetExistingSsps(string sortdatafield, string sortorder, int pagesize, int pagenum)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var data = SDRC.GetExistingSsps(CompanyCode, sortdatafield, sortorder, pagesize, pagenum, FilterQuery);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetBySSPId(int SelectedSspIdValue)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            var data = SDRC.GetBySSPId(SelectedSspIdValue, CompanyCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DetachSSP(int EntityId, string EntityType)
        {
            SDRC.DetachSSP(EntityId, EntityType,null);
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AttachSSP(int EntityId, string EntityType, int SspId)
        {
            SDRC.AttachSSP(EntityId, EntityType, SspId, null);
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
    }
}
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LProductContractDurationController : Controller
    {
        // string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        ILProductContractDurationRestClient RestClient = new LProductContractDurationRestClient();

        // GET: LLocalPOB
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Contract Duration";
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.FormType = "Create";
            System.Web.HttpContext.Current.Session["Title"] = "Create Contract Duration";
            return View();
        }


        public JsonResult GetDurationsByProductId(string ProductId)
        {
            var ApiData = RestClient.GetByProductId(ProductId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

       
    }
}
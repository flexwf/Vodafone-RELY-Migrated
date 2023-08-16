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
    public class LProductsSSPController : Controller
    {
        // string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        ILProductSSPRestClient RestClient = new LProductSSPRestClient();

        // GET: LLocalPOB
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Product SSP";
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.FormType = "Create";
            System.Web.HttpContext.Current.Session["Title"] = "Create Product SSP";
            return View();
        }

       
        public JsonResult GetSSPsByProductId(string ProductId)
        {
            var ApiData = RestClient.GetByProductId(ProductId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

              
    }
}
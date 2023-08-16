using Newtonsoft.Json;
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
    public class LLocalPobSSPController : Controller
    {
        // string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        ILLocalPobSSPRestClient RestClient = new LLocalPobSSPRestClient();

        // GET: LLocalPOB
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Local POB SSP";
            return View();
        }

        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Local POB SSP";
            return View();
        }

        
        public JsonResult GetLocalPobSSPs(int LocalPobId)
        {
            var CompanyCode = "DE";
            var ApiData = RestClient.GetByLocalPobId( LocalPobId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int SSPId)
        {
            var ApiData = RestClient.GetById(SSPId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(string jgGridData)
        {
            dynamic data = JsonConvert.DeserializeObject<LLocalPobSspViewModel>(jgGridData);
            return View();
        }
    }
}
using RELY_APP.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    public class LFSSurveyConfigController : Controller
    {
        // GET: SurveyConfig
        ILFinancialSurveysRestClient FSRC = new LFinancialSurveysRestClient();
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetSurveyData()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = FSRC.GetByCompanyCode(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
    }
}
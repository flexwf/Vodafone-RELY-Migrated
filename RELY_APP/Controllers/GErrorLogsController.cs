using RELY_APP.Helper;
using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    public class GErrorLogsController : Controller
    {
        IErrorLogsRestClient RestClient = new ErrorLogsRestClient();
        // GET: GErrorLogs
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetExceptionSummaryCounts()
        {
            var ExceptionCount = RestClient.GetExceptionSummaryCounts();
            return Json(ExceptionCount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExceptionSummary(string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum)
        {
            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetExceptionSummary(sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GErrorLogGrid(Nullable<int> pagesize, Nullable<int> pagenum, string sortdatafield, string sortorder)
        {

            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetGErrorlogGrid(Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), sortdatafield, sortorder, FilterQuery);

            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //This method addedd  to count grid data from Api
       // [ControllerActionFilter]
        public JsonResult GetGErrorLogcounts()
        {
            var ErrorCount = RestClient.GetGErrorLogscounts();
            return Json(ErrorCount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExceptionChart()
        {
            var ApiData = RestClient.GetExceptionChart();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

    }
}
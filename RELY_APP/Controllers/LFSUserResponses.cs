using RELY_APP.Helper;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LFSUserResponses : Controller
    {
        [HttpGet][CustomAuthorize]
        public ActionResult CaptureUserResponse()
        {
            System.Web.HttpContext.Current.Session["Title"] = "CaptureUserResponse ";
            ViewBag.Title = "CaptureUserResponse";
            //Below line will decide how to display User Response Partial View
            ViewBag.CaptureUserResponse = new CaptureUserResponseViewModel { SurveyId = 1, ChapterCode = "", SectionCode = "" };
            return View();
        }
    }
}
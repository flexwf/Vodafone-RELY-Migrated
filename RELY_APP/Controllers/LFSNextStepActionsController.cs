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
    public class LFSNextStepActionsController : Controller
    {
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();
        //int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
        //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;

        ILFSNextStepActionsRestClient RestClient = new LFSNextStepActionsRestClient();
        // GET: LFSNextStepActions
        [CustomAuthorize]
        public ActionResult Index(int EntityId, string EntityType)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage NextStep Action";
            ViewBag.Title = "Manage NextStep Action";
            ViewBag.EntityId = EntityId;
            ViewBag.EntityType = EntityType;

            return View();
        }

        [CustomAuthorize]
        public JsonResult GetGridData(int EntityId, string EntityType)
        {
            IRLFSNextStepRestClient NSRC = new LFSNextStepRestClient();
            var ApiData = NSRC.GetNextStepActionsByEntityIdType(EntityId, EntityType);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Edit(int id)
        //{


        //    var model = RestClient.GetById(id);
        //    System.Web.HttpContext.Current.Session["Title"] = "Edit Next Step Action";
        //    if (model == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(model);
        //}
        [CustomAuthorize]
        public ActionResult Edit(int TransactionId, int EntityId)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Edit Next Step Action";
            ViewBag.Title = "Edit Next Step Action";
            ViewBag.EntityId = EntityId;
            var model = RestClient.GetById(TransactionId);
            return View(model);
        }

        [HttpPost][CustomAuthorize]
        public JsonResult SaveData(LFSNextStepActionViewModel model, int EntityId)
        {
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            model.UpdatedById = LoggedInUserId;
            model.UpdatedDateTime = DateTime.Now;
            RestClient.Update(model, null);
            ViewBag.EntityId = EntityId;
            return Json(new { success =  true}, JsonRequestBehavior.AllowGet);
        }

        
    }
}
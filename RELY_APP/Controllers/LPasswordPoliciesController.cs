using RELY_APP.Helper;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    public class LPasswordPoliciesController : Controller
    {
        ILPasswordPoliciesRestClient RestClient = new LPasswordPoliciesRestClient();

        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Password Policies";
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();


            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());

            var ApiData = RestClient.GetByCompanyCode(CompanyCode, LoggedInUserId);
            ViewBag.PasswordPolicyData = ApiData;
            return View();
        }

        
        public JsonResult Edit(int Id)
        {
             
           
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetPasswordPolicyData(Id, CompanyCode);

            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetByCompanyCode()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
          
               
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());

            var ApiData = RestClient.GetByCompanyCode(CompanyCode, LoggedInUserId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SaveData(LPasswordPolicyViewModel model)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
           model.CompanyCode = CompanyCode;
            RestClient.SaveData(model,null);
            
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {

            RestClient.Delete(id,null);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PutPasswordPolicy(LPasswordPolicyViewModel model,int id)
        {

            model.CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
           
            RestClient.PutPasswordPolicy(id, model);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}
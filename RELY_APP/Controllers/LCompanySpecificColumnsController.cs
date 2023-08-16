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
    public class LCompanySpecificColumnsController : Controller
    {
        // GET: LCompanySpecificColumns////

        ILCompanySpecificColumnsRestClient RestClient = new LCompanySpecificColumnsRestClient();
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        //display grid
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = " Attribute Filed Configuration";
            ViewBag.Title = " Attribute Filed Configuration";
            
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            ILDropDownsRestClient LDRC = new LDropDownsRestClient();
            ViewBag.DropdownId = new SelectList(LDRC.GetByComapnyCode(CompanyCode),"Id","Name");

            
            ViewBag.TableName = GetTableNamesByCompanyCode(null);

            return View();
        }
        //Save Data from Grid//
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult SaveGridSelection(string[] model, string TableName,string SelectorType)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            RestClient.Add(model[0], TableName, SelectorType, CompanyCode);
            TempData["Message"] = "Company Specific Columns has been sucessfully added";
            return RedirectToAction("Index", new { TableName = TableName, SelectorType = SelectorType });
        }

        //Get CompanySpecific Columns//
        [ControllerActionFilter]
        public JsonResult GetCompanySpecificColumnsGrid(string TableName,string SelecterType)
        {
            var ApiData1 = RestClient.GetLCompanySpecificColumnsByTableName(TableName,SelecterType).Select(p => new {p.DropDownId,p.OrdinalPosition,p.DataType, IsNullable=(!p.IsMandatory), p.ColumnName, IsMandatory = p.IsMandatory, CanBeDisplayed = p.DisplayOnForms,p.DisplayInGrid,p.AuditEnabled,p.Label,p.LdName,p.MaximumLength,p.DefaultValue });
            return Json(ApiData1, JsonRequestBehavior.AllowGet);
        }

        //get selecter type by table
        [ControllerActionFilter]
        public JsonResult GetSelecterTypeByTable(string TableName)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            var SelecterTypes = RestClient.GetSelecterTypeByTableName(TableName,CompanyCode);
            return Json(SelecterTypes,JsonRequestBehavior.AllowGet);
        }

        public SelectList GetTableNamesByCompanyCode(int? Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            var ApiData = RestClient.GetTableNamesByCompanyCode(CompanyCode);
            var x = new SelectList(ApiData,"Value", "DisplayTableName", Selected);
            return x;
            // return Json(TableNames, JsonRequestBehavior.AllowGet);
        }
    }
}
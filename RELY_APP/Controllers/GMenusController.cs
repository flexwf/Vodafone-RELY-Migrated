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
    public class GMenusController : Controller
    {
        IGMenusRestClient RestClient = new GMenusRestClient();
        // GET: GMenus
        [CustomAuthorize]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Menus ";
            ViewBag.Title = "Manage Menus";

            return View();
        }

        [CustomAuthorize]
        public JsonResult GetMenuList(string UserName, string WorkFlow)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetAll(CompanyCode).Select(p => new { id = p.Id, parentid = p.ParentId, text = p.MenuName, value = p.Id });
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize]
        public JsonResult GetMenuItems(string id)
        {
            var grp = Convert.ToInt32(id);
            var result = new[]{ new { text = "<a href='/GMenus/Create/"+grp+"'>Create New Menu</a>", parentid = "-1", id = 0 },
                    new{text = "<a href='/GMenus/Edit/"+grp+"'>Edit Menu</a>", parentid = "-1", id =1},
                    new{text = "<a href='/GMenus/Delete/"+grp+"'>Delete Menu</a>", parentid = "-1", id =2}};
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProcessMenuItems(GMenuViewModel GMenu, string MenuLabel, int MenuId, int ParentMenuId)
        {
            try
            {
                string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
                RestClient.ProcessMenuItems(GMenu, MenuLabel, MenuId, ParentMenuId, CompanyCode, null);
                string DisplayMessage = "";
                if (MenuLabel == "Add Item Above" || MenuLabel == "Add Item Below" || MenuLabel == "Add Child Item")
                {
                    DisplayMessage = "Menu Item added successfully";
                }
                else if (MenuLabel == "Remove Item")
                {
                    DisplayMessage = "Menu Item removed successfully";
                }
                else if (MenuLabel == "Move up")
                {
                    DisplayMessage = "Menu Item moved up successfully";
                }
                else if (MenuLabel == "Move down")
                {
                    DisplayMessage = "Menu Item moved down successfully";
                }
                else
                {
                    DisplayMessage = "Menu Item processed successfully";
                }


                var OutputJson = new { ErrorMessage = "", PopupMessage = DisplayMessage, RedirectToUrl = "/GMenus/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetMenuById(int id)
         {
            var ApiData = RestClient.GetMenuById(id);
            //FnSetMenuSessionAndRedirect(168, '')31
            for (int i = 0; i < ApiData.Count(); i++)
            {
                string MenuURL = ApiData.ElementAt(i).MenuURL;
                if (MenuURL != null)
                {
                    MenuURL = MenuURL.Replace(" ", "");
                    int index = MenuURL.IndexOf(',');
                    MenuURL = MenuURL.Substring(index+2, MenuURL.Length - (index+4));
                }
                ApiData.ElementAt(i).MenuURL = MenuURL;
            }
                return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult Update(GMenuViewModel GMenu,int id,int ParentMenuId,int OrdinalPosition)
        public JsonResult Update(GMenuViewModel GMenu,int id)
        {
            // GMenu.Id = id;
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            string MenuURL = "FnSetMenuSessionAndRedirect(" + id + ", '" + GMenu.MenuURL + "')";
            GMenu.MenuURL = MenuURL;
            GMenu.CompanyCode = CompanyCode;
            RestClient.Update(GMenu,id);
            var OutputJson = new { ErrorMessage = "", PopupMessage = "Data updated successfully", RedirectToUrl = "/GMenus/Index" };
            return Json(OutputJson, JsonRequestBehavior.AllowGet);
        }

    }
    }
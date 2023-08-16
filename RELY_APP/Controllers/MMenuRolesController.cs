using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    public class MMenuRolesController : Controller
    {
        // GET: MMenuRoles
        IMMenuRolesRestClient MMRRC = new MMenuRolesRestClient();
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage MenuRole Mapping";
            ViewBag.Title = "Manage MenuRole Mapping";
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            ViewBag.MenuRolesColumn = MMRRC.GetColumnForMenuRoles(CompanyCode);
            return View();
        }

        public JsonResult GetDataForMenuRoleMapping()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = MMRRC.GetDataForMenuRoleMapping(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDataForMenuRole(object[] Args)
        {
            try
            {
                string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
                List<UpdateMappingMenuRoleViewModel> modelList = new List<UpdateMappingMenuRoleViewModel>();
                // int Count = 0;
                foreach (string[] Data in Args)
                {

                    var data = Data;
                    var ArgsArray = data;




                    var model = new UpdateMappingMenuRoleViewModel

                    {
                        MenuId = Convert.ToInt32(ArgsArray[0]),
                        ColumnName = ArgsArray[1].ToString(),
                        NewResponse = (ArgsArray[2].ToString().ToLower() == "true") ? true : false,
                        // NewResponse = Convert.ToBoolean(ArgsArray[Count][2].ToString()),
                        OldResponse = (ArgsArray[3].ToString().ToLower() == "true") ? true : false

                        // AuthorizableId = Convert.ToInt32(ArgsArray[Count][0]),
                        // ColumnName = ArgsArray[Count][1].ToString(),
                        // NewResponse = (ArgsArray[Count][2].ToString().ToLower() == "true") ? true : false,
                        //// NewResponse = Convert.ToBoolean(ArgsArray[Count][2].ToString()),
                        // OldResponse = (ArgsArray[Count][3].ToString().ToLower() == "true") ? true : false
                    };
                    // Count = Count + 1;
                    modelList.Add(model);

                }
                MMRRC.SaveDataForMenuRole(modelList, CompanyCode, null);
                var OutputJson = new { Success = "Success", ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/MMenuRolesRole/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)

            {

                string redirectUrl = ex.Data["RedirectToUrl"] as string;
                if (ex.Data["ErrorCode"] != null)
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)Globals.ExceptionType.Type1:
                            return Json(new { success = false, ErrorMessage = ex.Data["ErrorMessage"] }, JsonRequestBehavior.AllowGet);

                        case (int)Globals.ExceptionType.Type2:

                            ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                            return Json(new { success = false, ErrorMessage = ex.Data["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                        case (int)Globals.ExceptionType.Type3:
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                            return Json(new { success = false, ErrorMessage = ex.Data["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                        case (int)Globals.ExceptionType.Type4:
                            ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                            return Json(new { success = false, ErrorMessage = ex.Data["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                        default:
                            return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);



            }



        }

    }
}
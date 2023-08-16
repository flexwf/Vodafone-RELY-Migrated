using Newtonsoft.Json;
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    public class MAuthorizableObjectRoleController : Controller
    {
       

        IMAuthorizableObjectsRolesRestClient AORC = new MAuthorizableObjectsRolesRestClient();

        public ActionResult Index(string WorkFlow)
        {
            


            System.Web.HttpContext.Current.Session["Title"] = "Manage AuthorizableObjectRole";
            ViewBag.Title = "Manage AuthorizableObjectRole";
            if (string.IsNullOrEmpty(WorkFlow))
            {
                WorkFlow = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            }
            else
            {
                System.Web.HttpContext.Current.Session["WorkFlow"] = WorkFlow;
            }

            ViewBag.AuthorizableObjectColumn= AORC.GetColumnForAuthorizableObjectRole();
            return View();
        }
        // GET: MAuthorizableObjectRole
        public JsonResult GetDataForAuthorizableObjectRole()
        {
            var ApiData = AORC.GetDataForAuthorizableObjectRole();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDataForAuthorizableObjectRole(object[] Args)
        {
            try
            {

                List<UpdateMappingAuthorizableObjectRoleViewModel> modelList = new List<UpdateMappingAuthorizableObjectRoleViewModel>();
               // int Count = 0;
                    foreach (string[] Data in Args)
                    {

                    var data = Data;
                    var ArgsArray = data;


                   
                    
                    var model = new UpdateMappingAuthorizableObjectRoleViewModel

                    {
                        AuthorizableId = Convert.ToInt32(ArgsArray[0]),
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
                AORC.SaveDataForAuthorizableObjectRole(modelList,null);
                var OutputJson = new { Success = "Success", ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/MAuthorizableObjectRole/Index" };
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

        //public JsonResult  TestMultiThreading()
        //{
        //   int RequestId = 223;
        //     AORC.TestMultiThreading(RequestId);
        //    var OutputJson = new { Success = "Success", ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/MAuthorizableObjectRole/Index" };
        //    return Json(OutputJson, JsonRequestBehavior.AllowGet);

            
          

        //}

        
    }
}

using System.Web.Mvc;
using System.Threading.Tasks;
using RELY_APP.ViewModel;
using System.Collections.Generic;
using System;
using System.Linq;
using RELY_APP.Helper;
using RELY_APP;
using System.Configuration;
using RELY_APP.Utilities;

namespace WebAPP.Controllers
{
    //[SessionExpire]
    //[HandleCustomError]
    public class HomeController : Controller
    {
        ILAuditRestClient laudit = new LAuditRestClient();
        IRWorkFlowsRestClient rworkflows = new RWorkFlowsRestClient();
        //string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);

        public ActionResult ErrorPage()
        {
            return View("Error");
        }
        public ActionResult Index()
        { //set session variable for no user logged in for use in exception handling
            System.Web.HttpContext.Current.Session["LoginEmail"] = "No User Logged in";
            var Url = TempData["LastActiveUrl"];
            ViewBag.LastActiveUrl = Url;
            return View();
        }

        public ActionResult SessionTimeOutLogin()
        {
            var Url = TempData["LastActiveUrl"];
           // TempData["LastActiveUrl"] = Url;
            ViewBag.LastActiveUrl = Url;
            System.Web.HttpContext.Current.Session["LoginEmail"] = "No User Logged in";
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //This method  will load data in dropdown of user role
        //[ControllerActionFilter]
        [CustomAuthorize]
        public JsonResult GetRolesList()
        {
            var Roles = System.Web.HttpContext.Current.Session["Roles"] as List<LRoleViewModel>;
            if (Roles != null)
            {
                if (Roles.Count > 0)
                    return Json(Roles.Select(p => p.RoleName), JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        //This method is used  to display menu items  in Layout file by getting data from MGMenusAspnetRoles
        // [ControllerActionFilter]
        [CustomAuthorize]
        public JsonResult GetMenuItems()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            IGMenusRestClient MGARC = new GMenusRestClient();
            var Role = (System.Web.HttpContext.Current.Session["CurrentRoleName"]==null)? null : System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();
            //If role is defined in session for current user then dsplay its menu items
            if (!string.IsNullOrEmpty(Role))
            {
                var MenuItems = MGARC.GetByUserRole(Role, CompanyCode).Select(p => new { id = p.MenuId, text = (string.IsNullOrEmpty(p.MenuURL)) ? 
                    "<a href='#'>" + p.MenuName + "</a>"/* : (p.MenuURL.EndsWith(".pdf')")) ? "<a target='_blank'  href='" + p.MenuURL + "'>" + p.MenuName + "</a>" */
                    :  "<a href='#' onclick=" + p.MenuURL + ";>" + p.MenuName + "</a>", parentid = p.ParentId, p.OrdinalPosition }).OrderBy(p => p.OrdinalPosition).ToList();
                return Json(MenuItems, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
       // [CustomAuthorize]
        public JsonResult SetMenuIdInSession(int? MenuId)
        {
            System.Web.HttpContext.Current.Session["CurrentMenuId"] = MenuId;
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMenuIdFromSession()
        {
            int MenuId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentMenuId"]);
            return Json(MenuId, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize]
        public JsonResult SaveReturnPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                System.Web.HttpContext.Current.Session["from"] = path;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        //This methos will be called when user clicks on go button after changing role
        //[ControllerActionFilter]
        [CustomAuthorize]
        public ActionResult ChangeUserRole(string UserRole)
        {
            //Update session role
            if (!string.IsNullOrEmpty(UserRole))
            {
                var RolesList = System.Web.HttpContext.Current.Session["Roles"] as List<LRoleViewModel>;
                System.Web.HttpContext.Current.Session["CurrentRoleName"] = UserRole;
                System.Web.HttpContext.Current.Session["CurrentRoleId"] = RolesList.Where(p => p.RoleName == UserRole).FirstOrDefault().Id;
            }
            
                return RedirectToAction("DecideDashboard", "Home");
        }

        [CustomAuthorize]
        public async Task<ActionResult> DecideDashboard()
        {
            var CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var CurrentRole = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();
            System.Web.HttpContext.Current.Session["Title"] = CurrentRole + " Dashboard";
            System.Web.HttpContext.Current.Session["CurrentMenuId"] = null; //SG nullifying MenuId from session
            ViewBag.RoleId= Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            ViewBag.UserId= Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());

            if (CurrentRole == "L2Admin")
            {
                return RedirectToAction("L2AdminDashboard", "Home");
            }
            else
            {
                return View();
            }
            

           /* if (CurrentRole.ToLower() == "accountant")
            {
                //commenting this code as we are now checking useraccount from DB, which is required
              //  if (System.Web.HttpContext.Current.Session["Password"].ToString() == "Rely#123")//hardcoding password, it will be updated later
               // {
                    return RedirectToAction("AccountantDashboard", "Home");
                //}
                //else
                //{
                //    var xx = "Username/Password is incorrect";
                //    var ex = new Exception(String.Format("{0},{1}",xx , Globals.ExceptionType.Type4));
                //    ex.Data.Add("ErrorCode", Globals.ExceptionType.Type4);
                //    ex.Data.Add("RedirectToUrl", "");
                //    //string source = response.Content;
                //    //dynamic data = JsonConvert.DeserializeObject(source);
                //   // string xx = data.Message;
                //    ex.Data.Add("ErrorMessage", xx);
                //    throw ex;
                //}
               // return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }*/
        }

        public ActionResult L2AdminDashboard()
        {

            //calling method to get the list of dynamic columns for the new item grid on L2AdminPage
            var NewItemscolumnlist = GetNewItemscolumnlist(null, null, null, null, null);
            ViewBag.NewItemscolumnlist = NewItemscolumnlist.Data;

            //GetCompletedItemscolumnlist
            var CompletedItemscolumnlist = GetCompletedItemscolumnlist(null, null, null, null);
            ViewBag.CompletedItemscolumnlist = CompletedItemscolumnlist.Data;

            ViewBag.CompanyCode = GetAllCompanyCode(null);
            return View();
          //  return View("L2AdminDashboard.cshtml");
            //L2AdminDashboard
        }


        public JsonResult GetCompletedItemscolumnlist(string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum)
        {
            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            //var qry = Request.QueryString;
            var FilterQuery = "";
            var ApiData = rworkflows.GetCompletedListcolumnlist(sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNewItemscolumnlist(string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum, Nullable<int> Intervalid)
        {
            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            if (Intervalid == null) Intervalid = 1;
            //var qry = Request.QueryString;
            var FilterQuery = "";
            var ApiData = laudit.GetNewItemscolumnlist(sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery, Convert.ToInt32(Intervalid));
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        private SelectList GetAllCompanyCode(int? Selected)
        {
            IGCompaniesRestClient gc = new GCompaniesRestClient();
            var ApiData = gc.GetAll();
            var x = new SelectList(ApiData, "Id", "CompanyCode", Selected);
            return x;
        }
        [CustomAuthorize]
        public JsonResult GetSupportingDocuments(string EntityType,int EntityId)
        {
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var ApiData = LSDRC.GetByEntityType(EntityType,EntityId);
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataForDashBoardToDo(int RoleId,int UserId)
        {
            IDashBoardRestClient DashBoardToDo = new DashBoardToDoRestClient();
            var ApiData = DashBoardToDo.GetDataForDashBoardToDo(RoleId, UserId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ClearSessionVariables(string AttributesList)
        {
            Globals.ClearSessionVariable(AttributesList);
            return Json(String.Empty, JsonRequestBehavior.AllowGet);
        }
    }
}
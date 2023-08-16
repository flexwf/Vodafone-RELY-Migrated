using Newtonsoft.Json;
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Configuration;
using System.Data;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{

    [SessionExpire]
    [HandleCustomError]
    public class LAuditController : Controller
    {
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();
        //int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
        //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;

        ILAuditRestClient RestClient = new LAuditRestClient();

        // GET: LAudit
        [CustomAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize]
        public JsonResult GetDetails(string EntityType, int EntityId)
        {
            
            var ApiData = RestClient.GetByTypeEntityId(EntityType, EntityId, null);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created By: Rakhi Singh
        /// Date:26 june
        /// Method to download HistoryGrid
        /// </summary>
        /// <param name="EntityType","EntityId"></param>
        /// <returns></returns>
        public ActionResult DownloadHistoryDetails(string EntityType, int EntityId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.DownloadGetByTypeEntityId(EntityType, EntityId, null);
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(ApiData, (typeof(DataTable)));

            if (dt.Columns.Count > 0)
            {
                //line to remove unwanted columns from data-table
                dt.Columns.Remove("Id");
                dt.Columns.Remove("ActionType");
                dt.Columns.Remove("RelyProcessName");
                dt.Columns.Remove("VFProcessName");
                dt.Columns.Remove("ControlCode");
                dt.Columns.Remove("ControlDescription");
                dt.Columns.Remove("ActionedById");
                dt.Columns.Remove("ActionedByRoleId");
                dt.Columns.Remove("OldStatus");
                dt.Columns.Remove("NewStatus");
                dt.Columns.Remove("EntityType");
                dt.Columns.Remove("EntityId");
                dt.Columns.Remove("EntityName");
                dt.Columns.Remove("WorkFlowId");
                dt.Columns.Remove("CompanyCode");
                dt.Columns.Remove("Action");

                //line to rename columns from data-table as to show on front-view
                dt.Columns["UserName"].ColumnName = "User";
                dt.Columns["ActionDateTime"].ColumnName = "DateTime";
                dt.Columns["UserRole"].ColumnName = "Role";
                dt.Columns["ActionLabel"].ColumnName = "Action";

                //line that sets the arrangement of columns on front-view
                dt.Columns["DateTime"].SetOrdinal(0);
                dt.Columns["User"].SetOrdinal(1);
                dt.Columns["Role"].SetOrdinal(2);
                dt.Columns["Action"].SetOrdinal(3);
                dt.Columns["Comments"].SetOrdinal(4);
            }
            
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Filename = "HistoryDetails_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            Globals.ExportToExcel(dt, path, Filename);
            return File(path + "/" + Filename, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Filename);
           
        }

        public JsonResult GetDataForNewItems(string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum, Nullable<int> Intervalid)
        {
            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            if (Intervalid == null) Intervalid = 1;

            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);           
            var ApiData = RestClient.GetDataForNewItemColumns(sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery, Convert.ToInt32(Intervalid));
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //GetCountsForNewItems
        public JsonResult GetCountsForNewItems()       
        {  
            var ApiData = RestClient.GetCountsForNewItems();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

    }
 }
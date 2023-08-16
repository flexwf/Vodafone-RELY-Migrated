using RELY_APP.Helper;
using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LSupportingDocumentsController : Controller
    {
        ILSupportingDocumentsRestClient RestClient = new LSupportingDocumentsRestClient();
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();
        //int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
        //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"].ToString();

        // GET: LSupportingDocuments
        [CustomAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        //Method to download supporting Doc
        [CustomAuthorize]
        public ActionResult DownloadSupportingDocument(int id)
        {
            var SupportingDoc = RestClient.GetById(id);
            var CompleteFileName = SupportingDoc.FilePath + "/" + SupportingDoc.FileName;
            //if(System.IO.File.Exists(CompleteFileName))
            //{
            var FileData=Globals.DownloadFromS3(CompleteFileName);
                return File(FileData, "application/unknown", SupportingDoc.OriginalFileName);
            //}
            //TempData["Error"] = "No Data/File found";
            //return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }

        //Method to delete supporting Doc
        [CustomAuthorize]
        public ActionResult Delete(int id)
        {
            string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"].ToString();
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());

            RestClient.Delete(id,LoggedInUserId, UserRoleId,WorkFlowName,CompanyCode);
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }
    }
}
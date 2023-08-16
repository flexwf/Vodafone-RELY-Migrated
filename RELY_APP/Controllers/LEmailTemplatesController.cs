using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{
    public class LEmailTemplatesController : Controller
    {

        ILEmailTemplatesRestClient RestClient = new LEmailTemplatesRestClient();
        // GET: LEmailTemplates
        public ActionResult Index()
        {

            // ViewBag.EntityId = EntityId;
            //ViewBag.EntityType = EntityType;
            System.Web.HttpContext.Current.Session["Title"] = "Manage Email Templates";
            return View();
        }

        public ActionResult Edit(int id)
        {

            //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            //int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
            //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;

            ViewBag.FormType = "Edit";
            System.Web.HttpContext.Current.Session["Title"] = "Edit Email Template";
            LEmailTemplateViewModel model = RestClient.GetLEmailTemplateById(id);
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true, ExistingFilesList = null };
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LEmailTemplates";
            return View(model);
        }

        public ActionResult Create(int? TemplateId)
        {

            System.Web.HttpContext.Current.Session["Title"] = "Configure Template";
            ViewBag.Title = "Configure Template";
            ViewBag.FormType = "Create";
           
            ViewBag.StepId = 0;
            LEmailTemplateViewModel model = new LEmailTemplateViewModel();
           
            //Below line will decide how to display UploadDocument PartialView on the page
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = true, CanUploadMultipleFiles = true, SaveFileInBucket = true, ExistingFilesList = null };
           
            ViewBag.EntityId = model.Id;
            ViewBag.EntityType = "LEmailTemplates";
            return View(model);
           
        }



         [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public JsonResult SaveTemplate(LEmailTemplateViewModel model,string EmailBody)
        {
            //When we add placeholder value after space in EmailBody then it takes &nbsp; so at the time of Edit it is showing as its in EmailBody in front End. So we replaced it with space.
            EmailBody = EmailBody.Replace("&nbsp;", " ");


            model.CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            model.EmailBody = EmailBody;

            //Upload Utility Section to move files to specified location and  save FileList  in db starts
            string FileList = "";
            string OriginalFileList = "";
            string FilePath = "";
            string SupportingDocumentsDescription = "";
            if (System.Web.HttpContext.Current.Session["UploadedFilesList"] != null)
            {
                var AttachedSupportingFiles = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
                FileList = string.Join(",", AttachedSupportingFiles.Select(p => p.FileName));
                OriginalFileList = string.Join(",", AttachedSupportingFiles.Select(p => p.OriginalFileName));
                SupportingDocumentsDescription = string.Join(",", AttachedSupportingFiles.Select(p => p.Description));
                FilePath = AttachedSupportingFiles.ElementAt(0).FilePath;
               
                System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//Clear Session now
            }
            //Section Ends
            RestClient.SaveTemplate(model,null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList);
           // ViewBag.EntityId = EntityId;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        //Get data from LEmailTemplate
        public JsonResult GetLEmailTemplates()
        {

            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = RestClient.GetLEmailTemplates(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateTemplate(LEmailTemplateViewModel model, string EmailBody)
        {
           
            model.CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

            //When we add placeholder value after space in EmailBody then it takes &nbsp; so at the time of Edit it is showing as its in EmailBody in front End. So we replaced it with space.
            EmailBody = EmailBody.Replace("&nbsp;", " ");
            model.EmailBody = EmailBody;
            int id = model.Id;

            //Upload Utility Section to move files to specified location and  save FileList  in db starts
            string FileList = "";
            string OriginalFileList = "";
            string FilePath = "";
            string SupportingDocumentsDescription = "";
            if (System.Web.HttpContext.Current.Session["UploadedFilesList"] != null)
            {
                var AttachedSupportingFiles = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
                FileList = string.Join(",", AttachedSupportingFiles.Select(p => p.FileName));
                OriginalFileList = string.Join(",", AttachedSupportingFiles.Select(p => p.OriginalFileName));
                SupportingDocumentsDescription = string.Join(",", AttachedSupportingFiles.Select(p => p.Description));
                FilePath = AttachedSupportingFiles.ElementAt(0).FilePath;

                System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;//Clear Session now
            }
            //Section Ends

            RestClient.UpdateTemplate(id,model, null, FileList, SupportingDocumentsDescription, FilePath, OriginalFileList);
            // ViewBag.EntityId = EntityId;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteLEmailTemplate(int id)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            try
            {
                RestClient.DeleteLEmailTemplate(id, CompanyCode, null);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "Email Templatea deleted successfully", RedirectToUrl = "" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                    }
                    var OutputJson = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Email Template could not be deleted";
                    throw ex;
                }
            }
        }

    }
}
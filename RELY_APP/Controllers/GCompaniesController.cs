using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    public class GCompaniesController : Controller
    {
        IGCompaniesRestClient RestClient = new GCompaniesRestClient();


        // GET: GCompanies
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = null;
            ViewBag.FileUploaderParameters = new FileUploaderParametersViewModel { CanDisplayDescriptionBox = false, CanUploadMultipleFiles = false, SaveFileInBucket = true, ExistingFilesList = null };
            return View();
        }

        public JsonResult GetCompanies()
        {
            
            var ApiData = RestClient.GetAll();

            //we need to copy the file from S3 path to our solution in content folder otherwise it will not show the image
            string LogoPath = "";
            for (int i = 0; i < ApiData.Count(); i++)
            {
                LogoPath = ApiData.ElementAt(i).LogoPath;
                if (LogoPath != null)
                {
                    string path = Globals.SaveLogoImageInSolution(LogoPath);
                    ApiData.ElementAt(i).LogoPath = path;
                }
            }

            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyInfo(String CompanyCode)
        {

            var ApiData = RestClient.GetCompanyInfo(CompanyCode);
            string LogoPath = "";

            //we need to copy the file from S3 path to our solution in content folder otherwise it will not show the image
            for (int i = 0; i < ApiData.Count(); i++)
            {
                LogoPath = ApiData.ElementAt(i).LogoPath;
                if (LogoPath != null)
                {
                    string path = Globals.SaveLogoImageInSolution(LogoPath);
                    ApiData.ElementAt(i).LogoPath = path;
                }
            }
           

            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveData(CreateCompanyViewModel model)
        {
            try
            {
                model.LogoPath = System.Web.HttpContext.Current.Session["LogoImagePath"].ToString();

                RestClient.SaveData(model, null);
                System.Web.HttpContext.Current.Session["LogoImagePath"] = null;
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult UpdateData(CreateCompanyViewModel model)
        {
            try
            {
                model.LogoPath = System.Web.HttpContext.Current.Session["LogoImagePath"].ToString();

                RestClient.UpdateData(model);

                System.Web.HttpContext.Current.Session["LogoImagePath"] = null;


                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult DeleteCompany(string CompanyCode)
        {


            RestClient.DeleteCompany(CompanyCode,"");

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLogoImage()
        {
            try
            {
                var File = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>; //Session["UploadedFilesList"] as List<FileUploadViewModel>;
                                                                                                                     // File = File.Where(p => p.ActivityType == "Upload").ToList();
                if (File != null && File.Count() > 0)
                {
                    Session["fname"] = File[0].OriginalFileName;
                    Session["FileName"] = File[0].FileName;
                    Session["FilePath"] = File[0].FilePath;

                }
                
                string path = Globals.SaveLogoImageInSolution(File[0].FilePath + "/" + File[0].FileName);

                return Json(path, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        
    }
}
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    public class RelyDataDownloadController : Controller
    {
        //object to connect restclient
        IRelyDataDownloadRestClient rdd = new RelyDataDownloadRestClient();
       
        //it returns the view of module
        public ActionResult RelyDataDownload()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Data Download";
            ViewBag.CompanyName = GetAllCompanyName(null);
            return View();
        }

        //method to fill dropdown at the time of pageload
        private SelectList GetAllCompanyName(int? Selected)
        {
            var ApiData = rdd.GetAllCompanyName();
            var x = new SelectList(ApiData, "Id", "CompanyName", Selected);
            return x;
        }

        /// <summary>
        /// This method is used to download Rely data dump (zip file of all CSVs) from server path 
        /// </summary>
        public ActionResult GenerateRelyDataDump(int? companyid)
        {

            GenericNameAndIdViewModel model = new GenericNameAndIdViewModel();
            RelyDataDownload par = new RelyDataDownload();
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

            if (companyid != null)
            {
                par = rdd.GetCompanyCodeById(companyid); //restclient method to get the company code on the basis of id
                model = rdd.DownloadRelyDataDump(par.CompanyCode); //restclient method to generate rely data dump
            }
            else
            {
                par.CompanyCode = CompanyCode;
                model = rdd.DownloadRelyDataDump(par.CompanyCode); //restclient method to generate rely data dump
            }
            string FileName = "";
            if (model != null)
            {
                FileName = model.Name;
            }
            string IntermediatePath = "RelyData";
            string S3TargetPath = "/" + par.CompanyCode.ToLower() + "/" + IntermediatePath + "/" + FileName;
            var fileData = Globals.DownloadFromS3(S3TargetPath);
            var newfileName = "Rely-Data-Dump" + "_" + DateTime.UtcNow.ToString("dd-MM-yyyy") + ".zip";
            return File(fileData, "application/zip", newfileName);
        }
    }
}
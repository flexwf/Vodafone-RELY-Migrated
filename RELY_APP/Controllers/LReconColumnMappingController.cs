using RELY_APP.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    public class LReconColumnMappingController : Controller
    {
        IRSysCatRestClient SysCatRestClient = new RSysCatRestClient();
        LReconFileFormatsRestClient FileFormatRestClient = new LReconFileFormatsRestClient();
        ILReconColumnMappingRestClient RestClient = new LReconColumnMappingRestClient();
        // GET: LReconColumnMapping
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Recon Column Mappings";
            return View();
        }

        //GetSysCat
        public JsonResult BindSysCatDropDown()
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var SysCatName = SysCatRestClient.GetSysCatforDropDown(CompanyCode);
            return Json(SysCatName, JsonRequestBehavior.AllowGet);
        }

        //GetFileFormat
        public JsonResult BindFileFormatDropDown(int SysCatId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var FileFormat = RestClient.GetFileFormatBySysCat(SysCatId, CompanyCode);
            return Json(FileFormat, JsonRequestBehavior.AllowGet);
        }

        //Get ReconColumnMapping
        public JsonResult GetLReconColumnMappingBySysCat(int FileFormatId,int SysCatId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            var SelectSysCatId = RestClient.GetLReconColumnMappingBySysCat(CompanyCode, FileFormatId, SysCatId);
            return Json(SelectSysCatId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SaveGridSelection(string[] GridData, int FileFormatId, int SysCatId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            RestClient.Add(GridData[0], FileFormatId, SysCatId, CompanyCode);
           // TempData["Message"] = "Recon Column Mapping has been sucessfully added";
            return RedirectToAction("Index", new { FileFormatId = FileFormatId, SysCatId = SysCatId });
        }
    }
}
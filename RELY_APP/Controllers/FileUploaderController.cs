using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RELY_APP.Helper;
using RELY_APP.ViewModel;
using System.IO;
using System.Configuration;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class FileUploaderController : Controller
    {
        FilesUploadHelper filesUploadHelper;
        String LocalStoragePath = ConfigurationManager.AppSettings["LocalTempUploadFolder"]+"/";

       // [CustomAuthorize]
        public string StorageRoot
        {
            get { return Path.Combine((LocalStoragePath)); }
        }
        private string UrlBase = ConfigurationManager.AppSettings["LocalTempUploadFolder"]+"/";
        String DeleteURL = "/FileUpload/DeleteFile/?file=";
        String DeleteType = "GET";

        //[CustomAuthorize]
        public FileUploaderController()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            String S3tempPath = "/" + Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]).ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];//ConfigurationManager.AppSettings["TempUploadFolderName"];//+"/"+System.Web.HttpContext.Current.Session["CompanyCode"]+"/Uploads";
            filesUploadHelper = new FilesUploadHelper(DeleteURL, DeleteType, StorageRoot, UrlBase, S3tempPath,CompanyCode,LocalStoragePath);
        }
        // GET: FileUploader

        [CustomAuthorize]
        public ActionResult Index()
        {
            return PartialView("~/Views/Shared/_FileUploaderPartial.cshtml");
        }

        [HttpPost]
        [CustomAuthorize]
        public JsonResult UploadDocuments(string AttachedComments,bool SaveFileInBucket,bool IsDataUpload)
        {
            var resultList = new List<FilesUploadViewModel>();
            var CurrentContext = HttpContext;
            filesUploadHelper.UploadAndShowResults(CurrentContext, resultList,SaveFileInBucket);
            JsonFiles files = new JsonFiles(resultList);
            bool isEmpty = !resultList.Any();
            string[] AllowedFileTypes = new string[]{ "doc", "docx", "xls","xlsx","ppt","pptx","jpg","jpeg","png","txt","csv" };
            if (isEmpty)
            {
                return Json("Error");
            }
            else
            {
                String S3tempPath = "/" + Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]).ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];//ConfigurationManager.AppSettings["TempUploadFolderName"];//+"/"+System.Web.HttpContext.Current.Session["CompanyCode"]+"/Uploads";
                var ExistingFiles = new List<FileUploadViewModel>();
                if ((System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>) != null)
                {
                    ExistingFiles = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
                    
                }
                foreach (var yy in files.files)
                {
                    //file type checking
                    string extension = yy.FileName.Substring(yy.FileName.IndexOf('.') + 1);
                    if(!AllowedFileTypes.Contains(extension))
                    {
                       return Json("Error");
                    }

                    var model = new FileUploadViewModel();
                    model.FileName = yy.FileName;
                    model.OriginalFileName = yy.OriginalFileName;
                    model.Description = AttachedComments;
                    model.FilePath = S3tempPath;
                    model.ActivityType = (IsDataUpload) ? "Upload" : "Attachment";
                    ExistingFiles.Add(model);
                }
                System.Web.HttpContext.Current.Session["UploadedFilesList"] = ExistingFiles;
                return Json(files);
            }
        }

       // [CustomAuthorize]
        public JsonResult GetUploadedFileList(string Comments)
        {
            var FileModelList = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
            if (FileModelList != null)
            {
                var FileList = FileModelList.Select(p => p.OriginalFileName);
                UpdateCommentsInFileList(Comments);
                return Json(FileList, JsonRequestBehavior.AllowGet);
            }
            return Json(String.Empty, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize]
        public JsonResult UpdateComments(string Comments)
        {
            UpdateCommentsInFileList(Comments);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //Update Comments when dilog box is closed
        [CustomAuthorize]
        public void UpdateCommentsInFileList(string Comments)
        {
            if (!string.IsNullOrEmpty(Comments))
            {
                var FileModelList = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
                foreach(var FileModel in FileModelList)
                {
                    FileModel.Description = Comments;
                }
            }
        }

        [CustomAuthorize]
        public JsonResult DeleteFileFromList(int FileIndex)
        {
            var FileList = System.Web.HttpContext.Current.Session["UploadedFilesList"] as List<FileUploadViewModel>;
            FileList.RemoveAt(FileIndex);
            System.Web.HttpContext.Current.Session["UploadedFilesList"] = FileList;
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
    }
}
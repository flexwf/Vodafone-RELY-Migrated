using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using RELY_APP.Utilities;
using System.Drawing;

namespace RELY_APP.Helper
{
    public class FilesUploadHelper
    {
        string CurrentTimeStr = DateTime.UtcNow.ToString("dd_MM_yyyy_HH_mm");
        String DeleteURL = null;
        String DeleteType = null;
        String StorageRoot = null;
        String UrlBase = null;
        String S3TempPath = null;
        string CompanyCode = "";
        string LocalStoragePath = "";
        string FullFileName = "";
        //ex:"~/Files/something/";
        // String serverMapPath = null;
        string UserFrindllyName = string.Empty;
        public FilesUploadHelper() { }

        public FilesUploadHelper(String DeleteURL, String DeleteType, String StorageRoot, String UrlBase, String S3TempPath,string CompanyCode,string LocalStoragePath)
        {
            this.DeleteURL = DeleteURL;
            this.DeleteType = DeleteType;
            this.StorageRoot = StorageRoot;
            this.UrlBase = UrlBase;
            this.S3TempPath = S3TempPath;
            this.CompanyCode = CompanyCode;
            this.LocalStoragePath = LocalStoragePath;
        }
        public void DeleteFiles(String pathToDelete)
        {

            string path = HostingEnvironment.MapPath(pathToDelete);

            System.Diagnostics.Debug.WriteLine(path);
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo fi in di.GetFiles())
                {
                    System.IO.File.Delete(fi.FullName);
                    System.Diagnostics.Debug.WriteLine(fi.Name);
                }

                di.Delete(true);
            }
        }

        public String DeleteFile(String file)
        {
            System.Diagnostics.Debug.WriteLine("DeleteFile");
            //    var req = HttpContext.Current;
            System.Diagnostics.Debug.WriteLine(file);

            String fullPath = Path.Combine(StorageRoot, file);
            System.Diagnostics.Debug.WriteLine(fullPath);
            System.Diagnostics.Debug.WriteLine(System.IO.File.Exists(fullPath));
            String thumbPath = "/" + file + "80x80.jpg";
            String partThumb1 = Path.Combine(StorageRoot, "thumbs");
            String partThumb2 = Path.Combine(partThumb1, file + "80x80.jpg");

            System.Diagnostics.Debug.WriteLine(partThumb2);
            System.Diagnostics.Debug.WriteLine(System.IO.File.Exists(partThumb2));
            if (System.IO.File.Exists(fullPath))
            {
                //delete thumb 
                if (System.IO.File.Exists(partThumb2))
                {
                    System.IO.File.Delete(partThumb2);
                }
                System.IO.File.Delete(fullPath);
                String succesMessage = "Ok";
                return succesMessage;
            }
            String failMessage = "Error Delete";
            return failMessage;
        }

        public class JsonFiles
        {
            public FilesUploadViewModel[] files;
            public string TempFolder { get; set; }
            public JsonFiles(List<FilesUploadViewModel> filesList)
            {
                files = new FilesUploadViewModel[filesList.Count];
                for (int i = 0; i < filesList.Count; i++)
                {
                    files[i] = filesList.ElementAt(i);
                }

            }
        }

        public JsonFiles GetFileList()
        {

            var r = new List<FilesUploadViewModel>();

            String fullPath = Path.Combine(StorageRoot);
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo dir = new DirectoryInfo(fullPath);
                foreach (FileInfo file in dir.GetFiles())
                {
                    int SizeInt = unchecked((int)file.Length);
                    r.Add(UploadResult(file.Name, SizeInt, fullPath+"/"+file.FullName));
                }

            }
            JsonFiles files = new JsonFiles(r);
            return files;
        }

        public FilesUploadViewModel UploadResult(String FileName, int fileSize, String FileFullPath)
        {
            String getType = System.Web.MimeMapping.GetMimeMapping(FileFullPath);
            var FileArray = FileName.Split('.');
            string FileExt = FileArray.LastOrDefault();
            string FileNameWithoutExt = "";
            for(var i=0;i<(FileArray.Length-1);i++)
            {
                FileNameWithoutExt += FileArray[i];
            }
            FileNameWithoutExt += CurrentTimeStr;
            var result = new FilesUploadViewModel()
            {
                FileName = FileNameWithoutExt+"."+FileExt,
                OriginalFileName=FileName,
                size = fileSize,
                type = getType,
                url = UrlBase + FileNameWithoutExt + "." + FileExt,
                deleteUrl = DeleteURL + FileName,
                thumbnailUrl = "/Content/thumbs/"+FileName+".jpeg",//CheckThumb(getType, FileName),
                deleteType = DeleteType,
            };
            return result;
        }

        public void UploadAndShowResults(HttpContextBase ContentBase, List<FilesUploadViewModel> resultList,bool SaveFileInBucket)
        {
            var httpRequest = ContentBase.Request;
            System.Diagnostics.Debug.WriteLine(Directory.Exists(S3TempPath));

            String fullPath = Path.Combine(StorageRoot);
            Directory.CreateDirectory(fullPath);
            // Create new folder for thumbs
            Directory.CreateDirectory(fullPath + "thumbs/");

            foreach (String inputTagName in httpRequest.Files)
            {

                var headers = httpRequest.Headers;

                var file = httpRequest.Files[inputTagName];
                System.Diagnostics.Debug.WriteLine(file.FileName);

                if (string.IsNullOrEmpty(headers["X-File-Name"]))
                {

                    UploadWholeFile(file, resultList,SaveFileInBucket,file.ContentLength);
                }
                else
                {

                    UploadPartialFile(headers["X-File-Name"], ContentBase, resultList);
                }
            }
        }
        private void UploadWholeFile(HttpPostedFileBase requestContext, List<FilesUploadViewModel> statuses,bool SaveFileInBucket,int ContentLength)
        {

            //var request = requestContext.Request;
            //for (int i = 0; i < request.Files.Count; i++)
            //{
            //var file = request.Files[i];
            var file = requestContext;
                String pathOnServer = Path.Combine(StorageRoot);
                if (!System.IO.Directory.Exists(pathOnServer))
                {
                    System.IO.Directory.CreateDirectory(pathOnServer);
                }

                var FileArray = file.FileName.Split('.');
                string FileExt = FileArray.LastOrDefault();
                string FileNameWithoutExt = "";
                for (var j = 0; j < (FileArray.Length - 1); j++)
                {
                    FileNameWithoutExt += FileArray[j];
                }
                FileNameWithoutExt +=CurrentTimeStr;
                FullFileName = FileNameWithoutExt + "." + FileExt;
                var fullPath = Path.Combine(pathOnServer, Path.GetFileName(FileNameWithoutExt+"."+FileExt));
               
                //Above 3 lines commented by VG and added below one line instead
                // bool FileUploadSucceeded = Global.UploadToS3(file);
                // bool getFiles = Global.DownloadFromS3();
                //using (MemoryStream target = new MemoryStream())
                //{
                if (SaveFileInBucket == true)
                {
                    //Belowsection will upload the file Uploaded using generic utility directlyinto bucket instead of saving to physical path
                    //Example File Format de/uploads/supporting_documents/abc_20171224_1715.ppt
                    var S3FilePath = S3TempPath + "/" + FullFileName;
                    Globals.UploadStreamToS3(file.InputStream, S3FilePath);
                }
                else
                {
                    //Save file in LocalStorage
                     file.SaveAs(fullPath);
                }
                // }

                //Create thumb
                string[] imageArray = file.FileName.Split('.');
                if (imageArray.Length != 0)
                {
                    String extansion = imageArray[imageArray.Length - 1].ToLower();
                //SS Created a new method to save thumbnail of the file and that would be displayed on the Supporting Doc Screen
                SaveImageThumbnail(file.FileName, fullPath);
                    //if (extansion != "jpg" && extansion != "png" && extansion != "jpeg") //Do not create thumb if file is not an image
                    //{

                //}
                //else
                //{
                //    var ThumbfullPath = Path.Combine(pathOnServer, "thumbs");
                //    //String fileThumb = file.FileName + ".80x80.jpg";
                //    String fileThumb = Path.GetFileNameWithoutExtension(file.FileName) + "80x80.jpg";
                //    var ThumbfullPath2 = Path.Combine(ThumbfullPath, fileThumb);
                //    using (MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(fullPath)))
                //    {
                //        var thumbnail = new WebImage(stream).Resize(80, 80);
                //        thumbnail.Save(ThumbfullPath2, "jpg");
                //    }

                //}
            }
                var FileSize = ContentLength;
                statuses.Add(UploadResult(file.FileName,FileSize, fullPath));
            //}
        }

        private void UploadPartialFile(string fileName, HttpContextBase requestContext, List<FilesUploadViewModel> statuses)
        {
            var request = requestContext.Request;
            if (request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];
            var inputStream = file.InputStream;
            String patchOnServer = Path.Combine(StorageRoot);
            var fullName = Path.Combine(patchOnServer, Path.GetFileName(file.FileName));
            var ThumbfullPath = Path.Combine(fullName, Path.GetFileName(file.FileName + "80x80.jpg"));
            ImageHandler handler = new ImageHandler();

            var ImageBit = ImageHandler.LoadImage(fullName);
            handler.Save(ImageBit, 80, 80, 10, ThumbfullPath);
            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            statuses.Add(UploadResult(file.FileName, file.ContentLength, file.FileName));
        }

   
        public String CheckThumb(String type, String FileName)
        {
            var splited = type.Split('/');
            if (splited.Length == 2)
            {
                string extansion = splited[1].ToLower();
                if (extansion.Equals("jpeg") || extansion.Equals("jpg") || extansion.Equals("png") || extansion.Equals("gif"))
                {
                    String thumbnailUrl = UrlBase + "thumbs/" + Path.GetFileNameWithoutExtension(FileName) + "80x80.jpg";
                    return thumbnailUrl;
                }
                //    else
                //    {
                //if (extansion.Equals("octet-stream")) //Fix for exe files
                //{
                //    return "/Content/Free-file-icons/48px/exe.png";

                //}
                //        if (extansion.Contains("zip")) //Fix for exe files
                //        {
                //            return "/Content/Free-file-icons/48px/zip.png";
                //        }
                //        String thumbnailUrl = "/Content/Free-file-icons/48px/" + extansion + ".png";
                //        return thumbnailUrl;
                //    }
            }
            
            return FileName;
            //else
            //{
            //    return UrlBase + "/thumbs/" + Path.GetFileNameWithoutExtension(FileName) + "80x80.jpg";
            //}

        }
        //SS added the following method to disply thumbnail to user
        private void SaveImageThumbnail(string FileName, string ThumbnailPath)
        {
            PointF firstLocation = new PointF(1f, 1f);
            string imageFilePath = HttpContext.Current.Server.MapPath("/Content/thumbs/"+FileName+".jpeg");
            int w = 80;
            int h = 80;

            System.Drawing.Color c = System.Drawing.Color.White;
            System.Drawing.Bitmap bt = new System.Drawing.Bitmap(w, h);
            System.Drawing.Graphics oGraphics = System.Drawing.Graphics.FromImage(bt);
            System.Drawing.Brush brush = new System.Drawing.SolidBrush(c);
            oGraphics.FillRectangle(brush, 0, 0, w, h);
            //oGraphics.DrawImage(img, 0, 0, img.Width, img.Height);
            oGraphics.DrawString(FileName, new Font("Arial", 8, FontStyle.Italic), SystemBrushes.WindowText, new PointF(5, 5));

            bt.Save(imageFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }

    public class JsonFiles
    {
        public FilesUploadViewModel[] files { get; set; }
        public string TempFolder { get; set; }
        public JsonFiles(List<FilesUploadViewModel> filesList)
        {
            files = new FilesUploadViewModel[filesList.Count];
            for (int i = 0; i < filesList.Count; i++)
            {
                files[i] = filesList.ElementAt(i);
            }

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public partial class FileUploadViewModel
    {
        public FileUploadViewModel()
        {
            //By Default we assume that this functionality is used for uploading Supporting Documents.
            //But if we encounter a situation when we are using same utility for Uploading Data and Supporting Doc, then ActivityType will be changed to Upload in that case.
            ActivityType = "Attachment";
        }
        //SS created this model to add uploaded file in Session Memory for Supporting Documents
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        //Possible Values:Upload and Attachment
        public string ActivityType { get; set; }
    }
}
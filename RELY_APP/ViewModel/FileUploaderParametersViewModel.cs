using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    //Below model will be used to pass information to partial view to upload documents
    public partial class FileUploaderParametersViewModel
    {
        public bool CanUploadMultipleFiles { get; set; }
        public bool CanDisplayDescriptionBox { get; set; }
        public bool SaveFileInBucket { get; set; }
        
        public List<string> ExistingFilesList { get; set; }
    }

    public partial class SectionItemsParametersViewModel
    {
        public string Width { get; set; }
        public string Height { get; set; }
        public bool Disabled { get; set; }

    }
}
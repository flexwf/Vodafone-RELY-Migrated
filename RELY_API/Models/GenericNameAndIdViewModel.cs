using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public partial class GenericNameAndIdViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public partial class DownloadFileNameViewModel
    {       
        public string FileName { get; set; }
    }
}
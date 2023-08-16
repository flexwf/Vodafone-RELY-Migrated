using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class RProductOfReportViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }
        public int SysCatId { get; set; }
        public string SysCat { get; set; }
        public Nullable<int> RequestId { get; set; }
    }
   
}
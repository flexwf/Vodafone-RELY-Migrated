using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LBatchViewModelGrid
    {
        public int Id { get; set; }
        public string XStatus { get; set; }
        public int XBatchNumber { get; set; }
        public string LbfFileName { get; set; }
        public int? XRecordCount { get; set; }
        public DateTime XUploadStartDateTime { get; set; }
        public int? IsImport { get; set; }
    }

    public partial class LBatchViewModelForRequestGrid
    {
        public int Id { get; set; }
        public string XStatus { get; set; }
        public int XBatchNumber { get; set; }
        public string LbfFileName { get; set; }
        public int? XRecordCount { get; set; }
        public DateTime XUploadStartDateTime { get; set; }
        public int? IsImport { get; set; }

    }
}
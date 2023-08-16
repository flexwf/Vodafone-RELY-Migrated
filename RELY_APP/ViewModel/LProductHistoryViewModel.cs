using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LProductHistoryViewModel
    {
        public string TableName { get; set; }
        public string SelecterType { get; set; }
        public string CompanyCode { get; set; }
        public bool DisplayOnForms { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string WfType { get; set; }
        public string Name { get; set; }
        public string ColumnName { get; set; }
        public string Label { get; set; }
    }
}
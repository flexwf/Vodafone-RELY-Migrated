using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class PostLReconColumnMappingViewModel
    {
        public string GridData { get; set; }
        public string CompanyCode { get; set; }
        public int SysCatId { get; set; }
        public int FileFormatId { get; set; }
        public int Id { get; set; }
        
        public string ColumnName { get; set; }
        public string Label { get; set; }
        public bool DisplayOnForm { get; set; }
        public int OrdinalPosition { get; set; }
        public bool IsProductCodeColumn { get; set; }
    }
}
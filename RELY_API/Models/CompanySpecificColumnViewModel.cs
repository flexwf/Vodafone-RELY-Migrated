using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public partial class CompanySpecificColumnViewModel
    {
        public string ColumnName { get; set; }
        public bool? IsMandatory { get; set; }
        public string DataType { get; set; }
        public int? OrdinalPosition { get; set; }
        public int? Id { get; set; }
        public string Label { get; set; }
        public bool? DisplayOnForms { get; set; }
        public string LdName { get; set; }
        public bool DisplayInGrid { get; set; }
        public bool AuditEnabled { get; set; }
        
        public int? DropDownId { get; set; }
        public string DefaultValue { get; set; }

        public int? MaximumLength { get; set; }

    }
}
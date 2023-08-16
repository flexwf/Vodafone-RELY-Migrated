
using RELY_APP.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class LCompanySpecificColumnViewModel
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = " Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Table Name")]
        [Required(ErrorMessage = "Table Name is required")]
        [MaxLength(255, ErrorMessage = " Table Name can be maximum 255 characters")]
        public string TableName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Column Name")]
        [Required(ErrorMessage = "Column Name is required")]
        [MaxLength(255, ErrorMessage = " Column Name can be maximum 255 characters")]
        public string ColumnName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Label")]

        [MaxLength(50, ErrorMessage = " Label can be maximum 50 characters")]
        public string Label { get; set; }

        [Display(Name = "Display OnForms")]
        [Required(ErrorMessage = "Display OnForms is required")]
        public bool DisplayOnForms { get; set; }

        [Display(Name = "Display In Grid")]
        [Required(ErrorMessage = "Display In Grid is required")]
        public bool DisplayInGrid { get; set; }


        [Display(Name = "AuditEnabled")]
        [Required(ErrorMessage = "AuditEnabled is required")]
        public bool AuditEnabled{ get; set; }

        [Display(Name = "Ordinal Position")]
        [Required(ErrorMessage = "Ordinal Position is required")]
        public int OrdinalPosition { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Export Header")]
        [MaxLength(255, ErrorMessage = " Export Header can be maximum 255 characters")]
        public string ExportHeader { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Default Value")]
        [MaxLength(255, ErrorMessage = " Default Value can be maximum 255 characters")]
        public string DefaultValue { get; set; }

        [Display(Name = "IsReportParameter")]
        [Required(ErrorMessage = "IsReportParameter is required")]
        public bool IsReportParameter { get; set; }

        public int ReportParameterOrdinal{get;set;}

        [RestrictSpecialChar]
        [Display(Name = "Data Type")]
        [Required(ErrorMessage = "Data Type is required")]
        [MaxLength(50, ErrorMessage = " Data Type can be maximum 50 characters")]
        public string DataType { get; set; }

        [Display(Name = "Maximum Length")]
        public Nullable<int> MaximumLength { get; set; }

        [Display(Name = "Digits AfterDecimal")]
        public Nullable<int> DigitsAfterDecimal { get; set; }

        [Display(Name = "IsMultiline")]
        [Required(ErrorMessage = "IsMultiline is required")]
        public bool IsMultiline { get; set; }

        [Display(Name = "IsMandatory")]
        [Required(ErrorMessage = "IsMandatory is required")]
        public bool IsMandatory { get; set; }

        [Display(Name = "DropDownId")]
        [Required(ErrorMessage = "DropDownId is required")]
        public Nullable<int> DropDownId { get; set; }

        [RestrictSpecialChar]
        public string SelecterType { get; set; }

        public string LdName { get; set; }
    }



    public partial class CompanYSpecificColumnForGridViewModel
    {
        public int OrdinalPosition { get; set; }
        public string ColumnName { get; set; }
        public string Label { get; set; }
        public string DataType { get; set; }
    }
}
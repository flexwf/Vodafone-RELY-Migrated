using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LReconColumnMappingViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = " Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [Display(Name = "File Format Id")]
        [Required(ErrorMessage = "FileFormatId is required")]
        public int FileFormatId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Column Name")]
        [Required(ErrorMessage = "Column Name is required")]
        [MaxLength(255, ErrorMessage = "The Column Name can be maximum 255 characters")]
        public string ColumnName { get; set; }

        [Display(Name = "Label")]
        [MaxLength(50, ErrorMessage = "Label can be maximum 50 characters")]
        public string Label { get; set; }

        public string FormatName { get; set; }
        public bool DisplayOnForm { get; set; }
        public int OrdinalPosition { get; set; }

        public bool IsProductCodeColumn { get; set; }

        public int SysCatId { get; set; }
    }
}
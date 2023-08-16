using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LReconFileFormatsViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = " Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [Display(Name = "SysCatId")]
        [Required(ErrorMessage = "SysCatId is required")]
        public int SysCatId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Format Name")]
        [Required(ErrorMessage = "Format Name is required")]
        [MaxLength(50, ErrorMessage = "The Format Name can be maximum 50 characters")]
        public string FormatName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "SysCat")]
        [Required(ErrorMessage = "SysCat is required")]
        [MaxLength(255, ErrorMessage = "The SysCat can be maximum 255 characters")]
        public string SysCat { get; set; }
    }
}
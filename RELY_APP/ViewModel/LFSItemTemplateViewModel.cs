using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LFSItemTemplateViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code  is required")]
        [MaxLength(2, ErrorMessage = " Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name  is required")]
        [MaxLength(255, ErrorMessage = " Question Name can be maximum 255 characters")]
        public string Name { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "HTML Text")]
        [Required(ErrorMessage = "HTML Text  is required")]
        public string HTMLText { get; set; }
    }
}
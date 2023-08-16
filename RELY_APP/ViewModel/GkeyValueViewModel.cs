using RELY_APP.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public partial class GKeyValueViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Key")]
        [Required(ErrorMessage = "Key is required")]
        [MaxLength(255, ErrorMessage = "Key can be maximum 255 characters")]
        public string Key { get; set; }

        //[RestrictSpecialChar] -- by RS as allowing Value to have special characters as per requirement
        [Display(Name = "Value")]
        [Required(ErrorMessage = "Value is required")]
        [MaxLength(4000, ErrorMessage = "Value can be maximum 4000 characters")]
        public string Value { get; set; }

        // [RestrictSpecialChar] -- by RS as allowing description to have special characters  as per requirement
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(500, ErrorMessage = "Description can be maximum 500 characters")]
        public string Description { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "Company Code can be maximum 2 characters")]

        public string CompanyCode { get; set; }

        public int CompanyId { get; set; } //added by RS for the requirement in L2Admin Page
        
    }
}
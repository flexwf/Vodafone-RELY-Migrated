using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class RSysCatViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "System ")]
        [Required(ErrorMessage = "System is required")]
        [MaxLength(255, ErrorMessage = "System can be maximum 255 characters")]
        public string System { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Category")]
        [Required(ErrorMessage = "Category is required")]
        [MaxLength(255, ErrorMessage = "Category can be maximum 255 characters")]
        public string Category { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "System Category")]
        [Required(ErrorMessage = "System Category is required")]
        [MaxLength(255, ErrorMessage = "System Category can be maximum 255 characters")]
        public string SysCat { get; set; }

        [Display(Name = "SystemId")]
        [Required(ErrorMessage = "SystemId is required")]
        public int SystemId { get; set; }

        [Display(Name = "CategoryId")]
        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }

        public string CatName { get; set; }
        public string SysName { get; set; }

        [Display(Name = "SysCatCode")]
        [Required(ErrorMessage = "System Category code is required")]
        public string SysCatCode { get; set; }

    }
}
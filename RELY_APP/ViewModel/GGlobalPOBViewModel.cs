using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class GGlobalPOBViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Type")]
        [Required(ErrorMessage = "Type is required")]
        [MaxLength(255, ErrorMessage = "Type can be maximum 255 characters")]
        public string Type { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(255, ErrorMessage = "Name can be maximum 255 characters")]
        public string Name { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Description")]
        [MaxLength(2000, ErrorMessage = "Description can be maximum 2000 characters")]
        public string Description { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Category")]
        [Required(ErrorMessage = "Category is required")]
        [MaxLength(255, ErrorMessage = "Category can be maximum 255 characters")]
        public string Category { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "IFRS15 Account")]
        [Required(ErrorMessage = "IFRS15 Account is required")]
        [MaxLength(255, ErrorMessage = "IFRS15 Account can be maximum 255 characters")]
        public string IFRS15Account { get; set; }

        [Display(Name = "CreatedBy Id")]
        [Required(ErrorMessage = "CreatedBy Id is required")]
        public int CreatedById { get; set; }

        [Display(Name = "Created DateTime")]
        [Required(ErrorMessage = "Created DateTime is required")]
        public System.DateTime CreatedDateTime { get; set; }

        [Display(Name = "UpdatedBy Id")]
        [Required(ErrorMessage = "UpdatedBy Id is required")]
        public int UpdatedById { get; set; }

        [Display(Name = "Updated DateTime")]
        [Required(ErrorMessage = "Updated DateTime is required")]
        public System.DateTime UpdatedDateTime { get; set; }

        public string Label { get; set; }
        public string CompanyCode { get; set; }
    }
}
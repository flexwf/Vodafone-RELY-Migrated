using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class GCopaDimensionViewModel
    {
        public string CompanyCode {   get; set; }

        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Display(Name = "Class")]
        [Required(ErrorMessage = "Class is required")]
        public int Class { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Copa Value")]
        [Required(ErrorMessage = "Copa Value is required")]
        [MaxLength(255, ErrorMessage = "Copa Value can be maximum 255 characters")]
        public string CopaValue { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "DimentionClassDescription")]
        [Required(ErrorMessage = "DimentionClassDescription is required")]
        [MaxLength(255, ErrorMessage = "DimentionClassDescription can be maximum 255 characters")]
        public string DimentionClassDescription { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(255, ErrorMessage = "Description can be maximum 255 characters")]
        public string Description { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Dimension")]
        [Required(ErrorMessage = "Dimension is required")]
        [MaxLength(255, ErrorMessage = "Dimension can be maximum 255 characters")]
        public string Dimension { get; set; }

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

    }
}
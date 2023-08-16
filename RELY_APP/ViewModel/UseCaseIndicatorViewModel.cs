using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class UseCaseIndicatorViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }
        
        [RestrictSpecialChar]
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(255, ErrorMessage = "Description can be maximum 255 characters")]
        public string Description { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Indicator")]
        [Required(ErrorMessage = "Indicator is required")]
        [MaxLength(255, ErrorMessage = "Indicator can be maximum 255 characters")]
        public string Indicatior { get; set; }


        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start Date is required")]
        public System.DateTime EffectiveStartDate { get; set; }


        [Display(Name = "End Date")]
        [Required(ErrorMessage = "End Date is required")]
        public System.DateTime EffectiveEndDate { get; set; }
        
    }
}
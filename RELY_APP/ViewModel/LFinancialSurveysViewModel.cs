using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LFinancialSurveysViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code  is required")]
        [MaxLength(2, ErrorMessage = " Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [Display(Name = "SurveyLevel Id")]
        [Required(ErrorMessage = "SurveyLevel Id is required")]
        public int SurveyLevelId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Survey Name")]
        [Required(ErrorMessage = "Survey Name is required")]
        [MaxLength(255, ErrorMessage = "Survey Name can be maximum 255 characters")]
        public string SurveyName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Description")]
        [MaxLength(2000, ErrorMessage = " Description can be maximum 4000 characters")]
        public string Description { get; set; }
        [Display(Name = "Created By Id")]
        [Required(ErrorMessage = "CreatedById is required")]
        public int CreatedById { get; set; }


        [Display(Name = "Created Date Time")]
        [Required(ErrorMessage = "Created Date Time is Required.")]
        public DateTime CreatedDateTime { get; set; }

        [Display(Name = "Updated By Id")]
        [Required(ErrorMessage = "UpdatedById is required")]
        public int UpdatedById { get; set; }


        [Display(Name = "Updated Date Time")]
        [Required(ErrorMessage = "Updated Date Time is Required.")]
        public DateTime UpdatedDateTime { get; set; }

        [Display(Name = "IsActive")]
        //[Required(ErrorMessage = "IsActive is Required.")]
        //[Range(typeof(bool), "", "false", ErrorMessage = "IsActive is Required.")]
        public bool IsActive { get; set; }

        //[RestrictSpecialChar]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(255, ErrorMessage = "Name can be maximum 255 characters")]
        public string Name { get; set; }
    }
}
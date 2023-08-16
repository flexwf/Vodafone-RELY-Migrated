using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LNotificationViewModel
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
        [Display(Name = "Email Type")]
         [Required(ErrorMessage = "Type is required")]
        [MaxLength(20, ErrorMessage = " Type can be maximum 20 characters")]
        public string Type { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Description")]
        [MaxLength(255, ErrorMessage = " Description can be maximum 255 characters")]
        public string Description { get; set; }

        [Display(Name = "RemindAfterDays")]
        public int? RemindAfterDays { get; set; }

        [Display(Name = "TemplateId")]
        [Required(ErrorMessage = "TemplateId is required")]
        public int TemplateId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Email Template")]
        [Required(ErrorMessage = "Template Name is required")]
        [MaxLength(255, ErrorMessage = " Template Name can be maximum 255 characters")]
        public string TemplateName { get; set; }

        [Display(Name = "LandingStepId")]
        [Required(ErrorMessage = "LandingStepId is required")]
        public int LandingStepId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Landing Step")]
        [Required(ErrorMessage = "Step Name is required")]
        [MaxLength(50, ErrorMessage = " Name can be maximum 50 characters")]
        public string Name { get; set; }

        [Display(Name = "RecipientRoleId")]
        [Required(ErrorMessage = "RecipientRoleId is required")]
        public int RecipientRoleId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Recipient Role")]
        [Required(ErrorMessage = "RoleName is required")]
        [MaxLength(255, ErrorMessage = "RoleName can be maximum 255 characters")]
        public string RoleName { get; set; }

        [Display(Name = "Originating Step")]
        public string OriginatingStepName { get; set; }
        public int OriginatingStepId { get; set; }
        public string Workflow { get; set; }
        public int WorkFlowId { get; set; }

    }
}
using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class CreateCompanyViewModel
    {

        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "Company Name is required")]
        [MaxLength(255, ErrorMessage = " Company Name can be maximum 255 characters")]
        public string CompanyName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = " Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }


        [MaxLength(4000)]
        [Display(Name = "Logo")]
        public string LogoPath { get; set; }

        [MaxLength(4000)]
        [Display(Name = "PunchLine")]
        public string PunchLine { get; set; }


        [Display(Name = "Domain Address")]
        [Required(ErrorMessage = "DomainAddress is required")]
        [MaxLength(500)]
        public string DomainAddress { get; set; }

       

        [Display(Name = "Min Length")]
        [Required(ErrorMessage = "MinLength is required")]
        public int MinLength { get; set; }

        [Display(Name = "Min UpperCase")]
        [Required(ErrorMessage = "MinUpperCase is required")]
        public int MinUpperCase { get; set; }


        [Display(Name = "Min LowerCase")]
        [Required(ErrorMessage = "MinLowerCase is required")]
        public int MinLowerCase { get; set; }

        [Display(Name = "Min Numbers")]
        [Required(ErrorMessage = "MinNumbers is required")]
        public int MinNumbers { get; set; }

        [Display(Name = "Min Special Chars")]
        [Required(ErrorMessage = "MinSpecialChars is required")]
        public int MinSpecialChars { get; set; }

        [Display(Name = "MinAgeDays")]
        [Required(ErrorMessage = "MinAgeDays is required")]
        public int MinAgeDays { get; set; }

        [Display(Name = "Max AgeDays")]
        [Required(ErrorMessage = "MaxAgeDays is required")]
        public int MaxAgeDays { get; set; }

        [Display(Name = "ReminderDays")]
        [Required(ErrorMessage = "ReminderDays is required")]
        public int ReminderDays { get; set; }

        [Display(Name = "Prevent Reuse")]
        [Required(ErrorMessage = "PreventReuse is required")]
        public int PreventReuse { get; set; }


        [Display(Name = "LockoutFailedAttempts")]
        [Required(ErrorMessage = "LockoutFailedAttempts is required")]
        public int LockoutFailedAttempts { get; set; }

        [Display(Name = "LockoutMinutes")]
        [Required(ErrorMessage = "LockoutMinutes is required")]
        public int LockoutMinutes { get; set; }

        


    }
}
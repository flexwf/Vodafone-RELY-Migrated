using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public partial class LPasswordPolicyViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [Display(Name = "MinLength")]
        [Required(ErrorMessage = "MinLength is required")]
        public int MinLength { get; set; }

        [Display(Name = "MinUpperCase")]
        [Required(ErrorMessage = "MinUpperCase is required")]
        public int MinUpperCase { get; set; }


        [Display(Name = "MinLowerCase")]
        [Required(ErrorMessage = "MinLowerCase is required")]
        public int MinLowerCase { get; set; }

        [Display(Name = "MinNumbers")]
        [Required(ErrorMessage = "MinNumbers is required")]
        public int MinNumbers { get; set; }

        [Display(Name = "MinSpecialChars")]
        [Required(ErrorMessage = "MinSpecialChars is required")]
        public int MinSpecialChars { get; set; }

        [Display(Name = "MinAgeDays")]
        [Required(ErrorMessage = "MinAgeDays is required")]
        public int MinAgeDays { get; set; }

        [Display(Name = "MaxAgeDays")]
        [Required(ErrorMessage = "MaxAgeDays is required")]
        public int MaxAgeDays { get; set; }

        [Display(Name = "ReminderDays")]
        [Required(ErrorMessage = "ReminderDays is required")]
        public int ReminderDays { get; set; }

        [Display(Name = "PreventReuse")]
        [Required(ErrorMessage = "PreventReuse is required")]
        public int PreventReuse { get; set; }


        [Display(Name = "LockoutFailedAttempts")]
        [Required(ErrorMessage = "LockoutFailedAttempts is required")]
        public int LockoutFailedAttempts { get; set; }

        [Display(Name = "LockoutMinutes")]
        [Required(ErrorMessage = "LockoutMinutes is required")]
        public int LockoutMinutes { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Status")]
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(255, ErrorMessage = "Status can be maximum 255 characters")]
        public string Status { get; set; }

        //Added to get how many days left to expire password
        public int DaysToExpirePassword { get; set; }


    }
}
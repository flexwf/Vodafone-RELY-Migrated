using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string Id { get; set; }
        public List<LRoleViewModel> Roles { get; set; }
        public string FullName { get; set; }
        public string CompanyCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string LastLoginMessage { get; set; }
        public string MFAOTP { get; set; }
        public string LastActiveUrl { get; set; }
        public string ClientIPAddress { get; set; }

    }

    public class ChangePasswordBindingModel
    {

        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string Password { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Question Id")]
        [Required(ErrorMessage = "Question is Required")]
        public int QuestionId { get; set; }//name need to be updated after mapping table creation

        public string Question { get; set; }
        [Required(ErrorMessage = "Question1 is required")]

        [Display(Name = "Question1")]
        public int Question1 { get; set; }
        [Display(Name = "Question2")]
        [Required(ErrorMessage = "Question2 is required")]

        public int Question2 { get; set; }
        [Display(Name = "Question3")]
        [Required(ErrorMessage = "Question3 is required")]

        public int Question3 { get; set; }
        [RestrictSpecialChar]
        [Required(ErrorMessage = "Answer1 is required")]
        [MaxLength(200, ErrorMessage = "Answer1 can be maximum 200 characters")]

        public string Answer1 { get; set; }
        [RestrictSpecialChar]
        [Required(ErrorMessage = "Answer2 is required")]
        [MaxLength(200, ErrorMessage = "Answer2 can be maximum 200 characters")]

        public string Answer2 { get; set; }
        [Required(ErrorMessage = "Answer3 is required")]
        [RestrictSpecialChar]
        [MaxLength(200, ErrorMessage = "Answer3  can be maximum 200 characters")]
        public string Answer3 { get; set; }
        //public string MAugsqAnswer { get; set; }
        public string UserId { get; set; }

        //for forgot password utility
        [MaxLength(8)]
        public string OTP { get; set; }
        public string OTPValidUpto { get; set; }


    }

    public class ChangePasswordViewModel
    {

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            public string UserId { get; set; }
            public int Id { get; set; }
            [Required]
            public string Question { get; set; }
            public string Answer { get; set; }
        
            [Display(Name = "OTP")]
            [StringLength(8)]
            public string OTP { get; set; }
            public DateTime OTPValidUpto { get; set; }

            

    }

    }
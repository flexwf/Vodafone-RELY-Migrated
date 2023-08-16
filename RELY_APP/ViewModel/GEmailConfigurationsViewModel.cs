using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class GEmailConfigurationsViewModel
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Account Name")]
        [Required(ErrorMessage = "Account Name is required")]
        [MaxLength(255, ErrorMessage = " Account Name  can be maximum 255 characters")]
        public string AccountName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Account Description")]
        [MaxLength(500, ErrorMessage = " Account Description  can be maximum 500 characters")]
        public string AccountDescription { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Email Id")]
        [Required(ErrorMessage = "Email Id is required")]
        [MaxLength(255, ErrorMessage = "Email Id  can be maximum 255 characters")]
        public string EmailId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Display Name")]
        [Required(ErrorMessage = "Display Name is required")]
        [MaxLength(255, ErrorMessage = "Display Name  can be maximum 255 characters")]
        public string DisplayName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Reply To")]
        [MaxLength(255, ErrorMessage = " Reply To  can be maximum 255 characters")]
        public string ReplyTo { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Smtp Host")]
        [Required(ErrorMessage = "Smtp Host is required")]
        [MaxLength(1000, ErrorMessage = "Smtp Host  can be maximum 1000 characters")]
        public string SmtpHost { get; set; }


        [Required(ErrorMessage = "Port Number is required")]
        [Display(Name = "Port Number")]
        public int PortNumber { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Smtp LoginId")]
        [Required(ErrorMessage = "Smtp LoginId is required")]
        [MaxLength(500, ErrorMessage = "Smtp LoginId  can be maximum 500 characters")]
        public string SmtpLoginId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Smtp Password")]
        [Required(ErrorMessage = "Smtp Password is required")]
        [MaxLength(255, ErrorMessage = "Smtp Password  can be maximum 255 characters")]
        public string SmtpPassword { get; set; }

        [Display(Name = "Requires SSL")]
        [Required(ErrorMessage = "Requires SSL is required")]
        public bool RequiresSSL { get; set; }
        
    }
}
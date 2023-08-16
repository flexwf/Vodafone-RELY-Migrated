using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public partial class LEmailTemplateViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Template Name")]
        [Required(ErrorMessage = "Template Name is required")]
        [MaxLength(255, ErrorMessage = "Template Name can be maximum 255 characters")]
        public string TemplateName { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Email Subject")]
        [Required(ErrorMessage = "Email Subject is required")]
        [MaxLength(500, ErrorMessage = "Email Subject can be maximum 500 characters")]
        public string EmailSubject { get; set; }



        [RestrictSpecialChar]
        [Display(Name = "Email Body")]
        [Required(ErrorMessage = "Email Body is required")]
        public string EmailBody { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Signature")]
        //[Required(ErrorMessage = "Signature is required")]
        [MaxLength(4000, ErrorMessage = "Signature can be maximum 4000 characters")]
        public string Signature { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        
   

    }
}

using System.ComponentModel.DataAnnotations;
using RELY_APP.Utilities;

namespace RELY_APP.ViewModel
{
    public class GCompanyViewModel
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
        public string LogoPath { get; set; }

        [MaxLength(4000)]
        public string PunchLine { get; set; }



    }
}
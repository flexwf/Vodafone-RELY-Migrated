using System.ComponentModel.DataAnnotations;


namespace RELY_APP.ViewModel
{
    public class RelyDataDownload
    {
        
        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "Company Name is required")]       
        public string CompanyName { get; set; }

        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }
        
        [Display(Name = "CompanyCode")]
        [Required(ErrorMessage = "Company Code is required")]       
        public string CompanyCode { get; set; }

        
        [Display(Name = "LogoPath")]
        [MaxLength(255, ErrorMessage = " Logo path can be maximum 255 characters")]
        public string LogoPath { get; set; }

        [Display(Name = "PunchLine")]
        [MaxLength(255, ErrorMessage = " PunchLine can be maximum 255 characters")]
        public string PunchLine { get; set; }

        public string ZipFileName { get; set; }

    }
}
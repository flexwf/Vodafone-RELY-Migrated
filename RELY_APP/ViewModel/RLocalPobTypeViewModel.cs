using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;


namespace RELY_APP.ViewModel
{
    public class RLocalPobTypeViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Name ")]
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(255, ErrorMessage = "Name can be maximum 255 characters")]
        public string Name { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Description")]
        [MaxLength(2000, ErrorMessage = "Description can be maximum 2000 characters")]
        public string Description { get; set; }
    }
}
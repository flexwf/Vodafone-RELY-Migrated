using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;


namespace RELY_APP.ViewModel
{
    public partial class LDropDownViewModel
    {
        public int Id { get; set; }
        public string CompanyCode { get; set; }

        [RestrictSpecialChar]       
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(255, ErrorMessage = "The Name can be maximum 255 characters")]
        public string Name { get; set; }

        [RestrictSpecialChar]
        public string Description { get; set; }
    }
}
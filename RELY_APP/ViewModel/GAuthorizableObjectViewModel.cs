using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{

    public partial class GAuthorizableObjectViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Controller Name")]
        [Required(ErrorMessage = "Controller Name is required")]
        [MaxLength(255, ErrorMessage = "The Controller Name can be maximum 255 characters")]
        public string ControllerName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Controller Method Name")]
        [Required(ErrorMessage = "Method Name is required")]
        [MaxLength(255, ErrorMessage = "The Method Name can be maximum 255 characters")]
        public string MethodName { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Authorizable Object Discription ")]
        [MaxLength(500, ErrorMessage = "The Authorizable Object Description can be maximum 500 characters")]
        public string Description { get; set; }


      
    }
}
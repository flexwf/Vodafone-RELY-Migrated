using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;


namespace RELY_APP.ViewModel
{
    public partial class GSecurityQuestionViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Question")]
        [Required(ErrorMessage = "Question is required")]
        [MaxLength(4000, ErrorMessage = "The Question can be maximum 4000 characters")]
        public string Question { get; set; }

       

    }
}
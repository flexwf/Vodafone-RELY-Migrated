

using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class MLUsersGSecurityQuestionViewModel
    {
        public int Id { get; set; }

       
        public int UserId { get; set; }

       
        public int QuestionId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Answer")]
        [MaxLength(255, ErrorMessage = "Answer can be maximum 255 characters")]
        public string Answer { get; set; }

        
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
        [RestrictSpecialChar]
        [Display(Name = "Questions")]
        [MaxLength(4000, ErrorMessage = "The Questions  can be maximum 4000 characters")]
        [Required]
        public string Question { get; set; }

    }
}
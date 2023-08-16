using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class MUserRoleViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Display(Name = "User Id")]
        [Required(ErrorMessage = "User Id is required")]
        public int UserId { get; set; }

        [Display(Name = "Role Id")]
        [Required(ErrorMessage = "Role Id is required")]
        public int RoleId { get; set; }

    }
}
using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{

    public partial class LRoleViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company  Code")]
        [Required(ErrorMessage = "Company  Code is required")]
        [MaxLength(2, ErrorMessage = "The Controller Name can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Role Name")]
        [Required(ErrorMessage = "Role Name is required")]
        [MaxLength(255, ErrorMessage = "The Role Name can be maximum 255 characters")]
        public string RoleName { get; set; }
        public bool Select { get; set; }

        public bool IsDefault { get; set; }

    }

    public  partial class PageAccessRoleViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
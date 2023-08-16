using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public partial class MAuthorizableObjectsRoleViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Display(Name = "Authorizable Object Id")]
        [Required(ErrorMessage = "Authorizable Object Id is required")]
        public int AuthorizableObjectId { get; set; }

        [Display(Name = "Role Id")]
        [Required(ErrorMessage = "Role Id is required")]
        public int RoleId { get; set; }

    }

    public partial class UpdateMappingAuthorizableObjectRoleViewModel
    {
        public int AuthorizableId { get; set; }

        public string ColumnName { get; set; }

        public bool NewResponse { get; set; }

        public bool OldResponse { get; set; }


    }
}
using RELY_APP.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public partial class MMenuRoleViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Display(Name = "Menu Id")]
        //Need to confirm which annoattaions are to be used here, as the members are Foreign key
        [Required(ErrorMessage = "Menu Id is required")]
        public int MenuId { get; set; }

        [Display(Name = "Role Id")]
        [Required(ErrorMessage = "Role Id is required")]
        public int RoleId { get; set; }

        public string MenuName { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string MenuURL { get; set; }
        public int OrdinalPosition { get; set; }

    }

    public partial class UpdateMappingMenuRoleViewModel
    {
        public int MenuId { get; set; }

        public string ColumnName { get; set; }

        public bool NewResponse { get; set; }

        public bool OldResponse { get; set; }


    }
}
using System.ComponentModel.DataAnnotations;
using RELY_APP.Utilities;
using System;

namespace RELY_APP.ViewModel
{
    public partial class GMenuViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Display(Name = "Parent Id")]
        public Nullable<int> ParentId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Menu Name")]
        [Required(ErrorMessage = "Menu Name is required")]
        [MaxLength(255, ErrorMessage = " Menu Name can be maximum 255 characters")]
        public string MenuName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Menu URL")]
        [MaxLength(2000, ErrorMessage = "The Menu URL  can be maximum 2000 characters")]
        public string MenuURL { get; set; }

        [Display(Name = "Ordinal Position")]
        [Required(ErrorMessage = "Ordinal Position is required")]
        public int OrdinalPosition { get; set; }

        public string CompanyCode { get; set; }
    }
}
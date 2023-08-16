using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LDropDownValueViewModel
    {
        public int Id { get; set; }
        public int DropDownId { get; set; }

        [RestrictSpecialChar]
        public string Description { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Value")]
        [Required(ErrorMessage = "Value is required")]
        [MaxLength(255, ErrorMessage = "The Value can be maximum 255 characters")]
        public string Value { get; set; }
    }
}
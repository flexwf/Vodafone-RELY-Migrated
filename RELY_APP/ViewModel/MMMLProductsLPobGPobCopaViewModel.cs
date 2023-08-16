using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class MMMLProductsLPobGPobCopaViewModel
    {

        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "MMLProductsLPobGPob Id")]
        [Required(ErrorMessage = "MMLProductsLPobGPob Id is required")]
        public int MMLProductsLPobGPobId { get; set; }

        [Display(Name = "CopaDimension Id")]
        [Required(ErrorMessage = "CopaDimension Id is required")]
        public int CopaDimensionId { get; set; }
    }
}
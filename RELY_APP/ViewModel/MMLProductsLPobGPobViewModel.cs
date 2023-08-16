using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class MMLProductsLPobGPobViewModel
    {

        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "MLProductsLPob Id")]
        [Required(ErrorMessage = "MLProductsLPob Id is required")]
        public int MLProductsLPobId { get; set; }

        [Display(Name = "GPob Id")]
        [Required(ErrorMessage = "GPob Id is required")]
        public int GPobId { get; set; }

        [Display(Name = "UseCaseIndicator Id")]
        [Required(ErrorMessage = "UseCaseIndicator Id is required")]
        public int UseCaseIndicatorId { get; set; }

    }
}
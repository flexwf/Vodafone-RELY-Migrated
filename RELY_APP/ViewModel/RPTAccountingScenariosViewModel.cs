using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class RPTAccountingScenariosViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(255, ErrorMessage = "Name can be maximum 255 characters")]
        public string Name { get; set; }


        public string CreateDate { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Product")]
        [Required(ErrorMessage = "Product is required")]
        [MaxLength(255, ErrorMessage = "Product can be maximum 255 characters")]
        public string Product { get; set; }

        public string Scenario { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 1")]
        [MaxLength(255, ErrorMessage = "Attribute 1 can be maximum 255 characters")]
        public string AttributeC01 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 2")]
        [MaxLength(255, ErrorMessage = "Attribute 2 can be maximum 255 characters")]
        public string AttributeC02 { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start Date is required")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        [Required(ErrorMessage = "End Date is required")]
        public string EndDate { get; set; }
        public string ScenarioCategory { get; set; }
        public string ScenarioTrigger { get; set; }
    }
}
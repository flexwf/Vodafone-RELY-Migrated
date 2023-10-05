using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class SSPDimensionViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Display(Name = "SSP Id")]
        [Required(ErrorMessage = "SSP Id is required")]
        public int SspId { get; set; }

        [Display(Name = "SSP Amount")]
        //[Required(ErrorMessage = "SSP Amount is required")]
        public decimal? SspAmount { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start Date is required")]
        public System.DateTime EffectiveStartDate { get; set; }

        [Display(Name = "End Date")]
        [Required(ErrorMessage = "End Date is required")]
        public System.DateTime EffectiveEndDate { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }
        public int CreatedById { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        public System.DateTime UpdatedDateTime { get; set; }
        
    }

    /// <summary>
    /// Created XSSPDimensionViewModel because of Operation Column added in New XSSPDimensions Table
    /// </summary>
    public class XSSPDimensionViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Display(Name = "SSP Id")]
        [Required(ErrorMessage = "SSP Id is required")]
        public int SspId { get; set; }

        [Display(Name = "SSP Amount")]
        public decimal? SspAmount { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start Date is required")]
        public System.DateTime EffectiveStartDate { get; set; }

        [Display(Name = "End Date")]
        [Required(ErrorMessage = "End Date is required")]
        public System.DateTime EffectiveEndDate { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }
        [RestrictSpecialChar]
        [Display(Name = "Operation")]
        [Required(ErrorMessage = "Operation is required")]
        [MaxLength(2, ErrorMessage = "Operation can be maximum 1 character")]
        public string Operation { get; set; }

        public bool SaveFileInBucket { get; set; }

        public int CreatedById { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        public System.DateTime UpdatedDateTime { get; set; }

    }
}
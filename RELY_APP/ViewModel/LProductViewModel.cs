using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class LProductViewModel
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
        [Display(Name = "Product Code")]
        // [Required(ErrorMessage = "Product Code is required")]
        [MaxLength(255, ErrorMessage = "Product Code can be maximum 255 characters")]
        public string ProductCode { get; set; }

        public string Product { get; set; } //used for Request form dropdown
        public string SysCat { get; set; } //used for Request form Grid
        public string SelecterType { get; set; } //used for Request form Grid
        public string Source { get; set; } //to be used for referencing Source of Product forms

        [RestrictSpecialChar]
        [Display(Name = "Description")]
       // [Required(ErrorMessage = "Name is required")]//CR1.5 - its description and nullable
        [MaxLength(255, ErrorMessage = "Description can be maximum 255 characters")]
        public string Name { get; set; }

        [Display(Name = "Version")]
        [Required(ErrorMessage = "Version is required")]
        public int Version { get; set; }

        public string CategoryName { get; set; }

        [Display(Name = "RequestId")]
        public Nullable<int> RequestId { get; set; }

        [Display(Name = "SystemCategory")]
        public int SysCatId { get; set; }

        public string ErrorMessage { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Attribute 1")]
        [MaxLength(255, ErrorMessage = "Attribute 1 can be maximum 255 characters")]
        public string AttributeC01 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 2")]
        [MaxLength(255, ErrorMessage = "Attribute 2 can be maximum 255 characters")]
        public string AttributeC02 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 3")]
        [MaxLength(255, ErrorMessage = "Attribute 3 can be maximum 255 characters")]
        public string AttributeC03 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 4")]
        [MaxLength(255, ErrorMessage = "Attribute 4 can be maximum 255 characters")]
        public string AttributeC04 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 5")]
        [MaxLength(255, ErrorMessage = "Attribute 5 can be maximum 255 characters")]
        public string AttributeC05 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 6")]
        [MaxLength(255, ErrorMessage = "Attribute 6 can be maximum 255 characters")]
        public string AttributeC06 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 7")]
        [MaxLength(255, ErrorMessage = "Attribute 7 can be maximum 255 characters")]
        public string AttributeC07 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 8")]
        [MaxLength(255, ErrorMessage = "Attribute 8 can be maximum 255 characters")]
        public string AttributeC08 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 9")]
        [MaxLength(255, ErrorMessage = "Attribute 9 can be maximum 255 characters")]
        public string AttributeC09 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 10")]
        [MaxLength(255, ErrorMessage = "Attribute 10 can be maximum 255 characters")]
        public string AttributeC10 { get; set; }
        public string AttributeC11 { get; set; }
        public string AttributeC12 { get; set; }
        public string AttributeC13 { get; set; }
        public string AttributeC14 { get; set; }
        public string AttributeC15 { get; set; }
        public string AttributeC16 { get; set; }
        public string AttributeC17 { get; set; }
        public string AttributeC18 { get; set; }
        public string AttributeC19 { get; set; }
        public string AttributeC20 { get; set; }
        public string AttributeM01 { get; set; }
        public string AttributeM02 { get; set; }
        public string AttributeM03 { get; set; }
        public string AttributeM04 { get; set; }
        public string AttributeM05 { get; set; }
        public Nullable<int> AttributeI01 { get; set; }
        public Nullable<int> AttributeI02 { get; set; }
        public Nullable<int> AttributeI03 { get; set; }
        public Nullable<int> AttributeI04 { get; set; }
        public Nullable<int> AttributeI05 { get; set; }
        public Nullable<int> AttributeI06 { get; set; }
        public Nullable<int> AttributeI07 { get; set; }
        public Nullable<int> AttributeI08 { get; set; }
        public Nullable<int> AttributeI09 { get; set; }
        public Nullable<int> AttributeI10 { get; set; }

        [Display(Name = "Attribute 11")]
        public Nullable<decimal> AttributeN01 { get; set; }


        [Display(Name = "Attribute 12")]
        public Nullable<decimal> AttributeN02 { get; set; }


        [Display(Name = "Attribute 13")]
        public Nullable<decimal> AttributeN03 { get; set; }


        [Display(Name = "Attribute 14")]
        public Nullable<decimal> AttributeN04 { get; set; }



        [Display(Name = "Attribute 15")]
        public Nullable<decimal> AttributeN05 { get; set; }

        [Display(Name = "Attribute 16")]
        public Nullable<decimal> AttributeN06 { get; set; }


        [Display(Name = "Attribute 17")]
        public Nullable<decimal> AttributeN07 { get; set; }


        [Display(Name = "Attribute 18")]
        public Nullable<decimal> AttributeN08 { get; set; }

        [Display(Name = "Attribute 19")]
        public Nullable<decimal> AttributeN09 { get; set; }


        [Display(Name = "Attribute 20")]
        public Nullable<decimal> AttributeN10 { get; set; }

        [Display(Name = "Attribute 21")]
        public Nullable<System.DateTime> AttributeD01 { get; set; }

        [Display(Name = "Attribute 22")]
        public Nullable<System.DateTime> AttributeD02 { get; set; }

        [Display(Name = "Attribute 23")]
        public Nullable<System.DateTime> AttributeD03 { get; set; }

        [Display(Name = "Attribute 24")]
        public Nullable<System.DateTime> AttributeD04 { get; set; }

        [Display(Name = "Attribute 25")]
        public Nullable<System.DateTime> AttributeD05 { get; set; }
        public Nullable<System.DateTime> AttributeD06 { get; set; }
        public Nullable<System.DateTime> AttributeD07 { get; set; }
        public Nullable<System.DateTime> AttributeD08 { get; set; }
        public Nullable<System.DateTime> AttributeD09 { get; set; }
        public Nullable<System.DateTime> AttributeD10 { get; set; }

        [Display(Name = "Attribute 26")]
        public bool? AttributeB01 { get; set; }

        [Display(Name = "Attribute 27")]
        public bool? AttributeB02 { get; set; }

        [Display(Name = "Attribute 28")]
        public bool? AttributeB03 { get; set; }

        [Display(Name = "Attribute 29")]
        public bool? AttributeB04 { get; set; }

        [Display(Name = "Attribute 30")]
        public bool? AttributeB05 { get; set; }
        public bool? AttributeB06 { get; set; }
        public bool? AttributeB07 { get; set; }
        public bool? AttributeB08 { get; set; }
        public bool? AttributeB09 { get; set; }
        public bool? AttributeB10 { get; set; }

        [Display(Name = "WF Ordinal")]
        public Nullable<int> WFOrdinal { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "WFStatus")]
        [MaxLength(50, ErrorMessage = "WF Status can be maximum 50 characters")]
        public string WFStatus { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "WF Type")]
        [MaxLength(20, ErrorMessage = "WF Type can be maximum 20 characters")]
        public string WFType { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Comments")]
        [MaxLength(4000, ErrorMessage = "WF Comments can be maximum 4000 characters")]
        public string WFComments { get; set; }


        [Display(Name = "WF Requester Id")]
        public Nullable<int> WFRequesterId { get; set; }



        [Display(Name = "WF Analyst Id")]
        public Nullable<int> WFAnalystId { get; set; }


        [Display(Name = "WF Manager Id")]
        public Nullable<int> WFManagerId { get; set; }

        [Display(Name = "WF CurrentOwner Id")]
        public Nullable<int> WFCurrentOwnerId { get; set; }

        [Display(Name = "WFRequester Role Id")]
        public Nullable<int> WFRequesterRoleId { get; set; }
        [Display(Name = "CreatedBy Id")]
        [Required(ErrorMessage = "CreatedBy Id is required")]
        public int CreatedById { get; set; }

        [Display(Name = "UpdatedBy Id")]
        [Required(ErrorMessage = "UpdatedBy Id is required")]
        public int UpdatedById { get; set; }

        [Display(Name = "Created DateTime")]
        [Required(ErrorMessage = "Created DateTime is required")]
        public System.DateTime CreatedDateTime { get; set; }

        [Display(Name = "Updated DateTime")]
        [Required(ErrorMessage = "Updated DateTime is required")]
        public System.DateTime UpdatedDateTime { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Status")]
        [MaxLength(20, ErrorMessage = "Status can be maximum 20 characters")]
        public string Status { get; set; }

        [Display(Name = "Survey")]
        public Nullable<int> SurveyId { get; set; }
        [Display(Name = "Survey")]
        public string SurveyName { get; set; }
        public int? SourceProductId { get; set; }
        public DateTime? WFStatusDateTime { get; set; }

        public string Action { get; set; }
        public int? SspId { get; set; }

       // public string LocalPobTypeName {get;set;}
        public string AuthorizationNumber { get; set; }
        public string RequestName { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime EffectiveEndDate { get; set; }
    }

    public partial class ProductForRequestDetailViewModel
    {
        public int ProductCount { get; set; }
        public string SelecterType { get; set; }
    }
}
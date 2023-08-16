using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class PostLLocalPOBViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Article Number")]
        [Required(ErrorMessage = "Article Number is required")]
        [MaxLength(255, ErrorMessage = "Article Number can be maximum 255 characters")]
        public string ArticleNumber { get; set; }



        [RestrictSpecialChar]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(255, ErrorMessage = "Name can be maximum 255 characters")]
        public string Name { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Description")]
        [MaxLength(2000, ErrorMessage = "Description can be maximum 2000 characters")]
        public string Description { get; set; }

        [Display(Name = "POB Indicator")]
        [Required(ErrorMessage = "POB Indicator is required")]
        public bool PobIndicator { get; set; }


        [Display(Name = "Bundle Indicator")]
        [Required(ErrorMessage = "Bundle Indicator is required")]
        public bool BundleIndicator { get; set; }

        [Display(Name = "Usage Indicator")]
        [Required(ErrorMessage = "Usage Indicator is required")]
        public bool UsageIndicator { get; set; }


        [Display(Name = "Is Hardware Type")]
        [Required(ErrorMessage = "Is Hardware Type is required")]
        public bool IsHardwareType { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Special Indicator")]
        [MaxLength(255, ErrorMessage = "Special Indicator can be maximum 255 characters")]
        public string SpecialIndicator { get; set; }


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
        [Display(Name = "Attribute004")]
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
        [Display(Name = "Attribute009")]
        [MaxLength(255, ErrorMessage = "Attribute009 can be maximum 255 characters")]
        public string AttributeC09 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 10")]
        [MaxLength(255, ErrorMessage = "Attribute 10 can be maximum 255 characters")]
        public string AttributeC10 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 11")]
        [MaxLength(255, ErrorMessage = "Attribute 11 can be maximum 255 characters")]
        public string AttributeN01 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 12")]
        [MaxLength(255, ErrorMessage = "Attribute 12 can be maximum 255 characters")]
        public string AttributeN02 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 13")]
        [MaxLength(255, ErrorMessage = "Attribute 13 can be maximum 255 characters")]
        public string AttributeN03 { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Attribute 14")]
        [MaxLength(255, ErrorMessage = "Attribute 14 can be maximum 255 characters")]
        public string AttributeN04 { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Attribute 15")]
        [MaxLength(255, ErrorMessage = "Attribute 15 can be maximum 255 characters")]
        public string AttributeN05 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 16")]
        [MaxLength(255, ErrorMessage = "Attribute 16 can be maximum 255 characters")]
        public string AttributeN06 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 17")]
        [MaxLength(255, ErrorMessage = "Attribute 17 can be maximum 255 characters")]
        public string AttributeN07 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 18")]
        [MaxLength(255, ErrorMessage = "Attribute 18 can be maximum 255 characters")]
        public string AttributeN08 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 19")]
        [MaxLength(255, ErrorMessage = "Attribute 19 can be maximum 255 characters")]
        public string Attribute019 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 20")]
        [MaxLength(255, ErrorMessage = "Attribute 20 can be maximum 255 characters")]
        public string AttributeN10 { get; set; }


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
        [Display(Name = "WF Comments")]
        [MaxLength(4000, ErrorMessage = "WF Comments can be maximum 4000 characters")]
        public string WFComments { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "WF Requester")]
        [MaxLength(255, ErrorMessage = "WF Requester can be maximum 255 characters")]
        public string WFRequester { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "WF Analyst")]
        [MaxLength(255, ErrorMessage = "WF Analyst can be maximum 255 characters")]
        public string WFAnalyst { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "WF Manager")]
        [MaxLength(255, ErrorMessage = "WF Manager can be maximum 255 characters")]
        public string WFManager { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "WF CurrentOwner")]
        [MaxLength(255, ErrorMessage = "WF Current Owner can be maximum 255 characters")]
        public string WFCurrentOwner { get; set; }

        [Display(Name = "WF Manager")]
        public Nullable<int> WFRequesterRoleId { get; set; }

        //WHO columns will have getters only
        [Display(Name = "Created By")]
        public string CreatedBy { get; }

        [Display(Name = "Updated By")]
        public string UpdatedBy { get; }

        [Display(Name = "Created DateTime")]
        public System.DateTime CreatedDateTime { get; }

        [Display(Name = "Updated DateTime")]
        public System.DateTime UpdatedDateTime { get; }

        public List<LProductContractDurationViewModel> ProductContractDuration { get; set; }

        public  List<LProductSSPViewModel> ProductSSP { get; set; }
        public List<LProductAuthorizationViewModel> ProductAuthorization { get; set; }

    }
}
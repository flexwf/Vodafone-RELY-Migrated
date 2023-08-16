using System;
using System.Collections.Generic;
using System.Linq;
using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LRequestViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        //[Display(Name = "Request Code")]
        //[Required(ErrorMessage = "Request Code is required")]
        //public int RequestCode { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Name or ID")]
        [Required(ErrorMessage = "Name or ID is required")]
        [MaxLength(255, ErrorMessage = "Name or ID can be maximum 255 characters")]
        public string Name { get; set; }

        [Display(Name = "System")]
        public Nullable<int> SystemId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Authorization#")]
        [MaxLength(255, ErrorMessage = "Authorization# can be maximum 255 characters")]
        public string AuthorizationNumber { get; set; }

        [Display(Name = "Date")]
        public Nullable<DateTime> Date { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Description")]
        [MaxLength(2000, ErrorMessage = "Description can be maximum 2000 characters")]
        public string Description { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Notes")]
        [MaxLength(255, ErrorMessage = "Notes can be maximum 255 characters")]
        public string Notes { get; set; }

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

        public int? AttributeI01 { get; set; }
        public int? AttributeI02 { get; set; }
        public int? AttributeI03 { get; set; }
        public int? AttributeI04 { get; set; }
        public int? AttributeI05 { get; set; }
        public int? AttributeI06 { get; set; }
        public int? AttributeI07 { get; set; }
        public int? AttributeI08 { get; set; }
        public int? AttributeI09 { get; set; }
        public int? AttributeI10 { get; set; }

        [Display(Name = "Attribute 11")]
        public decimal? AttributeN01 { get; set; }


        [Display(Name = "Attribute 12")]
        public decimal? AttributeN02 { get; set; }


        [Display(Name = "Attribute 13")]
        public decimal? AttributeN03 { get; set; }


        [Display(Name = "Attribute 14")]
        public decimal? AttributeN04 { get; set; }



        [Display(Name = "Attribute 15")]
        public decimal? AttributeN05 { get; set; }

        [Display(Name = "Attribute 16")]
        public decimal? AttributeN06 { get; set; }


        [Display(Name = "Attribute 17")]
        public decimal? AttributeN07 { get; set; }


        [Display(Name = "Attribute 18")]
        public decimal? AttributeN08 { get; set; }

        [Display(Name = "Attribute 19")]
        public decimal? AttributeN09 { get; set; }


        [Display(Name = "Attribute 20")]
        public decimal? AttributeN10 { get; set; }
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

        public string SystemName { get; set; }//to display the name of the System associated on index page.

        //setters are removed in WF columns as creating issues while creating record in LRequest Table 
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

        public string Status { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsAccMemoBuilt { get; set; }
        public bool IsMgmtSummaryBuilt { get; set; }

        public Nullable<int> SurveyId { get; set; }

        
        public int? Age { get; set; }

        public string SurveyStatus {get;set;}
        public string DateInterval { get; set; }
        public string CreationDate { get; set; }
        public string StatusDate { get; set; }

        [Display(Name = "Status Date")]
        [Required(ErrorMessage = "Status Date is required")]
        public DateTime? WFStatusDateTime { get; set; }
        public DateTime? WFIsReadyDateTime { get; set; }
        public int Version { get; set; }
        public bool SaveFileInBucket { get; set; }
        public List<string> ExistingFilesList { get; set; }

    }
    public partial class LRequestsUploadViewModelForReviewGrid
    {
        public string ColumnName { get; set; }
        public string ColumnLabel { get; set; }
    }
}
using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LReferencesViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Display(Name = "Reference Type Id")]
        [Required(ErrorMessage = "Reference Type is required")]
        public int ReferenceTypeId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(255, ErrorMessage = "Name can be maximum 255 characters")]
        public string Name { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "CompanyCode")]
        [Required(ErrorMessage = "CompanyCode is required")]
        [MaxLength(2, ErrorMessage = "CompanyCode can be maximum 2 characters")]
        public string CompanyCode { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Output FileName")]
        [MaxLength(255, ErrorMessage = "Output FileName can be maximum 255 characters")]
        public string ExtractFileName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Description")]
        [MaxLength(2000, ErrorMessage = " Description can be maximum 2000 characters")]
        public string Description { get; set; }

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

        [Display(Name = "WF RequesterId")]
        public Nullable<int> WFRequesterId { get; set; }

        [Display(Name = "WF AnalystId")]
        public Nullable<int> WFAnalystId { get; set; }

        [Display(Name = "WF ManagerId")]
        public Nullable<int> WFManagerId { get; set; }

        [Display(Name = "WF CurrentOwnerId")]
        public Nullable<int> WFCurrentOwnerId { get; set; }

        [Display(Name = "WF RequesterRoleId")]
        public Nullable<int> WFRequesterRoleId { get; set; }

        
        [Display(Name = "CreatedBy Id")]
        [Required(ErrorMessage = "CreatedBy Id is required")]
        public int CreatedById { get; set; }

        [Display(Name = "Created DateTime")]
        [Required(ErrorMessage = "Created DateTime is required")]
        public System.DateTime CreatedDateTime { get; set; }

        [Display(Name = "UpdatedBy Id")]
        [Required(ErrorMessage = "UpdatedBy Id is required")]
        public int UpdatedById { get; set; }

        [Display(Name = "Updated DateTime")]
        [Required(ErrorMessage = "Updated DateTime is required")]
        public System.DateTime UpdatedDateTime { get; set; }

        public string Status { get; set; }
        

        //Below variables are for ReferenceData
        public string GridArray { get; set; }
        public int collength { get; set; }//cannot make it nullable as its being used as loop counter in a for-loop in api

        public int Version { get; set; }
        public DateTime? WFStatusDateTime { get; set; }
        public DateTime? WFIsReadyDateTime { get; set; }

         public string ErrorMessage { get; set; }//standard column used in all view models
                                                 
        // public string AttributeC01 { get; set; }//not being used

    }
}
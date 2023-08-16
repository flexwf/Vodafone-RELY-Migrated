using RELY_APP.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class LScenarioDemandViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        
        [Display(Name = "Business Area")]
        public Nullable<int> BusinessAreaId { get; set; }

        [Display(Name = "Business Area")]
        public int ProductId { get; set; }

        [Display(Name = "Business Area")]
        public Nullable<int> RequestId { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Question Code")]
        [Required(ErrorMessage = "Question Code is required")]
        [MaxLength(255, ErrorMessage = "Question Code can be maximum 255 characters")]
        public string QuestionCode { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Description")]
        [MaxLength(2000, ErrorMessage = "Description can be maximum 2000 characters")]
        public string ScenarioDescription { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Standards")]
        [MaxLength(2000, ErrorMessage = "Standards can be maximum 2000 characters")]
        public string Standards { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Inbound Flow")]
        public string CommentInbound { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Arguments")]
        public string Arguments { get; set; }

        [Display(Name = "Inception")]
        public bool Inception { get; set; }

        [Display(Name = "Add Good/Service")]
        public bool AddGoodService { get; set; }

        [Display(Name = "Fulfilment")]
        public bool Fulfilment { get; set; }

        [Display(Name = "Price Change")]
        public bool PriceChange { get; set; }

        [Display(Name = "Remove Good/Service")]
        public bool RemoveGoodService { get; set; }

        [Display(Name = "Termination")]
        public bool Termination { get; set; }

        [Display(Name = "Bill Run Reconciliation")]
        public bool BillRunReconciliation { get; set; }


        [Display(Name = "Contract Freeze")]
        public bool ContractFreeze { get; set; }


        [Display(Name = "Usage Fulfilment")]
        public bool UsageFulfillment { get; set; }


        [Display(Name = "Update (Sync/Copa)")]
        public bool UpdateSyncCopa { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(255, ErrorMessage = "First Name can be maximum 255 characters")]
        public string ContactFName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(255, ErrorMessage = "Last Name can be maximum 255 characters")]
        public string ContactLName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Contact Email")]
        [Required(ErrorMessage = "Contact Email is required")]
        [MaxLength(255, ErrorMessage = "Contact Email can be maximum 255 characters")]
        public string ContactEmail { get; set; }

        public int Version { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string Status { get; set; }
        public Nullable<int> WFOrdinal { get; set; }
        public string WFStatus { get; set; }
        public string WFType { get; set; }
        public string WFComments { get; set; }
        public Nullable<int> WFRequesterId { get; set; }
        public Nullable<int> WFAnalystId { get; set; }
        public Nullable<int> WFManagerId { get; set; }
        public Nullable<int> WFCurrentOwnerId { get; set; }
        public Nullable<int> WFRequesterRoleId { get; set; }
        public Nullable<DateTime> WFStatusDateTime { get; set; }
        public Nullable<DateTime> WFIsReadyDateTime { get; set; }

        public string ErrorMessage { get; set; }

        [Display(Name = "Point of Contact")]
        public string PointOfContact { get; set; }

        public Nullable<DateTime> ImplementationDate { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(255, ErrorMessage = "Name can be maximum 255 characters")]
        public string Name { get; set; }

        public int? Age { get; set; }

        public string SurveyStatus { get; set; }
        public string DateInterval { get; set; }
        public string CreationDate { get; set; }
        public string StatusDate { get; set; }
        public string DemandStatus { get; set; }
        public string DemandCreationDate { get; set; }
        public string DemandStatusDate { get; set; }

    }
}
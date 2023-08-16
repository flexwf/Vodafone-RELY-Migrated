using RELY_APP.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public partial class LUserViewModel
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
        [Display(Name = "Login Email")]
        [Required(ErrorMessage = "Login Email is required")]
        [MaxLength(255, ErrorMessage = "Login Email can be maximum 255 characters")]
        [EmailAddress]
        public string LoginEmail { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(255, ErrorMessage = "First Name can be maximum 255 characters")]
        public string FirstName { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Last Name")]
        [MaxLength(255, ErrorMessage = "Last Name can be maximum 255 characters")]
        public string LastName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Phone")]
        [MaxLength(255, ErrorMessage = "Phone can be maximum 255 Length")]
        public string Phone { get; set; }

        [Display(Name = "Block Notification")]
        [Required(ErrorMessage = "Block Notification is required")]
        public bool BlockNotification { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Status")]
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(255, ErrorMessage = "Status can be maximum 255 characters")]
        public string Status { get; set; }

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

        [Display(Name = "Lockout Until")]
        public System.DateTime? LockoutUntil { get; set; }

        public Nullable<int> WFOrdinal { get; set; }
        [RestrictSpecialChar]
        public string WFStatus { get; set; }
        [RestrictSpecialChar]
        public string WFType { get; set; }
        [RestrictSpecialChar]
        [Display(Name ="Comments")]
        public string WFComments { get; set; }

        [RestrictSpecialChar]
        public string Comments { get; set; }
        public Nullable<int> WFRequesterId { get; set; }
        public Nullable<int> WFAnalystId { get; set; }
        public Nullable<int> WFManagerId { get; set; }
        public Nullable<int> WFCurrentOwnerId { get; set; }
        public Nullable<int> WFRequesterRoleId { get; set; }

        public int Version { get; set; }
        public DateTime? WFStatusDateTime { get; set; }
        public String ErrorMessage { get; set; }
        public bool ChangePwdAtNextLogin { get; set; }
    }
}
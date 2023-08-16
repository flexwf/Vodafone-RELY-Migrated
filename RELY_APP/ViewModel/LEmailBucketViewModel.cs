using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LEmailBucketViewModel
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Recipient List")]
        [Required(ErrorMessage = "Recipient List is required")]
        [MaxLength(4000, ErrorMessage = " Recipient List  can be maximum 4000 characters")]
        public string RecipientList { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "CCList")]
        [MaxLength(4000, ErrorMessage = " CCList  can be maximum 4000 characters")]
        public string CCList { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "BCCList")]
        [MaxLength(4000, ErrorMessage = " BCCList  can be maximum 4000 characters")]
        public string BCCList { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "ReplyToList")]
        [MaxLength(4000, ErrorMessage = " ReplyToList  can be maximum 4000 characters")]
        public string ReplyToList { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Subject")]
        [Required(ErrorMessage = "Subject is required")]
        [MaxLength(500, ErrorMessage = " Subject  can be maximum 500 characters")]
        public string Subject { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Body")]
        [Required(ErrorMessage = "Body is required")]
        public string Body { get; set; }

        [Display(Name ="IsHTML")]
        [Required(ErrorMessage = "IsHTML is required")]
        public bool IsHTML { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Email Type")]
        [Required(ErrorMessage = "Email Type is required")]
        [MaxLength(255, ErrorMessage = " Email Type  can be maximum 255 characters")]
        public string EmailType { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Priority")]
        [Required(ErrorMessage = "Priority is required")]
        [MaxLength(255, ErrorMessage = " Priority  can be maximum 255 characters")]
        public string Priority { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attachment List")]
        [MaxLength(4000, ErrorMessage = " Attachment List  can be maximum 4000 characters")]
        public string AttachmentList { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Status")]
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(255, ErrorMessage = " Status  can be maximum 255 characters")]
        public string Status { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Comments")]
        [MaxLength(4000, ErrorMessage = " Comments  can be maximum 4000 characters")]
        public string Comments { get; set; }

        [Display(Name = "Created DateTime")]
        [Required(ErrorMessage = "Created DateTime is required")]
        public System.DateTime CreatedDateTime { get; set; }

        [Display(Name = "Updated DateTime")]
        [Required(ErrorMessage = "Updated DateTime is required")]
        public System.DateTime UpdatedDateTime { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Created By")]
        [Required(ErrorMessage = "Created By is required")]
        public int CreatedById { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Updated By")]
        [Required(ErrorMessage = "Updated By is required")]
        public int UpdatedById { get; set; }

        [Display(Name = "Sender ConfigId")]
        [Required(ErrorMessage = "Sender ConfigId is required")]
        public int SenderConfigId { get; set; }

        public int EmailSent { get; set; }

        [RestrictSpecialChar]
        public string CompanyCode { get; set; }

    }
}
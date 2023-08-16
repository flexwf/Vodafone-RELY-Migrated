using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LFSNextStepViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code  is required")]
        [MaxLength(2, ErrorMessage = " Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [Display(Name = "Question Code")]
        [Required(ErrorMessage = "Question Code is required")]
        public string QuestionCode { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Answer Option")]
        [Required(ErrorMessage = "Answer Option  is required")]
        [MaxLength(255, ErrorMessage = " Answer Option can be maximum 255 characters")]
        public string AnswerOption { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Next Step Text")]
        [Required(ErrorMessage = "Next Step Text  is required")]
        [MaxLength(2000, ErrorMessage = " Next Step Text can be maximum 2000 characters")]
        public string NextStepText { get; set; }

        [Display(Name = "Created By Id")]
        [Required(ErrorMessage = "CreatedById is required")]
        public int CreatedById { get; set; }


        [Display(Name = "Created Date Time")]
        [Required(ErrorMessage = "Created Date Time is Required.")]
        public DateTime CreatedDateTime { get; set; }

        [Display(Name = "Updated By Id")]
        [Required(ErrorMessage = "UpdatedById is required")]
        public int UpdatedById { get; set; }


        [Display(Name = "Updated Date Time")]
        [Required(ErrorMessage = "Updated Date Time is Required.")]
        public DateTime UpdatedDateTime { get; set; }
        [RestrictSpecialChar]
        [Display(Name = "Internal Notes")]
        [MaxLength(4000, ErrorMessage = " Internal Notes can be maximum 4000 characters")]
        public string InternalNotes { get; set; }
    }
}
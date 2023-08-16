using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class LFSQuestionBankViewModel
    {
        [Display(Name ="Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code  is required")]
        [MaxLength(2, ErrorMessage = " Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [Display(Name = "ItemTypeId")]
        [Required(ErrorMessage = "ItemTypeId is required")]
        public int ItemTypeId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Question Code")]
        [Required(ErrorMessage = "Question Code  is required")]
        [MaxLength(20, ErrorMessage = " Question Code can be maximum 20 characters")]
        public string QuestionCode { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Question Name")]
        [Required(ErrorMessage = "Question Name  is required")]
        [MaxLength(4000, ErrorMessage = " Question Name can be maximum 4000 characters")]
        public string QuestionName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Comments")]
        [Required(ErrorMessage = "Comments  is required")]
        [MaxLength(4000, ErrorMessage = " Comments can be maximum 4000 characters")]
        public string Comments { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Readable Name")]
        [Required(ErrorMessage = "Readable Name  is required")]
        [MaxLength(4000, ErrorMessage = " Readable Name can be maximum 4000 characters")]
        public string ReadableName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Question Text")]
        [Required(ErrorMessage = "Question Text  is required")]
        [MaxLength(4000, ErrorMessage = " Question Text can be maximum 4000 characters")]
        public string QuestionText { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "ToolTip")]
        [Required(ErrorMessage = "ToolTip  is required")]
        //[MaxLength(, ErrorMessage = " ToolTip can be max characters")]
        public string ToolTip { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "VGAP Reference")]
        [Required(ErrorMessage = "VGAP Reference Text  is required")]
        [MaxLength(4000, ErrorMessage = " VGAP Reference Text can be maximum 4000 characters")]
        public string VGAPReference { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "IFRS Reference")]
        [Required(ErrorMessage = "IFRS Reference Text  is required")]
        [MaxLength(4000, ErrorMessage = " IFRS Reference Text can be maximum 4000 characters")]
        public string IFRSReference { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "IsActive")]
        [Required(ErrorMessage = "IsActive  is required")]
        public bool IsActive { get; set; }

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
        
        [MaxLength(4000, ErrorMessage = " Internal Notes Text can be maximum 4000 characters")]
        public string InternalNotes { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Name")]

        [MaxLength(255, ErrorMessage = " Name Text can be maximum 255 characters")]
        public string Name { get; set; }

    }

    public partial class QuestionDetailsForDemandViewModel
    {
        public int Id { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionText { get; set; }
        public string Response { get; set; }

    }

}

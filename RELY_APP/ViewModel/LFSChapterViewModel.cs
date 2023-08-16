using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LFSChapterViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "ChapterCode")]
        [Required(ErrorMessage = "Chapter Code  is required")]
        [MaxLength(20, ErrorMessage = " Chapter Code can be maximum 20 characters")]
        public string ChapterCode { get; set; }

        [Display(Name = "Survey Id")]
        [Required(ErrorMessage = "Survey Id is required")]
        public int SurveyId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name  is required")]
        [MaxLength(255, ErrorMessage = " Name can be maximum 255 characters")]
        public string Name { get; set; }

        [Display(Name = "Ordinal")]
        [Required(ErrorMessage = "Ordinal is required")]
        public int Ordinal { get; set; }

        [Display(Name = "CreatedById")]
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
        [Display(Name = "InternalNotes")]
        [MaxLength(4000, ErrorMessage = " InternalNotes can be maximum 4000 characters")]
        public string InternalNotes { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "SurveyName")]
        [MaxLength(255, ErrorMessage = " SurveyName can be maximum 255 characters")]
        public string SurveyName { get; set; }


    }
}
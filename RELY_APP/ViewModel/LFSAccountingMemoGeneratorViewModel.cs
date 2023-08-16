using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LFSAccountingMemoGeneratorViewModel
    {
        [RestrictSpecialChar]
        [Display(Name = "Entity Type")]
        [Required(ErrorMessage = "Entity Type  is required")]
        [MaxLength(10, ErrorMessage = " Entity Type can be maximum 10 characters")]
        public string EntityType { get; set; }

        [Display(Name = "EntityId")]
        [Required(ErrorMessage = "EntityId is required")]
        public int EntityId { get; set; }

        [Display(Name = "SurveyId")]
        [Required(ErrorMessage = "SurveyId is required")]
        public int SurveyId { get; set; }


        public int UserId { get; set; }
    }
}
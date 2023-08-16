using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LFSManualAccScenarioViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Display(Name = "Entity Id")]
        [Required(ErrorMessage = "Entity Id is required")]
        public int EntityId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Entity Type")]
        [Required(ErrorMessage = "Entity Type  is required")]
        [MaxLength(255, ErrorMessage = " Entity Type can be maximum 255 characters")]
        public string EntityType { get; set; }

        [Display(Name = "Referance")]
        [Required(ErrorMessage = "Referance is required")]
        public int Referance { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Situation")]
        [Required(ErrorMessage = "Situation  is required")]
        [MaxLength(2000, ErrorMessage = " Situation can be maximum 2000 characters")]
        public string Situation { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Object Type")]
        [Required(ErrorMessage = "Object Type  is required")]
        [MaxLength(255, ErrorMessage = " Object Type can be maximum 255 characters")]
        public string ObjectType { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description  is required")]
        [MaxLength(2000, ErrorMessage = " Description can be maximum 2000 characters")]
        public string Description { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Comments")]
        [Required(ErrorMessage = "Comments  is required")]
        [MaxLength(4000, ErrorMessage = " Comments can be maximum 4000 characters")]
        public string Comments { get; set; }
    }
}
using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LFSItemTypeViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code  is required")]
        [MaxLength(2, ErrorMessage = " Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name  is required")]
        [MaxLength(255, ErrorMessage = " Name can be maximum 255 characters")]
        public string Name { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description  is required")]
        [MaxLength(2000, ErrorMessage = " Description can be maximum 2000 characters")]
        public string Description { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "IsUserRespondable")]
        [Required(ErrorMessage = "IsUserRespondable  is required")]
        public bool IsUserRespondable { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "IsQuestion")]
        [Required(ErrorMessage = "IsQuestion  is required")]
        public bool IsQuestion { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "WordStyle")]
        [Required(ErrorMessage = "WordStyle  is required")]
        [MaxLength(255, ErrorMessage = " WordStyle can be maximum 255 characters")]
        public string WordStyle { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "BGColor")]
        [Required(ErrorMessage = "BGColor  is required")]
        [MaxLength(255, ErrorMessage = " BGColor can be maximum 255 characters")]
        public string BGColor { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "TextColor")]
        [Required(ErrorMessage = "Text Color  is required")]
        [MaxLength(255, ErrorMessage = " TextColor can be maximum 255 characters")]
        public string TextColor { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Font")]
        [Required(ErrorMessage = "Font  is required")]
        [MaxLength(255, ErrorMessage = " Font can be maximum 255 characters")]
        public string Font { get; set; }

        [Display(Name = "FontSize")]
        [Required(ErrorMessage = "Font Size is required")]
        public int FontSize { get; set; }
    }
}
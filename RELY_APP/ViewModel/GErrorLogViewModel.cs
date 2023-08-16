using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class GErrorLogViewModel
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Error DateTime is required")]
        [Display(Name = " Error Date Time")]
        public System.DateTime ErrorDateTime { get; set; }

        [MaxLength(255, ErrorMessage = " Source Project can be maximum 255 characters")]
        [Required(ErrorMessage = "Source Project  is required")]
        [Display(Name = "Source Project")]
        [RestrictSpecialChar]
        public string SourceProject { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Controller")]
        [MaxLength(255, ErrorMessage = "Controller can be maximum 255 characters")]
        public string Controller { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Method")]
        [MaxLength(255, ErrorMessage = " Method can be maximum 255 characters")]
        public string Method { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Stact Trace")]
        public string StackTrace { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "User Name")]
        [MaxLength(255, ErrorMessage = " User Name  can be maximum 255 characters")]
        public string UserName { get; set; }

        [MaxLength(255, ErrorMessage = "Error Type can be maximum 255 characters")]
        [Display(Name = "Error Type")]
        [RestrictSpecialChar]
        public string ErrorType { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Error Description")]
        [MaxLength(2000, ErrorMessage = " Error Description can be maximum 2000 characters")]
        public string ErrorDescription { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Resolution")]
        [MaxLength(2000, ErrorMessage = " Resolution can be maximum 2000 characters")]
        public string Resolution { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Error Owner")]
        [MaxLength(255, ErrorMessage = " Error Owner can be maximum 255 characters")]
        public string ErrorOwner { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Field Name")]
        [MaxLength(255, ErrorMessage = " Field Name can be maximum 255 characters")]
        public string FieldName { get; set; }


        [Display(Name = "Batch  Number")]
        public Nullable<int> BatchNumber { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Status")]
        [Required(ErrorMessage =" Status is required")]
        [MaxLength(255, ErrorMessage = " Status can be maximum 255 characters")]
        public string Status { get; set; }
        public int Exceptions { get; set; }

    }
}
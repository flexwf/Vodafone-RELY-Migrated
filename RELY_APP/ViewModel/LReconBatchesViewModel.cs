using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LReconBatchesViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = " Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [Display(Name = "File Format Id")]
        [Required(ErrorMessage = "FileFormatId is required")]
        public int FileFormatId { get; set; }

        [Display(Name = "Batch Number")]
        [Required(ErrorMessage = "Batch Number is required")]
        public int BatchNumber { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Source File Name")]
        [Required(ErrorMessage = "Source File Name is required")]
        [MaxLength(255, ErrorMessage = "The SourceFileName can be maximum 255 characters")]
        public string SourceFileName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Upload Mode")]
        [Required(ErrorMessage = "Upload Mode is required")]
        [MaxLength(20, ErrorMessage = "The UploadMode can be maximum 20 characters")]
        public string UploadMode { get; set; }

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

    }
}
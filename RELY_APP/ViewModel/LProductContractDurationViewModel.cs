

using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class LProductContractDurationViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }


        [Display(Name = "Product Id")]
        [Required(ErrorMessage = "Product Id is required")]
        public int ProductId { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Duration")]
        [Required(ErrorMessage = "Duration is required")]
        [MaxLength(255, ErrorMessage = "Duration can be maximum 255 characters")]
        public string Duration { get; set; }

        [Display(Name = "Effective StartDate")]
        [Required(ErrorMessage = "Effective StartDate is required")]
        public System.DateTime EffectiveStartDate { get; set; }

        [Display(Name = "Effective EndDate")]
        [Required(ErrorMessage = "Effective EndDate is required")]
        public System.DateTime EffectiveEndDate { get; set; }

    }
}
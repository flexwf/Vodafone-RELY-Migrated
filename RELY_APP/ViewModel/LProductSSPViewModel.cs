using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class LProductSSPViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Display(Name = "Product Id")]
        [Required(ErrorMessage = "Product Id is required")]
        public int ProductId { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }

        [Display(Name = "Effective StartDate")]
        [Required(ErrorMessage = "Effective StartDate is required")]
        public System.DateTime EffectiveStartDate { get; set; }

        [Display(Name = "Effective EndDate")]
        [Required(ErrorMessage = "Effective EndDate is required")]
        public System.DateTime EffectiveEndDate { get; set; }

    }
}
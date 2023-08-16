

using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class LLocalPobSspViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

       
        [Display(Name = "LocalPob Id")]
        [Required(ErrorMessage = " LocalPob Id is required")]
        public int LocalPobId { get; set; }

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
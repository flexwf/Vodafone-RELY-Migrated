
using RELY_APP.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace RELY_APP.ViewModel
{
    public class LProductAuthorizationViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Product Id")]
        [Required(ErrorMessage = "Product Id is required")]
        [MaxLength(2, ErrorMessage = "Product Id can be maximum 2 characters")]
        public string ProductId { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "System")]
        [Required(ErrorMessage = "System is required")]
        [MaxLength(255, ErrorMessage = "System can be maximum 255 characters")]
        public string System { get; set; }

        
        [RestrictSpecialChar]
        [Display(Name = "Ticket Number")]
        [Required(ErrorMessage = "Ticket Number is required")]
        [MaxLength(255, ErrorMessage = "Ticket Number can be maximum 255 characters")]
        public string TicketNumber { get; set; }


        [Display(Name = "Date")]
        public Nullable<DateTime> Date { get; set; }
    }
}
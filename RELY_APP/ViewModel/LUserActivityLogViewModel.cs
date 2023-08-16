using RELY_APP.Utilities;
using System.ComponentModel.DataAnnotations;


namespace RELY_APP.ViewModel
{
    public partial class LUserActivityLogViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }
        
        [RestrictSpecialChar]
        [Display(Name = "Activity")]
        [Required(ErrorMessage = "Activity is required")]
        [MaxLength(255, ErrorMessage = "Activity can be maximum 255 characters")]
        public string Activity { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Remarks")]
        [MaxLength(2000, ErrorMessage = "Remarks can be maximum 2000 characters")]
        public string Remarks { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Company Code")]
        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "Company Code can be maximum 2 characters")]
        public string CompanyCode { get; set; }

        [Display(Name = "IsActivitySucceeded")]
        [Required(ErrorMessage = "IsActivitySucceeded is required")]
        public bool IsActivitySucceeded { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "HostIP")]
        [MaxLength(255, ErrorMessage = "HostIP can be maximum 255 characters")]
        public string HostIP { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Host Browser Details")]
        [MaxLength(2000, ErrorMessage = "Host Browser Details can be maximum 2000 characters")]
        public string HostBrowserDetails { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Host TimeZone")]
        [MaxLength(255, ErrorMessage = "Host TimeZone can be maximum 255 characters")]
        public string HostTimeZone { get; set; }

        [Display(Name = "ActivityDateTime")]
        [Required(ErrorMessage = "ActivityDateTime is required")]
        public System.DateTime ActivityDateTime { get; set; }


      
        [Display(Name = "ActionBy")]
        [Required(ErrorMessage = "ActionBy is required")]
        public int ActionById { get; set; }

       
        [Display(Name = "ActionForId")]
        [Required(ErrorMessage = "ActionForId is required")]
        public int ActionForId{ get; set; }

        [RestrictSpecialChar]
        [Display(Name = "ActionFor")]
        [Required(ErrorMessage = "ActionFor is required")]
        [MaxLength(255, ErrorMessage = "ActionFor can be maximum 255 characters")]
        public string ActionFor { get; set; }


    }
}
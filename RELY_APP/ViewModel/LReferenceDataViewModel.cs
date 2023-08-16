using RELY_APP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LReferenceDataViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Display(Name = "Reference Id")]
        [Required(ErrorMessage = "ReferenceId is required")]
        public int ReferenceId { get; set; }


        [Display(Name = "Effective Start Date")]
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }

        [Display(Name = "Effective End Date")]
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }

        [Display(Name = "CreatedBy Id")]
        [Required(ErrorMessage = "CreatedBy Id is required")]
        public int CreatedById { get; set; }

        [Display(Name = "Created DateTime")]
        [Required(ErrorMessage = "Created DateTime is required")]
        public System.DateTime CreatedDateTime { get; set; }

        [Display(Name = "UpdatedBy Id")]
        [Required(ErrorMessage = "UpdatedBy Id is required")]
        public int UpdatedById { get; set; }

        [Display(Name = "Updated DateTime")]
        [Required(ErrorMessage = "Updated DateTime is required")]
        public System.DateTime UpdatedDateTime { get; set; }


        [RestrictSpecialChar]
        [Display(Name = "Attribute 1")]
        [MaxLength(255, ErrorMessage = "Attribute 1 can be maximum 255 characters")]
        public string AttributeC01 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 2")]
        [MaxLength(255, ErrorMessage = "Attribute 2 can be maximum 255 characters")]
        public string AttributeC02 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 3")]
        [MaxLength(255, ErrorMessage = "Attribute 3 can be maximum 255 characters")]
        public string AttributeC03 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 4")]
        [MaxLength(255, ErrorMessage = "Attribute 4 can be maximum 255 characters")]
        public string AttributeC04 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 5")]
        [MaxLength(255, ErrorMessage = "Attribute 5 can be maximum 255 characters")]
        public string AttributeC05 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 6")]
        [MaxLength(255, ErrorMessage = "Attribute 6 can be maximum 255 characters")]
        public string AttributeC06 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 7")]
        [MaxLength(255, ErrorMessage = "Attribute 7 can be maximum 255 characters")]
        public string AttributeC07 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 8")]
        [MaxLength(255, ErrorMessage = "Attribute 8 can be maximum 255 characters")]
        public string AttributeC08 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 9")]
        [MaxLength(255, ErrorMessage = "Attribute 9 can be maximum 255 characters")]
        public string AttributeC09 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 10")]
        [MaxLength(255, ErrorMessage = "Attribute 10 can be maximum 255 characters")]
        public string AttributeC10 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 11")]
        [MaxLength(255, ErrorMessage = "Attribute 11 can be maximum 255 characters")]
        public string AttributeC11 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 12")]
        [MaxLength(255, ErrorMessage = "Attribute 12 can be maximum 255 characters")]
        public string AttributeC12 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 13")]
        [MaxLength(255, ErrorMessage = "Attribute 13 can be maximum 255 characters")]
        public string AttributeC13 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 14")]
        [MaxLength(255, ErrorMessage = "Attribute 14 can be maximum 255 characters")]
        public string AttributeC14 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 15")]
        [MaxLength(255, ErrorMessage = "Attribute 15 can be maximum 255 characters")]
        public string AttributeC15 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 16")]
        [MaxLength(255, ErrorMessage = "Attribute 16 can be maximum 255 characters")]
        public string AttributeC16 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 17")]
        [MaxLength(255, ErrorMessage = "Attribute 17 can be maximum 255 characters")]
        public string AttributeC17 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 18")]
        [MaxLength(255, ErrorMessage = "Attribute 18 can be maximum 255 characters")]
        public string AttributeC18 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 19")]
        [MaxLength(255, ErrorMessage = "Attribute 19 can be maximum 255 characters")]
        public string AttributeC19 { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Attribute 20")]
        [MaxLength(255, ErrorMessage = "Attribute 20 can be maximum 255 characters")]
        public string AttributeC20 { get; set; }

        //[RestrictSpecialChar]
        //[Display(Name = "Attribute 21")]
        //[MaxLength(255, ErrorMessage = "Attribute 21 can be maximum 255 characters")]
        //public string AttributeC21 { get; set; }

        //[RestrictSpecialChar]
        //[Display(Name = "Attribute 22")]
        //[MaxLength(255, ErrorMessage = "Attribute 22 can be maximum 255 characters")]
        //public string AttributeC22 { get; set; }

        //[RestrictSpecialChar]
        //[Display(Name = "Attribute 23")]
        //[MaxLength(255, ErrorMessage = "Attribute 23 can be maximum 255 characters")]
        //public string AttributeC23 { get; set; }

        //[RestrictSpecialChar]
        //[Display(Name = "Attribute 24")]
        //[MaxLength(255, ErrorMessage = "Attribute 24 can be maximum 255 characters")]
        //public string AttributeC24 { get; set; }



        //[RestrictSpecialChar]
        //[Display(Name = "Attribute 25")]
        //[MaxLength(255, ErrorMessage = "Attribute 25 can be maximum 255 characters")]
        //public string AttributeC25 { get; set; }

        //[RestrictSpecialChar]
        //[Display(Name = "Attribute 26")]
        //[MaxLength(255, ErrorMessage = "Attribute 26 can be maximum 255 characters")]
        //public string AttributeC26 { get; set; }

        //[RestrictSpecialChar]
        //[Display(Name = "Attribute 27")]
        //[MaxLength(255, ErrorMessage = "Attribute 27 can be maximum 255 characters")]
        //public string AttributeC27 { get; set; }

        //[RestrictSpecialChar]
        //[Display(Name = "Attribute 28")]
        //[MaxLength(255, ErrorMessage = "Attribute 28 can be maximum 255 characters")]
        //public string AttributeC28 { get; set; }

        //[RestrictSpecialChar]
        //[Display(Name = "Attribute 29")]
        //[MaxLength(255, ErrorMessage = "Attribute 29 can be maximum 255 characters")]
        //public string AttributeC29 { get; set; }

        //[RestrictSpecialChar]
        //[Display(Name = "Attribute 30")]
        //[MaxLength(255, ErrorMessage = "Attribute 30 can be maximum 255 characters")]
        //public string AttributeC30 { get; set; }

        [Display(Name = "Attribute 31")]
        public Nullable<decimal>AttributeN01 { get; set; }

        [Display(Name = "Attribute 32")]
        public Nullable<decimal> AttributeN02 { get; set; }

        [Display(Name = "Attribute 33")]
        public Nullable<decimal> AttributeN03 { get; set; }

        [Display(Name = "Attribute 34")]
        public Nullable<decimal> AttributeN04 { get; set; }

        [Display(Name = "Attribute 35")]
        public Nullable<decimal> AttributeN05 { get; set; }

        [Display(Name = "Attribute 36")]
        public Nullable<decimal> AttributeN06 { get; set; }

        [Display(Name = "Attribute 37")]
        public Nullable<decimal> AttributeN07 { get; set; }

        [Display(Name = "Attribute 38")]
        public Nullable<decimal> AttributeN08 { get; set; }

        [Display(Name = "Attribute 39")]
        public Nullable<decimal> AttributeN09 { get; set; }

        [Display(Name = "Attribute 40")]
        public Nullable<decimal> AttributeN10 { get; set; }


        [Display(Name = "Attribute 41")]
        public Nullable<int> AttributeI01 { get; set; }
        

        [Display(Name = "Attribute 42")]
        public Nullable<int> AttributeI02 { get; set; }

        [Display(Name = "Attribute 43")]
        public Nullable<int> AttributeI03 { get; set; }

        [Display(Name = "Attribute 44")]
        public Nullable<int> AttributeI04 { get; set; }

        [Display(Name = "Attribute 45")]
        public Nullable<int> AttributeI05 { get; set; }

        [Display(Name = "Attribute 46")]
        public Nullable<int> AttributeI06 { get; set; }

        [Display(Name = "Attribute 47")]
        public Nullable<int> AttributeI07 { get; set; }

        [Display(Name = "Attribute 48")]
        public Nullable<int> AttributeI08 { get; set; }

        [Display(Name = "Attribute 49")]
        public Nullable<int> AttributeI09 { get; set; }

        [Display(Name = "Attribute 50")]
        public Nullable<int> AttributeI10 { get; set; }

        [Display(Name = "Attribute 51")]
        public Nullable<System.DateTime> AttributeD01 { get; set; }

        [Display(Name = "Attribute 52")]
        public Nullable<System.DateTime> AttributeD02 { get; set; }

        [Display(Name = "Attribute 53")]
        public Nullable<System.DateTime> AttributeD03 { get; set; }

        [Display(Name = "Attribute 54")]
        public Nullable<System.DateTime> AttributeD04 { get; set; }

        [Display(Name = "Attribute 55")]
        public Nullable<System.DateTime> AttributeD05 { get; set; }

        [Display(Name = "Attribute 56")]
        public string AttributeM01 { get; set; }

        [Display(Name = "Attribute 57")]
        public string AttributeM02{ get; set; }

        [Display(Name = "Attribute 58")]
        public string AttributeM03 { get; set; }

        [Display(Name = "Attribute 59")]
        public string AttributeM04 { get; set; }

        [Display(Name = "Attribute 60")]
        public string AttributeM05 { get; set; }

        [Display(Name = "Attribute 61")]
        public Nullable<bool> AttributeB01 { get; set; }

        [Display(Name = "Attribute 62")]
        public Nullable<bool> AttributeB02 { get; set; }

        [Display(Name = "Attribute 63")]
        public Nullable<bool> AttributeB03 { get; set; }

        [Display(Name = "Attribute 64")]
        public Nullable<bool> AttributeB04 { get; set; }

        [Display(Name = "Attribute 65")]
        public Nullable<bool> AttributeB05 { get; set; }

        [Display(Name = "Attribute 66")]
        public Nullable<bool> AttributeB06 { get; set; }

        [Display(Name = "Attribute 67")]
        public Nullable<bool> AttributeB07{ get; set; }

        [Display(Name = "Attribute 68")]
        public Nullable<bool> AttributeB08 { get; set; }

        [Display(Name = "Attribute 69")]
        public Nullable<bool> AttributeB09 { get; set; }

        [Display(Name = "Attribute 70")]
        public Nullable<bool> AttributeB10 { get; set; }

        public int RowCounts { get; set; }
    }
}
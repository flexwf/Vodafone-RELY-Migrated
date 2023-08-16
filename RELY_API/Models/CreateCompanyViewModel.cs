using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class CreateCompanyViewModel
    {
        public int Id { get; set; }

        
        public string CompanyName { get; set; }

       
        public string CompanyCode { get; set; }


       
        public string LogoPath { get; set; }

        
        public string PunchLine { get; set; }


        
        public string DomainAddress { get; set; }



       
        public int MinLength { get; set; }

       
        public int MinUpperCase { get; set; }


       
        public int MinLowerCase { get; set; }

       
        public int MinNumbers { get; set; }

        
        public int MinSpecialChars { get; set; }

       
        public int MinAgeDays { get; set; }

       
        public int MaxAgeDays { get; set; }

       
        public int ReminderDays { get; set; }

        
        public int PreventReuse { get; set; }


        
        public int LockoutFailedAttempts { get; set; }

        
        public int LockoutMinutes { get; set; }


    }
}
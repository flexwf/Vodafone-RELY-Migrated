using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class PasswordPolicyViewModel
    {
        public int Id { get; set; }


        public string CompanyCode { get; set; }


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


        public string Status { get; set; }

        //Added to get how many days left to expire password
        public int DaysToExpirePassword { get; set; }
    }
}
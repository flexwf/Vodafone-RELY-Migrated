using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class AccountViewModel
    {
    }

    //method to login user and Get User Details To WebApp application
    //Class for sending user details to webApp
    public class LoginViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public List<RoleViewModel> Roles { get; set; }//using single object to return list of Id and Name of Role assigned to User
        public string FullName { get; set; }
        public string CompanyCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string LastLoginMessage { get; set; }
    }

    public partial class RoleViewModel
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public bool IsDefault { get; set; }
        public bool IsMFAEnabled { get; set; }
    }


}
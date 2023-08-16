using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class MLUsersGSecurityQuestionViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public int Question1 { get; set; }
        public int Question2 { get; set; }
        public int Question3 { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
    }
}
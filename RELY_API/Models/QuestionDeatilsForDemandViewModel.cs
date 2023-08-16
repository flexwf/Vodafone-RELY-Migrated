using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class QuestionDetailsForDemandViewModel
    {
        public int Id { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionText { get; set; }
        public string Response { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class LFSNextStepActionViewModel
    {

        public int Id { get; set; }
        public int ResponseId { get; set; }

        
        public int NextStepId { get; set; }
        public bool IsDone { get; set; }
        public string ActionTaken { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int UpdatedById { get; set; }
        public DateTime UpdatedDateTime { get; set; }

        public string Response { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionName { get; set; }
        public string QuestionText { get; set; }
        public string NextStepText { get; set; }
    }
}
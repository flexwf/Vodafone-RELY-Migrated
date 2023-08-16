using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public partial class GetAccountingScenarioMatrixViewModel
    {
        public int ResponseId { get; set; }
        public string Category { get; set; }
        public string QuestionCode { get; set; }
        public string ReadableName { get; set; }
        public string ObjectType { get; set; }
        public string SysCat { get; set; }
        public string Response { get; set; }
        public string ScenarioDescription { get; set; }
        public string Comments { get; set; }
        public string QuestionName { get; set; }

        public int ProductId { get; set; }
    }
}
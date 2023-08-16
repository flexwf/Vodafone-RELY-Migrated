using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class RWorkFlowViewModel
    {
        public string CompanyCode { get; set; }
        public int LocalPobs { get; set; }
        public int Users { get; set; }
        public int RequestsPPM { get; set; }

        public int Products { get; set; }
        public int References { get; set;}

        public int AccountingScenario { get; set; }
    }
}
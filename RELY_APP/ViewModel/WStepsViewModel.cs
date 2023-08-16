using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public partial class WStepsViewModel
    {
        public int Id { get; set; }
        public string CompanyCode { get; set; }
        public int WorkFlowId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public int Ordinal { get; set; }
        public bool Skip { get; set; }
        public string SkipFunctionName { get; set; }
        public string Banner { get; set; }
        public bool DoNotNotify { get; set; }
        public bool IsReady { get; set; }
        //Extra column to get Default tab
        public bool IsDefault { get; set; }
    }

    public partial class StepsDropDownViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public partial class StepForWorkFlowinSessionViewModel
    {
        public int Id { get; set; }
        public string WorkFlowName { get; set; }
        public int WorkFlowId { get; set; }
    }

}
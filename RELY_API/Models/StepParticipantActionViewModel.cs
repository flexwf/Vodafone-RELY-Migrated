using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class StepParticipantActionViewModel
    {
        public int Id { get; set; }
        public int WActionId { get; set; }
        public string ActionName { get; set; }
        public string Label { get; set; }
        public string Glymph { get; set; }
        public int ShowInStepId { get; set; }
        public bool ButtonOnWfGrid { get; set; }
        public bool ButtonOnForm { get; set; }
        public string VisibilityFunction { get; set; }
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; }
        public string ParticipantType { get; set; }
        public string Description { get; set; }
        public bool IsLinkOverWFGrid { get; set; }
        public string ValidationFunction { get; set; }

        public int? SendToStepId { get; set; }
        public string SendToStepName { get; set; }
        public string ActionUrl { get; set; }
    }


    public partial class StepParticipantActionForWorkflowViewModel
    {
        public int WorkFlowId { get; set; }
        public int StepId { get; set; }
        public int Id { get; set; }
        public string ActionName { get; set; }
        public string WorkflowName { get; set; }
    }
}
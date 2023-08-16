using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class SupportingDocumentViewModel
    {
        public int? StepId { get; set; }
        public string FileList { get; set; }
        public string OriginalFileNameList { get; set; }
        public string FilePath { get; set; }
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public int CreatedByRoleId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string Description { get; set; }
        
    }



    public partial class AuditViewModel
    {
        public string RelyProcessName { get; set; }
        public string VFProcessName { get; set; }
        public string ControlCode { get; set; }
        public string ControlDecription { get; set; }
        public string Action { get; set; }
        public string ActionType { get; set; }
        public int ActionedById { get; set; }
        public int ActionedByRoleId { get; set; }
        public DateTime ActionDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public string EntityTyppe { get; set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }
        public int WorkFlowId { get; set; }
        public string Comments { get; set; }
        public int? StepId { get; set; }
        public string ActionLabel { get; set; }
    }
}
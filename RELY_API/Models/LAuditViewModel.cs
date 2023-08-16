using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_API.Models
{
    public class LAuditViewModel
    {

        public int Id { get; set; }
        public string RelyProcessName { get; set; }
        public string VFProcessName { get; set; }
        public string ControlCode { get; set; }
        public string ControlDescription { get; set; }
        public string Action { get; set; }
        public string ActionType { get; set; }
        public int ActionedById { get; set; }
        public Nullable<int> ActionedByRoleId { get; set; }
        public DateTime ActionDateTime { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }
        public Nullable<int> WorkFlowId { get; set; }
        public string CompanyCode { get; set; }
        public string Comments { get; set; }
        public string ActionLabel { get; set; }

        public Nullable<int> StepId { get; set; }

        public Nullable<int> SupportingDocumentId { get; set; }
        //Foreign key Properties 
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string StepName { get; set; }
    }
}